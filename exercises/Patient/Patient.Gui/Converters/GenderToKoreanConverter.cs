using System;
using System.Globalization;
using Avalonia.Data.Converters;
using PatientApp.Core.Models;

namespace PatientApp.Gui.Converters;

public sealed class GenderToKoreanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Gender g)
        {
            return g == Gender.Male ? "남" : "여";
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            return s == "남" ? Gender.Male : Gender.Female;
        }
        return Gender.Male;
    }
}


