using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class Enum2ArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Add Localization code for translation here...

            return Enum.GetValues(value as Type);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}