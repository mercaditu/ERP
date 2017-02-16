using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class BoolToActiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == true.ToString())
            {
                return "Active";
            }
            else
            {
                return "Inactive";
            }
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class BoolToActiveReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == true.ToString())
            {
                return "Inactive";
            }
            else
            {
                return "Active";
            }
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}