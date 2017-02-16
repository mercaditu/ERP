using entity;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Panels
{
    /// <summary>
    /// Interaction logic for pnl_ItemMovement.xaml
    /// </summary>
    public partial class pnl_ProductionAccount : UserControl
    {
        private CollectionViewSource production_accountViewSource;
        public ExecutionDB ExecutionDB { get; set; }
        public production_execution_detail production_execution_detail { get; set; }
        public List<production_service_account> production_accountList { get; set; }

        public pnl_ProductionAccount()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            production_accountViewSource = ((CollectionViewSource)(FindResource("production_accountViewSource")));
            List<production_service_account> production_service_accountList = ExecutionDB.production_service_account.Where(a => a.id_company == CurrentSession.Id_Company).ToList();
            production_accountViewSource.Source = production_service_accountList.Where(a => a.Balance >= production_execution_detail.quantity).ToList();
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            item_inventory_detailDataGrid.CancelEdit();
            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            production_accountList = production_accountViewSource.View.OfType<production_service_account>().ToList();
            //  quantity = item_inventoryList.Sum(y => y.value_counted);
            //ProductMovementDB.SaveChanges();
            btnCancel_Click(sender, null);
        }
    }
}