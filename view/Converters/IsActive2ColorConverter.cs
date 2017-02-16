using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Cognitivo.Converters
{
    public class IsActive2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                                            System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == true.ToString())
            {
                return Brushes.PaleGreen;
            }
            else
            {
                return Brushes.Crimson;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                                System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}