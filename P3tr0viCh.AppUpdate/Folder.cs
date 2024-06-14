using P3tr0viCh.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace P3tr0viCh.AppUpdate
{
    public partial class Folder : IUpdater
    {
        private readonly Config config;

        public Folder(Config config)
        {
            this.config = config;
        }

        public Uri GetLatestRelease()
        {
            return new Uri(config.Path);
        }

        public async Task<Version> GetLatestVersionAsync()
        {
            var versionFile = Utils.GetFileName(config.VersionFile, AppUpdate.DefaultVersionFile);

            versionFile = Path.Combine(config.Path, versionFile);
            
            DebugWrite.Line($"{versionFile}");

            return await Task.Run(() =>
            {
                if (!File.Exists(versionFile)) throw new VersionFileNotFoundException();

                var versionStr = File.ReadAllText(versionFile);

                return Utils.GetVersion(versionStr);
            });
        }

        public async Task DownloadAsync(string downloadDir)
        {
            var fileName = Utils.GetFileName(config.ArchiveFile, AppUpdate.DefaultArchiveFile);

            var latestFileName = Path.Combine(config.Path, fileName);

            var downloadFileName = Path.Combine(downloadDir, fileName);

            DebugWrite.Line($"{latestFileName} > {downloadFileName}");

            await Task.Run(() =>
            {
                if (!File.Exists(latestFileName)) throw new LatestFileNotFoundException();

                Utils.FileCopy(latestFileName, downloadFileName);
            });
        }
    }
}