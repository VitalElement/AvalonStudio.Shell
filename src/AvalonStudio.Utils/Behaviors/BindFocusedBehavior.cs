using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AvalonStudio.Utils.Behaviors
{
    class BindFocusedBehavior : Behavior<Control>
    {
        private CompositeDisposable _disposables;

        protected override void OnAttached()
        {
            base.OnAttached();

            _disposables = new CompositeDisposable {
            this.GetObservable(IsFocusedProperty).Subscribe(focused =>
            {
                if(focused)
                {
                    AssociatedObject.Focus();
                }
            }),
            Observable.FromEventPattern(AssociatedObject, nameof(AssociatedObject.LostFocus)).Subscribe(_ =>
            {
                IsFocused = false;
            }),
            Observable.FromEventPattern(AssociatedObject, nameof(AssociatedObject.GotFocus)).Subscribe(_ =>
            {
                IsFocused = true;
            })};
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            _disposables.Dispose();
        }

        public static readonly StyledProperty<bool> IsFocusedProperty =
            AvaloniaProperty.Register<BindFocusedBehavior, bool>(nameof(IsFocused), defaultBindingMode: BindingMode.TwoWay);

        public bool IsFocused
        {
            get => GetValue(IsFocusedProperty);
            set => SetValue(IsFocusedProperty, value);
        }
    }
}
