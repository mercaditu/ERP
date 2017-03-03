using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cognitivo.Configs
{
    public partial class Wallpaper : Page
    {
        private string domain = AppDomain.CurrentDomain.BaseDirectory;

        public Wallpaper()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string dir = domain + "bin\\Debug\\Images\\Wallpaper\\";
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] aryFi = di.GetFiles("*.jpg");
            int i = 0;

            foreach (System.IO.FileInfo fi in aryFi)
            {
                string file = dir + fi.Name;
                dynamic taskAuth = System.Threading.Tasks.Task.Factory.StartNew(() => paperThread(file, i));
            }
        }

        private void paperThread(string file, int i)
        {
            Dispatcher.BeginInvoke((Action)(() =>
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(file, UriKind.Absolute));
                    img.AddHandler(Image.MouseUpEvent, new RoutedEventHandler(Image_MouseUp));
                    img.Height = 150;
                    img.Width = 200;
                    img.Cursor = System.Windows.Input.Cursors.Hand;
                    Thickness margin = new Thickness(10);
                    img.Margin = margin;
                    RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
                    wrapWallpaper.Children.Insert(i, img);
                    i += 1;
                }));
        }

        private void Image_MouseUp(object sender, EventArgs e)
        {
            Cognitivo.Menu.MainWindow MainWindow = Window.GetWindow(this) as Menu.MainWindow;
            Image imageSelected = sender as Image;

            if (MainWindow != null)
            {
                Properties.Settings.Default.wallpaper_Image = imageSelected.Source.ToString();
                Properties.Settings.Default.Save();
            }
        }

        private void btnRandom(object sender, RoutedEventArgs e)
        {
            double width = SystemParameters.WorkArea.Width;
            double height = SystemParameters.WorkArea.Height;
            string img = String.Format("https://source.unsplash.com/user/cognitivo/likes/{0}x{1}", width, height);

            Cognitivo.Properties.Settings.Default.wallpaper_Image = img;
            Cognitivo.Properties.Settings.Default.Save();
        }
    }
}