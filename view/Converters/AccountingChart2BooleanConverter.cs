using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    class AccountingChart2BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object ConverterParameter, System.Globalization.CultureInfo culture)
        {
            if (value!= null)
            {
                System.Data.Entity.EntityState state = (System.Data.Entity.EntityState)value;
                if (state == System.Data.Entity.EntityState.Added || state == System.Data.Entity.EntityState.Modified)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else { return true; }
          
        }

        public object ConvertBack(object value, Type targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
