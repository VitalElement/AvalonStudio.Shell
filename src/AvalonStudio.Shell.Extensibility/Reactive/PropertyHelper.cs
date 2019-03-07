using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace AvalonStudio.Extensibility.Reactive
{
    public class PropertyHelper<TRet> : IDisposable
    {
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public PropertyHelper(IReactiveObject source, IObservable<TRet> observable, string propertyName)
        {
            observable.Subscribe(
                v =>
                {
                    source.RaisePropertyChanging(propertyName);
                    Value = v;
                    source.RaisePropertyChanged(propertyName);
                }).DisposeWith(Disposables);
        }

        public TRet Value { get; set; }

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