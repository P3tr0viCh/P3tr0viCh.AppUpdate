﻿using System;
using System.IO;

namespace P3tr0viCh.AppUpdate
{
    public class LocalFileNotFoundException : FileNotFoundException { }

    public class LocalFileBadFormatException : FileNotFoundException { }

    public class LocalFileWrongLocationException : FileNotFoundException { }

    public class GitHubEmptyTagsException : Exception { }

    public class VersionFileNotFoundException : FileNotFoundException { }

    public class LatestFileNotFoundException : FileNotFoundException { }

    public class ArchiveBadFormatException : FileNotFoundException { }
}