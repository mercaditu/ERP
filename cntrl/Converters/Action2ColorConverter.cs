using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Cognitivo.Converters
{
    public class Action2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            entity.payment_schedual.ActionsStatus ActionsStatus = (entity.payment_schedual.ActionsStatus)value;

            if (ActionsStatus == entity.payment_schedual.ActionsStatus.Red)
            {
                return new SolidColorBrush(Colors.Red);
            }

            return new SolidColorBrush(Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                                System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}