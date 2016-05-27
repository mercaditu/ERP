
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    class AccountChartSubType2BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value != null)
                {
                    entity.accounting_chart.ChartSubType Value_ChartSubType = (entity.accounting_chart.ChartSubType)value;
                    entity.accounting_chart.ChartSubType Parameter_ChartSubType = (entity.accounting_chart.ChartSubType)System.Convert.ToInt32(parameter);

                    if (Value_ChartSubType > 0 && Parameter_ChartSubType > 0)
                    {
                        if (Value_ChartSubType == Parameter_ChartSubType)
                        {
                            return true;
                        }
                    }
                }
            }
            catch { }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
