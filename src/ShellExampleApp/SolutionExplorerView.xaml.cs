using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellExampleApp
{
    public class SolutionExplorerView : UserControl
    {
        public SolutionExplorerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}