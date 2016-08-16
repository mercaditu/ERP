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
            // entity.App.Names appname = (entity.App.Names)value;
            entity.Brillo.Security security = new entity.Brillo.Security(0);
            entity.Privilage.Privilages Privilages = (entity.Privilage.Privilages)value;
            if (security.SpecialSecurity_ReturnsBoolean(Privilages))
                return false;
            else
                return true;
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
