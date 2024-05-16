using P3tr0viCh.Utils;
using System;
using System.IO;

namespace P3tr0viCh.AppUpdate
{
    internal class Utils
    {
        public const string DownloadedDirName = "downloaded";
        public const string LatestDirName = "latest";

        public static void DirectoryCreate(string path)
        {
            DebugWrite.Line($"{path}");

            Directory.CreateDirectory(path);
        }

        public static void DirectoryRename(string sourceDirFullName, string destDirOnlyName)
        {
            DebugWrite.Line($"{sourceDirFullName} > {destDirOnlyName}");

            Files.DirectoryRename(sourceDirFullName, destDirOnlyName);
        }

        public static void DirectoryMove(string sourceDirName, string destDirName)
        {
            DebugWrite.Line($"{sourceDirName} > {destDirName}");

            Directory.Move(sourceDirName, destDirName);
        }

        public static void DirectoryDelete(string path)
        {
            DebugWrite.Line($"{path}");

            Files.DirectoryDelete(path);
        }

        public static string GetProgramRoot(string fileName)
        {
            return Path.GetDirectoryName(Path.GetDirectoryName(fileName));
        }

        public static string GetParentName(string fileName)
        {
            return Path.GetFileName(Path.GetDirectoryName(fileName));
        }

        public static string GetDownloaded(string fileName, string programRoot)
        {
            if (programRoot.IsEmpty())
            {
                programRoot = Path.GetDirectoryName(Path.GetDirectoryName(fileName));
            }

            var downloadedDir = Path.Combine(programRoot, DownloadedDirName);

            return downloadedDir;
        }

        public static string CreateDownloaded(string fileName, string programRoot)
        {
            var downloadedDir = GetDownloaded(fileName, programRoot);

            DirectoryDelete(downloadedDir);

            DirectoryCreate(downloadedDir);

            return downloadedDir;
        }

        public static string CreateMoveDir(string downloadedDir, string fileName)
        {
            var moveDir = downloadedDir;

            var fileNameOnly = Path.GetFileName(fileName);

            if (File.Exists(Path.Combine(downloadedDir, fileNameOnly)))
            {
                return moveDir;
            }

            foreach (var directory in Directory.EnumerateDirectories(downloadedDir))
            {
                if (File.Exists(Path.Combine(directory, fileNameOnly)))
                {
                    return directory;
                }

                break;
            }

            throw new FileNotExistsInArchiveException();
        }

        public static string CreateLatest(string programRoot, string fileName)
        {
            var latestDir = Path.Combine(programRoot, LatestDirName);

            string versionName;

            if (File.Exists(fileName))
            {
                var version = Misc.GetFileVersion(fileName);

                versionName = version.ToString();

                var versionDir = Path.Combine(programRoot, versionName);

                if (Directory.Exists(versionDir))
                {
                    versionName += "_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                }

            } else
            {
                versionName = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            }

            DirectoryRename(latestDir, versionName);

            return latestDir;
        }
    }
}