using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity;

namespace cntrl
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class toolBarNotification : UserControl
    {
        //db db = new db();
        CollectionViewSource app_notificationViewSource;
        public toolBarNotification()
        {
            InitializeComponent();
        }

        public App.Names id_application { get; set; }
        public int ref_id { get; set; }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            using (db db = new db())
            {

                app_notificationViewSource = FindResource("app_notificationViewSource") as CollectionViewSource;
                await db.app_notification.Where(x => x.is_read == false && ( x.id_application == id_application && x.id_user == CurrentSession.Id_User || x.notified_department == CurrentSession.UserRole.app_department)).LoadAsync();
                app_notificationViewSource.Source = db.app_notification.Local;

                CollectionViewSource app_departmentViewSource = FindResource("app_departmentViewSource") as CollectionViewSource;
                app_departmentViewSource.Source = await db.app_department.ToListAsync();

                CollectionViewSource security_userViewSource = FindResource("security_userViewSource") as CollectionViewSource;
                security_userViewSource.Source = await db.security_user.ToListAsync();

                //If ref Id exists in notification viewsource, then put focus on that row.
            }
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
            //db.SaveChanges();
            using (db db = new db())
            {
                string comment = commentTextBox.Text;
                string userName = CurrentSession.User.name_full + ": " + comment;
                app_notification app_notification = new app_notification();
                if (cbxDepartment.SelectedItem != null)
                {
                    app_notification.notified_department = cbxDepartment.SelectedItem as app_department;
                }
                if (cbxUser.SelectedItem != null)
                {
                    app_notification.notified_user = cbxUser.SelectedItem as security_user;
                }

                app_notification.comment = userName;
                db.app_notification.Add(app_notification);
                db.SaveChangesAsync();
            }
        }

        private void dgvnotification_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //improve code, make async and wait 5 seconds before launching code.
            using (db db = new db())
            {
                app_notification app_notification = app_notificationViewSource.View.CurrentItem as app_notification;
                app_notification.is_read = true;
                db.SaveChangesAsync();

                ref_id = app_notification.ref_id;
                //Where is event fire??
            }
        }
    }
}
