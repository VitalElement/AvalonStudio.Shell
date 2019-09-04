using Avalonia.Media;
using System.Collections.Generic;
using System.Windows.Input;

namespace AvalonStudio.Menus
{
    public interface IMenuItem
    {
        string Label { get; }
        DrawingGroup Icon { get; }

        ICommand Command { get; }

        IEnumerable<string> Gestures { get; }
    }
}
