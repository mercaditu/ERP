using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class Byte2Opacity : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "1")
                return (decimal)0.75;
            else //if ((int)value == 0)
                return (decimal)0.20;
        }
    }
}