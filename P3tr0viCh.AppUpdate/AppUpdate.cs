using Newtonsoft.Json;
using P3tr0viCh.Utils;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using static P3tr0viCh.Utils.Converters;

namespace P3tr0viCh.AppUpdate
{
    public partial class AppUpdate
    {
        public const string ParentDirNamePattern = @"\d\.\d\.\d\.\d";

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

            [LocalizedAttribute.Category("Category.Common", "Properties.Resources.AppUpdate")]
            [LocalizedAttribute.DisplayName("Config.LocalFile.DisplayName", "Properties.Resources.AppUpdate")]
            [LocalizedAttribute.Description("Config.LocalFile.Description", "Properties.Resources.AppUpdate")]
            [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
            public string LocalFile { get; set; }

            [LocalizedAttribute.Category("Category.UpdateLocation", "Properties.Resources.AppUpdate")]
            [LocalizedAttribute.DisplayName("Config.GitHub.DisplayName", "Properties.Resources.AppUpdate")]
            [LocalizedAttribute.Description("Config.GitHub.Description", "Properties.Resources.AppUpdate")]
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

                    if (string.Compare(Path.GetExtension(LocalFile), ".exe", true) != 0) throw new LocalFileBadFormatException();

                    CheckLocalVersion();

                    var parentDirName = Utils.GetParentName(LocalFile);

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

                var status = Status.Start(AppUpdate.Status.CheckLocal);

                localVersion = null;

                try
                {
                    localVersion = Misc.GetFileVersion(LocalFile);
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

                    var downloadDir = Utils.GetDownload(programRoot);

                    var moveDir = Utils.CreateMoveDir(downloadDir, LocalFile);

                    var verisonDir = Utils.CreateVersion(programRoot, LocalFile);

                    Utils.DirectoryMove(moveDir, verisonDir);

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

                var status = Status.Start(AppUpdate.Status.Download);

                try
                {
                    Check();

                    var programRoot = Utils.GetProgramRoot(LocalFile);

                    var downloadDir = Utils.CreateDownload(programRoot);

                    var archiveFileName = Path.Combine(downloadDir, GitHub.ArchiveFile);

                    await GitHub.DownloadAsync(latestVersionStr, GitHub.ArchiveFile, archiveFileName);

                    await ArchiveExtractAsync(archiveFileName, downloadDir);

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