using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class Text2Visibility : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string str = (string)value;
                int count = str.Count();
                if (count > 0)
                { return Visibility.Visible; }
            }
            return Visibility.Collapsed;
        }
    }
}