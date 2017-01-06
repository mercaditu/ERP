using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using entity;
using System.Deployment.Application;
using System.Reflection;

namespace Cognitivo.Menu
{
    public partial class mainLogIn : Page
    {
        Frame myFrame;
        Task taskAuth;
        MainWindow myWindow = Application.Current.MainWindow as MainWindow;

        public Version AssemblyVersion
        {
            get
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
        }
        public Version LocalVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public mainLogIn()
        {
            InitializeComponent();

            try
            {
                lblVersion.Content = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch
            {
                var obj = Assembly.GetExecutingAssembly().GetName().Version;
                lblVersion.Content = string.Format("Cognitivo ERP Version {0}.{1}.{2}.{3}", obj.Major, obj.Minor, obj.Build, obj.Revision);
            }

            myFrame = myWindow.mainFrame;

            //Threads DB Creation Code on StartUp.
            Task taskdb = Task.Factory.StartNew(() => check_createdb());
        }

        private async void check_createdb()
        {
            entity.Properties.Settings Settings = new entity.Properties.Settings();

            await Dispatcher.BeginInvoke((Action)(() =>
            {
                if (Settings.user_Name != null || Settings.user_UserName != "")
                {
                    tbxUser.Text = Settings.user_UserName;
                    tbxPassword.Focus();
                    chbxRemember.IsChecked = true;
                }
                else
                {
                    tbxUser.Focus();
                }
            }));

            using (db db = new db())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                if (db.Database.Exists() == false)
                {
                    await Dispatcher.BeginInvoke((Action)(() => { myFrame.Navigate(new StartUp()); }));
                    return;
                }
            }
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            if (taskAuth == null)
            {
                string u = tbxUser.Text;
                string p = tbxPassword.Password;
                taskAuth = Task.Factory.StartNew(() => auth_Login(u, p));
            }
        }

        private void auth_Login(string u, string p)
        {
            Dispatcher.BeginInvoke((Action)(() => { this.Cursor = Cursors.AppStarting; }));
            Dispatcher.BeginInvoke((Action)(() => { progBar.IsIndeterminate = true; }));

            security_user User = null;
            security_role Role = null;

            //Originally inside Current Session. I have removed it to take advantage of Warm Queries, to help speed up process.
            using (db db = new db())
            {
                db.Configuration.AutoDetectChangesEnabled = false;

                User = db.security_user.Where(x => x.name == u
                                                 && x.password == p
                                                 && x.id_company == CurrentSession.Id_Company)
                                       .FirstOrDefault();
                if (User != null)
                {
                    Role = User.security_role;
                }
                else
                {
                    //Incorrect user credentials.
                    Dispatcher.BeginInvoke((Action)(() => 
                    {
                        tbxPassword.Focus();
                        Cursor = Cursors.Arrow;
                        progBar.IsIndeterminate = false;
                    }));
                    taskAuth = null;
                    return;
                }
            }

            try
            {
                Properties.Settings ViewSettings = new Properties.Settings();
                CurrentSession.ConnectionString = ViewSettings.MySQLconnString;

                CurrentSession.Start(User, Role);

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (chbxRemember.IsChecked == true)
                    {
                        entity.Properties.Settings Settings = new entity.Properties.Settings();
                        Settings.user_UserName = tbxUser.Text;
                        Settings.Save();
                    }
                }));

                myWindow.is_LoggedIn = true;
                Dispatcher.BeginInvoke((Action)(() => myFrame.Navigate(new mainMenu_Corporate())));
            }
            catch { } //Do Nothing
            finally
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    Cursor = Cursors.Arrow;
                    progBar.IsIndeterminate = false;
                }));
                taskAuth = null;
            }
        }

        private void SetUp_MouseUp(object sender, EventArgs e)
        {
            myFrame.Navigate(new StartUp());
        }

        private void Settings_MouseUp(object sender, EventArgs e)
        {
            myFrame.Navigate(new Configs.Settings());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("sdf");
        }

     
    }
}
