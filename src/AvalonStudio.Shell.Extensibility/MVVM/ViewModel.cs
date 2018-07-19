using ReactiveUI;

namespace AvalonStudio.MVVM
{
    public enum Location
    {
        None,
        Left,
        Right,
        Bottom,
    }

    public abstract class ViewModel : ViewModel<object>
    {
        protected ViewModel() : base(null)
        {
        }
    }
}