using System;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class ItemProduct2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int v = (int)value;
                if (v == 1 || v == 4)
                {
                    v = 0;
                }

                if (v.ToString() == parameter.ToString())
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (value == parameter)
                {
                    return Visibility.Collapsed;
                }
            }
            return Visibility.Visible;
        }
    }
}