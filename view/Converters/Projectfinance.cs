using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class Projectfinance : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (System.Convert.ToBoolean(value[0]) != true)
            {
                if (value[2] != null)
                {
                    return value[2].ToString();
                }
            }

            return value[1].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            object[] a = new object[3];
            a[0] = true;
            a[1] = value;
            a[2] = value;
            return a;
            //throw new NotSupportedException("Cannot convert back");
        }
    }
}