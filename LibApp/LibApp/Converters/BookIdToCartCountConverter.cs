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
    public class BookIdToCartCountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type? targetType, object? parameter, CultureInfo? culture)
        {
            var (userId, bookId) = ((long)values[0], (long)values[1]);
            using var db = new DbUnit();
            return db.Carts.GetAll().FirstOrDefault(c => c.IdClient == userId && c.IdBook == bookId)?.Count ?? 0;
        }

        public object[] ConvertBack(object value, Type[]? targetTypes, object? parameter, CultureInfo? culture)
        {
            return Array.Empty<object>();
        }
    }
}
