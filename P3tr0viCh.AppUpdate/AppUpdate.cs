﻿using P3tr0viCh.Utils;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace P3tr0viCh.AppUpdate
{
    public partial class AppUpdate
    {
        public const string ParentDirNamePattern = @"\d+\.\d+\.\d+\.\d+";

        public const string DefaultArchiveFile = "latest.zip";

        public const string DefaultVersionFile = "version";

        public ProgramStatus Status { get; } = new ProgramStatus();

        public Versions Versions { get; } = new Versions();

        public Config Config { get; set; } = new Config();


        public event AfterUpdateEventHandler AfterUpdate;

        public Uri GetLatestRelease()
        {
            var updater = UpdaterFactory.GetUpdater(Config);

            return updater.GetLatestRelease();
        }

        public void Check()
        {
            DebugWrite.Line("start");

            var status = Status.Start(UpdateStatus.Check);

            try
            {
                if (Config.LocalFile.IsEmpty()) throw new NullReferenceException("local file is empty");

                switch (Config.Location)
                {
                    case Location.GitHub:
                        if (Config.GitHub is null ||
                            Config.GitHub.Owner.IsEmpty() ||
                            Config.GitHub.Repo.IsEmpty()) throw new NullReferenceException("config.github is empty");
                        break;
                    case Location.Folder:
                        if (Config.Folder is null ||
                            Config.Folder.Path.IsEmpty()) throw new NullReferenceException("config.folder is empty");
                        break;
                }

                if (!File.Exists(Config.LocalFile)) throw new LocalFileNotFoundException();

                if (string.Compare(Path.GetExtension(Config.LocalFile), ".exe", true) != 0) throw new LocalFileBadFormatException();

                CheckLocalVersion();

                var parentDirName = Utils.GetParentName(Config.LocalFile);

                if (!Regex.IsMatch(parentDirName, ParentDirNamePattern)) throw new LocalFileWrongLocationException();
            }
            finally
            {
                Status.Stop(status);
            }

            DebugWrite.Line("done");
        }

        public void CheckLocalVersion()
        {
            DebugWrite.Line("start");

            var status = Status.Start(UpdateStatus.CheckLocal);

            Versions.Local = null;

            try
            {
                Versions.Local = Files.GetFileVersion(Config.LocalFile);

                DebugWrite.Line(Versions.Local?.ToString());
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);

                throw new LocalFileBadFormatException();
            }
            finally
            {
                Status.Stop(status);
            }

            DebugWrite.Line("done");
        }

        public async Task CheckLatestVersionAsync()
        {
            DebugWrite.Line("start");

            var status = Status.Start(UpdateStatus.CheckLatest);

            Versions.Latest = null;

            try
            {
                var updater = UpdaterFactory.GetUpdater(Config);

                Versions.Latest = await updater.GetLatestVersionAsync();

                DebugWrite.Line(Versions.Latest?.ToString());
            }
            finally
            {
                Status.Stop(status);
            }

            DebugWrite.Line("done");
        }

        private async Task ArchiveExtractAsync(string archiveFileName, string destinationDir)
        {
            DebugWrite.Line("start");

            var status = Status.Start(UpdateStatus.ArchiveExtract);

            try
            {
                await Archive.ZipExtractAsync(archiveFileName, destinationDir);
            }
            finally
            {
                Status.Stop(status);
            }

            DebugWrite.Line("done");
        }

        private string GetArchiveFile()
        {
            switch (Config.Location)
            {
                case Location.GitHub:
                    return Utils.GetFileName(Config.GitHub.ArchiveFile, DefaultArchiveFile);
                case Location.Folder:
                    return Utils.GetFileName(Config.Folder.ArchiveFile, DefaultArchiveFile);
                default:
                    throw new ArgumentException();
            }
        }

        public async Task UpdateAsync()
        {
            DebugWrite.Line("start");

            var status = Status.Start(UpdateStatus.Update);

            try
            {
                await DownloadAsync();

                var currentDir = Utils.GetCurrentDir(Config.LocalFile);

                var programRoot = Utils.GetProgramRoot(Config.LocalFile);

                var downloadDir = Utils.GetDownloadDir(programRoot);

                var archiveFile = GetArchiveFile();

                var archiveFilePath = Path.Combine(downloadDir, archiveFile);

                await ArchiveExtractAsync(archiveFilePath, downloadDir);

                File.Delete(archiveFilePath);

                /* %PROGRAM_NAME_DIR%
                 * |- %PROGRAM_NAME%.exe (starter)
                 * |- %VERSION_DIR%
                 *    |- %PROGRAM_NAME%.exe
                 *    |- *.dll
                 *    |- other dirs&files
                 */

                // %PROGRAM_NAME_DIR%
                var dirs = Directory.GetDirectories(downloadDir);

                if (dirs.Length != 1) throw new ArchiveBadFormatException();

                var latestProgramRoot = dirs[0];

                // %VERSION_DIR%
                dirs = Directory.GetDirectories(latestProgramRoot);

                if (dirs.Length != 1) throw new ArchiveBadFormatException();

                var latestDir = dirs[0];

                var fileNameOnly = Path.GetFileName(Config.LocalFile);

                // %PROGRAM_NAME%.exe (starter)
                var latestStarter = Path.Combine(latestProgramRoot, fileNameOnly);

                if (!File.Exists(latestStarter)) throw new ArchiveBadFormatException();

                // %PROGRAM_NAME%.exe
                var latestFilePath = Path.Combine(latestDir, fileNameOnly);

                if (!File.Exists(latestFilePath)) throw new ArchiveBadFormatException();

                var currentStarter = Path.Combine(programRoot, fileNameOnly);

                if (File.Exists(currentStarter))
                {
                    var currentStarterVersion = Files.GetFileVersion(currentStarter);

                    if (Files.GetFileVersion(latestStarter).CompareTo(currentStarterVersion) > 0)
                    {
                        var currentStarterBackup = Utils.GetFileNameBackup(currentStarter, currentStarterVersion);

                        Utils.FileReplace(latestStarter, currentStarter, currentStarterBackup);
                    }
                }
                else
                {
                    Utils.FileMove(latestStarter, currentStarter);
                }

                var destDirName = Utils.GetMoveDir(programRoot, latestDir);

                Utils.DirectoryMove(latestDir, destDirName);

                latestDir = destDirName;

                Utils.DirectoryDelete(downloadDir);

                AfterUpdateEvent(new AfterUpdateEventArgs(currentDir, latestDir));
            }
            finally
            {
                Status.Stop(status);
            }

            DebugWrite.Line("done");
        }

        public async Task DownloadAsync()
        {
            DebugWrite.Line("start");

            var status = Status.Start(UpdateStatus.Download);

            try
            {
                Check();

                var programRoot = Utils.GetProgramRoot(Config.LocalFile);

                var downloadDir = Utils.CreateDownloadDir(programRoot);

                var updater = UpdaterFactory.GetUpdater(Config);

                await updater.DownloadAsync(downloadDir);
            }
            finally
            {
                Status.Stop(status);
            }

            DebugWrite.Line("done");
        }

        internal void AfterUpdateEvent(AfterUpdateEventArgs args)
        {
            if (Config.CopySettings)
            {
                var settingsFileName = Files.SettingsFileName(Config.LocalFile);

                var currentSettingsFilePath = Path.Combine(args.CurrentDir, settingsFileName);

                if (File.Exists(currentSettingsFilePath))
                {
                    Utils.FileCopy(currentSettingsFilePath, Path.Combine(args.LatestDir, settingsFileName));
                }
            }

            AfterUpdate?.Invoke(this, args);
        }
    }
}