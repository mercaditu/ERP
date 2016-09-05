using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Data;
using WPFLocalizeExtension.Extensions;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using System.Windows.Media;

namespace Cognitivo.Menu
{
    public partial class mainMenu_Corporate : Page
    {
        AppList appList = new AppList();
        MainWindow rootWindow = Application.Current.MainWindow as MainWindow;

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

            cntrl.moduleIcon Icon = new cntrl.moduleIcon();
            Icon.Tag = "Fav";
            Icon.ModuleName = "Favorite";
            get_Apps(Icon, null);
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

            //e.Handled = true;
        }

        private void get_Apps(object sender, EventArgs e)
        {
            string _modName;

            cntrl.moduleIcon ico = sender as cntrl.moduleIcon;

            if (ico != null)
            { 
                _modName = ico.Tag.ToString();
                //Check to see if other icon (not module) has been clicked.
                lblModuleName.Content = (sender as cntrl.moduleIcon).ModuleName.ToString();
                
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

                    if (arrNamespace.Contains(_namespace))
                    {
                        cntrl.applicationIcon appIcon = appList.get_AppIcon(app);
                        appIcon.Click += new cntrl.applicationIcon.ClickedEventHandler(open_App);
                        appIcon.ClickedFav += new cntrl.applicationIcon.ClickedFavEventHandler(Add2Favorites);

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

                        stck.Children.Add(appIcon);
                        wrapApps.Children.Add(stck);

                        arrNamespace.Add(_namespace);
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

        public void open_App(object sender, RoutedEventArgs e)
        {
            cntrl.applicationIcon appName = (sender as cntrl.applicationIcon);
            string name = appName.Tag.ToString();

            if (Properties.Settings.Default.open_Window)
            {
                ApplicationWindow appWindow = new ApplicationWindow();
                appWindow.appName = name;
                appWindow.Icon = appName.imgSource;
                appWindow.Show();
            }
            else
            {
                dynamic taskAuth = Task.Factory.StartNew(() => open_PageThread(name));
            }

            e.Handled = true;
        }

        public void open_Report(object sender, RoutedEventArgs e)
        {
            cntrl.applicationIcon appName = (sender as cntrl.applicationIcon);
            //string name = appName.Tag.ToString();

            string name = "Cognitivo.Reporting.Views." + appName.Uid;

            try
            {
                ApplicationWindow appWindow = new ApplicationWindow();
                appWindow.appName = name;
                appWindow.Icon = appName.imgSource;
                appWindow.Show();
            }
            catch { }

            e.Handled = true;
        }

        private void open_PageThread(string appName)
        {
            Dispatcher.BeginInvoke((Action)(() => this.Cursor = Cursors.AppStarting));
            MainWindow rootWindow = App.Current.MainWindow as MainWindow;
            Page objPage = default(Page);
            Type PageInstanceType = null;
            PageInstanceType = Type.GetType(appName, true, true);

            Dispatcher.BeginInvoke((Action)(() =>
            {
                objPage = (Page)Activator.CreateInstance(PageInstanceType);
                rootWindow.mainFrame.Navigate(objPage);
                this.Cursor = Cursors.Arrow;
            }));
        }
    }
}