using P3tr0viCh.Utils;

namespace P3tr0viCh.AppUpdate
{
    public partial class GitHub
    {
        public class Config
        {
            [LocalizedAttribute.DisplayName("GitHub.Owner.DisplayName", "Properties.Resources.AppUpdate")]
            [LocalizedAttribute.Description("GitHub.Owner.Description", "Properties.Resources.AppUpdate")]
            public string Owner { get; set; } = string.Empty;

            [LocalizedAttribute.DisplayName("GitHub.Repo.DisplayName", "Properties.Resources.AppUpdate")]
            [LocalizedAttribute.Description("GitHub.Repo.Description", "Properties.Resources.AppUpdate")]
            public string Repo { get; set; } = string.Empty;

            [LocalizedAttribute.DisplayName("GitHub.ArchiveFile.DisplayName", "Properties.Resources.AppUpdate")]
            [LocalizedAttribute.Description("GitHub.ArchiveFile.Description", "Properties.Resources.AppUpdate")]
            public string ArchiveFile { get; set; } = string.Empty;
        }
    }
}