using System;
using System.Windows.Media;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class Type2Enable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                entity.production_order_detail production_order_detail = (entity.production_order_detail)value;
                if (production_order_detail.item!=null)
                {
                    entity.item.item_type status = production_order_detail.item.id_item_type;

                    if (status != entity.item.item_type.Task && production_order_detail.status == entity.Status.Production.Approved)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            else { return false; }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                                System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
