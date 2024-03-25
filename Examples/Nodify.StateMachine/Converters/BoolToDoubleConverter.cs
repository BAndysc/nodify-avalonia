using System;
using System.Globalization;

namespace Nodify.StateMachine;

public class BoolToDoubleConverter : IValueConverter
{
    public static BoolToDoubleConverter Instance { get; } = new BoolToDoubleConverter();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? 6.0 : 3.0;
        return 1.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}