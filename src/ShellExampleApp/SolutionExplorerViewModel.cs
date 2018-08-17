using AvalonStudio.Extensibility;
using AvalonStudio.MVVM;
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
    public class SolutionExplorerViewModel : ToolViewModel, IExtension
    {
        public override Location DefaultLocation => Location.Top;

        public SolutionExplorerViewModel()
        {
            Title = "Solution Explorer";
        }
    }
}
