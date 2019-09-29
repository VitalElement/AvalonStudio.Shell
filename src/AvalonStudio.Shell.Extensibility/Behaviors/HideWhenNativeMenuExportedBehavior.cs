using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using System;
using System.Reactive.Linq;

namespace AvalonStudio.Shell.Extensibility.Behaviors
{
    public class HideWhenNativeMenuExportedBehavior : Behavior<Visual>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            Observable.FromEventPattern(AssociatedObject, nameof(AssociatedObject.AttachedToVisualTree))
                .Take(1)
                .Subscribe(x =>
                {
                    if (AssociatedObject.GetVisualRoot() is TopLevel tl)
                    {
                        if (NativeMenu.GetIsNativeMenuExported(tl))
                        {
                            AssociatedObject.IsVisible = false;
                        }
                    }
                });
        }        
    }
}
