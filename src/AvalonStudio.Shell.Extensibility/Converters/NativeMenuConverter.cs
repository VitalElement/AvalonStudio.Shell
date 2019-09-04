using System;
using System.Globalization;
using Avalonia.Data.Converters;
using AvalonStudio.Menus.ViewModels;

namespace AvalonStudio.Extensibility.Converters
{
    public class NativeMenuConverter : IValueConverter
    {
        public static NativeMenuConverter Instance = new NativeMenuConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is MenuViewModel mvm)
            {
                //var nativeMenu = new NativeMenu();
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
