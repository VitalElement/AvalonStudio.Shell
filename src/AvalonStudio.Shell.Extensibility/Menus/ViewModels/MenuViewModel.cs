using AvalonStudio.Menus.Models;
using ReactiveUI;
using System.Collections.Immutable;

namespace AvalonStudio.Menus.ViewModels
{
	public class MenuViewModel : ReactiveObject
	{
		private bool _isVisible = true;

		public IImmutableList<MenuItemModel> Items { get; }

		public bool IsVisible
		{
			get => _isVisible;
			set => this.RaiseAndSetIfChanged(ref _isVisible, value);
		}

		public MenuViewModel(IImmutableList<MenuItemModel> items)
		{
			Items = items;
		}
	}
}
