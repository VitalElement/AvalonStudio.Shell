using AvalonStudio.Extensibility.Dialogs;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reflection;

namespace ShellExampleApp
{
    public class AboutDialogViewModel : ModalDialogViewModelBase
    {
        public AboutDialogViewModel() : base("About", true, false)
        {
            OKCommand = ReactiveCommand.Create(()=>Close()).DisposeWith(Disposables);
        }

        public override ReactiveCommand OKCommand { get; protected set; }

        public string Version => FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).FileVersion;	        
    }
}
