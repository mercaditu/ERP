using System.Windows;
using System.Windows.Controls;

namespace cntrl.Controls
{
    public partial class NotificationButton : UserControl
    {
        /// <summary>
        /// Sets the Notiifcation Number to be shown in red circle.
        /// </summary>
        private static readonly DependencyProperty NumberProperty
            = DependencyProperty.Register("Number", typeof(int), typeof(NotificationButton), new PropertyMetadata());
        public int Number
        {
            get { return (int)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        /// <summary>
        /// Sets the Text.
        /// </summary>
        private static readonly DependencyProperty TextProperty
            = DependencyProperty.Register("Text", typeof(string), typeof(NotificationButton), new PropertyMetadata());
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(NumberProperty, value); }
        }


        /// <summary>
        /// Sets the Icon for the Button. Based on Cognitivo Font.
        /// </summary>
        private static readonly DependencyProperty IconProperty
            = DependencyProperty.Register("Icon", typeof(string), typeof(NotificationButton), new PropertyMetadata());
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public event RoutedEventHandler Click;
        private void NotificationIcon_Click(object sender, RoutedEventArgs e)
        {
            if (Click != null)
            { Click(this, new RoutedEventArgs()); }
        }

        public NotificationButton()
        {
            InitializeComponent();
        }
    }
}
