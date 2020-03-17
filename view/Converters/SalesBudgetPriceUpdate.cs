using System;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class SalesBudgetPriceUpdate : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            if (value[0]!= DependencyProperty.UnsetValue)
            {
                if ((int)value[0] > 0 && (bool)value[1] == false)
                {
                    return true;
                }
                else
                { return false; }
            }

            return false;

            //throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}