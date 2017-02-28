using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cognitivo.Menu
{
    public partial class mainConfiguration : Page
    {
        public mainConfiguration()
        {
            InitializeComponent();
        }

        private void config_MouseUp(object sender, EventArgs e)
        {
            //Get the clicked Icon.
            TextBlock tbxConfig = (TextBlock)sender;
            string configName = tbxConfig.Tag.ToString();

            try
            {
                ///Check existance of Security. If existance is not there, will go into Catch.
                ///If it goes into catch, we will need to update the Tag of the Icon in XAML to be the same as the Enum.

                entity.Brillo.Security security = new entity.Brillo.Security((entity.App.Names)Enum.Parse(typeof(entity.App.Names), configName, true));
                if (security.view == true)
                {
                    //Start Thread to open the Page under same frame.
                    dynamic taskAuth = Task.Factory.StartNew(() => load_Thread(configName));
                }
            }
            catch
            {
                //MessageBox.Show(configName);
                dynamic taskAuth = Task.Factory.StartNew(() => load_Thread(configName));
            }
        }

        private void load_Thread(string configName)
        {
            Dispatcher.BeginInvoke((Action)(() => Cursor = Cursors.AppStarting));
            Type Object = null;

            string _app = string.Empty;
            string _namespace = string.Empty;

            AppList appList = new AppList();
            foreach (DataRow app in appList.dtApp.Select("name = '" + configName + "'"))
            {
                _app = app["app"].ToString();
                configName = "Cognitivo." + _app;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {
                    Page objPage = default(Page);

                    Object = Type.GetType(configName, true, true);
                    objPage = (Page)Activator.CreateInstance(Object);
                    objPage.Tag = 0;

                    MainWindow rootWindow = Application.Current.MainWindow as MainWindow;
                    rootWindow.mainFrame.Navigate(objPage);
                }
                catch
                {
                    MessageBox.Show(configName);
                }
                finally
                {
                    Cursor = Cursors.Arrow;
                }
            }));
        }

        private void showMenu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow rootWindow = Application.Current.MainWindow as MainWindow;
            rootWindow.mainFrame.Navigate(null);
        }
    }
}