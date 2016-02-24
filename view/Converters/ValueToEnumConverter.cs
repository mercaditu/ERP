using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    class ValueToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //throw new NotImplementedException();
            return (Cognitivo.Configs.KeyGestureSettings.MyKeys)Enum.Parse(typeof(Cognitivo.Configs.KeyGestureSettings.MyKeys), value.ToString(), true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //throw new NotImplementedException();
            return value.ToString();
        }
    }
}
