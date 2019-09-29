using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using AvalonStudio.Menus.Models;
using AvalonStudio.Menus.ViewModels;

namespace AvalonStudio.Extensibility.Converters
{
    

    public class NativeMenuConverter : IValueConverter
    {
        public static NativeMenuConverter Instance = new NativeMenuConverter();

        private IList<NativeMenuItemBase> GetNativeItems (IEnumerable<MenuItemModel> items, NativeMenu menu = null)
        {
            var result = new List<NativeMenuItemBase>();

            foreach (var item in items)
            {
                var gesture = item.Gesture;

                var nativeItem = new NativeMenuItem
                {
                    Header = item.Label,
                    Command = item.Command,
                };

                if(gesture != null)
                {
                    nativeItem.Gesture = KeyGesture.Parse(gesture);
                }

                if(nativeItem.Header == null)
                {
                    nativeItem.Header = "";
                }

                if (item.Children != null && item.Children.Any())
                {
                    var nativeMenu = new NativeMenu();
                    GetNativeItems(item.Children, nativeMenu);

                    nativeItem.Menu = nativeMenu;
                }

                if (menu != null)
                {
                    menu.Add(nativeItem);
                }
                else
                {
                    result.Add(nativeItem);
                }
            }

            return result;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<MenuItemModel> mvm)
            {
                return GetNativeItems(mvm);
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
