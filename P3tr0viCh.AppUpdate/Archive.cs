using P3tr0viCh.Utils;
using System.IO.Compression;
using System.Threading.Tasks;

namespace P3tr0viCh.AppUpdate
{
    internal class Archive
    {
        public static async Task ZipExtractAsync(string archiveFileName, string destinationDir)
        {
            DebugWrite.Line($"{archiveFileName} > {destinationDir}");

            try
            {
                await Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(archiveFileName, destinationDir);
                });
            }
            finally
            {
                DebugWrite.Line("done");
            }
        }
    }
}