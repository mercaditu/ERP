using System.Windows;
using System.Windows.Controls;


namespace Cognitivo.Configs
{
    public partial class Status : Page
    {
        entity.dbContext entity = new entity.dbContext();
        //CollectionViewSource app_statusViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();

        public Status()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //app_statusViewSource = ((CollectionViewSource)(this.FindResource("app_statusViewSource")));
            //entity.db.app_status.Where(a=>a.id_company == _entity.company_ID).OrderByDescending(a => a.is_active).Load();
            //app_statusViewSource.Source = entity.db.app_status.Local;
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pnl_Status_Edit_Click(object sender, int intStatusId)
        {

        }
    }
}
