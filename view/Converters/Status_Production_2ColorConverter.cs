﻿using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Cognitivo.Converters
{
    public class Status_Production_2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                entity.Status.Production status = (entity.Status.Production)value;

                if (status == entity.Status.Production.Approved)
                {
                    return Brushes.PaleGreen;
                }
                else if (status == entity.Status.Production.InProcess)
                {
                    return Brushes.Gold;
                }
                else if (status == entity.Status.Production.Executed)
                {
                    return Brushes.Coral;
                }
                else if (status == entity.Status.Production.QA_Check)
                {
                    return Brushes.Purple;
                }
                else if (status == entity.Status.Production.QA_Rejected)
                {
                    return Brushes.Crimson;
                }
                else if (status == entity.Status.Production.Anull)
                {
                    return Brushes.Crimson;
                }
                else
                {
                    return Brushes.WhiteSmoke; //Pending
                }
            }
            else
            {
                return Brushes.WhiteSmoke;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                                System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}