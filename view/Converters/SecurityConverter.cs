using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    class SecurityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            entity.App.Names appname = (entity.App.Names)value;
            entity.Brillo.Security security = new entity.Brillo.Security(appname);
            entity.Privilage.Privilages Privilages=(entity.Privilage.Privilages)parameter;
            if (security.SpecialSecurity_ReturnsBoolean(Privilages))
                return true;
            else
                return false;
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
