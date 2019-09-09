using AvalonStudio.Shell;
using Dock.Model;
using Dock.Model.Controls;

namespace AvalonStudio.Docking
{
	public class AvalonStudioDocumentDock : DocumentDock
    {
        public override bool OnClose()
        {
            ShellViewModel.Instance.RemoveDock(this);
            return base.OnClose();
        }

        public override IDockable Clone()
        {
            var result = base.Clone() as AvalonStudioDocumentDock;

            result.VisibleDockables = this.VisibleDockables;

            return result;
        }
    }
}
