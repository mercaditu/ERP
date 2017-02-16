using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class Boolean2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == true.ToString())
                return "Yes";
            else
                return "No";
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}