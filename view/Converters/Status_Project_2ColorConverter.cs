﻿using System;
using System.Windows.Media;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class Status_Project_2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                entity.Status.Project status = (entity.Status.Project)value;

                if (status == entity.Status.Project.Approved)
                {
                    return Brushes.PaleGreen;
                }
                else if (status == entity.Status.Project.InProcess)
                {
                    return Brushes.Gold;
                }
                else if (status == entity.Status.Project.Executed)
                {
                    return Brushes.Coral;
                }
                else if (status == entity.Status.Project.Rejected)
                {
                    return Brushes.Crimson;
                }
             
                else
                { //Pending
                    return Brushes.WhiteSmoke;
                }
            }
            else { return Brushes.WhiteSmoke; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, 
                                                System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
