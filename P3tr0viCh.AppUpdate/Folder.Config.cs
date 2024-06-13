using P3tr0viCh.Utils;

namespace P3tr0viCh.AppUpdate
{
    public partial class Folder
    {
        public class Config
        {
            [LocalizedAttribute.DisplayName("Folder.Path.DisplayName", "Properties.Resources.AppUpdate")]
            [LocalizedAttribute.Description("Folder.Path.Description", "Properties.Resources.AppUpdate")]
            public string Path { get; set; } = string.Empty;

            [LocalizedAttribute.DisplayName("Folder.ArchiveFile.DisplayName", "Properties.Resources.AppUpdate")]
            [LocalizedAttribute.Description("Folder.ArchiveFile.Description", "Properties.Resources.AppUpdate")]
            public string ArchiveFile { get; set; } = string.Empty;

            [LocalizedAttribute.DisplayName("Folder.VersionFile.DisplayName", "Properties.Resources.AppUpdate")]
            [LocalizedAttribute.Description("Folder.VersionFile.Description", "Properties.Resources.AppUpdate")]
            public string VersionFile { get; set; } = string.Empty;
        }
    }
}