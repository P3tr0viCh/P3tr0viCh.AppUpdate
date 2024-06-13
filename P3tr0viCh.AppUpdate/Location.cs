using P3tr0viCh.Utils;
using static P3tr0viCh.Utils.Converters;
using System.ComponentModel;

namespace P3tr0viCh.AppUpdate
{
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum Location
    {
        [LocalizedAttribute.Description("Config.Location.GitHub.Description", "Properties.Resources.AppUpdate")]
        GitHub,
        [LocalizedAttribute.Description("Config.Location.Folder.Description", "Properties.Resources.AppUpdate")]
        Folder,
    }
}