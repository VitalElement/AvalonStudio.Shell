using AvalonStudio.Commands;
using AvalonStudio.Extensibility;
using AvalonStudio.Shell;
using ReactiveUI;
using System;
using System.Composition;
using System.Reactive.Disposables;

namespace ShellExampleApp.Commands
{
    internal class HelpCommands : IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        [ExportCommandDefinition("Help.About")]
        public CommandDefinition AboutCommand { get; }

        private IShell _shell;

        [ImportingConstructor]
        public HelpCommands(CommandIconService commandIconService)
        {
            _shell = IoC.Get<IShell>();

            AboutCommand = new CommandDefinition(
                "About", commandIconService.GetCompletionKindImage("Undo"), ReactiveCommand.Create(ShowAboutDialog).DisposeWith(Disposables));
        }

        private void ShowAboutDialog()
        {
            _shell.ModalDialog = new AboutDialogViewModel().DisposeWith(Disposables);
            _shell.ModalDialog.ShowDialogAsync().DisposeWith(Disposables);
        }

        #region IDisposable Support
        private volatile bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disposables?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
