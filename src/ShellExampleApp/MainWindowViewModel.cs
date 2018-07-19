using AvalonStudio.Extensibility;
using AvalonStudio.Shell;

namespace ShellExampleApp
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            Shell = IoC.Get<IShell>();

            Shell.AddOrSelectDocument(() => new DocumentViewModel());
            Shell.AddOrSelectDocument(() => new WelcomeScreenViewModel());
        }

        public IShell Shell { get; }
    }
}
