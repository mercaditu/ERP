using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;

namespace cntrl.Converters
{
    internal class sbxContact_CheckType : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //StringCollection val = (StringCollection)value;
            //List<string> ValueLIst = val.Cast<string>().ToList();


            //if (ValueLIst.Where(x => x == "Code").Count() > 0 && (int)parameter == 1)
            //{
            //    return true;
            //}
            //if (ValueLIst.Where(x => x == "Name").Count() > 0 && (int)parameter == 2)
            //{
            //    return true;
            //}
            //if (ValueLIst.Where(x => x == "GovID").Count()> 0 && (int)parameter == 3)
            //{
            //    return true;
            //}

            //if (ValueLIst.Where(x => x == "Tel").Count() > 0 && (int)parameter == 4)
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