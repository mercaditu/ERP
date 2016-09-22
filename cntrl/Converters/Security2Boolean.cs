using entity;
using System;

namespace cntrl.Converters
{
    class Security2Boolean
    {
        public object Convert(Privilage.Privilages value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Privilage.Privilages Privilage = (Privilage.Privilages)value;
            if (value != null)
            {
                entity.Brillo.Security Security = new entity.Brillo.Security(App.Names.SalesInvoice);
                return Security.SpecialSecurity_ReturnsBoolean(value);
            }

            //If no value found, return true. this way software doesn't stop working.
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
