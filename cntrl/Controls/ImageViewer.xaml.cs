using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

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

		public static DependencyProperty ApplicationNameProperty = DependencyProperty.Register("ApplicationName", typeof(entity.App.Names), typeof(ImageViewer));

		public entity.App.Names ApplicationName
		{
			get { return (entity.App.Names)GetValue(ApplicationNameProperty); }
			set { SetValue(ApplicationNameProperty, value); }
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

		private void GetImage()
		{
			CollectionViewSource app_attachmentViewSource = ((CollectionViewSource)(FindResource("app_attachmentViewSource")));
			app_attachmentViewSource.Source = null;

			if (ReferenceID > 0)
			{
				using (entity.db db = new entity.db())
				{
					if (db.app_attachment.Where(x => x.application == ApplicationName && x.reference_id == ReferenceID).Any())
					{
						app_attachmentViewSource.Source = db.app_attachment
							.Where(x => x.application == ApplicationName && x.reference_id == ReferenceID).ToList();
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

		private void FlipView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            CollectionViewSource app_attachmentViewSource = ((CollectionViewSource)(FindResource("app_attachmentViewSource")));
            entity.app_attachment app_attachment = app_attachmentViewSource.View.CurrentItem as entity.app_attachment;

            if (app_attachment != null)
            {
                string mime = app_attachment.mime;
               string Path= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\" + app_attachment.id_attachment + "." + mime.Substring(mime.IndexOf("/")+1);
                File.WriteAllBytes(Path, app_attachment.file);
                System.Diagnostics.Process.Start(@Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\" + app_attachment.id_attachment + "." + mime.Substring(mime.IndexOf("/")+1));                
                //             ImageControl ImageControl = new ImageControl();

                //             ImageControl.file = app_attachment.file; // Path of the rdlc file
                //             ImageControl.RaisePropertyChanged("file");
                //             Window window = new Window
                //             {
                //                 Title = "Image",
                //                 Content = ImageControl
                //             };

                //             window.ShowDialog();

                //             //FlowDocumentWindow Flow = new FlowDocumentWindow();

                //             //Flow.Browser.Source = GetBitmapImage(app_attachment.file).BaseUri; // Path of the rdlc file
                //             ////ImageControl.RaisePropertyChanged("file");
                //             //Window window = new Window
                //             //{
                //             //    Title = "PDF",
                //             //    Content = Flow
                //             //};

                //             //window.ShowDialog();
            }
             

        }

        public BitmapImage GetBitmapImage(byte[] imageBytes)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageBytes);
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}