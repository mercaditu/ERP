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
    public partial class pnl_FractionExecustion : UserControl
    {
        CollectionViewSource production_execution_detailViewSource;
        public ExecutionDB ExecutionDB { get; set; }
     
        public List<production_execution_detail> productionExecustionList { get; set; }
        public pnl_FractionExecustion()
        {
            InitializeComponent();
        }

        private void item_transfer_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            production_execution_detail production_execution_detail = e.NewItem as production_execution_detail;
            add_item(production_execution_detail);
        }

        public void add_item(production_execution_detail production_execution_detail)
        {
        
            production_execution_detail.quantity = productionExecustionList.FirstOrDefault().quantity;
            production_execution_detail.id_item = productionExecustionList.FirstOrDefault().id_item;
            production_execution_detail.item = productionExecustionList.FirstOrDefault().item;
            
            production_execution_detail.IsSelected = true;
            production_execution_detail.State = EntityState.Added;

            productionExecustionList.FirstOrDefault().production_execution.production_execution_detail.Add(production_execution_detail);
            if (production_execution_detail.id_project_task > 0)
            {
                if (ExecutionDB.project_task.Where(x => x.id_project_task == production_execution_detail.id_project_task).FirstOrDefault() != null)
                {
                    project_task project_task = ExecutionDB.project_task.Where(x => x.id_project_task == production_execution_detail.id_project_task).FirstOrDefault();
                    if (ExecutionDB.project_task_dimension.Where(x => x.id_project_task == project_task.id_project_task).ToList() != null)
                    {
                        List<project_task_dimension> project_task_dimensionList = ExecutionDB.project_task_dimension.Where(x => x.id_project_task == project_task.id_project_task).ToList();
                        foreach (project_task_dimension project_task_dimension in project_task_dimensionList)
                        {
                            production_execution_dimension production_execution_dimension = new production_execution_dimension();
                            production_execution_dimension.id_dimension = project_task_dimension.id_dimension;
                            production_execution_dimension.value = project_task_dimension.value;
                            production_execution_detail.production_execution_dimension.Add(production_execution_dimension);
                        }


                    }
                }

            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            production_execution_detailViewSource = ((CollectionViewSource)(FindResource("production_execution_detailViewSource")));
           // InventoryDB.item_inventory_detail.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            production_execution_detailViewSource.Source = productionExecustionList;


            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            ExecutionDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            app_dimensionViewSource.Source = ExecutionDB.app_dimension.Local;

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            app_measurementViewSource.Source = ExecutionDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).ToList();

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
            productionExecustionList = production_execution_detailViewSource.View.OfType<production_execution_detail>().ToList();
          //  quantity = item_inventoryList.Sum(y => y.value_counted);
            //ProductMovementDB.SaveChanges();
            btnCancel_Click(sender, null);
        }






    }
}
