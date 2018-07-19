using AvalonStudio.Extensibility;
using AvalonStudio.Shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShellExampleApp
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            Shell = IoC.Get<IShell>();
        }

        public IShell Shell { get; }
    }
}
