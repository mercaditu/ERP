using entity;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity.Brillo;
using System.ComponentModel;

namespace cntrl.Panels
{
    public partial class pnl_ItemMovement : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private CollectionViewSource item_inventory_detailViewSource;
        public db InventoryDB { get; set; }
        public List<item_inventory_detail> item_inventoryList { get; set; }
        // List<item_inventory_detail> item_inventoryDetailList = new List<item_inventory_detail>();
        public List<StockList> Items_InStockLIST { get; set; }

        public pnl_ItemMovement()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            if (Items_InStockLIST != null)
            {
                    if (Items_InStockLIST.FirstOrDefault() != null)
                    {
                        item_inventoryList.FirstOrDefault().value_system = (decimal)Items_InStockLIST.FirstOrDefault().Quantity;
                        item_inventoryList.FirstOrDefault().batch_code = Items_InStockLIST.FirstOrDefault().BatchCode;
                        item_inventoryList.FirstOrDefault().expire_date = Items_InStockLIST.FirstOrDefault().ExpiryDate;
                        item_inventoryList.FirstOrDefault().unit_value = (decimal)Items_InStockLIST.FirstOrDefault().Cost;
                        item_inventoryList.FirstOrDefault().timestamp = Items_InStockLIST.FirstOrDefault().TranDate;
                        item_inventoryList.FirstOrDefault().item_inventory_dimension.Clear();
                        int MovementID = (int)Items_InStockLIST.FirstOrDefault().MovementID;
                        if (MovementID > 0)
                        {
                            if (InventoryDB.item_movement.Where(x => x.id_movement == MovementID).FirstOrDefault() != null)
                            {
                                item_movement item_movement = InventoryDB.item_movement.Where(x => x.id_movement == MovementID).FirstOrDefault();
                                if (item_movement.item_movement_dimension != null)
                                {

                                    foreach (item_movement_dimension item_movement_dimension in item_movement.item_movement_dimension)
                                    {
                                        item_inventory_dimension item_inventory_dimension = new item_inventory_dimension();
                                        item_inventory_dimension.id_dimension = item_movement_dimension.id_dimension;
                                        item_inventory_dimension.value = item_movement_dimension.value;
                                        item_inventoryList.FirstOrDefault().item_inventory_dimension.Add(item_inventory_dimension);
                                    }
                                }
                            }
                        }


                        if (InventoryDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault() != null)
                        {
                            item_inventoryList.FirstOrDefault().id_currencyfx = InventoryDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault().id_currencyfx;
                            item_inventoryList.FirstOrDefault().currency = InventoryDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault().app_currency.name;
                        }
                    }
              //  }
                for (int i = item_inventoryList.Count(); i < Items_InStockLIST.Count(); i++)
                {
                    item_inventory_detail item_inventory_detail;
                    StockList item = Items_InStockLIST.ElementAt(i);

                    item_inventory_detail = new item_inventory_detail();


                    item_inventory_detail.id_inventory = item_inventoryList.FirstOrDefault().id_inventory;
                    item_inventory_detail.value_system = (decimal)item.Quantity;
                    item_inventory_detail.id_item_product = item_inventoryList.FirstOrDefault().id_item_product;
                    item_inventory_detail.item_product = item_inventoryList.FirstOrDefault().item_product;
                    item_inventory_detail.id_location = item_inventoryList.FirstOrDefault().id_location;
                    item_inventory_detail.expire_date = item.ExpiryDate;
                    item_inventory_detail.batch_code = item.BatchCode;




                    item_inventory_detail.IsSelected = true;
                    item_inventory_detail.State = EntityState.Added;
                    item_inventory_detail.unit_value = (decimal)item.Cost;
                    item_inventory_detail.timestamp = item.TranDate;


                    if (InventoryDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault() != null)
                    {
                        item_inventory_detail.id_currencyfx = InventoryDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault().id_currencyfx;
                        item_inventory_detail.currency = InventoryDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault().app_currency.name;
                    }


                    item_inventoryList.FirstOrDefault().item_inventory.item_inventory_detail.Add(item_inventory_detail);
                    item_inventory_detail.item_inventory_dimension.Clear();
                    if (item.MovementID > 0)
                    {
                        if (InventoryDB.item_movement.Where(x => x.id_movement == item.MovementID).FirstOrDefault() != null)
                        {
                            item_movement item_movement = InventoryDB.item_movement.Where(x => x.id_movement == item.MovementID).FirstOrDefault();
                            if (item_movement.item_movement_dimension != null)
                            {

                                foreach (item_movement_dimension item_movement_dimension in item_movement.item_movement_dimension)
                                {
                                    item_inventory_dimension item_inventory_dimension = new item_inventory_dimension();
                                    item_inventory_dimension.id_dimension = item_movement_dimension.id_dimension;
                                    item_inventory_dimension.value = item_movement_dimension.value;
                                    item_inventory_detail.movement_id = item.MovementID;
                                    item_inventory_detail.item_inventory_dimension.Add(item_inventory_dimension);
                                }
                            }
                        }
                    }
                }

               

            }


            int id_item_product = item_inventoryList.FirstOrDefault().id_item_product;
            int id_location = item_inventoryList.FirstOrDefault().id_location;
            item_inventory_detailViewSource = FindResource("item_inventory_detailViewSource") as CollectionViewSource;

            item_inventory_detailViewSource.Source = item_inventoryList.FirstOrDefault().item_inventory.item_inventory_detail.Where(x=>x.id_item_product== id_item_product && x.id_location == id_location).ToList();
            CollectionViewSource app_dimensionViewSource = FindResource("app_dimensionViewSource") as CollectionViewSource;
            InventoryDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            app_dimensionViewSource.Source = InventoryDB.app_dimension.Local;
        }

        private void item_transfer_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            item_inventory_detail item_inventory_detail = e.NewItem as item_inventory_detail;
            if (item_inventory_detail != null)
            {
                add_item(item_inventory_detail);
            }
        }

        public void add_item(item_inventory_detail item_inventory_detail)
        {
            item_inventory_detail.id_inventory = item_inventoryList.FirstOrDefault().id_inventory;
            item_inventory_detail.id_item_product = item_inventoryList.FirstOrDefault().id_item_product;
            item_inventory_detail.item_product = item_inventoryList.FirstOrDefault().item_product;
            item_inventory_detail.id_location = item_inventoryList.FirstOrDefault().id_location;
            item_inventory_detail.IsSelected = true;
            item_inventory_detail.State = EntityState.Added;
            item_inventory_detail.timestamp = item_inventoryList.FirstOrDefault().item_inventory.trans_date;
            app_currencyfx app_currencyfx = InventoryDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault();
            if (app_currencyfx != null)
            {
                item_inventory_detail.id_currencyfx = app_currencyfx.id_currencyfx;
                item_inventory_detail.currency = app_currencyfx.app_currency.name;
            }

            item_inventoryList.FirstOrDefault().item_inventory.item_inventory_detail.Add(item_inventory_detail);
            if (item_inventory_detail.id_item_product > 0)
            {
                if (InventoryDB.item_product.Where(x => x.id_item_product == item_inventory_detail.id_item_product).FirstOrDefault() != null)
                {
                    item_product item_product = InventoryDB.item_product.Where(x => x.id_item_product == item_inventory_detail.id_item_product).FirstOrDefault();
                    if (InventoryDB.item_dimension.Where(x => x.id_item == item_product.id_item).ToList() != null)
                    {
                        List<item_dimension> item_dimensionList = InventoryDB.item_dimension.Where(x => x.id_item == item_product.id_item).ToList();
                        foreach (item_dimension item_dimension in item_dimensionList)
                        {
                            item_inventory_dimension item_inventory_dimension = new item_inventory_dimension();
                            item_inventory_dimension.id_dimension = item_dimension.id_app_dimension;
                            item_inventory_dimension.value = item_dimension.value;
                            item_inventory_detail.item_inventory_dimension.Add(item_inventory_dimension);
                        }
                    }
                }
            }
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
            item_inventoryList = item_inventory_detailViewSource.View.OfType<item_inventory_detail>().Where(x => x.value_system > 0).ToList();
            btnCancel_Click(sender, null);
        }


    }
}