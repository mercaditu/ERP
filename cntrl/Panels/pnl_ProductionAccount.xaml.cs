using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using System.Data;
using entity;
using System.Data.Entity.Validation;
using System.Windows.Input;

namespace cntrl.Panels
{
    /// <summary>
    /// Interaction logic for pnl_ItemMovement.xaml
    /// </summary>
    public partial class pnl_ProductionAccount : UserControl
    {
        CollectionViewSource production_accountViewSource;
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
            ExecutionDB.production_service_account.Where(a => a.id_company == CurrentSession.Id_Company &&  (a.debit-a.child.Sum(x=>x.credit))> production_execution_detail.quantity).Load();
            production_accountViewSource.Source = ExecutionDB.production_service_account.Local;


          



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
