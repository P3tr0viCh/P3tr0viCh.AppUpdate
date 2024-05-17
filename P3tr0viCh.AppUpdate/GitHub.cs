using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using P3tr0viCh.Utils;
using Newtonsoft.Json;

namespace P3tr0viCh.AppUpdate
{
    public class GitHub
    {
        private const string Url = "https://github.com";
        private const string UrlApi = "https://api.github.com";

        private const string UrlTags = "/repos/{0}/{1}/tags";
        private const string UrlLatestRelease = "{0}/{1}/releases/latest";
        private const string UrlDownloadFile = "{0}/{1}/releases/download/{2}/{3}";

        private const string MediaTypeTags = "application/vnd.github+json";
        private const string MediaTypeDownload = "application/vnd.github.raw+json";

        [LocalizedAttribute.DisplayName("GitHub.Owner.DisplayName", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.Description("GitHub.Owner.Description", "Properties.Resources.AppUpdate")]
        public string Owner { get; set; } = string.Empty;

        [LocalizedAttribute.DisplayName("GitHub.Repo.DisplayName", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.Description("GitHub.Repo.Description", "Properties.Resources.AppUpdate")]
        public string Repo { get; set; } = string.Empty;

        [LocalizedAttribute.DisplayName("GitHub.ArchiveFile.DisplayName", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.Description("GitHub.ArchiveFile.Description", "Properties.Resources.AppUpdate")]
        public string ArchiveFile { get; set; } = string.Empty;

        internal class Tags
        {
            public string name = string.Empty;
        }

        internal Uri GetLatestRelease()
        {
            var baseUri = new Uri(Url);
            return new Uri(baseUri, string.Format(UrlLatestRelease, Owner, Repo));
        }

        internal async Task<string> GetLatestVersionAsync()
        {
            using (var client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }))
            {
                Http.SetClientBaseAddress(client, UrlApi);
                Http.SetClientHeader(client);
                Http.SetClientMediaType(client, MediaTypeTags);

                var requestUri = string.Format(UrlTags, Owner, Repo);

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

        internal async Task DownloadAsync(string version,
            string fileName, string downloadFileName)
        {
            if (fileName.IsEmpty()) fileName = AppUpdate.DefaultArchiveFile;

            var downloadUri = string.Format(UrlDownloadFile, Owner, Repo, version, fileName);

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