using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{

    public class DecimalFormat: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Int16? DecimalSpaces = System.Convert.ToInt16(value.ToString().Replace("N", ""));

            if (DecimalSpaces != null)
            {
                return DecimalSpaces;
            }

            return "2";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Int16? DecimalSpaces = System.Convert.ToInt16(value);

            if (DecimalSpaces != null)
            {
                return "N" + DecimalSpaces;
            }

            return "N2";
        }
    }
}
