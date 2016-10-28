using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    class ProuctionPending2BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object ConverterParameter, System.Globalization.CultureInfo culture)
        {
            if (value!= null)
            {
                entity.Status.Production status = (entity.Status.Production)value;
                if (status == entity.Status.Production.Pending)
                {
                    return true;
                }
            }
            return false;
          
        }

        public object ConvertBack(object value, Type targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
