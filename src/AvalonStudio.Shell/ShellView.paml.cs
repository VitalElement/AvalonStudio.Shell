using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvalonStudio.Extensibility.Theme;
using AvalonStudio.GlobalSettings;
using AvalonStudio.Shell.Controls;

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
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
