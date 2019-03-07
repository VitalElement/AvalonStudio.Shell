using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace AvalonStudio.Extensibility.Dialogs
{
	public class ModalDialogViewModelBase : ReactiveObject, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        private bool cancelButtonVisible;

		private bool isVisible;

		private bool okayButtonVisible;

		private string title;

		private TaskCompletionSource<bool> dialogCloseCompletionSource;

		public ModalDialogViewModelBase(string title, bool okayButton = true, bool cancelButton = true)
		{
			OKButtonVisible = okayButton;
			CancelButtonVisible = cancelButton;

			isVisible = false;
			this.title = title;

			CancelCommand = ReactiveCommand.Create(() => Close(false)).DisposeWith(Disposables);
		}

		public bool CancelButtonVisible
		{
			get { return cancelButtonVisible; }
			set { this.RaiseAndSetIfChanged(ref cancelButtonVisible, value); }
		}

		public bool OKButtonVisible
		{
			get { return okayButtonVisible; }
			set { this.RaiseAndSetIfChanged(ref okayButtonVisible, value); }
		}

		public virtual ReactiveCommand OKCommand { get; protected set; }
		public ReactiveCommand CancelCommand { get; }

		public string Title
		{
			get { return title; }
			private set { this.RaiseAndSetIfChanged(ref title, value); }
		}

		public bool IsVisible
		{
			get { return isVisible; }
			set { this.RaiseAndSetIfChanged(ref isVisible, value); }
		}

		public Task<bool> ShowDialogAsync()
		{
			IsVisible = true;

			dialogCloseCompletionSource = new TaskCompletionSource<bool>();

			return dialogCloseCompletionSource.Task;
		}

		public void Close(bool success = true)
		{
			IsVisible = false;

			dialogCloseCompletionSource.SetResult(success);
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
