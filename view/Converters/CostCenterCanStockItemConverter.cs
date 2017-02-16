using System;
using System.Linq;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    internal class CostCenterCanStockCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (entity.db db = new entity.db())
                {
                    int costcenter_id = (int)value;
                    if (db.app_cost_center.Where(x => x.id_cost_center == costcenter_id).FirstOrDefault() != null)
                    {
                        if (db.app_cost_center.Where(x => x.id_cost_center == costcenter_id).FirstOrDefault().is_product == true)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class CostCenterCanStockItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (entity.db db = new entity.db())
                {
                    int costcenter_id = (int)value;
                    if (db.app_cost_center.Where(x => x.id_cost_center == costcenter_id).FirstOrDefault() != null)
                    {
                        if (db.app_cost_center.Where(x => x.id_cost_center == costcenter_id).FirstOrDefault().is_product == true)
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