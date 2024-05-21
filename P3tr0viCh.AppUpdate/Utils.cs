using P3tr0viCh.Utils;
using System;
using System.IO;

namespace P3tr0viCh.AppUpdate
{
    internal class Utils
    {
        private const string DownloadDirName = "download";

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
            var programRoot = Path.GetDirectoryName(Path.GetDirectoryName(fileName));

            DebugWrite.Line($"ProgramRoot: {programRoot}");

            return programRoot;
        }

        public static string GetParentName(string fileName)
        {
            return Path.GetFileName(Path.GetDirectoryName(fileName));
        }

        public static string GetDownloadDir(string programRoot)
        {
            var downloadDir = Path.Combine(programRoot, DownloadDirName);

            DebugWrite.Line($"DownloadDir: {downloadDir}");

            return downloadDir;
        }

        public static string CreateDownloadDir(string programRoot)
        {
            var downloadDir = GetDownloadDir(programRoot);

            DirectoryDelete(downloadDir);

            DirectoryCreate(downloadDir);

            return downloadDir;
        }

        public static string GetMoveDir(string downloadDir, string fileNameOnly)
        {
            DebugWrite.Line($"DownloadDir: {downloadDir}"); 
            
            var moveDir = downloadDir;

            if (File.Exists(Path.Combine(downloadDir, fileNameOnly)))
            {
                DebugWrite.Line($"MoveDir: {moveDir}");

                return moveDir;
            }

            foreach (var directory in Directory.EnumerateDirectories(downloadDir))
            {
                if (File.Exists(Path.Combine(directory, fileNameOnly)))
                {
                    return directory;
                }

                break;
            }

            throw new FileNotExistsInArchiveException();
        }

        public static string GetVersionDir(string programRoot, string moveDir, string fileNameOnly)
        {
            var version = Misc.GetFileVersion(Path.Combine(moveDir, fileNameOnly));

            var versionName = version.ToString();

            var versionDir = Path.Combine(programRoot, versionName);

            if (Directory.Exists(versionDir))
            {
                versionDir += "_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            }

            return versionDir;
        }
    }
}