using System;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class NotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool is_input = !(bool)value;
            if (is_input.ToString() == true.ToString())
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}