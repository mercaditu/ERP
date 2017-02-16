using entity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class ExpireDate2Visible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                item item = (item)value;

                item_product item_product = item.item_product.FirstOrDefault();
                if (item_product != null)
                {
                    if (item_product.can_expire)
                    {
                        return Visibility.Visible;
                    }
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}