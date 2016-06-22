using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace cntrl.Converters
{
    class sbxItem_CheckType : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //System.Collections.Specialized.StringCollection val = (System.Collections.Specialized.StringCollection)value;


            //if (val.Contains("Code") && (int)parameter == 1)
            //{
            //    return true;
            //}

            //if (val.Contains("Name") && (int)parameter == 2)
            //{
            //    return true;
            //}



            //if (val.Contains("Tag") && (int)parameter == 3)
            //{
            //    return true;
            //}

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
}
