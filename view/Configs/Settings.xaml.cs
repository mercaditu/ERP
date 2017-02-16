using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    public partial class Settings : Page
    {
        private CollectionViewSource app_companyViewSource;

        private Menu.MainWindow mainWindow = App.Current.MainWindow as Menu.MainWindow;

        private entity.db db = new entity.db();

        public Settings()
        {
            InitializeComponent();

            app_companyViewSource = ((CollectionViewSource)(this.FindResource("app_companyViewSource")));
            db.app_company.Include(v => v.app_branch).LoadAsync();

            app_companyViewSource.Source = db.app_company.Local;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (app_companyViewSource.View != null)
            {
                int new_CompanyID = ((entity.app_company)app_companyViewSource.View.CurrentItem).id_company;

                entity.Properties.Settings.Default.Save();

                if (new_CompanyID != entity.CurrentSession.Id_Company &&
                    MessageBox.Show(entity.Brillo.Localize.StringText("Applicationrestartwillberequired"), "Cognitivo ERP",
                    MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK) == MessageBoxResult.OK)
                {
                    //Restart Application because Company has changed.
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
                else
                {
                    entity.CurrentSession.Id_Company = entity.Properties.Settings.Default.company_ID;
                    entity.CurrentSession.Id_Branch = entity.Properties.Settings.Default.branch_ID;
                    entity.CurrentSession.Id_Terminal = entity.Properties.Settings.Default.terminal_ID;
                    entity.CurrentSession.Id_Account = entity.Properties.Settings.Default.account_ID;
                    entity.Properties.Settings.Default.Save();

                    //GoBack with changed data.
                    mainWindow.mainFrame.NavigationService.GoBack();
                }
            }
        }
    }
}