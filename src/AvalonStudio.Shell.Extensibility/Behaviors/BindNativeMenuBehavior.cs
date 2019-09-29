using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace AvalonStudio.Shell.Extensibility.Behaviors
{
    public class BindNativeMenuBehavior : Behavior<NativeMenu>
    {
        private IList<NativeMenuItemBase> _items;

        /// <summary>
        /// Defines the <see cref="Items"/> property.
        /// </summary>
        public static readonly DirectProperty<BindNativeMenuBehavior, IList<NativeMenuItemBase>> ItemsProperty =
            AvaloniaProperty.RegisterDirect<BindNativeMenuBehavior, IList<NativeMenuItemBase>>(nameof(Items), o => o.Items, (o, v) => o.Items = v);

        public IList<NativeMenuItemBase> Items
        {
            get { return _items; }
            set { SetAndRaise(ItemsProperty, ref _items, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.WhenAnyValue(x => x.Items)
                .Subscribe(x =>
                {
                    AssociatedObject.Items.Clear();

                    if (Items != null)
                    {
                        foreach (var item in Items)
                        {
                            AssociatedObject.Items.Add(item);
                        }
                    }
                });
                
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
