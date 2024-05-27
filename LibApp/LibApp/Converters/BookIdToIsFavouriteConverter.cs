using LibApp.Repositories;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LibApp.Converters
{
    public class BookIdToIsFavouriteConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type? targetType, object? parameter, CultureInfo? culture)
        {
            var (userId, bookId) = ((long)values[0], (long)values[1]);
            using var db = new DbUnit();
            return db.Favourites.IsFavourite(userId, bookId);
        }

        public object[] ConvertBack(object value, Type[]? targetTypes, object? parameter, CultureInfo? culture)
        {
            return Array.Empty<object>();
        }
    }
}
