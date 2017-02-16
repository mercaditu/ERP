using entity;
using System;
using System.Windows.Data;
using WPFLocalizeExtension.Extensions;

namespace Cognitivo.Converters
{
    internal class BooleanToIsStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int _value = (int)value;
            if ((int)Status.Documents_General.Approved == _value)
            {
                return LocExtension.GetLocalizedValue<string>("Cognitivo:local:" + "Approve"); ;
            }
            else
            {
                return LocExtension.GetLocalizedValue<string>("Cognitivo:local:" + "Pending"); ;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}