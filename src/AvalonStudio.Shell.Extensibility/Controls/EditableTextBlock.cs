using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Threading;
using System;
using System.Reactive.Disposables;

namespace AvalonStudio.Controls
{
    public class EditableTextBlock : TemplatedControl, IDisposable
    {
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        private string _text;
        private string _editText;
        private TextBox _textBox;
        private DispatcherTimer _editClickTimer;

        static EditableTextBlock()
        {
            PseudoClass<EditableTextBlock>(InEditModeProperty, ":editing");
        }

        public EditableTextBlock()
        {
            _editClickTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500),                
            };

            _editClickTimer.Tick += _editClickTimer_Tick;

            this.GetObservable(TextProperty).Subscribe(t =>
            {
                EditText = t;
            }).DisposeWith(Disposables);

            this.GetObservable(InEditModeProperty).Subscribe(mode =>
            {
                if (mode && _textBox != null)
                {
                    EnterEditMode();
                }
            }).DisposeWith(Disposables);

            AddHandler(PointerPressedEvent, (sender, e)=>
            {
                _editClickTimer.Stop();

                if (!InEditMode)
                {                    
                    if (e.ClickCount == 1 && e.InputModifiers == InputModifiers.LeftMouseButton && IsFocused)
                    {
                        _editClickTimer.Start();
                    }                    
                }
                else
                {
                    var hit = this.InputHitTest(e.GetPosition(this));

                    if (hit == null)
                    {
                        ExitEditMode();
                    }
                }
            }, RoutingStrategies.Tunnel).DisposeWith(Disposables);
        }

        private void _editClickTimer_Tick(object sender, EventArgs e)
        {
            _editClickTimer.Stop();

            if (IsFocused && !InEditMode)
            {
                EnterEditMode();
            }
        }

        public static readonly DirectProperty<EditableTextBlock, string> TextProperty = TextBlock.TextProperty.AddOwner<EditableTextBlock>(
                o => o.Text,
                (o, v) => o.Text = v,
                defaultBindingMode: BindingMode.TwoWay,
                enableDataValidation: true);

        [Content]
        public string Text
        {
            get { return _text; }
            set
            {
                SetAndRaise(TextProperty, ref _text, value);
            }
        }

        public string EditText
        {
            get => _editText;
            set
            {
                SetAndRaise(EditTextProperty, ref _editText, value);
            }
        }

        public static readonly DirectProperty<EditableTextBlock, string> EditTextProperty =
                AvaloniaProperty.RegisterDirect<EditableTextBlock, string>(nameof(EditText), o => o.EditText, (o, v) => o.EditText = v);

        public static readonly StyledProperty<bool> InEditModeProperty =
            AvaloniaProperty.Register<EditableTextBlock, bool>(nameof(InEditMode), defaultBindingMode: BindingMode.TwoWay);

        public bool InEditMode
        {
            get { return GetValue(InEditModeProperty); }
            set { SetValue(InEditModeProperty, value); }
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);

            _textBox = e.NameScope.Find<TextBox>("PART_TextBox");

            if (InEditMode)
            {
                EnterEditMode();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    ExitEditMode();
                    e.Handled = true;
                    break;

                case Key.Escape:
                    ExitEditMode(true);
                    e.Handled = true;
                    break;
            }

            base.OnKeyUp(e);
        }

        private void EnterEditMode()
        {
            EditText = Text;
            InEditMode = true;
            (VisualRoot as IInputRoot).MouseDevice.Capture(_textBox);
            _textBox.CaretIndex = Text.Length;
            _textBox.SelectionStart = 0;
            _textBox.SelectionEnd = Text.Length;

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _textBox.Focus();
            });
        }

        private void ExitEditMode(bool restore = false)
        {
            if (!restore)
            {
                Text = EditText;
            }

            InEditMode = false;
            (VisualRoot as IInputRoot).MouseDevice.Capture(null);
        }

        #region IDisposable Support
        private volatile bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _editClickTimer.Tick -= _editClickTimer_Tick;
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
