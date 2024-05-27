using LibApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LibApp.Converters
{
    public class OrderStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return (value as OrderStatus)?.Name switch
            {
                Constants.ProcessingOrderStatusName => "у апрацоўцы",
                Constants.ShippedOrderStatusName => "адпраўлены",
                Constants.DeliveredOrderStatusName => "атрыманы",
                Constants.CanceledOrderStatusName => "скасаваны",
                Constants.ReturnedOrderStatusName => "вернуты",
                _ => "статус невядомы"
            };
        }

        public object? ConvertBack(object value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return null;
        }
    }
}
