using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class State2IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, 
                                            System.Globalization.CultureInfo culture)
        {
            if (value != null && value.ToString() == System.Data.Entity.EntityState.Added.ToString())
            {
                return "+";
            }
            else if (value != null && value.ToString() == System.Data.Entity.EntityState.Deleted.ToString())
            {
                return "d";
            }
            else if (value != null && value.ToString() == System.Data.Entity.EntityState.Modified.ToString())
            {
                return "e";
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, 
                                                System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
