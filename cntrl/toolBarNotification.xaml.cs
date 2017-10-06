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
using entity;
using System.Data.Entity;

namespace cntrl.Controls
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : UserControl
    {
        db db = new db();
        CollectionViewSource app_notificationViewSource;
        public NotificationWindow()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource app_notificationViewSource = FindResource("app_notificationViewSource") as CollectionViewSource;
            db.app_notification.Load();
            app_notificationViewSource.Source = db.app_notification.Local;

            CollectionViewSource app_departmentViewSource = FindResource("app_departmentViewSource") as CollectionViewSource;
            app_departmentViewSource.Source = db.app_department.ToList();

            CollectionViewSource security_userViewSource = FindResource("security_userViewSource") as CollectionViewSource;
            security_userViewSource.Source = db.security_user.ToList();

            app_notification app_notification = new app_notification();
            db.app_notification.Add(app_notification);
            app_notificationViewSource.View.MoveCurrentToLast();

        }

        private void rbtnDepartment_Checked(object sender, RoutedEventArgs e)
        {
            if (rbtnDepartment.IsChecked==true)
            {
                stpDepartment.Visibility = Visibility.Visible;
                stpUser.Visibility = Visibility.Collapsed;
            }
            if (rbtnUser.IsChecked==true)
            {
                stpDepartment.Visibility = Visibility.Collapsed;
                stpUser.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            db.SaveChanges();
        }
    }
}
