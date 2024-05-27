using System;
using System.Globalization;
using System.Windows.Data;

namespace LibApp.Converters
{
    public class NumberUpDownValueConverter : IValueConverter
    {
        public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return value ?? parameter;
        }

        public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return value ?? parameter;
        }
    }
}
