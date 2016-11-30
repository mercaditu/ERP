using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class FavoriteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                bool IsFavorite = false;
                IsFavorite = (bool)value;

                if (IsFavorite)
                {
                    return "F";
                }
            }

            return "f";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
