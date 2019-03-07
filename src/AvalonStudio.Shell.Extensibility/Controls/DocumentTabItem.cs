using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Reactive.Disposables;

namespace AvalonStudio.Controls
{
    public class DocumentTabItem : ContentControl, IDisposable
    {
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        static DocumentTabItem()
        {
            PseudoClass<DocumentTabItem, bool>(IsFocusedProperty, o => o, ":focused");
            PseudoClass<DocumentTabItem, Avalonia.Controls.Dock>(DockPanel.DockProperty, o => o == Avalonia.Controls.Dock.Right, ":dockright");
            PseudoClass<DocumentTabItem, Avalonia.Controls.Dock>(DockPanel.DockProperty, o => o == Avalonia.Controls.Dock.Left, ":dockleft");
            PseudoClass<DocumentTabItem, Avalonia.Controls.Dock>(DockPanel.DockProperty, o => o == Avalonia.Controls.Dock.Top, ":docktop");
            PseudoClass<DocumentTabItem, Avalonia.Controls.Dock>(DockPanel.DockProperty, o => o == Avalonia.Controls.Dock.Bottom, ":dockbottom");
        }

        public DocumentTabItem()
        {
            this.GetObservable(DockPanel.DockProperty).Subscribe(dock =>
            {
                var parent = Parent;
            }).DisposeWith(Disposables);
        }

        public static readonly AvaloniaProperty<string> TitleProprty =
            AvaloniaProperty.Register<DocumentTabItem, string>(nameof(Title));

        public string Title
        {
            get { return GetValue(TitleProprty); }
            set { SetValue(TitleProprty, value); }
        }

        public static readonly AvaloniaProperty<IBrush> HeaderBackgroundProperty =
            AvaloniaProperty.Register<DocumentTabItem, IBrush>(nameof(HeaderBackground), defaultValue: Brushes.WhiteSmoke);

        public IBrush HeaderBackground
        {
            get { return GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
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