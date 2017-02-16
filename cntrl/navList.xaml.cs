using System.Windows;
using System.Windows.Controls;

namespace cntrl
{
    public partial class navList : UserControl
    {
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(navList));

        public bool IsChecked
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(System.Data.Entity.EntityState), typeof(navList), new UIPropertyMetadata(System.Data.Entity.EntityState.Unchanged));

        public System.Data.Entity.EntityState State
        {
            get { return (System.Data.Entity.EntityState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(string), typeof(navList));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for recordName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty recordNameProperty =
            DependencyProperty.Register("recordName", typeof(string), typeof(navList));

        public string recordName
        {
            get { return (string)GetValue(recordNameProperty); }
            set { SetValue(recordNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for recordName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty recordCodeProperty =
            DependencyProperty.Register("recordCode", typeof(string), typeof(navList));

        public string recordCode
        {
            get { return (string)GetValue(recordCodeProperty); }
            set { SetValue(recordCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for recordName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty recordSecondaryNameProperty =
            DependencyProperty.Register("recordSecondaryName", typeof(string), typeof(navList));

        public string recordSecondaryName
        {
            get { return (string)GetValue(recordSecondaryNameProperty); }
            set { SetValue(recordSecondaryNameProperty, value); }
        }

        public navList()
        {
            InitializeComponent();
        }
    }
}