using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;


namespace Cognitivo.Configs
{
    public partial class Settings : Page
    {
        entity.dbContext _entity = new entity.dbContext();
        CollectionViewSource app_companyViewSource, security_userViewSource;

        Menu.MainWindow mainWindow = App.Current.MainWindow as Menu.MainWindow;

        public Settings()
        {
            InitializeComponent();
            app_companyViewSource = ((CollectionViewSource)(this.FindResource("app_companyViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            _entity.db.app_company.Include("app_branch").Load();
            app_companyViewSource.Source = _entity.db.app_company.Local;
            security_userViewSource = ((CollectionViewSource)(this.FindResource("security_userViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            _entity.db.security_user.Load();
            security_userViewSource.Source = _entity.db.security_user.Local;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            entity.Properties.Settings.Default.Save();
            mainWindow.mainFrame.NavigationService.GoBack();
        }
    }
}
