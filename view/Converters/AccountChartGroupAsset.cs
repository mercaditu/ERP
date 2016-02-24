
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    class AccountChartGroupAsset:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value!=null)
            {
                if (value.ToString() == entity.accounting_chart.ChartType.Asset.ToString())
                    return Visibility.Visible;
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else { return Visibility.Collapsed; }
         
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
