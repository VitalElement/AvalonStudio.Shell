using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellExampleApp
{
    public class DocumentView : UserControl
    {
        public DocumentView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}