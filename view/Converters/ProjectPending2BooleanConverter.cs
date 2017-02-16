using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class ProjectPending2BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object ConverterParameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                entity.Status.Project status = (entity.Status.Project)value;
                if (status == entity.Status.Project.Pending)
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}