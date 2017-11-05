using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity;
using System;
using System.Windows.Input;
using System.Timers;

namespace cntrl
{
    public partial class toolBarNotification : UserControl
    {
        CollectionViewSource app_notificationViewSource;

        public toolBarNotification()
        {
            InitializeComponent();
        }

        public App.Names id_application { get; set; }
        public int ref_id { get; set; }
        public int notificationAmount { get; set; }

        public event btnFocus_ClickedEventHandler btnFocus_Click;

        public delegate void btnFocus_ClickedEventHandler(object sender);

        public void btnFocus_MouseUp(object sender, EventArgs e)
        {
            btnFocus_Click?.Invoke(sender);
        }

        public event btnClear_ClickedEventHandler btnClear_Click;

        public delegate void btnClear_ClickedEventHandler(object sender);

        public void btnClear_MouseUp(object sender, EventArgs e)
        {

            btnClear_Click?.Invoke(sender);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            getData(null, null);
            //Load Basic Data into Timer.
            Timer myTimer = new Timer();
            myTimer.Elapsed += new ElapsedEventHandler(getData);
            myTimer.Interval = 300000;
            myTimer.Start();

            using (db db = new db())
            {
                CollectionViewSource app_departmentViewSource = FindResource("app_departmentViewSource") as CollectionViewSource;
                app_departmentViewSource.Source = await db.app_department.Where(x=>x.id_company==CurrentSession.Id_Company).ToListAsync();

                CollectionViewSource security_userViewSource = FindResource("security_userViewSource") as CollectionViewSource;
                security_userViewSource.Source = await db.security_user.Where(x => x.id_company == CurrentSession.Id_Company).ToListAsync();

                //If ref Id exists in notification viewsource, then put focus on that row.
            }
        }

        private async void getData(object sender, EventArgs e)
        {
            try
            {
                using (db db = new db())
                {
                    app_notificationViewSource = FindResource("app_notificationViewSource") as CollectionViewSource;
                    await db.app_notification.Where(x => x.is_read == false && x.id_application == id_application && x.id_company == CurrentSession.Id_Company &&
                    ((x.notified_user.id_user == CurrentSession.Id_User && x.notified_department == null) || x.notified_department.id_department == CurrentSession.UserRole.id_department))
                    .LoadAsync();
                    app_notificationViewSource.Source = db.app_notification.Local;
                    
                    notificationAmount = db.app_notification.Local.Count();
                    
                    if (notificationAmount == 0)
                    {
                        tabCreate.IsSelected = true;
                    }
                }
            }
            catch
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            using (db db = new db())
            {
                string comment = commentTextBox.Text;
                string userName = CurrentSession.User.name_full + ": " + comment;

                app_notification app_notification = new app_notification()
                {
                    comment = userName,
                    id_application = id_application,
                    ref_id = ref_id
                };
                
                if (rbtnDepartment.IsChecked == true)
                {
                    db.app_department.Attach(cbxDepartment.SelectedItem as app_department);
                    app_notification.notified_department = cbxDepartment.SelectedItem as app_department;
                }
                else if (rbtnUser.IsChecked == true)
                {
                    db.security_user.Attach(cbxUser.SelectedItem as security_user);
                    app_notification.notified_user = cbxUser.SelectedItem as security_user;
                }

                db.app_notification.Add(app_notification);
                db.SaveChangesAsync();
            }

            btnCancel_Click(null, null);
        }

        private void dgvnotification_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            app_notification app_notification = app_notificationViewSource.View.CurrentItem as app_notification;

            //improve code, make async and wait 5 seconds before launching code.
            if (app_notification!=null)
            {
                ref_id = app_notification.ref_id;
            }
               
            btnFocus_MouseUp(sender, e);
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            if (Parent is StackPanel stpparent)
            {
                stpparent.Children.Clear();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            btnClear_MouseUp(sender, e);
            StackPanel stpparent = this.Parent as StackPanel;

            if (stpparent != null)
            {
                stpparent.Children.Clear();
            }
        }

        private void hideNotification_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (app_notificationViewSource.View.CurrentItem is app_notification app_notification)
                {
                    using (db db = new db())
                    {
                        db.app_notification.Attach(app_notification);
                        app_notification.is_read = true;
                        db.SaveChanges();

                        //app_notificationViewSource.View.Refresh();
                        UserControl_Loaded(null, null);
                        btnCancel_Click(null, null);
                    }
                }
            }
            catch
            {
                //toolBar.msgError(ex);
            }
        }
    }
}
