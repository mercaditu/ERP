using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace cntrl.Panels
{
    /// <summary>
    /// Interaction logic for pnl_Curd.xaml
    /// </summary>
    public partial class pnl_Curd : UserControl
    {
        public pnl_Curd()
        {
            InitializeComponent();
           
        }

        private void pnlCurd_MouseEnter(object sender, MouseEventArgs e)
        {
            chbxSelected.Visibility = Visibility.Visible;
        }

        private void pnlCurd_MouseLeave(object sender, MouseEventArgs e)
        {
            if (chbxSelected.IsChecked != true)
            { chbxSelected.Visibility = Visibility.Collapsed; }
        }

        //Edit link click.
        public delegate void linkedit_ClickEventHandlar(object sender, int intId);
        public event linkedit_ClickEventHandlar linkEdit_click;

 
        private void txtName_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (linkEdit_click != null)
            {
                linkEdit_click(sender, Convert.ToInt32(Id));
            }
        }

        //StatusProperty
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(Brush), typeof(pnl_Curd),
            new FrameworkPropertyMetadata(Brushes.WhiteSmoke));
        public Brush Status
        {
            get { return (Brush)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        //DisplayNameProperty
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(pnl_Curd),
            new FrameworkPropertyMetadata(string.Empty));
        public string DisplayName
        {
            get { return Convert.ToString(GetValue(DisplayNameProperty)); }
            set { SetValue(DisplayNameProperty, value); }
        }

        //IdProperty
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(pnl_Curd),
            new FrameworkPropertyMetadata(0));
        public int Id
        {
            get { return Convert.ToInt32(GetValue(IdProperty)); }
            set { SetValue(IdProperty, value); }
        }
        //public static readonly DependencyProperty IsChangedProperty =
        //  DependencyProperty.Register("IsChanged", typeof(bool), typeof(pnl_Curd),
        //  new FrameworkPropertyMetadata(false));
        //public bool IsChanged
        //{
        //    get { return Convert.ToBoolean(GetValue(IsChangedProperty)); }
        //    set { SetValue(IsChangedProperty, value); }
        //}

        //private void chbxSelected_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (chbxSelected.IsChecked==true && IsChanged)
        //    {
        //        entity.CurrentSession.Id_Company = Id;
        //    }
        //}
    }
}
