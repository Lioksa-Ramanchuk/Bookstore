using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LibApp.Converters
{
    public class InputStrToStrArrayConverter : IValueConverter
    {
        public object? Convert(object value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return (value as string)?.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                     .Where(s => !string.IsNullOrWhiteSpace(s))
                                     .Select(s => s.Trim()).ToArray();
        }

        public object? ConvertBack(object value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return value is IEnumerable<string> tags ? string.Join(';',
                tags.Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim()))
                : null;
        }
    }
}
