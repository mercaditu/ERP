using MahApps.Metro.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cognitivo.Menu
{
    public partial class ApplicationWindow : MetroWindow
    {
        public entity.App.Names ApplicationName { get; set; }
        public string PagePath { get; set; }
        public int apptag { get; set; }

        public ApplicationWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (PagePath.ToString() != string.Empty)
            { Task taskAuth = Task.Factory.StartNew(() => open_PageThread(PagePath.ToString())); }
            else
            { this.Close(); }
        }

        private void open_PageThread(string PagePath)
        {
            Dispatcher.BeginInvoke((Action)(() => this.Cursor = Cursors.AppStarting));

            Page objPage = default(Page);
            Type PageInstanceType = null;

            Dispatcher.BeginInvoke((Action)(() =>
            {
                PageInstanceType = Type.GetType(PagePath, true, true);
                objPage = (Page)Activator.CreateInstance(PageInstanceType);
                objPage.Tag = apptag;
                mainFrame.Navigate(objPage);
                Cursor = Cursors.Arrow;
            }));
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