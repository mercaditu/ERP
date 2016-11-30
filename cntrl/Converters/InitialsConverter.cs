using System;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class InitialsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //throw new NotImplementedException();
            string str = value as string;
            if (str != null)
            {
                MatchCollection matches = Regex.Matches(str, @"(\b\w)");
                foreach (Match m in matches)
                {
                    return m.Value;
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
