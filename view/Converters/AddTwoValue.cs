using System;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class AddTwoValue : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value[0] != DependencyProperty.UnsetValue && value[1] != DependencyProperty.UnsetValue && value[0] != null && value[1] != null)
            {
                decimal i = (decimal)value[0];
                int j = (int)value[1];
                return (i - j).ToString();
            }
            return 0;

            //throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}