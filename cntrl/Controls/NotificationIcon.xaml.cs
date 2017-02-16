using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace cntrl.Controls
{
    public partial class NotificationIcon : UserControl
    {
        public NotificationIcon()
        {
            InitializeComponent();
        }

        private static readonly DependencyProperty NumberProperty
                    = DependencyProperty.Register("Number", typeof(int), typeof(NotificationIcon), new PropertyMetadata(OnTextChangedCallBack));

        public int Number
        {
            get { return (int)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        private static void OnTextChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NotificationIcon me = sender as NotificationIcon;
            int number = (int)e.NewValue;
            me.AnimateLabelRotation(null, number);
        }

        public void AnimateLabelRotation(object sender, int number)
        {
            if (number == 0)
            {
                //If control is zero, then control should not be seen.
                _NotificationIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                //If number is greater than zero, then control should be visible.
                _NotificationIcon.Visibility = Visibility.Visible;

                //Also animate
                Storyboard Animate = (Storyboard)FindResource("TextChanged");
                Animate.Begin(this);
            }
        }
    }
}