using System;
using System.IO;

namespace P3tr0viCh.AppUpdate
{
    public class LocalFileNotFoundException : FileNotFoundException { }

    public class LocalFileWrongLocationException : FileNotFoundException { }

    public class FileNotExistsInArchiveException : FileNotFoundException { }

    public class GitHubEmptyTagsException : Exception { }
}