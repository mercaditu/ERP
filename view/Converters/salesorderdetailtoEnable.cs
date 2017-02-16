using System;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class salesorderdetailtoEnable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            entity.project_task project_task = (entity.project_task)value;
            if (project_task != null)
            {
                if (project_task.sales_detail == null)
                {
                    return true;
                }
                else
                {
                    if (project_task.items != null)
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
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}