using P3tr0viCh.Utils;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace P3tr0viCh.AppUpdate
{
    public class AppUpdate
    {
        public const string ParentDirNamePattern = @"\d+\.\d+\.\d+\.\d+";

        public const string DefaultArchiveFile = "latest.zip";
        
        public const string DefaultVersionFile = "version";

        public ProgramStatus Status { get; } = new ProgramStatus();

        public Versions Versions { get; } = new Versions();

        public Config Config { get; set; } = new Config();

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
                Versions.Local = Misc.GetFileVersion(Config.LocalFile);

                DebugWrite.Line(Versions.Local.ToString());
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

                DebugWrite.Line(Versions.Latest.ToString());
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

        public async Task UpdateAsync()
        {
            DebugWrite.Line("start");

            var status = Status.Start(UpdateStatus.Update);

            try
            {
                await DownloadAsync();

                var programRoot = Utils.GetProgramRoot(Config.LocalFile);

                var downloadDir = Utils.GetDownloadDir(programRoot);

                var archiveFileName = Path.Combine(downloadDir, Config.GitHub.ArchiveFile);

                await ArchiveExtractAsync(archiveFileName, downloadDir);

                File.Delete(archiveFileName);

                var fileNameOnly = Path.GetFileName(Config.LocalFile);

                var moveDir = Utils.GetMoveDir(downloadDir, fileNameOnly);

                var versionDir = Utils.GetVersionDir(programRoot, moveDir, fileNameOnly);

                Utils.DirectoryMove(moveDir, versionDir);

                Utils.DirectoryDelete(downloadDir);
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

                var archiveFileName = Path.Combine(downloadDir, Config.GitHub.ArchiveFile);

                var gitHub = new GitHub(Config.GitHub);

                await gitHub.DownloadAsync(Versions.latestStr, archiveFileName);
            }
            finally
            {
                Status.Stop(status);
            }

            DebugWrite.Line("done");
        }
    }
}