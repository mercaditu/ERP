using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal money_value;
            if (decimal.TryParse(parameter.ToString(), out money_value))
            { }
            else
            {
                return parameter;
            }

            int id_currencyFx;
            if (int.TryParse(value.ToString(), out id_currencyFx))
            { }
            else
            {
                return parameter;
            }

            if (value != null && parameter != null)
            {
                using (entity.db db = new entity.db())
                {
                    decimal rate = 1;
                    rate = db.app_currencyfx.Where(x => x.id_currency == id_currencyFx).FirstOrDefault().buy_value;
                    return money_value / rate;
                }
            }
            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}