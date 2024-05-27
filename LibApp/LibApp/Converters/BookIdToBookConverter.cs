using LibApp.Models;
using LibApp.Repositories;
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
    public class BookIdToBookConverter : IValueConverter
    {
        public object? Convert(object value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (value is not long id)
            {
                return null;
            }
            using var db = new DbUnit();

            return Task.Run(() => db.Books.Get(id)).Result;
        }

        public object? ConvertBack(object value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return (value as Book)?.Id;
        }
    }
}
