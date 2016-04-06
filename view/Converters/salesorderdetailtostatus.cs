using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Cognitivo.Converters
{
    class salesorderdetailtostatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            entity.sales_order_detail sales_order_detail = (entity.sales_order_detail)value;
            if (sales_order_detail != null)
            {
                if (sales_order_detail.id_sales_order_detail.ToString() == 0.ToString())
                {
                    return Brushes.White;
                }
                else
                {
                    return Brushes.PaleGreen;
                }
            }
            //else if (sales_order_detail.project_task.sales_invoice_detail.Count() > 0)
            //{
            //    return Brushes.PaleGreen;

            //}
            else
            {
                return Brushes.White;
            }
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
