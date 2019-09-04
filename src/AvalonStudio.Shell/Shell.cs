using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using AvalonStudio.Extensibility;
using AvalonStudio.Shell.Extensibility.Platforms;
using Dock.Model;

namespace AvalonStudio.Shell
{
    public static class Shell
	{
        public delegate void ShellAppMainDelegate(string[] args);

        public static void StartShellApp<TAppBuilder>(this TAppBuilder builder, string appName, ShellAppMainDelegate main, string[] args, IFactory layoutFactory = null)  
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
		{
			builder
                .UseReactiveUI()
                .AfterSetup(_ =>
			{
				Platform.AppName = appName;
				Platform.Initialise();

				var extensionManager = new ExtensionManager();
				var container = CompositionRoot.CreateContainer(extensionManager);

				IoC.Initialise(container);

				ShellViewModel.Instance = IoC.Get<ShellViewModel>();

				ShellViewModel.Instance.Initialise(layoutFactory);

                main(args);
			}).StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);
		}
	}
}
