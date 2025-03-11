using System;

namespace P3tr0viCh.AppUpdate
{
    public class Versions
    {
        public Version Local { get; internal set; } = null;

        public Version Latest { get; internal set; } = null;

        public int Compare() => Latest.CompareTo(Local);

        public bool IsLatest() => Compare() != 1;
    }
}