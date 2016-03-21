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
            int i = (int)parameter;
            Controls.smartBoxContactSetting set = (Controls.smartBoxContactSetting)value;


            if (set.SearchFilter.Contains("Code") && i == 1)
            {
                return true;
            }

            if (set.SearchFilter.Contains("Name") && i == 2)
            {
                return true;
            }

            if (set.SearchFilter.Contains("GovID") && i == 3)
            {
                return true;
            }

            if (set.SearchFilter.Contains("Tel") && i == 4)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
