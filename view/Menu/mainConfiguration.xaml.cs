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
        MainWindow rootWindow = Application.Current.MainWindow as MainWindow;

        public mainConfiguration()
        {
            InitializeComponent();
        }

        private void config_MouseUp(object sender, EventArgs e)
        {
            TextBlock tbxConfig = (TextBlock)sender;
            string configName = tbxConfig.Tag.ToString();
            dynamic taskAuth = Task.Factory.StartNew(() => load_Thread(configName));
        }

        private void load_Thread(string configName)
        {
            Dispatcher.BeginInvoke((Action)(() => Cursor = Cursors.AppStarting));
            Page objPage = default(Page);
            Type PageInstanceType = null;

            string _app = string.Empty;
            string _namespace = string.Empty;
            string _img = string.Empty;

            AppList appList = new AppList();
            foreach (DataRow app in appList.dtApp.Select("name = '" + configName + "'"))
            {
                _app = app["app"].ToString();
                _img = app["img"].ToString();

                _img = "../Images/Application/128/" + _img + ".png";
                configName = "Cognitivo." + _app;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {
                    PageInstanceType = Type.GetType(configName, true, true);
                    objPage = (Page)Activator.CreateInstance(PageInstanceType);
                    //objPage.Tag = tag;
                    objPage.Tag = 0;
                    rootWindow.mainFrame.Navigate(objPage);
                }
                catch
                {
                    MessageBox.Show("a");
                }
                finally { Cursor = Cursors.Arrow; }
            }));
        }

        private void showMenu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            rootWindow.mainFrame.Navigate(null);
        }

       

   
    }
}
