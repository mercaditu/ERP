using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace cntrl
{
    public partial class toolIcon_Mini : UserControl
    {
        /// <summary>
        /// String Letter representing the Icon. Font = Cognitivo
        /// </summary>
        public static readonly DependencyProperty imgSourceProperty = DependencyProperty.Register("imgSource", typeof(string), typeof(toolIcon_Mini));
        public string imgSource
        {
            get { return (string)GetValue(imgSourceProperty); }
            set { SetValue(imgSourceProperty, value); }
        }

        /// <summary>
        /// String Name of Button. Shown as ToolTip
        /// </summary>
        public static readonly DependencyProperty icoNameProperty = DependencyProperty.Register("icoName", typeof(string), typeof(toolIcon_Mini));
        public string icoName
        {
            get { return (string)GetValue(icoNameProperty); }
            set { SetValue(icoNameProperty, value); }
        }

        /// <summary>
        /// ForeGround control for the Icon
        /// </summary>
        public Brush icoColor { get; set; }

        public toolIcon_Mini() 
        {
            InitializeComponent();
        }

        public event RoutedEventHandler Click;
        private void toolIcon_Click(object sender, RoutedEventArgs e)
        {
            if(Click != null)
            { Click(this, new RoutedEventArgs()); }
       }
    }
}
