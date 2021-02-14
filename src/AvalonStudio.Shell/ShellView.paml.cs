using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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

			ColorTheme.LoadTheme(ColorTheme.VisualStudioLight);
		}

        private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            Focus();
        }
    }
}
