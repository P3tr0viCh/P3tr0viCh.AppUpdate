using P3tr0viCh.Utils;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using static P3tr0viCh.Utils.Converters;

namespace P3tr0viCh.AppUpdate
{
    public partial class Config
    {
        [LocalizedAttribute.Category("Category.Common", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.DisplayName("Config.LocalFile.DisplayName", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.Description("Config.LocalFile.Description", "Properties.Resources.AppUpdate")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string LocalFile { get; set; }

        [LocalizedAttribute.Category("Category.Common", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.DisplayName("Config.CopySettings.DisplayName", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.Description("Config.CopySettings.Description", "Properties.Resources.AppUpdate")]
        public bool CopySettings { get; set; } = true;

        [LocalizedAttribute.Category("Category.UpdateLocation", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.DisplayName("Config.Location.DisplayName", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.Description("Config.Location.Description", "Properties.Resources.AppUpdate")]
        public Location Location { get; set; } = Location.GitHub;

        [LocalizedAttribute.Category("Category.UpdateLocation", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.DisplayName("Config.GitHub.DisplayName", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.Description("Config.GitHub.Description", "Properties.Resources.AppUpdate")]
        [TypeConverter(typeof(ExpandableObjectEmptyConverter))]
        public GitHub.Config GitHub { get; set; } = new GitHub.Config();

        [LocalizedAttribute.Category("Category.UpdateLocation", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.DisplayName("Config.Folder.DisplayName", "Properties.Resources.AppUpdate")]
        [LocalizedAttribute.Description("Config.Folder.Description", "Properties.Resources.AppUpdate")]
        [TypeConverter(typeof(ExpandableObjectEmptyConverter))]
        public Folder.Config Folder { get; set; } = new Folder.Config();
    }
}