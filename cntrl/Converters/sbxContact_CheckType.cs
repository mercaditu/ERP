using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace cntrl.Converters
{
    class sbxContact_CheckType : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //string val = (string)value;


            //if (val.Contains("Code") && (int)parameter == 1)
            //{
            //    return true;
            //}

            //if (val.Contains("Name") && (int)parameter == 2)
            //{
            //    return true;
            //}

            //if (val.Contains("GovID") && (int)parameter == 3)
            //{
            //    return true;
            //}

            //if (val.Contains("Tel") && (int)parameter == 4)
            //{
            //    return true;
            //}

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
