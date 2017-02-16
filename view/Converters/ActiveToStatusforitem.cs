using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class ActiveToStatusforitem : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == true.ToString())
                return entity.Status.Documents_General.Pending.ToString();
            else
                return entity.Status.Documents_General.Annulled.ToString();
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}