using Avalonia.Media;
using System.Collections.Generic;
using System.Windows.Input;

namespace AvalonStudio.Menus
{
    internal class HeaderMenuItem : IMenuItem
    {
        public string Label { get; }
        public DrawingGroup Icon { get; }

        public ICommand Command => null;

        public string Gesture => null;

        public HeaderMenuItem(string label, DrawingGroup icon)
        {
            Label = label;
            Icon = icon;
        }
    }
}
