using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class CustomStringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var mask = parameter as string;
            var inString = value as string;

            if (mask != null & inString != null)
            {
                MaskedTextProvider mtp = new MaskedTextProvider(mask);

                if (mtp.Set(inString))
                {
                    return mtp.ToDisplayString();
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}