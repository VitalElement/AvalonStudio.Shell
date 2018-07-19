using AvalonStudio.Extensibility;
using AvalonStudio.MVVM;
using System.Composition;

namespace ShellExampleApp
{
    [Export(typeof(IExtension))]
    [Export]
    [ExportToolControl]
    [Shared]
    public class ConsoleViewModel : ToolViewModel, IExtension
    {
        public override Location DefaultLocation => Location.Left;

        public ConsoleViewModel()
        {
            Title = "Console";
        }
    }
}
