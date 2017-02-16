using entity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class OrderTovisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                production_order_detail production_order_detail = value as production_order_detail;

                if (production_order_detail.is_input == false)
                {
                    item_product item_product = production_order_detail.item.item_product.FirstOrDefault();
                    if (item_product != null)
                    {
                        if (item_product.can_expire)
                        {
                            return Visibility.Visible;
                        }
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