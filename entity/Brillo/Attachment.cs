using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace entity.Brillo
{
   public class Attachment
    {
        public void SaveFile(DataObject data,int reference_id,app_attachment Document)
        {
            if (data.ContainsFileDropList())
            {
                var files = data.GetFileDropList();
                string extension = Path.GetExtension(files[0]);

                if (!string.IsNullOrEmpty(extension))
                {
                    app_attachment item_image;
                    if (Document!=null)
                    {
                        item_image = Document;
                    }
                    else
                    {
                        item_image = new app_attachment();
                    }
                
                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        item_image.mime = "image/" + extension.Substring(1);
                    }
                    else if (extension.ToLower() == ".pdf")
                    {
                        item_image.mime = "application/" + extension.Substring(1);
                    }
                    Brillo.Byte2FileConverter ByteConverter = new Brillo.Byte2FileConverter();
                  
                    item_image.file = ByteConverter.ResizeImage(files[0].ToString());
                    item_image.reference_id = reference_id;
                
                    item_image.application = entity.App.Names.Items;

                    using (db db = new db())
                    {
                        db.app_attachment.Add(item_image);
                        db.SaveChangesAsync();
                    }
                    
                }
                else
                {
                    MessageBox.Show("Images with .jpg, .jpeg, .png extensions are only allowed.");
                }
            }
        }
    }
}
