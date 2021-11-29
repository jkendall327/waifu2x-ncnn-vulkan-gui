using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace Waifu2xUI.Avalonia.Converters;

public class ComboBoxToIntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (int) value;

        return new ComboBoxItem { Content = val };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (ComboBoxItem) value;

        var content = val.Content.ToString() ?? throw new ArgumentException(null, nameof(value));
        
        return int.Parse(content);
    }
}