using Avalonia.Threading;
using AvalonStudio.Commands;
using AvalonStudio.Extensibility;
using AvalonStudio.Extensibility.Theme;
using AvalonStudio.GlobalSettings;
using AvalonStudio.Shell;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace ShellExampleApp.Commands
{
    internal class ThemeCommands
    {
        [ExportCommandDefinition("Theme.Light")]
        public CommandDefinition LightCommand { get; }

        [ExportCommandDefinition("Theme.Dark")]
        public CommandDefinition DarkCommand { get; }

        private IShell _shell;

        [ImportingConstructor]
        public ThemeCommands(CommandIconService commandIconService)
        {
            _shell = IoC.Get<IShell>();

            LightCommand = new CommandDefinition("Light", null, ReactiveCommand.Create(SetThemeLight));
            DarkCommand = new CommandDefinition("Dark", null, ReactiveCommand.Create(SetThemeDark));
        }

        public static void SetThemeLight()
        {
            Settings.SetSettings(new GeneralSettings() { Theme = ColorTheme.VisualStudioLight.Name });
            ColorTheme.LoadTheme(ColorTheme.VisualStudioLight);
        }

        public static void SetThemeDark()
        {
            Settings.SetSettings(new GeneralSettings() { Theme = ColorTheme.VisualStudioDark.Name });
            ColorTheme.LoadTheme(ColorTheme.VisualStudioDark);
        }
    }
}
