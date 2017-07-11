using System;
using System.Windows.Data;

namespace cntrl.Converters
{
    internal class sbxItem_CheckType : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Collections.Specialized.StringCollection val = (System.Collections.Specialized.StringCollection)value;

            if (val.IndexOf("Code") > 0 && (int)parameter == 1)
            {
                return true;
            }

            if (val.IndexOf("Name") > 0 && (int)parameter == 2)
            {
                return true;
            }

            if (val.IndexOf("Tag") > 0 && (int)parameter == 3)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
}