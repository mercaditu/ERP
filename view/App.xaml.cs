using Cognitivo.Menu;
using InteractivePreGeneratedViews;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
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
            var CmdSaveAll = new CommandBinding(Class.CustomCommands.SaveAll, HotkeyPressed);
            CommandManager.RegisterClassCommandBinding(typeof(Window), CmdSaveAll);
            var CmdNewAll = new CommandBinding(Class.CustomCommands.NewAll, HotkeyPressed);
            CommandManager.RegisterClassCommandBinding(typeof(Window), CmdNewAll);
            var CmdDeleteAll = new CommandBinding(Class.CustomCommands.DeleteAll, HotkeyPressed);
            CommandManager.RegisterClassCommandBinding(typeof(Window), CmdDeleteAll);
            var CmdCancelAll = new CommandBinding(Class.CustomCommands.CancelAll, HotkeyPressed);
            CommandManager.RegisterClassCommandBinding(typeof(Window), CmdCancelAll);
            var CmdEditAll = new CommandBinding(Class.CustomCommands.EditAll, HotkeyPressed);
            CommandManager.RegisterClassCommandBinding(typeof(Window), CmdEditAll);
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
                ApplicationWindow AppWin = Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) as ApplicationWindow;
                eBox.ex = e.Exception;

                eBox.Show();
                e.Handled = true;
            }
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            //Read External File.
            if (entity.Brillo.IO.FileExists(entity.CurrentSession.ApplicationFile_Path + "Entity\\ConnString.txt"))
            {
                string ConnName = "Cognitivo.Properties.Settings.MySQLconnString";
                string ConnString = System.IO.File.ReadAllText(@entity.CurrentSession.ApplicationFile_Path + "Entity\\ConnString.txt");

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.ConnectionStrings.ConnectionStrings[ConnName].ConnectionString = ConnString;
                config.Save(ConfigurationSaveMode.Modified, true);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            else
            {
            // do nothing
            }

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

            MainWindow MainWin = new MainWindow();

            try
            {
                using (entity.db db = new entity.db())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.AutoDetectChangesEnabled = false;
                    
                    if (entity.Brillo.IO.FileExists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\Entity\\View.xml") == false)
                    {
                        InteractiveViews.SetViewCacheFactory(db,
                            new FileViewCacheFactory(entity.Brillo.IO.CreateIfNotExists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\Entity\\View.xml")));
                    }

                    if (db.Database.Exists() == false)
                    {
                        //If database does not exist, then send to StartUp Page to decide if to change connection string or create database.
                        //MainWin.mainFrame.Navigate(new StartUp());
                        db.Database.CreateIfNotExists();
                        MainWin.mainFrame.Navigate(new MainSetup());
                    }
                    else
                    {
                        //Normal Login
                        await db.app_company.Select(x => x.id_company).FirstOrDefaultAsync();
                        MainWin.mainFrame.Navigate(new MainLogIn());
                    }
                }
            }
            catch
            {
                //If Server cannot be found, launch the ConnectionBuilder page to set it up.
                MainWin.mainFrame.Navigate(new Configs.ConnectionBuilder());
            }


            MainWin.Show();
            splash.Close();
        }
    }
}