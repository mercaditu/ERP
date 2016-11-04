using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for LocationRecords.xaml
    /// </summary>
    public partial class Location : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource locationViewSource;

        public Location()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            locationViewSource = ((CollectionViewSource)(this.FindResource("app_locationViewSource")));
            entity.db.app_location.Where(x => x.id_company == CurrentSession.Id_Company)
                .Include(x => x.app_branch)
                .OrderByDescending(a => a.is_active)
                .Load();
            locationViewSource.Source = entity.db.app_location.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.location objLocation = new cntrl.location();
            app_location app_location = new app_location();
            entity.db.app_location.Add(app_location);
            locationViewSource.View.MoveCurrentToLast();
            objLocation.app_locationViewSource = locationViewSource;
            objLocation._entity = entity;
            crud_modal.Children.Add(objLocation);
        }

        private void pnl_location_linkEdit_Click(object sender, int intLocationId)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.location objLocation = new cntrl.location();
            locationViewSource.View.MoveCurrentTo(entity.db.app_location.Where(x => x.id_location == intLocationId).FirstOrDefault());
            objLocation.app_locationViewSource = locationViewSource;
            objLocation._entity = entity;
            crud_modal.Children.Add(objLocation);
        }
    }
}
