using System.Collections.Generic;
using System.ComponentModel;

namespace AvalonStudio.Commands
{
    public class CommandDefinitionMetadata
    {
        public string Name { get; set; }

        [DefaultValue(null)]
        public string DefaultKeyGesture { get; set; }

        [DefaultValue(null)]
        public string WindowsKeyGesture { get; set; }

        [DefaultValue(null)]
        public string OSXKeyGesture { get; set; }

        [DefaultValue(null)]
        public string LinuxKeyGesture { get; set; }
    }
}
