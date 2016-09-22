using System;
using System.Windows.Data;


namespace Cognitivo.Converters
{
    class ChartOfAccountSelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int dbvalue = System.Convert.ToInt32(value);
            int paramval = System.Convert.ToInt32(parameter);
            if (dbvalue == paramval)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return parameter;
        }
    }
}
