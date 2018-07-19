using AvalonStudio.Toolbars;
using AvalonStudio.Menus;
using System.Composition;

namespace ShellExampleApp.Toolbars
{
    internal class StandardToolbar
    {
        [ExportToolbar("Standard")]
        public Toolbar Standard => new Toolbar("Standard");

        [ExportToolbarDefaultGroup("Standard", "Help")]
        [DefaultOrder(100)]
        public object BuildGroup => null;

        [ExportToolbarItem("Standard", "About")]
        [DefaultGroup("About")]
        [DefaultOrder(50)]
        public IMenuItem Clean => _menuItemFactory.CreateCommandMenuItem("Help.About");

        private readonly IMenuItemFactory _menuItemFactory;

        [ImportingConstructor]
        public StandardToolbar(IMenuItemFactory menuItemFactory)
        {
            _menuItemFactory = menuItemFactory;
        }
    }
}
