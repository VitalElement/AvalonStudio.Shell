using AvalonStudio.Extensibility;
using AvalonStudio.MVVM;
using AvalonStudio.Shell;
using System.Composition;

namespace ShellExampleApp
{
    [Export(typeof(IExtension))]
    [Export]
    [ExportToolControl]
    [Shared]
    public class ConsoleViewModel : ToolViewModel, IActivatableExtension
    {
        public override Location DefaultLocation => Location.Bottom;

        public ConsoleViewModel()
        {
            Title = "Console";
        }

        public void BeforeActivation()
        {
        }

        public void Activation()
        {
            IoC.Get<IShell>().MainPerspective.AddOrSelectTool(this);
        }
    }
}
