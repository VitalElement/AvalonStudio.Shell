using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellExampleApp
{
    public class WelcomeScreenView : UserControl
    {
        public WelcomeScreenView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}