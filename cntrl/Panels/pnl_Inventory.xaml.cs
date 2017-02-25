using entity;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Panels
{
    public partial class pnl_Inventory : UserControl
    {
        private CollectionViewSource item_inventory_detailViewSource;
        public InventoryDB InventoryDB { get; set; }
        public List<item_inventory_detail> item_inventoryList { get; set; }

        public pnl_Inventory()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            InitializeComponent();
        }

        private void item_transfer_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            item_inventory_detail item_inventory_detail = e.NewItem as item_inventory_detail;
            add_item(item_inventory_detail);
        }

        public void add_item(item_inventory_detail item_inventory_detail)
        {
            item_inventory_detail.id_inventory = item_inventoryList.FirstOrDefault().id_inventory;
            item_inventory_detail.value_system = item_inventoryList.FirstOrDefault().value_system;
            item_inventory_detail.id_item_product = item_inventoryList.FirstOrDefault().id_item_product;
            item_inventory_detail.item_product = item_inventoryList.FirstOrDefault().item_product;
            item_inventory_detail.id_location = item_inventoryList.FirstOrDefault().id_location;
            item_inventory_detail.IsSelected = true;
            item_inventory_detail.State = EntityState.Added;
            item_inventory_detail.timestamp = item_inventoryList.FirstOrDefault().item_inventory.trans_date;

            if (InventoryDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault() != null)
            {
                item_inventory_detail.id_currencyfx = InventoryDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault().id_currencyfx;
            }

            item_inventoryList.FirstOrDefault().item_inventory.item_inventory_detail.Add(item_inventory_detail);
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            item_inventory_detailViewSource = ((CollectionViewSource)(FindResource("item_inventory_detailViewSource")));
            item_inventory_detailViewSource.Source = item_inventoryList;

          
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
            item_inventoryList = item_inventory_detailViewSource.View.OfType<item_inventory_detail>().ToList();
            btnCancel_Click(sender, null);
        }
    }
}