using AvalonStudio.Extensibility;
using AvalonStudio.MVVM;
using AvalonStudio.Shell;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace ShellExampleApp
{
    [Export(typeof(IExtension))]
    [Export]
    [ExportToolControl]
    [Shared]
    public class SolutionExplorerViewModel : ToolViewModel, IActivatableExtension
    {
        public override Location DefaultLocation => Location.Right;

        public SolutionExplorerViewModel()
        {
            Title = "Solution Explorer";
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
