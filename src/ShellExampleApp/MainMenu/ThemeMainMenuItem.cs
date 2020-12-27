using AvalonStudio.MainMenu;
using AvalonStudio.Menus;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace ShellExampleApp.MainMenu
{
    internal class ThemeMainMenuItems
    {
        [ExportMainMenuItem("Theme")]
        [DefaultOrder(2000)]
        public IMenuItem Help => _menuItemFactory.CreateHeaderMenuItem("Theme", null);

        [ExportMainMenuDefaultGroup("Theme", "Light")]
        [DefaultOrder(0)]
        public object LightGroup => null;

        [ExportMainMenuDefaultGroup("Theme", "Dark")]
        [DefaultOrder(0)]
        public object DarkGroup => null;

        [ExportMainMenuItem("Theme", "Light")]
        [DefaultOrder(0)]
        [DefaultGroup("Light")]
        public IMenuItem Light => _menuItemFactory.CreateCommandMenuItem("Theme.Light");

        [ExportMainMenuItem("Theme", "Dark")]
        [DefaultOrder(0)]
        [DefaultGroup("Light")]
        public IMenuItem Dark => _menuItemFactory.CreateCommandMenuItem("Theme.Dark");

        private IMenuItemFactory _menuItemFactory;

        [ImportingConstructor]
        public ThemeMainMenuItems(IMenuItemFactory menuItemFactory)
        {
            _menuItemFactory = menuItemFactory;
        }
    }
}
