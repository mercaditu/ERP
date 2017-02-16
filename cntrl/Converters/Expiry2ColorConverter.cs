using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Cognitivo.Converters
{
    public class Expiry2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime ExpiryDate = System.Convert.ToDateTime(value);

            if (ExpiryDate < DateTime.Now)
            {
                return new SolidColorBrush(Colors.Crimson);
            }
            else if (ExpiryDate < DateTime.Now.AddDays(10))
            {
                return new SolidColorBrush(Colors.Orange);
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                                System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}