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
                entity.item item = (entity.item)value;
                entity.item.item_type status =item.id_item_type;

                if (status != entity.item.item_type.Task)
                {
                    return true;
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
