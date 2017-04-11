using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class Localize : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string key = value.ToString();

            if (key != string.Empty)
            {
                return entity.Brillo.Localize.StringText(key);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}