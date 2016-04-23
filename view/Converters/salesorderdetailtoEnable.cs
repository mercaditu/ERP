using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Cognitivo.Converters
{
    class salesorderdetailtoEnable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            entity.project_task project_task = (entity.project_task)value;
            if (project_task!=null)
            {
                if (project_task.sales_detail==null )
                {
                    return true;
                }
                else
                {
                    if (project_task.items!=null)
                    {
                        if (project_task.items.id_item_type != entity.item.item_type.Task)
                        {
                            return false;

                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                   
                }
            }
            //else if (sales_order_detail.project_task.sales_invoice_detail.Count() >0)
            //{
            //    return false;
               
            //}
            else
            {
                return true;
            }
          
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
