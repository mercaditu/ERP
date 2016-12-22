using cntrl.Curd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cntrl.Controls
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer : UserControl
    {
        public ImageViewer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ReferenceIDProperty = DependencyProperty.Register("ReferenceID", typeof(int), typeof(ImageViewer));
        public int ReferenceID
        {
            get { return (int)GetValue(ReferenceIDProperty); }
            set { SetValue(ReferenceIDProperty, value); GetImage(); }
        }

        public entity.App.Names ApplicationName { get; set; }

        private void GetImage()
        {
            CollectionViewSource app_attachmentViewSource = ((CollectionViewSource)(FindResource("app_attachmentViewSource")));
            if (ReferenceID > 0)
            {
                using (entity.db db = new entity.db())
                {
                    if (db.app_attachment.Where(x => x.application == ApplicationName && x.reference_id == ReferenceID && x.mime.Contains("image")).Any())
                    {
                        app_attachmentViewSource.Source = db.app_attachment
                            .Where(x => x.application == ApplicationName && x.reference_id == ReferenceID && x.mime.Contains("image")).ToList();
                    }
                }
            }
        }

        private void MenuItem_Delete(object sender, RoutedEventArgs e)
        {
            CollectionViewSource app_attachmentViewSource = ((CollectionViewSource)(FindResource("app_attachmentViewSource")));
            if (app_attachmentViewSource.View != null)
            {
                if (app_attachmentViewSource.View.CurrentItem != null)
                {
                    entity.app_attachment app_attachment = app_attachmentViewSource.View.CurrentItem as entity.app_attachment;

                    if (app_attachment != null)
                    {
                        using (entity.db db = new entity.db())
                        {
                            db.app_attachment.Remove(app_attachment);
                            db.SaveChanges();

                            GetImage();
                        }
                    }
                }
            }
        }

        private void MenuItem_New(object sender, RoutedEventArgs e)
        {

        }

        private void InsertImage(object sender, DataObject e)
        {
            if (ReferenceID > 0)
            {
                var data = e.Data as DataObject;
                entity.Brillo.Attachment Attachment = new entity.Brillo.Attachment();
                Attachment.SaveFile(data, ApplicationName, ReferenceID);
            }
            else
            {
                MessageBox.Show("Please Save Item before inserting an Image", "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
