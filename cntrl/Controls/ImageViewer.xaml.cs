using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;

namespace cntrl.Controls
{
    public partial class ImageViewer : UserControl, INotifyPropertyChanged
    {
        public ImageViewer()
        {
            InitializeComponent();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public static readonly DependencyProperty ReferenceIDProperty = DependencyProperty.Register("ReferenceID", typeof(int), typeof(ImageViewer), new PropertyMetadata(GetImageCallBack));

        public int ReferenceID
        {
            get { return (int)GetValue(ReferenceIDProperty); }
            set { SetValue(ReferenceIDProperty, value); }
        }

        private static void GetImageCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ImageViewer c = sender as ImageViewer;
            if (c != null)
            {
                c.GetImage();
            }
        }

        public entity.App.Names ApplicationName { get; set; }

        private void GetImage()
        {
            CollectionViewSource app_attachmentViewSource = ((CollectionViewSource)(FindResource("app_attachmentViewSource")));
            app_attachmentViewSource.Source = null;

            if (ReferenceID > 0)
            {
                using (entity.db db = new entity.db())
                {
                    if (db.app_attachment.Where(x => x.application == ApplicationName && x.reference_id == ReferenceID && x.mime.Contains("image")).Any())
                    {
                        app_attachmentViewSource.Source = db.app_attachment
                            .Where(x => x.application == ApplicationName && x.reference_id == ReferenceID && x.mime.Contains("image")).ToList();
                        app_attachmentViewSource.View.Refresh();
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
                            entity.app_attachment _app_attachment = db.app_attachment.Where(x => x.id_attachment == app_attachment.id_attachment).FirstOrDefault();
                            if (_app_attachment != null)
                            {
                                db.app_attachment.Remove(_app_attachment);
                                db.SaveChanges();
                            }

                            GetImage();
                        }
                    }
                }
            }

            imgContext.IsOpen = false;
        }

        private void MenuItem_New(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (ReferenceID > 0)
            {
                var Data = e.Data as DataObject;
                entity.Brillo.Attachment Attachment = new entity.Brillo.Attachment();
                Attachment.SaveFile(Data, ApplicationName, ReferenceID);
                GetImage();
            }
            else
            {
                MessageBox.Show("Please Save Item before inserting an Image", "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Image_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            imgContext.IsOpen = true;
        }
    }
}