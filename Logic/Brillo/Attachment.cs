using System.IO;
using System.Windows;

namespace entity.Brillo
{
    public class Attachment
    {
        public void SaveFile(DataObject data, App.Names Application, int reference_id)
        {
            if (data.ContainsFileDropList())
            {
                var files = data.GetFileDropList();
                string extension = Path.GetExtension(files[0]);

                if (!string.IsNullOrEmpty(extension))
                {
                    app_attachment app_attachment = new app_attachment();

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        app_attachment.mime = "image/" + extension.Substring(1);

                        Byte2FileConverter ByteConverter = new Byte2FileConverter();
                        app_attachment.file = ByteConverter.ResizeImage(files[0].ToString());
                    }
                    else if (extension.ToLower() == ".pdf")
                    {
                        app_attachment.mime = "application/" + extension.Substring(1);
                    }

                    app_attachment.reference_id = reference_id;
                    app_attachment.application = Application;

                    using (db db = new db())
                    {
                        db.app_attachment.Add(app_attachment);
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