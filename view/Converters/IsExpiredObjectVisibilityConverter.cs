using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class IsExpiredObjectVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            { return Visibility.Hidden; }

            using (entity.db db = new entity.db())
            {
                int item_id = (int)value;
                if (db.item_product.Where(x => x.id_item == item_id).FirstOrDefault() != null)
                {
                    if (db.item_product.Where(x => x.id_item == item_id).FirstOrDefault().can_expire == false)
                    {
                        return Visibility.Hidden;
                    }
                    else
                    {
                        return Visibility.Visible;
                    }
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
