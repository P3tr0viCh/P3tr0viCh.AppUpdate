using System;

namespace P3tr0viCh.AppUpdate
{
    internal class UpdaterFactory
    {
        public static IUpdater GetUpdater(Config config)
        {
            switch (config.Location)
            {
                case Location.GitHub:
                    return new GitHub(config.GitHub);
                case Location.Folder:
                    return new Folder(config.Folder);
                default:
                    throw new ArgumentException();
            }
        }
    }
}