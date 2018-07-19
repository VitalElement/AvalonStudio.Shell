using AvalonStudio.Extensibility;
using AvalonStudio.MVVM;
using System.Composition;

namespace ShellExampleApp
{
    [Export(typeof(IExtension))]
    [Export]
    [ExportToolControl]
    [Shared]
    public class ToolExplorerViewModel : ToolViewModel, IExtension
    {
        public override Location DefaultLocation => Location.Bottom;

        public ToolExplorerViewModel()
        {
            Title = "Tool Explorer";
        }
    }
}
