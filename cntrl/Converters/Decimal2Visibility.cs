using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class Decimal2Visibility : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string number = (string)value;
            if (value != null)
            {
                try
                {
                    value = new string(number.Where(c => char.IsDigit(c)).ToArray());
                    decimal Amount = System.Convert.ToDecimal(value);
                    if (Amount > 0)
                    { return Visibility.Visible; }
                }
                catch (Exception ex)
                { System.Windows.Forms.MessageBox.Show(ex.ToString()); }
            }
            return Visibility.Collapsed;
        }
    }
}