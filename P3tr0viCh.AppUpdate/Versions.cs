using System;

namespace P3tr0viCh.AppUpdate
{
    public class Versions
    {
        public Version Local { get; internal set; } = null;

        public Version Latest { get; internal set; } = null;

        public int Compare() => Latest != null ? Latest.CompareTo(Local) : 0;

        public bool IsLatest() => Compare() != 1;
    }
}