using P3tr0viCh.Utils;
using System;
using System.IO;
using System.IO.Compression;
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
            var versionFile = config.VersionFile;

            if (versionFile.IsEmpty()) versionFile = AppUpdate.DefaultVersionFile;

            return await Task.Run(() =>
            {
                if (!File.Exists(versionFile)) throw new VersionFileNotFoundException();

                var versionStr = File.ReadAllText(versionFile);

                return new Version(versionStr);
            });
        }
    }
}