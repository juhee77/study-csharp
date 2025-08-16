using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PatientApp.Gui.Converters;

public sealed class DateOnlyToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateOnly d)
        {
            return d.ToString("yyyy-MM-dd");
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s && DateOnly.TryParseExact(s, "yyyy-MM-dd", out var d))
        {
            return d;
        }
        return default(DateOnly);
    }
}


