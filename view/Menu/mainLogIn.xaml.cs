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
                lblVersion.Content = string.Format("Application Version {0}.{1}.{2}.{3}", obj.Major, obj.Minor, obj.Build, obj.Revision);
            }

            myFrame = myWindow.mainFrame;

            //string imgSource = "http://images2.fanpop.com/image/photos/12900000/Tony-Stark-tony-stark-12952919-387-528.jpg";
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.UriSource = new Uri(imgSource, UriKind.Absolute);
            //bitmap.EndInit();
            //imgAvatar.ImageSource = bitmap;

            Task taskdb = Task.Factory.StartNew(() => check_createdb());

            entity.Properties.Settings Settings = new entity.Properties.Settings();
            if (Settings.user_Name != null && Settings.user_UserName != "")
            {
                tbxUser.Text = Settings.user_UserName;
                tbxPassword.Focus();
                chbxRemember.IsChecked = true;
            }
            else
            {
                tbxUser.Focus();
            }
        }

        private void check_createdb()
        {
            using (var db = new db())
            {
                bool db_exists = db.Database.Exists();

                if (!db_exists)
                {
                    Dispatcher.BeginInvoke((Action)(() => { myFrame.Navigate(new StartUp()); }));
                }
                else
                {
                    string company_Alias = string.Empty;
                    app_company app_company = db.app_company.Where(x => x.id_company == entity.CurrentSession.Id_Company).FirstOrDefault();
                    
                    if (app_company != null)
                    {
                        if (!string.IsNullOrEmpty(app_company.alias))
                        {
                            company_Alias = app_company.alias;
                        }
                        else
                        {
                            company_Alias = app_company.name;
                        }
                    }

                    entity.Properties.Settings Settings = new entity.Properties.Settings();
                    Settings.company_Name = company_Alias;
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        Settings.Save();
                    }));
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
            
            try
            {
                entity.Properties.Settings Settings = new entity.Properties.Settings();
                if (Settings.company_ID == 0)
                {
                    using (db db = new db())
                    {
                        Settings.company_ID = db.app_company.FirstOrDefault().id_company;
                        Settings.Save();
                    }
                }


                Cognitivo.Properties.Settings ViewSettings = new Properties.Settings();
                CurrentSession.ConnectionString = ViewSettings.MySQLconnString;
                CurrentSession.Start(u, p);

                if (CurrentSession.User != null)
                {
                    Dispatcher.BeginInvoke((Action)(() => 
                    {
                        if (chbxRemember.IsChecked == true)
                        {
                            Settings.user_UserName = tbxUser.Text;
                            Settings.Save();
                        }
                    }));

                    myWindow.is_LoggedIn = true;
                    Dispatcher.BeginInvoke((Action)(() => myFrame.Navigate(new mainMenu_Corporate())));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => { tbxUser.Focus(); }));                        
                }
            }
            catch (Exception ex) 
            {
                Dispatcher.BeginInvoke((Action)(() => 
                {
                    MessageBox.Show(ex.InnerException.ToString(), "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Error);
                }));
            }
            finally
            {
                Dispatcher.BeginInvoke((Action)(() => { this.Cursor = Cursors.Arrow; }));
                Dispatcher.BeginInvoke((Action)(() => { progBar.IsIndeterminate = false; }));
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
    }
}
