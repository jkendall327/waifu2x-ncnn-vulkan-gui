using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Waifu2x_UI.Core;

namespace Waifu2xUI.Avalonia.Converters;

public class BoolToEnumConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (OutputFileType)value;

        return val switch
        {
            OutputFileType.Jpeg => false,
            OutputFileType.Png => true,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (bool)value;

        return val ? OutputFileType.Png : OutputFileType.Jpeg;
    }
}