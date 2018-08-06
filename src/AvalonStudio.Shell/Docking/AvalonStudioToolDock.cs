using AvalonStudio.Shell;
using Dock.Model.Controls;

namespace AvalonStudio.Docking
{
    public class AvalonStudioToolDock : ToolDock
    {
        public override bool OnClose()
        {
            ShellViewModel.Instance.RemoveDock(this);
            return base.OnClose();
        }
    }
}
