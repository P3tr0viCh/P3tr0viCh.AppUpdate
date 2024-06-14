using Newtonsoft.Json;
using P3tr0viCh.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace P3tr0viCh.AppUpdate
{
    public partial class GitHub : IUpdater
    {
        private const string Url = "https://github.com";
        private const string UrlApi = "https://api.github.com";

        private const string UrlTags = "/repos/{0}/{1}/tags";
        private const string UrlLatestRelease = "{0}/{1}/releases/latest";
        private const string UrlDownloadFile = "{0}/{1}/releases/download/{2}/{3}";

        private const string MediaTypeTags = "application/vnd.github+json";
        private const string MediaTypeDownload = "application/vnd.github.raw+json";

        private readonly Config config;

        public GitHub(Config config)
        {
            this.config = config;
        }

        internal class Tags
        {
            public string name = string.Empty;
        }

        public Uri GetLatestRelease()
        {
            var baseUri = new Uri(Url);
            return new Uri(baseUri, string.Format(UrlLatestRelease, config.Owner, config.Repo));
        }

        private async Task<string> GetLatestVersionTagAsync()
        {
            using (var client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }))
            {
                Http.SetClientBaseAddress(client, UrlApi);
                Http.SetClientHeader(client);
                Http.SetClientMediaType(client, MediaTypeTags);

                var requestUri = string.Format(UrlTags, config.Owner, config.Repo);

                var response = await client.GetAsync(requestUri);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                var tags = JsonConvert.DeserializeObject<List<Tags>>(result);

                if (tags == null || tags.Count == 0)
                {
                    throw new GitHubEmptyTagsException();
                }

                return tags[0].name;
            }
        }

        public async Task<Version> GetLatestVersionAsync()
        {
            var latestTag = await GetLatestVersionTagAsync();

            return Utils.GetVersion(latestTag);
        }

        public async Task DownloadAsync(string downloadDir)
        {
            var latestTag = await GetLatestVersionTagAsync();

            var fileName = Utils.GetFileName(config.ArchiveFile, AppUpdate.DefaultArchiveFile);

            var downloadUri = string.Format(UrlDownloadFile, config.Owner, config.Repo, latestTag, fileName);

            var downloadFileName = Path.Combine(downloadDir, fileName);

            DebugWrite.Line($"{downloadUri} > {downloadFileName}");

            using (var client = new HttpClient())
            {
                Http.SetClientBaseAddress(client, Url);
                Http.SetClientHeader(client);
                Http.SetClientMediaType(client, MediaTypeDownload);

                using (var response = await client.GetAsync(downloadUri))
                {
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(downloadFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }
            }
        }
    }
}