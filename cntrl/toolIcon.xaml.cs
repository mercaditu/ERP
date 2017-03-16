using System.Windows;
using System.Windows.Controls;

namespace cntrl
{
    public partial class toolIcon : UserControl
    {
        public static readonly DependencyProperty imgSourceProperty = DependencyProperty.Register("imgSource", typeof(string), typeof(toolIcon));

        public string imgSource
        {
            get { return (string)GetValue(imgSourceProperty); }
            set { SetValue(imgSourceProperty, value); }
        }

        public static readonly DependencyProperty icoNameProperty = DependencyProperty.Register("icoName", typeof(string), typeof(toolIcon));

        public string icoName
        {
            get { return (string)GetValue(icoNameProperty); }
            set { SetValue(icoNameProperty, value); }
        }

        public static readonly DependencyProperty qtyNotificationProperty = DependencyProperty.Register("qtyNotification", typeof(int), typeof(toolIcon));

        public int qtyNotification
        {
            get { return (int)GetValue(qtyNotificationProperty); }
            set { SetValue(qtyNotificationProperty, value); }
        }

        public toolIcon()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler Click;

        private void toolIcon_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, new RoutedEventArgs());
        }
    }
}