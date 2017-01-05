
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Cognitivo
{
    public partial class App
    {
        public App()
        {
            if (Cognitivo.Properties.Settings.Default.UpgradeSettings)
            {
                Cognitivo.Properties.Settings.Default.Upgrade();
                Cognitivo.Properties.Settings.Default.UpgradeSettings = false;
            }

            //string localAppData =
            //  Environment.GetFolderPath(
            //  Environment.SpecialFolder.LocalApplicationData);
            //string userFilePath
            //  = Path.Combine(localAppData, "MyCompany");

            //if (!Directory.Exists(userFilePath))
            //    Directory.CreateDirectory(userFilePath);

            ////if it's not already there, 
            ////copy the file from the deployment location to the folder
            //string sourceFilePath = Path.Combine(
            //  System.Windows.Forms.Application.StartupPath, "MyFile.txt");
            //string destFilePath = Path.Combine(userFilePath, "MyFile.txt");
            //if (!File.Exists(destFilePath))
            //    File.Copy(sourceFilePath, destFilePath);

            //var CmdSaveAll = new CommandBinding(Class.CustomCommands.SaveAll, HotkeyPressed);
            //CommandManager.RegisterClassCommandBinding(typeof(Window), CmdSaveAll);
            //var CmdNewAll = new CommandBinding(Class.CustomCommands.NewAll, HotkeyPressed);
            //CommandManager.RegisterClassCommandBinding(typeof(Window), CmdNewAll);
            //var CmdDeleteAll = new CommandBinding(Class.CustomCommands.DeleteAll, HotkeyPressed);
            //CommandManager.RegisterClassCommandBinding(typeof(Window), CmdDeleteAll);
            //var CmdCancelAll = new CommandBinding(Class.CustomCommands.CancelAll, HotkeyPressed);
            //CommandManager.RegisterClassCommandBinding(typeof(Window), CmdCancelAll);
            //var CmdEditAll = new CommandBinding(Class.CustomCommands.EditAll, HotkeyPressed);
            //CommandManager.RegisterClassCommandBinding(typeof(Window), CmdEditAll);
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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<entity.db, entity.Migrations.Configuration>());

            if (Cognitivo.Properties.Settings.Default.UpgradeRequired)
            {
                Cognitivo.Properties.Settings.Default.Upgrade();
                Cognitivo.Properties.Settings.Default.UpgradeRequired = false;
                Cognitivo.Properties.Settings.Default.Save();
            }
        } 
    }
}
