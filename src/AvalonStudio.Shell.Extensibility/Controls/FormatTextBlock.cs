using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.Collections.Generic;
using System;
using System.Reactive.Disposables;

namespace AvalonStudio.Controls
{
    public class FormattedTextBlock : TextBlock, IDisposable
    {
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public FormattedTextBlock()
        {
            this.GetObservable(SpansProperty).Subscribe(spans =>
            {
                if(spans == null)
                {
                    FormattedText.Spans = null;
                }
                else
                {
                    FormattedText.Spans = spans;
                }
            }).DisposeWith(Disposables);

            this.GetObservable(StyledTextProperty).Subscribe(styledText =>
            {
                if(styledText == null)
                {
                    Spans = null;
                    Text = null;
                }
                else
                {
                    Text = styledText.Text;
                    Spans = styledText.Spans;                    
                }
            }).DisposeWith(Disposables);
        }

        public static readonly AvaloniaProperty<StyledText> StyledTextProperty =
            AvaloniaProperty.Register<FormattedTextBlock, StyledText>(nameof(StyledText));

        public StyledText StyledText
        {
            get => GetValue(StyledTextProperty);
            set => SetValue(StyledTextProperty, value);
        }

        public static readonly AvaloniaProperty<IReadOnlyList<FormattedTextStyleSpan>> SpansProperty =
            AvaloniaProperty.Register<FormattedTextBlock, IReadOnlyList<FormattedTextStyleSpan>>(nameof(Spans));

        public IReadOnlyList<FormattedTextStyleSpan> Spans
        {
            get { return GetValue(SpansProperty); }
            set { SetValue(SpansProperty, value); }
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
