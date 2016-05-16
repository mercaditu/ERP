using System;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Cognitivo.Menu
{
    public partial class ApplicationWindow : MetroWindow
    {
        public string appName { get; set; }
        public int apptag { get; set; }

        public ApplicationWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (appName != string.Empty)
            { Task taskAuth = Task.Factory.StartNew(() => open_PageThread(appName)); }
            else 
            { this.Close(); }
        }

        private void open_PageThread(string appName)
        {
            Dispatcher.BeginInvoke((Action)(() => this.Cursor = Cursors.AppStarting));

            if (appName.Contains("Report"))
            {
                Window objWindow = default(Window);
                Type WindowInstanceType = null;

                Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WindowInstanceType = Type.GetType(appName, true, true);
                        objWindow = (Window)Activator.CreateInstance(WindowInstanceType);
                        objWindow.Show();
                        objWindow.Tag = apptag;
                        Cursor = Cursors.Arrow;
                        this.Close();
                    }
                    ));
            }
            else
            {
                try
                {
                    Page objPage = default(Page);
                    Type PageInstanceType = null;

                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        PageInstanceType = Type.GetType(appName, true, true);
                        objPage = (Page)Activator.CreateInstance(PageInstanceType);
                        objPage.Tag = apptag;
                        mainFrame.Navigate(objPage);
                        Cursor = Cursors.Arrow;

                        if (objPage.Title != null)
                        {
                            Title = objPage.Title;
                        }
                    }));
                }
                catch { }
            }
        }

        private void mainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            mainFrame.NavigationService.RemoveBackEntry();
        }

        //Handle Page Closing - Happy
        public interface ICanClose
        {
            bool CanClose();
        }
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ICanClose canClose = mainFrame.Content as ICanClose;

            if (canClose != null && !canClose.CanClose())
                e.Cancel = true;
        }
    }
}
