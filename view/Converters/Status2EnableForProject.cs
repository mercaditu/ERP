using System;
using System.Windows.Media;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class Status2EnableForProject : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {


                entity.Status.Project status = (entity.Status.Project)value;

                if (status == entity.Status.Project.Management_Approved)
                {
                    return true;
                }
                else
                { //Pending
                    return false;
                }
            }
            else { return true; }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                                System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
