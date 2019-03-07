using Avalonia.Controls;
using System.Reactive.Disposables;

namespace AvalonStudio.Utils.Behaviors
{
    public class CommandOnEnterBehavior : CommandBasedBehavior<TextBox>
    {
        private CompositeDisposable Disposables { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();

            Disposables?.Dispose();
            Disposables = new CompositeDisposable
            {
                AssociatedObject.AddHandler(TextBox.KeyDownEvent, (sender, e) =>
                {
                    if (e.Key == Avalonia.Input.Key.Enter)
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
