using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Cognitivo.Converters
{
    public class AttachmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                                            System.Globalization.CultureInfo culture)
        {
            List<entity.item_movement> movement = value as List<entity.item_movement>;

            if (movement.FirstOrDefault()!=null)
            {
                using (entity.db db = new entity.db())
                {
                    long id_movement = movement.FirstOrDefault().id_movement;
                    entity.app_attachment app_attachment = db.app_attachment.Where(x => x.reference_id == id_movement).FirstOrDefault();
                    return app_attachment.FileName;
                }

            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                                System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}