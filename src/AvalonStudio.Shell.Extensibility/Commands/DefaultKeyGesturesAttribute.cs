using System;
using System.Collections.Generic;
using System.Composition;

namespace AvalonStudio.Commands
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DefaultKeyGestureAttribute : Attribute
    {
        public string DefaultKeyGesture { get; }

        public string OSXKeyGesture { get; }

        public string LinuxKeyGesture { get; }

        public string WindowsKeyGesture { get; }

        public DefaultKeyGestureAttribute
            (string defaultGesture, string windowsKeyGesture = null, string osxKeyGesture = null, string linuxKeyGesture = null)
        {
            DefaultKeyGesture = defaultGesture;

            WindowsKeyGesture = windowsKeyGesture;

            OSXKeyGesture = osxKeyGesture;

            LinuxKeyGesture = linuxKeyGesture;
        }
    }
}
