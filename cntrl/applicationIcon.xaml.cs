using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace cntrl
{
    public partial class applicationIcon : UserControl
    {
        public static readonly DependencyProperty HasReportProperty =
        DependencyProperty.Register("HasReport", typeof(bool), typeof(applicationIcon));
        public bool HasReport
        {
            get { return (bool)GetValue(HasReportProperty); }
            set { SetValue(HasReportProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  
        //This enables animation, styling, binding, etc...
        public static readonly DependencyProperty imgSourceProperty =
            DependencyProperty.Register("imgSource", typeof(ImageSource), typeof(applicationIcon));
        public ImageSource imgSource
        {
            get { return (ImageSource)GetValue(imgSourceProperty); }
            set { SetValue(imgSourceProperty, value); }
        }

        //ApplicationNameProperty
        public static readonly DependencyProperty ApplicationNameProperty =
            DependencyProperty.Register("ApplicationName", typeof(string), typeof(applicationIcon),
            new FrameworkPropertyMetadata(string.Empty));
        public string ApplicationName
        {
            get { return Convert.ToString(GetValue(ApplicationNameProperty)); }
            set { SetValue(ApplicationNameProperty, value); }
        }

        public applicationIcon()
        {
            InitializeComponent();
        }

        public event ClickedEventHandler Click;
        public delegate void ClickedEventHandler(object sender, RoutedEventArgs e);
        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            if (Click != null)
            {
                Click(this, e);
            }
        }

        public event ClickedFavEventHandler ClickedFav;
        public delegate void ClickedFavEventHandler(object sender, RoutedEventArgs e);
        private void applicationIcon_ClickFavorites(object sender, RoutedEventArgs e)
        {
            if (ClickedFav != null)
            {
                ClickedFav(this, e);
            }
        }


        private void Report_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
