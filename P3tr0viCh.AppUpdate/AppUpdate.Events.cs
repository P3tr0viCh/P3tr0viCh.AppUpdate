using System;

namespace P3tr0viCh.AppUpdate
{
    public partial class AppUpdate
    {
        public class AfterUpdateEventArgs : EventArgs
        {
            public AfterUpdateEventArgs(string currentDir, string latestDir)
            {
                CurrentDir = currentDir;
                LatestDir = latestDir;
            }

            public string CurrentDir { get; private set; } = string.Empty;
            public string LatestDir { get; private set; } = string.Empty;
        }

        public delegate void AfterUpdateEventHandler(object sender, AfterUpdateEventArgs e);
    }
}