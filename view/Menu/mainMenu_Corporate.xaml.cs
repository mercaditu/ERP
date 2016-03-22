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

        public mainMenu_Corporate()
        {
            InitializeComponent();
            if (rootWindow.mainFrame.CanGoBack)
            {
                rootWindow.mainFrame.RemoveBackEntry();
            }
        }

        private void get_Apps(object sender, EventArgs e)
        {
            wrapApps.Children.Clear();

            string _modName;

            if (sender as cntrl.moduleIcon != null)
            { _modName = (sender as cntrl.moduleIcon).Tag.ToString(); }
            else
            { _modName = (sender as TextBlock).Tag.ToString(); }

            List<string> arrNamespace = new List<string>();

            //Check to see if other icon (not module) has been clicked.
            rootWindow.Nav_Frame(_modName);

            if (sender is cntrl.moduleIcon)
            {
                lblModuleName.Content = (sender as cntrl.moduleIcon).ModuleName.ToString();
                StackPanel stck = null;
                foreach (DataRow app in appList.dtApp.Select("module = '" + _modName + "'", "namespace ASC"))
                {

                    string _namespace = app["namespace"].ToString();
                    if (arrNamespace.Contains(_namespace))
                    {
                        cntrl.applicationIcon appIcon = appList.get_AppIcon(app);
                        appIcon.Click += new cntrl.applicationIcon.ClickedEventHandler(open_App);
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
            if (Cognitivo.Properties.Settings.Default.open_Window)
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