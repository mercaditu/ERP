using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;

namespace Cognitivo.Configs
{
    public partial class Settings : Page
    {
        int CompanyID = entity.Properties.Settings.Default.company_ID;
        CollectionViewSource app_companyViewSource;

        Menu.MainWindow mainWindow = App.Current.MainWindow as Menu.MainWindow;

        entity.db db = new entity.db();

        public Settings()
        {
            InitializeComponent();

            app_companyViewSource = ((CollectionViewSource)(this.FindResource("app_companyViewSource")));
            db.app_company.Include(v => v.app_branch).Load();

            app_companyViewSource.Source = db.app_company.Local;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (app_companyViewSource.View != null)
            {
                int new_CompanyID = ((entity.app_company)app_companyViewSource.View.CurrentItem).id_company;

                entity.Properties.Settings.Default.Save();

                if (new_CompanyID != CompanyID &&
                    MessageBox.Show(entity.Brillo.Localize.StringText("Applicationrestartwillberequired"), "Cognitivo ERP",
                    MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK) == MessageBoxResult.OK)
                {
                    //Restart Application because Company has changed.
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
                else
                {
                    //GoBack with changed data.
                    mainWindow.mainFrame.NavigationService.GoBack();
                }
            }
        }
    }
}
