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
    public partial class pnl_ItemMovement : UserControl
    {
        CollectionViewSource item_inventory_detailViewSource;
        public InventoryDB InventoryDB { get; set; }
        //public int id_item_product { get; set; }
        //public int id_location { get; set; }
        //public int id_inventory_detail { get; set; }
        //public int id_inventory { get; set; }
        //public decimal quantity { get; set; }
        //public decimal system_quantity { get; set; }
        //public DateTime Trans_date { get; set; }
        public List<item_inventory_detail> item_inventoryList { get; set; }
        public pnl_ItemMovement()
        {
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
            //   item_inventory_detail.item_inventory = item_inventoryList.FirstOrDefault().item_inventory;
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
                          //  item_inventory_dimension.id_measurement = item_dimension.id_measurement;
                            item_inventory_detail.item_inventory_dimension.Add(item_inventory_dimension);
                        }


                    }
                }

            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            item_inventory_detailViewSource = ((CollectionViewSource)(FindResource("item_inventory_detailViewSource")));
            // InventoryDB.item_inventory_detail.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            item_inventory_detailViewSource.Source = item_inventoryList;


            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            InventoryDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            app_dimensionViewSource.Source = InventoryDB.app_dimension.Local;

            //filter_detail();

            //if (item_inventory_detailViewSource.View.OfType<item_inventory_detail>().Count() == 0)
            //{
            //    item_inventory_detail item_inventory_detail = new item_inventory_detail();
            //    add_item(item_inventory_detail);
            //    InventoryDB.item_inventory_detail.Add(item_inventory_detail);
            //}




        }
        //public void filter_detail()
        //{
        //    if (id_inventory > 0)
        //    {


        //        if (item_inventory_detailViewSource != null)
        //        {
        //            if (item_inventory_detailViewSource.View != null)
        //            {

        //                item_inventory_detailViewSource.View.Filter = i =>
        //                {
        //                    item_inventory_detail item_inventory_detail = (item_inventory_detail)i;
        //                    if (item_inventory_detail.id_inventory_detail > 0)
        //                    {
        //                        if (item_inventory_detail.id_inventory == id_inventory && item_inventory_detail.id_location == id_location)
        //                            return true;
        //                        else
        //                            return false;
        //                    }
        //                    else
        //                        return true;
        //                };

        //            }
        //        }
        //    }
        //}
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
            //  quantity = item_inventoryList.Sum(y => y.value_counted);
            //ProductMovementDB.SaveChanges();
            btnCancel_Click(sender, null);
        }






    }
}
