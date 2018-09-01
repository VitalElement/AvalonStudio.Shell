﻿using Avalonia;
using Avalonia.Controls;
using AvalonStudio.Extensibility;
using AvalonStudio.Shell.Extensibility.Platforms;
using Dock.Model;
using System;

namespace AvalonStudio.Shell
{
	public static class Shell
	{
		public static void StartShellApp<TAppBuilder, TMainWindow>(this TAppBuilder builder, string appName, IDockFactory layoutFactory = null, Func<object> dataContextProvider = null) where TAppBuilder : AppBuilderBase<TAppBuilder>, new() where TMainWindow : Window, new()
		{
			builder.UseReactiveUI().AfterSetup(_ =>
			{
				Platform.AppName = appName;
				Platform.Initialise();

				var extensionManager = new ExtensionManager();
                // we shouldn't do this, instead a progress dialog should be shown
				var container = CompositionRoot.CreateExportProviderAsync(extensionManager).Result;

				IoC.Initialise(container);

				ShellViewModel.Instance = IoC.Get<ShellViewModel>();

				ShellViewModel.Instance.Initialise(layoutFactory);
			}).Start<TMainWindow>(dataContextProvider);
		}
	}
}
