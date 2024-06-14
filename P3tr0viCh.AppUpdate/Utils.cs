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

        public static void FileCopy(string sourceFileName, string destFileName)
        {
            DebugWrite.Line($"{sourceFileName} > {destFileName}");

            File.Copy(sourceFileName, destFileName);
        }

        public static void FileReplace(string sourceFileName, string destFileName, string destBackupFileName)
        {
            DebugWrite.Line($"{sourceFileName} > {destFileName} > {destBackupFileName}");

            File.Replace(sourceFileName, destFileName, destBackupFileName);
        }

        public static void FileMove(string sourceFileName, string destFileName)
        {
            DebugWrite.Line($"{sourceFileName} > {destFileName}");

            File.Move(sourceFileName, destFileName);
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

        public static Version GetVersion(string version)
        {
            var tempVersion = new Version(version);

            return new Version(tempVersion.Major, tempVersion.Minor,
                tempVersion.Build == -1 ? 0 : tempVersion.Build,
                tempVersion.Revision == -1 ? 0 : tempVersion.Revision);
        }

        private static string GetUnixTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        public static string GetFileName(string fileName, string defaultFileName)
        {
            return fileName.IsEmpty() ? defaultFileName : fileName;
        }

        public static string GetMoveDir(string programRoot, string sourceDirName)
        {
            var result = Path.Combine(programRoot, Path.GetFileName(sourceDirName));

            if (Directory.Exists(result))
            {
                result += "_" + GetUnixTime();
            }

            return result;
        }

        public static string GetFileNameBackup(string fileName, Version fileVersion)
        {
            var fileWithoutExt = Path.Combine(Path.GetDirectoryName(fileName),
                Path.GetFileNameWithoutExtension(fileName));

            var ext = Path.GetExtension(fileName);

            fileWithoutExt += "_" + fileVersion.ToString();

            var result = fileWithoutExt + ext;

            if (File.Exists(result))
            {
                result = fileWithoutExt + "_" + GetUnixTime() + ext;
            }

            return result;
        }
    }
}