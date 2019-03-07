using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System;
using System.Reactive.Disposables;

namespace AvalonStudio.Shell.Behaviors
{
    public class HideOnLostFocusBehavior : Behavior<Control>
    {
        private CompositeDisposable Disposables { get; set; }

        private Control _attachedControl;

        static readonly AvaloniaProperty<string> AttachedControlNameProperty = AvaloniaProperty.Register<HideOnLostFocusBehavior, string>(nameof(AttachedControlName));

        public string AttachedControlName
        {
            get { return GetValue(AttachedControlNameProperty); }
            set { SetValue(AttachedControlNameProperty, value); }
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();

            Disposables?.Dispose();
            Disposables = new CompositeDisposable();

            AssociatedObject.AttachedToLogicalTree += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(AttachedControlName))
                {
                    _attachedControl = AssociatedObject.FindControl<Control>(AttachedControlName);

                    if (_attachedControl == null)
                    {
                        throw new Exception($"Control: {AttachedControlName} was not found on the control.");
                    }

                    _attachedControl.GetObservable(Control.IsFocusedProperty).Subscribe(focused =>
                    {
                        if (!focused)
                        {
                            AssociatedObject.IsVisible = false;
                        }
                    }).DisposeWith(Disposables);
                }
                else
                {
                    AssociatedObject.GetObservable(Control.IsFocusedProperty).Subscribe(focused =>
                    {
                        if (!focused)
                        {
                            AssociatedObject.IsVisible = false;
                        }
                    }).DisposeWith(Disposables);
                }
            };
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            Disposables?.Dispose();
        }
    }
}
