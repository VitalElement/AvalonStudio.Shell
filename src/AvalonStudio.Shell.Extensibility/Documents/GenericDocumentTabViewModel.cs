﻿using AvalonStudio.Documents;
using AvalonStudio.Extensibility;
using AvalonStudio.MVVM;
using AvalonStudio.Shell;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace AvalonStudio.Controls
{
    public abstract class DocumentTabViewModel<T> : ViewModel<T>, IDocumentTabViewModel, IDisposable where T : class
    {
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        private Avalonia.Controls.Dock dock;
		private string _title;
		private bool _isTemporary;
		private bool _isHidden;
		private bool _isSelected;
        private bool _isDirty;
        private bool _isReadOnly;

        public DocumentTabViewModel(T model) : base(model)
		{
			Dock = Avalonia.Controls.Dock.Left;

			IsVisible = true;

            this.WhenAnyValue(x => x.IsDirty).Subscribe(dirty =>
            {
                this.RaisePropertyChanged(nameof(Title));
            }).DisposeWith(Disposables);
        }

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                this.RaiseAndSetIfChanged(ref _isDirty, value);

                if (value && IsTemporary)
                {
                    IsTemporary = false;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { this.RaiseAndSetIfChanged(ref _isReadOnly, value); }
        }

        public Avalonia.Controls.Dock Dock
		{
			get { return dock; }
			set { this.RaiseAndSetIfChanged(ref dock, value); }
		}

		public string Title
		{
			get => IsDirty ? _title + "*" : _title;
			set=> this.RaiseAndSetIfChanged(ref _title, value);
		}

		public bool IsTemporary
		{
			get
			{
				return _isTemporary;
			}
			set
			{
				if (value)
				{
					Dock = Avalonia.Controls.Dock.Right;
				}
				else
				{
					Dock = Avalonia.Controls.Dock.Left;
				}

				this.RaiseAndSetIfChanged(ref _isTemporary, value);
			}
		}

		public bool IsVisible
		{
			get { return _isHidden; }
			set { this.RaiseAndSetIfChanged(ref _isHidden, value); }
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
		}

		public virtual void Close()
		{
			IoC.Get<IShell>().RemoveDocument(this);
		}

		public virtual void OnSelected()
		{
		}

		public virtual void OnDeselected()
		{
		}

        public virtual bool OnClose()
        {
            IoC.Get<IShell>().RemoveDocument(this);
            return true;
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
