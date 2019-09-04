using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using AvalonStudio.Menus.Models;
using AvalonStudio.Menus.ViewModels;

namespace AvalonStudio.Extensibility.Converters
{
    public class NativeMenuConverter : IValueConverter
    {
        public static NativeMenuConverter Instance = new NativeMenuConverter();

        private List<NativeMenuItem> GetNativeItems (IEnumerable<MenuItemModel> items)
        {
            var result = new List<NativeMenuItem>();

            foreach (var item in items)
            {
                var nativeItem = new NativeMenuItem
                {
                    Header = item.Label,
                    Command = item.Command,
                    Gesture = item.Gesture
                };

                if(nativeItem.Header == null)
                {
                    nativeItem.Header = "";
                }

                if(item.Children != null && item.Children.Any())
                {
                    nativeItem.Items = GetNativeItems(item.Children);
                }

                result.Add(nativeItem);
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
