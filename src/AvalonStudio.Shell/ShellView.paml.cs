using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvalonStudio.Extensibility.Theme;
using AvalonStudio.GlobalSettings;
using AvalonStudio.Shell.Controls;
using Dock.Avalonia.Controls;

namespace AvalonStudio.Shell
{
	public class ShellView : UserControl
	{
		public ShellView()
		{
			InitializeComponent();

			DataContext = ShellViewModel.Instance;

			ShellViewModel.Instance.Overlay = this.FindControl<Panel>("PART_ExtensibleOverlay");

			KeyBindings.AddRange(ShellViewModel.Instance.KeyBindings);

			var generalSettings = Settings.GetSettings<GeneralSettings>();
			ColorTheme.LoadTheme(generalSettings.Theme);

            this.FindControl<DockControl>("Dock").TemplateApplied += ShellView_TemplateApplied;
		}

        private void ShellView_TemplateApplied(object sender, Avalonia.Controls.Primitives.TemplateAppliedEventArgs e)
        {
        }

        private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
