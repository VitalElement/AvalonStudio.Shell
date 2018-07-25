using Dock.Model;

namespace AvalonStudio.Shell
{
    public static class DockExtensions
    {
        public static void Dock(this IDock dock, IView view, bool add = true)
        {
            if (add)
            {
                dock.Views.Add(view);
                dock.Factory.Update(view, view, dock);
            }
            else
            {
                dock.Factory.Update(view, view, view.Parent);
            }

            dock.Factory.SetCurrentView(view);
        }
    }
}
