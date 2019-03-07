using AvalonStudio.Extensibility;
using AvalonStudio.Shell;
using System;
using System.Reactive.Disposables;

namespace ShellExampleApp
{
    public class MainWindowViewModel : IDisposable
    {
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public MainWindowViewModel()
        {
            Shell = IoC.Get<IShell>();

            Shell.AddOrSelectDocument(() => new DocumentViewModel().DisposeWith(Disposables));
            Shell.AddOrSelectDocument(() => new WelcomeScreenViewModel().DisposeWith(Disposables));
        }

        public IShell Shell { get; }

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
