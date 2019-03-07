using Avalonia.Controls;
using System.Reactive.Disposables;

namespace AvalonStudio.Utils.Behaviors
{
    public class CommandOnDoubleClickBehavior : CommandBasedBehavior<Control>
    {
        private CompositeDisposable Disposables { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();

            Disposables?.Dispose();
            Disposables = new CompositeDisposable
            {
                AssociatedObject.AddHandler(Control.PointerPressedEvent, (sender, e) =>
                {
                    if (e.ClickCount == 2)
                    {
                        e.Handled = ExecuteCommand();
                    }
                })
            };
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            Disposables?.Dispose();
        }
    }
}
