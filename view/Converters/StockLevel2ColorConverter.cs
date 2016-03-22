using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;


namespace Cognitivo.Converters
{
    public class StockLevel2ColorConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                decimal current = (decimal)value;
                decimal min_quantity = (decimal)parameter;

                if (current < min_quantity)
                {
                    return Brushes.Crimson;
                }
                else if (current == min_quantity)
                {
                    return Brushes.Gold;
                }
                else
                {
                    return Brushes.Black;
                }
            }
            else { return Brushes.WhiteSmoke; }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                                System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
