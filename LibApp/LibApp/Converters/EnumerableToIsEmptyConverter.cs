using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LibApp.Converters
{
    public class EnumerableToIsEmptyConverter : IValueConverter
    {
        public object? Convert(object value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return !(value as IEnumerable<object>)?.Any() ?? true;
        }

        public object? ConvertBack(object value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return null;
        }
    }
}
