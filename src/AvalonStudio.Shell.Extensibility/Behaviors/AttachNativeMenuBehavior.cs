using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using System;
using System.Reactive.Linq;

namespace AvalonStudio.Shell.Extensibility.Behaviors
{
    public class AttachNativeMenuBehavior : Behavior<Visual>
    {
        private NativeMenu _menu;

        /// <summary>
        /// Defines the <see cref="Menu"/> property.
        /// </summary>
        public static readonly DirectProperty<AttachNativeMenuBehavior, NativeMenu> MenuProperty =
            AvaloniaProperty.RegisterDirect<AttachNativeMenuBehavior, NativeMenu>(nameof(Menu), o => o.Menu, (o, v) => o.Menu = v);

        public NativeMenu Menu
        {
            get { return _menu; }
            set { SetAndRaise(MenuProperty, ref _menu, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            Observable.FromEventPattern(AssociatedObject, nameof(AssociatedObject.AttachedToVisualTree))
                .Take(1)
                .Subscribe(x =>
                {
                    if (Menu != null)
                    {
                        if (AssociatedObject.GetVisualRoot() is TopLevel tl)
                        {
                            NativeMenu.SetMenu(tl, Menu);
                        }
                    }
                });
        }
    }
}
