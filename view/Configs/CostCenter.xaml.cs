using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;

namespace Cognitivo.Configs
{
    public partial class CostCenter : Page
    {
        dbContext entity = new dbContext();
        CollectionViewSource cost_centerViewSource;

        public CostCenter()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cost_centerViewSource = ((CollectionViewSource)(this.FindResource("app_cost_centerViewSource")));
            entity.db.app_cost_center.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
            cost_centerViewSource.Source = entity.db.app_cost_center.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.cost_center cost_center = new cntrl.Curd.cost_center();
            app_cost_center app_cost_center = new app_cost_center();
            entity.db.app_cost_center.Add(app_cost_center);
            cost_centerViewSource.View.MoveCurrentToLast();
            cost_center.app_cost_centerViewSource = cost_centerViewSource;
            cost_center.entity = entity;
            crud_modal.Children.Add(cost_center);
        }

        private void pnl_CostCenter_Click(object sender, int intCostCenterId)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.cost_center cost_center = new cntrl.Curd.cost_center();
            cost_centerViewSource.View.MoveCurrentTo(entity.db.app_cost_center.Where(x => x.id_cost_center == intCostCenterId).FirstOrDefault());
            cost_center.app_cost_centerViewSource = cost_centerViewSource;
            cost_center.entity = entity;
            crud_modal.Children.Add(cost_center);
        }
    }
}
