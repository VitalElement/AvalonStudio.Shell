using System;
using System.Composition;

namespace AvalonStudio.Menus
{
    [MetadataAttribute]
    public class ExportOnPlatformAttribute : Attribute
    {
        public bool ExportOnWindows { get; }

        public bool ExportOnOsx { get; }

        public bool ExportOnLinux { get; }

        public ExportOnPlatformAttribute(bool windows = true, bool osx = true, bool linux = true)
        {
            ExportOnWindows = windows;
            ExportOnOsx = osx;
            ExportOnLinux = linux;
        }
    }
}
