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
    public partial class pnl_FractionOrder : UserControl
    {
        CollectionViewSource production_order_detailViewSource;
        public OrderDB OrderDB { get; set; }

        public List<production_order_detail> production_order_detailList { get; set; }
        public pnl_FractionOrder()
        {
            InitializeComponent();
        }

        private void item_transfer_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            production_order_detail production_order_detail = e.NewItem as production_order_detail;
            add_item(production_order_detail);
        }

        public void add_item(production_order_detail production_order_detail)
        {

            production_order_detail.quantity = production_order_detailList.FirstOrDefault().quantity;
            production_order_detail.id_item = production_order_detailList.FirstOrDefault().id_item;
            production_order_detail.item = production_order_detailList.FirstOrDefault().item;

            production_order_detail.IsSelected = true;
            production_order_detail.State = EntityState.Added;

            production_order_detailList.FirstOrDefault().production_order.production_order_detail.Add(production_order_detail);
            if (production_order_detail.id_project_task > 0)
            {
                if (OrderDB.project_task.Where(x => x.id_project_task == production_order_detail.id_project_task).FirstOrDefault() != null)
                {
                    project_task project_task = OrderDB.project_task.Where(x => x.id_project_task == production_order_detail.id_project_task).FirstOrDefault();
                    if (OrderDB.project_task_dimension.Where(x => x.id_project_task == project_task.id_project_task).ToList() != null)
                    {
                        List<project_task_dimension> project_task_dimensionList = OrderDB.project_task_dimension.Where(x => x.id_project_task == project_task.id_project_task).ToList();
                        foreach (project_task_dimension project_task_dimension in project_task_dimensionList)
                        {
                            production_order_dimension production_order_dimension = new production_order_dimension();
                            production_order_dimension.id_dimension = project_task_dimension.id_dimension;
                            production_order_dimension.value = project_task_dimension.value;
                            production_order_detail.production_order_dimension.Add(production_order_dimension);
                        }


                    }
                }

            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            production_order_detailViewSource = ((CollectionViewSource)(FindResource("production_order_detailViewSource")));
           // InventoryDB.item_inventory_detail.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            production_order_detailViewSource.Source = production_order_detailList;


            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            OrderDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            app_dimensionViewSource.Source = OrderDB.app_dimension.Local;

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            app_measurementViewSource.Source = OrderDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).ToList();

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
            production_order_detailList = production_order_detailViewSource.View.OfType<production_order_detail>().ToList();
          //  quantity = item_inventoryList.Sum(y => y.value_counted);
            //ProductMovementDB.SaveChanges();
            btnCancel_Click(sender, null);
        }






    }
}
