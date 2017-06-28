using System;

namespace Cognitivo.Converters
{


    public class DecimalFormat
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var Decimal = value as string;

            if (Decimal != null)
            {
                return "N" + Decimal;
            }

            return "N2";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var Decimal = value.ToString().Replace("N", "");

            if (Decimal != null)
            {
                return Decimal;
            }

            return "2";
        }
    }
}
