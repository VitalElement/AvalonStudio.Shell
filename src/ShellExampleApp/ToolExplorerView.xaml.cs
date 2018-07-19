using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellExampleApp
{
    public class ToolExplorerView : UserControl
    {
        public ToolExplorerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}