using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using WPFLocalizeExtension.Extensions;
using entity;
using System.Linq;
using System.Threading.Tasks;

namespace Cognitivo.Menu
{
    public partial class mainMenu_Corporate : Page
    {
        private AppList appList = new AppList();
        private MainWindow rootWindow;

        public bool SearchMode
        {
            get { return _SearchMode; }
            set
            {
                tbxSearch.Focus();
                _SearchMode = value;
            }
        }

        private bool _SearchMode;

        public mainMenu_Corporate()
        {
            InitializeComponent();

            cntrl.moduleIcon Icon = new cntrl.moduleIcon()
            {
                Tag = "Fav",
                ModuleName = "Favorite"
            };

            get_Apps(Icon, null);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            rootWindow = Window.GetWindow(this) as MainWindow;

            Task taskCheckOnline = Task.Factory.StartNew(() => OnlineRegistration());
        }

        private void OnlineRegistration()
        {
            entity.Brillo.Licence Licence = new entity.Brillo.Licence();

            using (db db = new db())
            {
                string SerialKey = db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).Select(x => x.version).FirstOrDefault();
                security_user UserInfo = db.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
                app_company CompanyInfo = db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault();

                if (string.IsNullOrEmpty(SerialKey) == false && UserInfo != null)
                {
                    Licence.VerifyCompanyLicence(SerialKey, (int)UserInfo.security_role.Version, db.security_user.Where(x => x.security_role.version == UserInfo.security_role.version && x.id_company == CurrentSession.Id_Company).Count());

                    if (Licence.CompanyLicence != null)
                    {
                        //Check if Company Name and data is correct.
                        if (Licence.CompanyLicence.company_name == CompanyInfo.name &&
                            Licence.CompanyLicence.company_code == CompanyInfo.gov_code)
                        {
                            //Check if Version is Lite. If it is, do nothing because it's free.
                            if (UserInfo.security_role.Version != CurrentSession.Versions.Lite)
                            {
                                //Check if Version is Expired Online.
                                if (Licence.CompanyLicence.versions.Where(x => x.version == UserInfo.security_role.Version).FirstOrDefault() != null)
                                {
                                    if (Licence.CompanyLicence.versions.Where(x => x.version == UserInfo.security_role.Version).FirstOrDefault().date_expiry >= DateTime.Now)
                                    {
                                        //Not Expired. Check for UserCount.
                                        int Local_UserCount = db.security_user.Where(x => x.security_role.version == UserInfo.security_role.version && x.is_active).Count();
                                        int Web_UserCount = Licence.CompanyLicence.versions.Where(x => x.version == UserInfo.security_role.Version).FirstOrDefault().web_user_count;

                                        if (Web_UserCount < Local_UserCount)
                                        {
                                            //Local Count exceeds Reigstered Count. Inactivate old users automatically.
                                            List<security_user> UserList = db.security_user
                                                .Where(x => x.security_role.version == UserInfo.security_role.version && x.is_active)
                                                .OrderBy(x => x.trans_date).Take(Web_UserCount - Local_UserCount).ToList();
                                            foreach (var user in UserList)
                                            {
                                                user.is_active = false;
                                            }

                                            db.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            //WebUser Count less than or equal Local User Count. 
                                            //Do Nothing
                                        }
                                    }
                                    else
                                    {
                                        //Expired. Change to Lite Version
                                        UserInfo.security_role.Version = CurrentSession.Versions.Lite;
                                        db.SaveChangesAsync();
                                    }
                                }
                            }
                            else
                            {
                                //Do Nothing
                            }
                        }
                        else
                        {
                            //Register new company. 
                            //and update Version back into database. Save Changes.
                            CompanyInfo.version = Licence.CreateLicence(UserInfo.name_full, CompanyInfo.gov_code, CompanyInfo.name, UserInfo.email, (int)CurrentSession.Versions.Full);
                            db.SaveChangesAsync();
                        }
                    }
                }
				else
				{
					//Register new company. 
					//and update Version back into database. Save Changes.
					CompanyInfo.version = Licence.CreateLicence(UserInfo.name_full, CompanyInfo.gov_code, CompanyInfo.name, UserInfo.email, (int)CurrentSession.Versions.Full);
					db.SaveChangesAsync();
				}
			}
        }

        private void Add2Favorites(object sender, RoutedEventArgs e)
        {
            Properties.Settings Settings = new Properties.Settings();
            string _Tag = (sender as cntrl.applicationIcon).Tag.ToString();

            if (Settings.AppFavList == null)
            {
                Settings.AppFavList = new System.Collections.Specialized.StringCollection();
            }

            if (Settings.AppFavList.Contains(_Tag) == false)
            {
                Settings.AppFavList.Add(_Tag);
                Settings.Save();
            }
            else if (Settings.AppFavList.Contains(_Tag))
            {
                Settings.AppFavList.Remove(_Tag);
                Settings.Save();
            }
        }

        private void tbxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbx = sender as TextBox;
            string SearchBy = tbx.Text;

            if (SearchBy.Length >= 3)
            {
                ListApps(tbx.Text, false);
            }
            else
            {
                wrapApps.Children.Clear();
            }
        }

        private void get_Apps(object sender, EventArgs e)
        {
            string _modName;

            if (sender is cntrl.moduleIcon ico)
            {
                _modName = ico.Tag.ToString();
                //Check to see if other icon (not module) has been clicked.
                var appLocApplicationName = new LocTextExtension("Cognitivo:local:" + (sender as cntrl.moduleIcon).ModuleName.ToString() + "").SetBinding(lblModuleName, Label.ContentProperty);

                //Get Icons
                if (ico.Tag.ToString() == "Fav")
                {
                    ListFav();
                }
                else
                {
                    ListApps(_modName, true);
                }
            }
            else
            {
                _modName = (sender as TextBlock).Tag.ToString();
                rootWindow.Nav_Frame(_modName);
            }
        }

        private void ListFav()
        {
            wrapApps.Children.Clear();
            StackPanel stck = null;
            List<string> arrNamespace = new List<string>();

            Properties.Settings Settings = new Properties.Settings();
            foreach (string _Icon in Settings.AppFavList)
            {
                if (_Icon.Contains("Blank"))
                {
                    continue;
                }

                string Ico = _Icon.Replace("Cognitivo.", "");
                string SearchBy = "app like '%" + Ico + "%'";

                foreach (DataRow app in appList.dtApp.Select(SearchBy, "namespace ASC"))
                {
                    string _namespace = app["namespace"].ToString();
                    entity.CurrentSession.Versions Version = (entity.CurrentSession.Versions)Enum.Parse(typeof(entity.CurrentSession.Versions), Convert.ToString(app["Version"]));

                    if (entity.CurrentSession.Version >= Version)
                    {
                        if (arrNamespace.Contains(_namespace))
                        {
                            cntrl.applicationIcon appIcon = appList.get_AppIcon(app);
                            appIcon.Click += new cntrl.applicationIcon.ClickedEventHandler(open_App);
                            appIcon.ClickedFav += new cntrl.applicationIcon.ClickedFavEventHandler(Add2Favorites);
                            if (appIcon.HasReport)
                            {
                                appIcon.ReportClick += new cntrl.applicationIcon.ReportClickEventHandler(open_Report);
                            }
                            stck.Children.Add(appIcon);
                        }
                        else
                        {
                            stck = new StackPanel();
                            Label lbl = new Label();
                            Style style = Application.Current.FindResource("H2") as Style;
                            lbl.Style = style;
                            lbl.Foreground = Brushes.White;
                            lbl.Effect = new DropShadowEffect
                            {
                                ShadowDepth = 0,
                                BlurRadius = 2
                            };

                            stck.Children.Add(lbl);
                            var appLocApplicationName = new LocTextExtension("Cognitivo:local:" + _namespace + "").SetBinding(lbl, Label.ContentProperty);
                            cntrl.applicationIcon appIcon = appList.get_AppIcon(app);
                            appIcon.Click += new cntrl.applicationIcon.ClickedEventHandler(open_App);
                            appIcon.ClickedFav += new cntrl.applicationIcon.ClickedFavEventHandler(Add2Favorites);

                            if (appIcon.HasReport)
                            {
                                appIcon.ReportClick += new cntrl.applicationIcon.ReportClickEventHandler(open_Report);
                            }

                            stck.Children.Add(appIcon);
                            wrapApps.Children.Add(stck);

                            arrNamespace.Add(_namespace);
                        }
                    }
                }
            }
        }

        private void ListApps(string SearchBy, bool IsModule)
        {
            wrapApps.Children.Clear();
            StackPanel stck = null;
            List<string> arrNamespace = new List<string>();

            if (IsModule)
            {
                SearchBy = "module = '" + SearchBy + "'";
            }
            else
            {
                SearchBy = "app like '%" + SearchBy + "%'";
            }

            foreach (DataRow app in appList.dtApp.Select(SearchBy, "namespace ASC"))
            {
                string _namespace = app["namespace"].ToString();
                entity.CurrentSession.Versions Version = (entity.CurrentSession.Versions)Enum.Parse(typeof(entity.CurrentSession.Versions), Convert.ToString(app["Version"]));
                if (entity.CurrentSession.Version >= Version)
                {
                    if (arrNamespace.Contains(_namespace))
                    {
                        cntrl.applicationIcon appIcon = appList.get_AppIcon(app);
                        appIcon.Click += new cntrl.applicationIcon.ClickedEventHandler(open_App);
                        appIcon.ClickedFav += new cntrl.applicationIcon.ClickedFavEventHandler(Add2Favorites);
                        if (appIcon.HasReport)
                        {
                            appIcon.ReportClick += new cntrl.applicationIcon.ReportClickEventHandler(open_Report);
                        }
                        stck.Children.Add(appIcon);
                    }
                    else
                    {
                        stck = new StackPanel();
                        Label lbl = new Label();
                        Style style = Application.Current.FindResource("H2") as Style;
                        lbl.Style = style;
                        lbl.Foreground = Brushes.White;
                        lbl.Effect = new DropShadowEffect
                        {
                            ShadowDepth = 0,
                            BlurRadius = 2
                        };

                        stck.Children.Add(lbl);
                        var appLocApplicationName = new LocTextExtension("Cognitivo:local:" + _namespace + "").SetBinding(lbl, Label.ContentProperty);
                        cntrl.applicationIcon appIcon = appList.get_AppIcon(app);
                        appIcon.Click += new cntrl.applicationIcon.ClickedEventHandler(open_App);
                        appIcon.ClickedFav += new cntrl.applicationIcon.ClickedFavEventHandler(Add2Favorites);

                        if (appIcon.HasReport)
                        {
                            appIcon.ReportClick += new cntrl.applicationIcon.ReportClickEventHandler(open_Report);
                        }

                        stck.Children.Add(appIcon);
                        wrapApps.Children.Add(stck);

                        arrNamespace.Add(_namespace);
                    }
                }
            }
        }

        public void open_App(object sender, RoutedEventArgs e)
        {
            cntrl.applicationIcon appName = (sender as cntrl.applicationIcon);
            string name = appName.Tag.ToString();

            if (name.Contains("ReportDesigner"))
            {
                Window objWin = default(Window);
                Type WinInstanceType = null;

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    WinInstanceType = Type.GetType(name, true, true);
                    objWin = (Window)Activator.CreateInstance(WinInstanceType);
                    objWin.Show();
                    Cursor = Cursors.Arrow;
                }));
            }
            else if (Properties.Settings.Default.open_Window)
            {
                ApplicationWindow appWindow = new ApplicationWindow()
                {
                    PagePath = name,
                    ApplicationName = (entity.App.Names)Enum.Parse(typeof(entity.App.Names), appName.Uid, true),
                    Title = entity.Brillo.Localize.StringText(appName.Uid),
                    Icon = appName.imgSource
                };
                appWindow.Show();
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() => this.Cursor = Cursors.AppStarting));
                MainWindow rootWindow = Window.GetWindow(this) as Menu.MainWindow;
                Page objPage = default(Page);
                Type PageInstanceType = null;
                PageInstanceType = Type.GetType(name, true, true);

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    objPage = (Page)Activator.CreateInstance(PageInstanceType);
                    rootWindow.mainFrame.Navigate(objPage);
                    this.Cursor = Cursors.Arrow;
                }));
            }

            e.Handled = true;
        }

        public void open_Report(object sender, RoutedEventArgs e)
        {
            cntrl.applicationIcon appName = (sender as cntrl.applicationIcon);
            string name = "Cognitivo.Reporting.ReportViewer";
            if ((entity.App.Names)Enum.Parse(typeof(entity.App.Names), appName.Uid, true) == entity.App.Names.Subscription)
            {
                name = "Cognitivo.Reporting.Views.Subscription";
            }

            try
            {
                ApplicationWindow appWindow = new ApplicationWindow()
                {
                    PagePath = name,
                    ApplicationName = (entity.App.Names)Enum.Parse(typeof(entity.App.Names), appName.Uid, true),
                    Title = entity.Brillo.Localize.StringText(appName.Uid),
                    Icon = appName.imgSource
                };
                appWindow.Show();
            }
            catch { }

            e.Handled = true;
        }
    }
}