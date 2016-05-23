﻿using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace cntrl.Converters
{
    class Security2Boolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Privilage.Privilages Privilage = (Privilage.Privilages)value;
            if (Privilage != null)
            {
                entity.Brillo.Security Security = new entity.Brillo.Security(App.Names.SalesInvoice);
                return Security.SpecialSecurity_ReturnsBoolean(Privilage);
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
