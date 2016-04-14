using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Data;
using System.Linq;
using System.Text;
using entity;

namespace Cognitivo.Converters
{
    public class TreeViewchildFilter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, 
                                            System.Globalization.CultureInfo culture)
        {
            item.item_type type = (item.item_type)parameter;
            if (type == item.item_type.Product)
            {
                List<production_order_detail> production_order_detail = (List<production_order_detail>)value;
                production_order_detail = production_order_detail.Where(x => x.item.id_item_type == item.item_type.Product || x.item.id_item_type == item.item_type.Task).ToList();
                if (production_order_detail.Count() >0)
                {
                    return production_order_detail;
                }
                else
                {
                    return null;
                }
            }
            else if (type == item.item_type.Service)
            {
                List<production_order_detail> production_order_detail = (List<production_order_detail>)value;
                production_order_detail = production_order_detail.Where(x => x.item.id_item_type == item.item_type.Service || x.item.id_item_type == item.item_type.Task).ToList();
                if (production_order_detail.Count() > 0)
                {
                    return production_order_detail;
                }
                else
                {
                    return null;
                }
            }
            else if (type == item.item_type.FixedAssets)
            {
                List<production_order_detail> production_order_detail = (List<production_order_detail>)value;
                production_order_detail = production_order_detail.Where(x => x.item.id_item_type == item.item_type.FixedAssets || x.item.id_item_type == item.item_type.Task).ToList();
                if (production_order_detail.Count() > 0)
                {
                    return production_order_detail;
                }
                else
                {
                    return null;
                }
            }
            else if (type == item.item_type.RawMaterial)
            {
                List<production_order_detail> production_order_detail = (List<production_order_detail>)value;
                production_order_detail = production_order_detail.Where(x => x.item.id_item_type == item.item_type.RawMaterial || x.item.id_item_type == item.item_type.Task).ToList();
                if (production_order_detail.Count() > 0)
                {
                    return production_order_detail;
                }
                else
                {
                    return null;
                }
            }
            else if (type == item.item_type.Supplies)
            {
                List<production_order_detail> production_order_detail = (List<production_order_detail>)value;
                production_order_detail = production_order_detail.Where(x => x.item.id_item_type == item.item_type.Supplies || x.item.id_item_type == item.item_type.Task).ToList();
                if (production_order_detail.Count() > 0)
                {
                    return production_order_detail;
                }
                else
                {
                    return null;
                }
            }
            else if (type == item.item_type.ServiceContract)
            {
                List<production_order_detail> production_order_detail = (List<production_order_detail>)value;
                production_order_detail = production_order_detail.Where(x => x.item.id_item_type == item.item_type.ServiceContract || x.item.id_item_type == item.item_type.Task).ToList();
                if (production_order_detail.Count() > 0)
                {
                    return production_order_detail;
                }
                else
                {
                    return null;
                }
            }
            else { return null; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, 
                                                System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}

