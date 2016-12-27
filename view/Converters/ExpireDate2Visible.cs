using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using entity;
namespace Cognitivo.Converters
{
    class ExpireDate2Visible : IValueConverter
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
