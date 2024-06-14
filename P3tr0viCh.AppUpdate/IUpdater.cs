using System;
using System.Threading.Tasks;

namespace P3tr0viCh.AppUpdate
{
    internal interface IUpdater
    {
        Uri GetLatestRelease();

        Task<Version> GetLatestVersionAsync();

        Task DownloadAsync(string downloadDir);
    }
}