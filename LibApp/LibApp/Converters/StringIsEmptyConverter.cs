using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LibApp.Converters
{
    public class StringIsEmptyConverter : IValueConverter
    {
        public object? Convert(object value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return (value as string)?.IsNullOrEmpty() ?? true;
        }

        public object? ConvertBack(object value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return null;
        }
    }
}
