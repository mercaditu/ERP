using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    class IsProductObjectVisibilityConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            using (entity.db db = new entity.db())
            {
                int item_id = (int)value;
                if (db.item_product.Where(x => x.id_item == item_id).FirstOrDefault() != null)
                {
                    return Visibility.Hidden;
                   
                }
                else
                {
                    return Visibility.Visible;
                }
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
