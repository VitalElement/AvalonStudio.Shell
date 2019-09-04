using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AvalonStudio.Utils.Behaviors
{
    public class PointerWheelValueBehavior : Behavior<Control>
    {
        private CompositeDisposable _disposables;

        protected override void OnAttached()
        {
            base.OnAttached();

            _disposables = new CompositeDisposable
            {
                AssociatedObject.AddHandler(Control.PointerWheelChangedEvent, (sender, e) =>
                {
                    if (e.KeyModifiers == KeyModifiers)
                    {
                        e.Handled = true;

                        var newValue = Value + (Math.Round(e.Delta.Y) * Scale);

                        if (newValue < MinValue)
                        {
                            newValue = MinValue;
                        }
                        else if (newValue > MaxValue)
                        {
                            newValue = MaxValue;
                        }

                        Value = newValue;
                    }
                }, Avalonia.Interactivity.RoutingStrategies.Tunnel)                
            };
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            _disposables.Dispose();
        }

        public static readonly StyledProperty<double> MinValueProperty = AvaloniaProperty.Register<PointerWheelValueBehavior, double>(nameof(MinValue), 0);

        public double MinValue
        {
            get => GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public static readonly StyledProperty<double> MaxValueProperty = AvaloniaProperty.Register<PointerWheelValueBehavior, double>(nameof(MaxValue), 100);

        public double MaxValue
        {
            get => GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly StyledProperty<double> ValueProperty = AvaloniaProperty.Register<PointerWheelValueBehavior, double>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

        public double Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly StyledProperty<double> ScaleProperty = AvaloniaProperty.Register<PointerWheelValueBehavior, double>(nameof(Scale), 1);

        public double Scale
        {
            get => GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public static readonly StyledProperty<KeyModifiers> KeyModifiersProperty = AvaloniaProperty.Register<PointerWheelValueBehavior, KeyModifiers>(nameof(KeyModifiers), KeyModifiers.None);

        public KeyModifiers KeyModifiers
        {
            get => GetValue(KeyModifiersProperty);
            set => SetValue(KeyModifiersProperty, value);
        }
    }
}
