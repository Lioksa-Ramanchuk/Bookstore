using LibApp.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LibApp.Converters
{
    public class BookIdToIsInCartConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type? targetType, object? parameter, CultureInfo? culture)
        {
            var (userId, bookId) = ((long)values[0], (long)values[1]);
            using var db = new DbUnit();
            return db.Carts.IsInCart(userId, bookId);
        }

        public object[] ConvertBack(object value, Type[]? targetTypes, object? parameter, CultureInfo? culture)
        {
            return Array.Empty<object>();
        }
    }
}
