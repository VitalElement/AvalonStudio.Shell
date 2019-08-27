using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvalonStudio.Extensibility.Theme;
using AvalonStudio.Shell;
using System;

namespace ShellExampleApp
{
    public class App : Application
    {
        [STAThread]
        private static void Main(string[] args)
        {
            BuildAvaloniaApp().StartShellApp("ShellExampleApp", AppMain, args);            
        }

        private static void AppMain(string[] args)
        {
            Dispatcher.UIThread.InvokeAsync(() => { ColorTheme.LoadTheme(ColorTheme.VisualStudioLight); });
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>().UsePlatformDetect();

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}