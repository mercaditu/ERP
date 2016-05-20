
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class Projectfinance : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (System.Convert.ToBoolean(value[0])!=true)
            {
                return value[2];
            }
            else
            {
                return value[1];
            }
            
          
            //throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
           object[] a= new object[3];
            a[0]=true;
            a[1]=0;
            a[2] = 0;
            return a;
            //throw new NotSupportedException("Cannot convert back");
        }
    }
}

