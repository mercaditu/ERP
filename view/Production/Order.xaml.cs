using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System;

namespace Cognitivo.Production
{
    public partial class Order : Page
    {
        OrderDB OrderDB = new OrderDB();
        //List<production_order> ProductionOrderList = new List<production_order>();

        CollectionViewSource project_task_dimensionViewSource, item_requestViewSource, item_transferViewSource, projectViewSource, production_orderViewSource, 
            production_lineViewSource, production_orderproduction_order_detailViewSource;
        cntrl.Curd.ItemRequest ItemRequest;

        public Order()
        {
            InitializeComponent();
        }

        private void toolBar_btnNew_Click(object sender)
        {
            production_order production_order = new production_order();
            production_order.State = EntityState.Added;
            production_order.status = Status.Production.Pending;
            production_order.IsSelected = true;
            OrderDB.Entry(production_order).State = EntityState.Added;
            production_orderViewSource.View.Refresh();
            production_orderViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    OrderDB.production_order.Remove((production_order)production_orderDataGrid.SelectedItem);
                    production_orderViewSource.View.MoveCurrentToFirst();
                    toolBar_btnSave_Click(sender);
                }

            }
            catch
            { }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (production_orderDataGrid.SelectedItem != null)
            {
                production_order production_order = (production_order)production_orderDataGrid.SelectedItem;
                production_order.IsSelected = true;
                production_order.State = EntityState.Modified;
                OrderDB.Entry(production_order).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            OrderDB.SaveChanges();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            OrderDB.CancelAllChanges();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int company_ID = entity.Properties.Settings.Default.company_ID;

            projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
            projectViewSource.Source = OrderDB.projects.Where(a => a.id_company == company_ID).ToList();

            production_lineViewSource = (CollectionViewSource)FindResource("production_lineViewSource");
            production_lineViewSource.Source = OrderDB.production_line.Where(x => x.id_company == company_ID).ToList();

            item_transferViewSource = (CollectionViewSource)FindResource("item_transferViewSource");
            item_transferViewSource.Source = OrderDB.item_transfer.Where(a => a.id_company == company_ID).ToList();

            item_requestViewSource = (CollectionViewSource)FindResource("item_requestViewSource");
            item_requestViewSource.Source = OrderDB.item_request.Where(a => a.id_company == company_ID).ToList();

            production_orderViewSource = ((CollectionViewSource)(FindResource("production_orderViewSource")));
            OrderDB.production_order.Where(a => a.id_company == company_ID).Load();
            production_orderViewSource.Source = OrderDB.production_order.Local;

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            app_dimensionViewSource.Source = OrderDB.app_dimension.Where(a => a.id_company == company_ID).ToList();

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            app_measurementViewSource.Source = OrderDB.app_measurement.Where(a => a.id_company == company_ID).ToList();


            production_orderproduction_order_detailViewSource = ((CollectionViewSource)(FindResource("production_orderproduction_order_detailViewSource")));

            if (production_orderproduction_order_detailViewSource.View != null)
            {
                production_orderproduction_order_detailViewSource.View.Filter = null;

                filter_task();
                production_orderproduction_order_detailViewSource.View.Refresh();
            }
        }

        public void filter_task()
        {
            if (production_orderproduction_order_detailViewSource != null)
            {
                if (production_orderproduction_order_detailViewSource.View != null)
                {
                    production_orderproduction_order_detailViewSource.View.Filter = i =>
                    {
                        production_order_detail objproduction_order_detail = (production_order_detail)i;
                        if (objproduction_order_detail.parent == null)
                            return true;
                        else
                            return false;
                    };
                }
            }
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            production_orderproduction_order_detailViewSource.View.Filter = null;
            List<production_order_detail> _production_order_detail = treeProject.ItemsSource.Cast<production_order_detail>().ToList();
            _production_order_detail = _production_order_detail.Where(x => x.IsSelected == true).ToList();
            production_execution production_execution = new production_execution();

            if (OrderDB.production_line.FirstOrDefault() != null)
            {
                production_execution.id_production_line = OrderDB.production_line.FirstOrDefault().id_production_line;
            }
            else
            {
                MessageBox.Show("please Create Line");
            }

            if (_production_order_detail.Count() > 0)
            {


                production_execution.id_production_order = _production_order_detail.FirstOrDefault().id_production_order;

                foreach (production_order_detail production_order_detail in _production_order_detail)
                {
                    //production_execution_detail production_execution_detail = new production_execution_detail();

                    //production_execution_detail.id_item = production_order_detail.id_item;
                    //production_execution.production_order = production_order_detail.production_order;
                    //production_execution.id_production_order = production_order_detail.id_production_order;
                    //production_execution_detail.quantity = production_order_detail.quantity;
                    //production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                    //production_execution_detail.id_project_task = production_order_detail.id_project_task;

                    //production_execution.production_execution_detail.Add(production_execution_detail);

                    production_order_detail.production_order.status = entity.Status.Production.Approved;

                }
            }

            OrderDB.production_execution.Add(production_execution);
            OrderDB.SaveChanges();
            toolBar.msgDone("Yay!");
            filter_task();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            List<production_order_detail> _production_order_detail = treeProject.ItemsSource.Cast<production_order_detail>().ToList();
            _production_order_detail = _production_order_detail.Where(x => x.IsSelected == true).ToList();

            foreach (production_order_detail production_order_detail in _production_order_detail)
            {
                production_order_detail.production_order.status = entity.Status.Production.QA_Rejected;

            }
            OrderDB.SaveChanges();
        }

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    //  int id_line =(int) cmbline.SelectedValue;
        //    crud_modal.Visibility = Visibility.Visible;
        //    cntrl.Curd.itemMovement itemMovement = new cntrl.Curd.itemMovement();
        //    itemMovement.operationMode = cntrl.Class.clsCommon.Mode.Add;
        //    item_transfer item_transfer = new entity.item_transfer();
        //    // item_transfer.id_location_destination = dbContext.production_line.Where(x => x.id_production_line == id_line).FirstOrDefault().id_location;
        //    itemMovement.item_transferViewSource = item_transferViewSource;
        //    itemMovement.dbContext=dbContext;
        //    crud_modal.Children.Add(itemMovement);
        //}

        private async void productionorderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
               
                int _id_production_order = 0;
                _id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;

                if (_id_production_order > 0)
                { filter_task();

                    var item_List = await (from IT in OrderDB.production_order_detail
                                       where IT.item.id_item_type != item.item_type.Service && 
                                       IT.item.id_item_type != item.item_type.Task && 
                                       (IT.production_order.status == Status.Production.Approved || IT.production_order.status == Status.Production.InProcess || IT.production_order.status == Status.Production.Executed || IT.production_order.status == Status.Production.Pending)
                                       && IT.production_order.status != null && IT.id_production_order == _id_production_order
                                       group IT by new { IT.item } into last
                                       select new
                                       {
                                         
                                           _id_item = last.Key.item.id_item != 0 ? last.Key.item.id_item : 0,
                                           _code = last.Key.item != null ? last.Key.item.code : "",
                                           _name = last.Key.item != null ? last.Key.item.name : "",
                                           _id_task = last.Max(x => x.id_project_task),
                                           _ordered_quantity = last.Sum(x => x.quantity) != 0 ? last.Sum(x => x.quantity) : 0,
                                           item = last.Key.item
                                       }).ToListAsync();

                    item_ProductDataGrid.ItemsSource = item_List.Where(x => x.item.id_item_type == item.item_type.Product);
                    item_RawDataGrid.ItemsSource = item_List.Where(x => x.item.id_item_type == item.item_type.RawMaterial);
                    item_CapitalDataGrid.ItemsSource = item_List.Where(x => x.item.id_item_type == item.item_type.FixedAssets);
                    item_SupplierDataGrid.ItemsSource = item_List.Where(x => x.item.id_item_type == item.item_type.Supplies);
                    //var rawlist = (from IT in OrderDB.production_order_detail
                    //               where IT.item.id_item_type == item.item_type.RawMaterial && (IT.production_order.status == Status.Production.Approved || IT.production_order.status == Status.Production.InProcess || IT.production_order.status == Status.Production.Executed || IT.production_order.status == Status.Production.Pending)
                    //               && IT.production_order.status != null && IT.id_production_order == _id_production_order

                    //               group IT by new { IT.item } into last
                    //               select new
                    //               {
                    //                   _id_item = last.Key.item.id_item != 0 ? last.Key.item.id_item : 0,
                    //                   _code = last.Key.item != null ? last.Key.item.code : "",
                    //                   _name = last.Key.item != null ? last.Key.item.name : "",
                    //                   _id_task = last.Max(x => x.id_project_task),
                    //                   _ordered_quantity = last.Sum(x => x.quantity) != 0 ? last.Sum(x => x.quantity) : 0,
                    //               }).ToList();
                    //item_RawDataGrid.ItemsSource = rawlist.ToList();

                    //var capitallist = (from IT in OrderDB.production_order_detail
                    //                   where IT.item.id_item_type == item.item_type.FixedAssets && (IT.production_order.status == Status.Production.Approved || IT.production_order.status == Status.Production.InProcess || IT.production_order.status == Status.Production.Executed || IT.production_order.status == Status.Production.Pending)
                    //                   && IT.production_order.status != null && IT.id_production_order == _id_production_order

                    //                   group IT by new { IT.item } into last
                    //                   select new
                    //                   {
                    //                       _id_item = last.Key.item.id_item != 0 ? last.Key.item.id_item : 0,
                    //                       _code = last.Key.item != null ? last.Key.item.code : "",
                    //                       _name = last.Key.item != null ? last.Key.item.name : "",
                    //                       _id_task = last.Max(x => x.id_project_task),
                    //                       _ordered_quantity = last.Sum(x => x.quantity) != 0 ? last.Sum(x => x.quantity) : 0,
                                          
                    //                   }).ToList();
                    //item_CapitalDataGrid.ItemsSource = capitallist.ToList();
                    
                    //var supplierlist = (from IT in OrderDB.production_order_detail
                    //                    where IT.item.id_item_type == item.item_type.Supplies && (IT.production_order.status == Status.Production.Approved || IT.production_order.status == Status.Production.InProcess || IT.production_order.status == Status.Production.Executed || IT.production_order.status == Status.Production.Pending)
                    //                    && IT.production_order.status != null && IT.id_production_order == _id_production_order

                    //                    group IT by new { IT.item } into last
                    //                    select new
                    //                    {
                    //                        _id_item = last.Key.item.id_item != 0 ? last.Key.item.id_item : 0,
                    //                        _code = last.Key.item != null ? last.Key.item.code : "",
                    //                        _name = last.Key.item != null ? last.Key.item.name : "",
                    //                        _id_task = last.Max(x => x.id_project_task),
                    //                        _ordered_quantity = last.Sum(x => x.quantity) != 0 ? last.Sum(x => x.quantity) : 0,
                    //                    }).ToList();
                    //item_SupplierDataGrid.ItemsSource = supplierlist.ToList();
                }
                else
                {
                    item_ProductDataGrid.ItemsSource = null;
                    item_RawDataGrid.ItemsSource = null;
                    item_CapitalDataGrid.ItemsSource = null;
                    item_SupplierDataGrid.ItemsSource = null;
                }
                production_orderViewSource.View.Refresh();

            }
            catch (System.Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void item_ProductDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int _id_production_order = 0;
            _id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;
            dynamic obj = (dynamic)item_ProductDataGrid.SelectedItem;
            if (obj != null)
            {
                int _id_item = obj._id_item;
                List<production_order_detail> list = OrderDB.production_order_detail.Where(IT => IT.item.id_item_type == item.item_type.Product && (IT.production_order.status != Status.Production.Pending || IT.production_order.status != null) && IT.id_production_order == _id_production_order && IT.id_item == _id_item).ToList();
                itemDataGrid.ItemsSource = list.ToList();
            }
        }

        private void item_RawDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int _id_production_order = 0;
            _id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;
            dynamic obj = (dynamic)item_RawDataGrid.SelectedItem;

            if (obj != null)
            {
                int _id_item = obj._id_item;
                List<production_order_detail> list = OrderDB.production_order_detail.Where(IT => IT.item.id_item_type == item.item_type.RawMaterial && IT.production_order.status != Status.Production.Pending && IT.production_order.status != null && IT.id_production_order == _id_production_order && IT.id_item == _id_item).ToList();
                itemDataGrid.ItemsSource = list.ToList();
            }
        }

        private void item_ServiceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int _id_production_order = 0;
            //_id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;
            //dynamic obj = (dynamic)item_ServiceDataGrid.SelectedItem;
            //if (obj!= null)
            //{
            //    int _id_item = obj._id_item;
            //    List<production_order_detail> list = dbContext.production_order_detail.Where(IT => IT.item.id_item_type == item.item_type.Service && IT.production_order.status != app_status.Production.Pending && IT.production_order.status != null && IT.id_production_order == _id_production_order && IT.id_item == _id_item).ToList();
            //    itemDataGrid.ItemsSource = list.ToList();
            //}
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    production_orderViewSource.View.Filter = i =>
                    {
                        production_order production_order = i as production_order;
                        if (production_order.name.ToLower().Contains(query.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                else
                {
                    production_orderViewSource.View.Filter = null;
                }
            }
            catch (Exception ex)
            { toolBar.msgError(ex); }
        }

        private void item_CapitalDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int _id_production_order = 0;
            _id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;
            dynamic obj = (dynamic)item_CapitalDataGrid.SelectedItem;
            if (obj != null)
            {
                int _id_item = obj._id_item;
                List<production_order_detail> list = OrderDB.production_order_detail.Where(IT => IT.item.id_item_type == item.item_type.FixedAssets && IT.production_order.status != Status.Production.Pending && IT.production_order.status != null && IT.id_production_order == _id_production_order && IT.id_item == _id_item).ToList();
                itemDataGrid.ItemsSource = list.ToList();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           

            if (itemDataGrid.ItemsSource != null)
            {
                List<production_order_detail> production_order_detaillist = OrderDB.production_order_detail.ToList();
                production_order_detaillist = production_order_detaillist.Where(x => x.IsSelected == true).ToList();
               
                if (production_order_detaillist.Count() > 0)
                {
                    ItemRequest = new cntrl.Curd.ItemRequest();
                    crud_modal_request.Visibility = Visibility.Visible;
                    ItemRequest.listdepartment = OrderDB.app_department.ToList();
                    ItemRequest.item_request_Click += item_request_Click;
                    crud_modal_request.Children.Add(ItemRequest);
                }
                else
                {
                    toolBar.msgWarning("Select a Task");
                }
            }
        }


        public void item_request_Click(object sender)
        {
            int id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;
            if (itemDataGrid.ItemsSource != null)
            {
                List<production_order_detail> production_order_detaillist = OrderDB.production_order_detail.ToList();
                production_order_detaillist = production_order_detaillist.Where(x => x.IsSelected == true).ToList();
                
                foreach (production_order_detail data in production_order_detaillist)
                {
                    item_request item_request = new item_request();
                    item_request.name = ItemRequest.name;
                    item_request.comment = ItemRequest.comment;
                    item_request.id_branch = data.project_task.project.id_branch;
                    item_request.id_department = ItemRequest.id_department;
                    item_request.id_production_order = data.id_production_order;
                    item_request.id_project= data.project_task.id_project;
                    item_request.request_date = data.trans_date;

                    item_request_detail item_request_detail = new entity.item_request_detail();
                    item_request_detail.date_needed_by = ItemRequest.neededDate;
                    item_request_detail.id_order_detail = data.id_order_detail;
                    item_request_detail.urgency = ItemRequest.Urgencies;
                    item_request_detail.comment = ItemRequest.comment;
                    item_request_detail.id_project_task = data.project_task.id_project_task;
                    int idItem = data.item.id_item;
                    item_request_detail.id_item = idItem;
                    item_request_detail.quantity = data.quantity;
                    item_request.item_request_detail.Add(item_request_detail);
                    OrderDB.item_request.Add(item_request);
                    OrderDB.SaveChanges();



                

                  
                }
                item_requestViewSource.View.Filter = i =>
                {
                    item_request item_request = (item_request)i;
                    if (item_request.id_production_order == id_production_order)
                        return true;
                    else
                        return false;
                };

            }
            crud_modal_request.Children.Clear();
            crud_modal_request.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void btnSaveTender_Click(object sender, RoutedEventArgs e)
        {
            crud_modal_request.Visibility = Visibility.Collapsed;
            OrderDB.SaveChanges();
        }

        private void lblCancel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            crud_modal_request.Visibility = Visibility.Collapsed;

        }

        private void item_ProductDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            DataGrid DataGrid = (DataGrid)sender;
            production_order_detail obj = (production_order_detail)DataGrid.SelectedItem;
            if (obj != null)
            {
                int _id_task = obj.project_task.id_project_task;
                project_task_dimensionViewSource = (CollectionViewSource)FindResource("project_task_dimensionViewSource");
                project_task_dimensionViewSource.Source = OrderDB.project_task_dimension.Where(x=>x.id_project_task==_id_task).ToList();
            }

        }

        private void item_SupplierDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int _id_production_order = 0;
            _id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;
            dynamic obj = (dynamic)item_SupplierDataGrid.SelectedItem;
            if (obj != null)
            {
                int _id_item = obj._id_item;
                List<production_order_detail> list = OrderDB.production_order_detail.Where(IT => IT.item.id_item_type == item.item_type.Supplies && IT.production_order.status != Status.Production.Pending && IT.production_order.status != null && IT.id_production_order == _id_production_order && IT.id_item == _id_item).ToList();
                itemDataGrid.ItemsSource = list.ToList();
            }
        }

        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
                item item = OrderDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (item != null && item.id_item > 0 && item.is_autorecepie && production_order != null)
                {
                    production_order_detail production_order_detail_output = new production_order_detail();
                    production_order_detail_output.quantity = 1;
                    production_order_detail_output.name = item.name;
                    production_order_detail_output.id_item = item.id_item;
                    production_order_detail_output.item = item;
                    production_order_detail_output.is_input = false;

                    foreach (item_recepie_detail item_recepie_detail in item.item_recepie.FirstOrDefault().item_recepie_detail)
                    {
                        production_order_detail production_order_detail = new production_order_detail();

                        production_order_detail.name = item_recepie_detail.item.name;
                        production_order_detail.id_item = item_recepie_detail.item.id_item;
                        production_order_detail.item = item_recepie_detail.item;
                        if (item_recepie_detail.quantity > 0)
                        {
                            production_order_detail.quantity = (decimal)item_recepie_detail.quantity;
                        }

                        production_order_detail.is_input = true;

                        production_order_detail_output.child.Add(production_order_detail);
                    }
                    production_order.production_order_detail.Add(production_order_detail_output);
                    filter_task();
                }
                else
                {
                    production_order_detail production_order_detail_output = new production_order_detail();
                    production_order_detail_output.quantity = 1;
                    production_order_detail_output.name = item.name;
                    production_order_detail_output.id_item = item.id_item;
                    production_order_detail_output.item = item;
                    production_order_detail_output.is_input = true;


                    production_order.production_order_detail.Add(production_order_detail_output);
                    filter_task();
                
                }
            }
        }
    }
}
