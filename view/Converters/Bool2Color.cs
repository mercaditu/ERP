using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Cognitivo.Converters
{
    internal class Bool2Color : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "false")
                return new SolidColorBrush(Colors.Crimson);
            else
                return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}