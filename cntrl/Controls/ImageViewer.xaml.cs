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

        private async void GetImage()
        {

        }

        private void MenuItem_Delete(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_New(object sender, RoutedEventArgs e)
        {

        }
    }
}
