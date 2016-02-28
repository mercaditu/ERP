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

        entity.Properties.Settings _settings = new entity.Properties.Settings();

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

            if (_settings.user_Name != null && _settings.user_Name != "")
            {
                tbxUser.Text = _settings.user_Name;
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
                     app_company app_company = db.app_company.Where(x => x.id_company == _settings.company_ID).FirstOrDefault();
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
                    _settings.company_Name = company_Alias;
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        _settings.Save();
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
                if(_settings.company_ID == 0)
                {
                    using (db db = new db())
                    {
                        _settings.company_ID = db.app_company.FirstOrDefault().id_company;
                        _settings.Save();
                    }
                }

                //CurrentSession.
                CurrentSession.Start(u, p);

                if (CurrentSession.User != null)
                {
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
                MessageBox.Show(ex.Message, "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Error);
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
