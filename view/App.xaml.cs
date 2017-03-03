using Cognitivo.Menu;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Cognitivo
{
    public partial class App : Application
    {
        public App()
        {
            if (Cognitivo.Properties.Settings.Default.UpgradeSettings)
            {
                Cognitivo.Properties.Settings.Default.Upgrade();
                Cognitivo.Properties.Settings.Default.UpgradeSettings = false;
            }
        }

        private void HotkeyPressed(object sender, ExecutedRoutedEventArgs e)
        {
            //Code To Affect Current Window Only
            Menu.ApplicationWindow AppWin = Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive && x.Name == "winApplicationWindow") as Menu.ApplicationWindow;
            if (AppWin != null)
            {
                System.Windows.Controls.Page contentPage = AppWin.mainFrame.Content as System.Windows.Controls.Page;
                if (contentPage != null)
                {
                    cntrl.toolBar toolBar = contentPage.FindName("toolBar") as cntrl.toolBar;
                    if (toolBar != null)
                    {
                        string commandName = ((RoutedCommand)e.Command).Name;
                        if (commandName == "Save")
                            toolBar.btnSave_MouseUp(null, null);
                        if (commandName == "New")
                            toolBar.btnNew_MouseUp(null, null);
                        if (commandName == "DeleteMain")
                            toolBar.btnDelete_MouseUp(null, null);
                        if (commandName == "Cancel")
                            toolBar.btnCancel_MouseUp(null, null);
                        if (commandName == "Edit")
                            toolBar.btnEdit_MouseUp(null, null);
                    }
                }
            }
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Handled)
            {
                //Do Nothing
            }
            else
            {
                ExceptionBox eBox = new ExceptionBox();
                Menu.ApplicationWindow AppWin = Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) as Menu.ApplicationWindow;
                eBox.ex = e.Exception;

                eBox.Show();
                e.Handled = true;
            }
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<entity.db, entity.Migrations.Configuration>());

            if (Cognitivo.Properties.Settings.Default.UpgradeRequired)
            {
                Cognitivo.Properties.Settings.Default.Upgrade();
                Cognitivo.Properties.Settings.Default.UpgradeRequired = false;
                Cognitivo.Properties.Settings.Default.Save();
            }

            Menu.SplashScreen splash = new Menu.SplashScreen();
            splash.Show();
            //Task taskAuth = Task.Factory.StartNew(() => check_createdb(splash));

            using (entity.db db = new entity.db())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                MainWindow MainWin = new MainWindow();
                
                if (db.Database.Exists() == false)
                {
                    MainWin.mainFrame.Navigate(new StartUp()); //}));
                }
                else
                {
                    await db.app_company.FirstOrDefaultAsync();
                    MainWin.mainFrame.Navigate(new mainLogIn());// }));
                }

                MainWin.Show();
                splash.Close();
            }
        }
    }
}