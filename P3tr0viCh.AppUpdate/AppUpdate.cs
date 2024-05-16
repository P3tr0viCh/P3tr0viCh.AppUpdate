using Newtonsoft.Json;
using P3tr0viCh.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using static P3tr0viCh.Utils.Converters;

namespace P3tr0viCh.AppUpdate
{
    public partial class AppUpdate
    {
        public const string DefaultArchiveFile = "latest.zip";

        public class Config
        {
            private Version localVersion = null;

            [JsonIgnore]
            [Browsable(false)]
            public Version LocalVersion => localVersion;

            private Version latestVersion = null;
            [JsonIgnore]
            [Browsable(false)]
            public Version LatestVersion => latestVersion;

            private string latestVersionStr = string.Empty;

            [Category("Общее")]
            [DisplayName("Программа")]
            [Description("Расположение исполняемого файла.\n" +
               "Файл (и все остальные компоненты обновляемой программы) должен находиться в каталоге «latest», " +
               "который располагается в каталоге обновляемой программы.\n" +
               "Например:\n" +
               "c:\\Program Files\\Updater\\latest\\Updater.exe")]
            [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
            public string LocalFile { get; set; }

            [Category("Расположение обновления")]
            [DisplayName("ГитХаб")]
            [Description("")]
            [TypeConverter(typeof(ExpandableObjectEmptyConverter))]
            public GitHub GitHub { get; } = new GitHub();

            private readonly ProgramStatus status = new ProgramStatus();
            [JsonIgnore]
            [Browsable(false)]
            public ProgramStatus Status => status;

            public Uri GetLatestRelease()
            {
                return GitHub.GetLatestRelease();
            }

            public int CompareVersions()
            {
                return LatestVersion.CompareTo(localVersion);
            }

            public bool IsLatestVersion()
            {
                return CompareVersions() != 1;
            }

            public void Check()
            {
                DebugWrite.Line("start");

                var status = Status.Start(AppUpdate.Status.Check);

                try
                {
                    if (LocalFile.IsEmpty() ||
                        GitHub is null ||
                        GitHub.Owner.IsEmpty()) throw new NullReferenceException();

                    if (!File.Exists(LocalFile)) throw new LocalFileNotFoundException();

                    var parentDirName = Utils.GetParentName(LocalFile);

                    if (parentDirName != Utils.LatestDirName) throw new LocalFileWrongLocationException();
                }
                finally
                {
                    Status.Stop(status);
                }
            }

            public void CheckLocalVersion()
            {
                DebugWrite.Line("start");

                var status = Status.Start(AppUpdate.Status.CheckLocal);

                localVersion = null;

                try
                {
                    if (File.Exists(LocalFile))
                    {
                        var info = FileVersionInfo.GetVersionInfo(LocalFile);

                        localVersion = Misc.GetFileVersion(info);
                    }
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

                var status = Status.Start(AppUpdate.Status.CheckLatest);

                latestVersion = null;

                try
                {
                    latestVersionStr = await GitHub.GetLatestVersionAsync();

                    var tempVersion = new Version(latestVersionStr);

                    latestVersion = new Version(tempVersion.Major, tempVersion.Minor,
                        tempVersion.Build == -1 ? 0 : tempVersion.Build,
                        tempVersion.Revision == -1 ? 0 : tempVersion.Revision);
                }
                finally
                {
                    Status.Stop(status);
                }

                DebugWrite.Line("done");
            }

            public async Task CheckVersionsAsync()
            {
                Check();
                CheckLocalVersion();
                await CheckLatestVersionAsync();
            }

            private async Task ArchiveExtractAsync(string archiveFileName, string destinationDir)
            {
                DebugWrite.Line("start");

                var status = Status.Start(AppUpdate.Status.ArchiveExtract);

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

                var status = Status.Start(AppUpdate.Status.Update);

                try
                {
                    await DownloadAsync();

                    var programRoot = Utils.GetProgramRoot(LocalFile);

                    var downloadedDir = Utils.GetDownloaded(LocalFile, programRoot);

                    var moveDir = Utils.CreateMoveDir(downloadedDir, LocalFile);

                    var latestDir = Utils.CreateLatest(programRoot, LocalFile);

                    Utils.DirectoryMove(moveDir, latestDir);

                    Utils.DirectoryDelete(downloadedDir);
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

                var status = Status.Start(AppUpdate.Status.Download);

                try
                {
                    Check();

                    var downloadedDir = Utils.CreateDownloaded(LocalFile, string.Empty);

                    var archiveFileName = Path.Combine(downloadedDir, GitHub.ArchiveFile);

                    await GitHub.DownloadAsync(latestVersionStr, GitHub.ArchiveFile, archiveFileName);

                    await ArchiveExtractAsync(archiveFileName, downloadedDir);

                    File.Delete(archiveFileName);
                }
                finally
                {
                    Status.Stop(status);
                }

                DebugWrite.Line("done");
            }
        }
    }
}