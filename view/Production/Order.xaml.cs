using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System;
using System.ComponentModel;

namespace Cognitivo.Production
{
    public partial class Order : Page
    {
        OrderDB OrderDB = new OrderDB();

        CollectionViewSource 
            project_task_dimensionViewSource,
            projectViewSource, 
            production_orderViewSource,
            production_lineViewSource, 
            production_orderproduction_order_detailViewSource;

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
            OrderDB.production_order.Add(production_order);

            production_orderViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                OrderDB.production_order.Remove((production_order)production_orderDataGrid.SelectedItem);
                production_orderViewSource.View.MoveCurrentToFirst();
                toolBar_btnSave_Click(sender);
            }
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
            production_order _production_order = (production_order)production_orderDataGrid.SelectedItem;
            if ((_production_order.work_number == null || _production_order.work_number == string.Empty) && _production_order.id_range > 0)
            {
                if (_production_order.id_branch > 0)
                {
                    entity.Brillo.Logic.Range.branch_Code = OrderDB.app_branch.Where(x => x.id_branch == _production_order.id_branch).FirstOrDefault().code;
                }
                if (_production_order.id_terminal > 0)
                {
                    entity.Brillo.Logic.Range.terminal_Code = OrderDB.app_terminal.Where(x => x.id_terminal == _production_order.id_terminal).FirstOrDefault().code;
                }

                app_document_range app_document_range = OrderDB.app_document_range.Where(x => x.id_range == _production_order.id_range).FirstOrDefault();
                _production_order.work_number = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);
                _production_order.RaisePropertyChanged("work_number");
            }
            OrderDB.SaveChanges();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            OrderDB.CancelAllChanges();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
            projectViewSource.Source = OrderDB.projects.Where(a => a.id_company == CurrentSession.Id_Company).ToList();

            production_lineViewSource = (CollectionViewSource)FindResource("production_lineViewSource");
            production_lineViewSource.Source = OrderDB.production_line.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

            //item_transferViewSource = (CollectionViewSource)FindResource("item_transferViewSource");
            //item_transferViewSource.Source = OrderDB.item_transfer.Where(a => a.id_company == CurrentSession.Id_Company).ToList();

            //item_requestViewSource = (CollectionViewSource)FindResource("item_requestViewSource");
            //item_requestViewSource.Source = OrderDB.item_request.Where(a => a.id_company == CurrentSession.Id_Company).ToList();

            production_orderViewSource = ((CollectionViewSource)(FindResource("production_orderViewSource")));
            OrderDB.production_order.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            production_orderViewSource.Source = OrderDB.production_order.Local;

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            app_dimensionViewSource.Source = OrderDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).ToList();

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            app_measurementViewSource.Source = OrderDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).ToList();

            production_orderproduction_order_detailViewSource = ((CollectionViewSource)(FindResource("production_orderproduction_order_detailViewSource")));

            if (production_orderproduction_order_detailViewSource.View != null)
            {
                production_orderproduction_order_detailViewSource.View.Filter = null;
                filter_task();
               // production_orderproduction_order_detailViewSource.View.Refresh();
            }

            cbxItemType.ItemsSource = Enum.GetValues(typeof(item.item_type)).Cast<item.item_type>().Where(x => !x.Equals(item.item_type.RawMaterial));
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.ProductionOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
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
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order.status = Status.Production.Executed;
            OrderDB.SaveChanges();
            toolBar.msgDone();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order.status = Status.Production.QA_Rejected;
            OrderDB.SaveChanges();
        }

        private void productionorderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update_request();
        }

        public async void Update_request()
        {
            try
            {
                int _id_production_order = 0;
                _id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;

                if (_id_production_order > 0)
                {
                    filter_task();

                    var item_List_group_basic = (from IT in OrderDB.production_order_detail
                                                 where IT.item.id_item_type != item.item_type.Service &&
                                                 IT.item.id_item_type != item.item_type.Task &&
                                                 (IT.production_order.status == Status.Production.Approved || IT.production_order.status == Status.Production.InProcess || IT.production_order.status == Status.Production.Executed || IT.production_order.status == Status.Production.Pending)
                                                 && IT.production_order.status != null && IT.id_production_order == _id_production_order
                                                 join IK in OrderDB.item_product on IT.id_item equals IK.id_item
                                                 join IO in OrderDB.item_movement on IK.id_item_product equals IO.id_item_product into a
                                                 from IM in a.DefaultIfEmpty()
                                                 group IT by new { IM, IT.item }
                                                     into last
                                                     select new
                                                     {
                                                         _id_item = last.Key.item.id_item != 0 ? last.Key.item.id_item : 0,
                                                         _code = last.Key.item != null ? last.Key.item.code : "",
                                                         _name = last.Key.item != null ? last.Key.item.name : "",
                                                         _id_task = last.Max(x => x.id_project_task),
                                                         _ordered_quantity = last.Sum(x => x.quantity) != 0 ? last.Sum(x => x.quantity) : 0,
                                                         item = last.Key.item,
                                                         avlqtyColumn = last.Key.IM.credit != null ? last.Key.IM.credit : 0 - last.Key.IM.debit != null ? last.Key.IM.debit : 0,
                                                         buyqty = (last.Sum(x => x.quantity) != 0 ? last.Sum(x => x.quantity) : 0) - (last.Key.IM.credit != null ? last.Key.IM.credit : 0 - last.Key.IM.debit != null ? last.Key.IM.debit : 0)
                                                     }).ToList();

                    var item_List_group = (from PL in item_List_group_basic
                                           group PL by new { PL.item }
                                               into last
                                               select new
                                               {
                                                   _id_item = last.Key.item.id_item != 0 ? last.Key.item.id_item : 0,
                                                   _code = last.Key.item != null ? last.Key.item.code : "",
                                                   _name = last.Key.item != null ? last.Key.item.name : "",
                                                   _id_task = last.Max(x => x._id_task),
                                                   _ordered_quantity = last.Max(x => x._ordered_quantity),
                                                   avlqtyColumn = last.Sum(x => x.avlqtyColumn),
                                                   buyqty = last.Sum(x => x.avlqtyColumn) < last.Max(x => x._ordered_quantity) ? (last.Max(x => x._ordered_quantity) != 0 ? last.Max(x => x._ordered_quantity) : 0) - (last.Sum(x => x.avlqtyColumn) != 0 ? last.Sum(x => x.avlqtyColumn) : 0) : 0,
                                                   item = last.Key.item
                                               }).ToList();

                    item_ProductDataGrid.ItemsSource = item_List_group.Where(x => x.item.id_item_type == item.item_type.Product);
                    item_RawDataGrid.ItemsSource = item_List_group.Where(x => x.item.id_item_type == item.item_type.RawMaterial);

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

                    item_CapitalDataGrid.ItemsSource = item_List.Where(x => x.item.id_item_type == item.item_type.FixedAssets);
                    item_SupplierDataGrid.ItemsSource = item_List.Where(x => x.item.id_item_type == item.item_type.Supplies);
                    item_ServiceContractDataGrid.ItemsSource = item_List.Where(x => x.item.id_item_type == item.item_type.ServiceContract);
                }
                else
                {
                    item_ProductDataGrid.ItemsSource = null;
                    item_RawDataGrid.ItemsSource = null;
                    item_CapitalDataGrid.ItemsSource = null;
                    item_SupplierDataGrid.ItemsSource = null;
                    item_ServiceContractDataGrid.ItemsSource = null;
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
                List<production_order_detail> list = OrderDB.production_order_detail.Where(IT => IT.item.id_item_type == item.item_type.RawMaterial && (IT.production_order.status != Status.Production.Pending || IT.production_order.status != null) && IT.id_production_order == _id_production_order && IT.id_item == _id_item).ToList();
                itemDataGrid.ItemsSource = list.ToList();
            }
        }

        private void item_ServiceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
                List<production_order_detail> list = OrderDB.production_order_detail.Where(IT => IT.item.id_item_type == item.item_type.FixedAssets && (IT.production_order.status != Status.Production.Pending || IT.production_order.status != null) && IT.id_production_order == _id_production_order && IT.id_item == _id_item).ToList();
                itemDataGrid.ItemsSource = list.ToList();
            }
        }

        private void btnRequestResource_Click(object sender, RoutedEventArgs e)
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
            production_order production_order = ((production_order)production_orderViewSource.View.CurrentItem);
            int id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;
            if (itemDataGrid.ItemsSource != null)
            {
                List<production_order_detail> production_order_detaillist = OrderDB.production_order_detail.ToList();
                production_order_detaillist = production_order_detaillist.Where(x => x.IsSelected == true).ToList();

                item_request item_request = new item_request();
                item_request.name = ItemRequest.name;
                item_request.comment = ItemRequest.comment;
            
                item_request.id_department = ItemRequest.id_department;
                item_request.id_production_order = id_production_order;
                if (production_order.project!=null)
                {
                    item_request.id_project = production_order.project.id_project;
                    item_request.id_branch = production_order.project.id_branch;
                }
            
                item_request.request_date = DateTime.Now;

                foreach (production_order_detail data in production_order_detaillist)
                {
                    item_request_detail item_request_detail = new entity.item_request_detail();
                    item_request_detail.date_needed_by = ItemRequest.neededDate;
                    item_request_detail.id_order_detail = data.id_order_detail;
                    item_request_detail.urgency = ItemRequest.Urgencies;
                    int idItem = data.item.id_item;
                    item_request_detail.id_item = idItem;
                    item item = OrderDB.items.Where(x => x.id_item == idItem).FirstOrDefault();
                    if (item != null)
                    {
                        item_request_detail.item = item;
                    }

                    if (data.project_task != null)
                    {
                        item_request_detail.id_project_task = data.project_task.id_project_task;
                        string comment = item_request_detail.item.name;
                        List<project_task_dimension> project_task_dimensionList = OrderDB.project_task_dimension.Where(x => x.id_project_task == data.project_task.id_project_task).ToList();
                        foreach (project_task_dimension project_task_dimension in project_task_dimensionList)
                        {
                            item_request_dimension item_request_dimension = new item_request_dimension();
                            item_request_dimension.id_dimension = project_task_dimension.id_dimension;
                            item_request_dimension.id_measurement = project_task_dimension.id_measurement;
                            item_request_dimension.value = project_task_dimension.value;
                           

                            comment += project_task_dimension.value.ToString();
                            comment += "X";

                            item_request_detail.comment = comment.Substring(0, comment.Length - 1);
                            item_request_detail.item_request_dimension.Add(item_request_dimension);
                        }
                    }

                
                    item_request_detail.quantity = data.quantity;

                    item_request.item_request_detail.Add(item_request_detail);
                }

                OrderDB.item_request.Add(item_request);
                OrderDB.SaveChanges();

                //item_requestViewSource.View.Filter = i =>
                //{
                //    item_request _item_request = (item_request)i;
                //    if (_item_request.id_production_order == id_production_order)
                //        return true;
                //    else
                //        return false;
                //};
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
                if (obj.project_task != null)
                {
                    int _id_task = obj.project_task.id_project_task;
                    project_task_dimensionViewSource = (CollectionViewSource)FindResource("project_task_dimensionViewSource");
                    project_task_dimensionViewSource.Source = OrderDB.project_task_dimension.Where(x => x.id_project_task == _id_task).ToList();
                }
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
                List<production_order_detail> list = OrderDB.production_order_detail.Where(IT => IT.item.id_item_type == item.item_type.Supplies && (IT.production_order.status != Status.Production.Pending || IT.production_order.status != null) && IT.id_production_order == _id_production_order && IT.id_item == _id_item).ToList();
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
                    production_order_detail production_order_detail_output = treeProject.SelectedItem as production_order_detail;
                    if (production_order_detail_output != null)
                    {
                        production_order_detail_output.id_item = item.id_item;
                        production_order_detail_output.item = item;
                        production_order_detail_output.name = item.name;
                        production_order_detail_output.RaisePropertyChanged("item");
                        production_order_detail_output.is_input = false;
                        production_order_detail_output.quantity = 1;
                        production_order_detail_output.production_order = production_order;
                        production_order_detail_output.id_production_order = production_order.id_production_order;
                        foreach (item_recepie_detail item_recepie_detail in item.item_recepie.FirstOrDefault().item_recepie_detail)
                        {
                            production_order_detail production_order_detail = new production_order_detail();

                            production_order_detail.name = item_recepie_detail.item.name;
                            production_order_detail.id_item = item_recepie_detail.item.id_item;
                            production_order_detail.item = item_recepie_detail.item;
                            production_order_detail.RaisePropertyChanged("item");
                            production_order_detail.production_order = production_order;
                            production_order_detail.id_production_order = production_order.id_production_order;
                            if (item_recepie_detail.quantity > 0)
                            {
                                production_order_detail.quantity = (decimal)item_recepie_detail.quantity;
                            }

                            production_order_detail.is_input = true;

                            production_order_detail_output.child.Add(production_order_detail);
                        }

                        filter_task();
                    }
                }
                else
                {
                    production_order_detail production_order_detail_output = treeProject.SelectedItem as production_order_detail;

                    if (production_order_detail_output != null)
                    {
                        production_order_detail_output.quantity = 1;
                        production_order_detail_output.name = item.name;
                        production_order_detail_output.id_item = item.id_item;
                        production_order_detail_output.item = item;
                        production_order_detail_output.RaisePropertyChanged("item");
                        production_order_detail_output.is_input = true;
                        production_order_detail_output.production_order = production_order;
                        production_order_detail_output.id_production_order = production_order.id_production_order;

                    }
                }
            }
        }

        private void btnNewTask_Click(object sender)
        {
            stpcode.IsEnabled = true;

            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order_detail production_order_detail = treeProject.SelectedItem as production_order_detail;

            if (production_order_detail != null)
            {
                //Adding a Child Item.
                if (production_order_detail.item != null)
                {
                    if (production_order_detail.item.id_item_type == entity.item.item_type.Task)
                    {
                        production_order_detail n_production_order_detail = new production_order_detail();
                        n_production_order_detail.id_production_order = production_order.id_production_order;
                        n_production_order_detail.production_order = production_order;
                        n_production_order_detail.production_order.status = Status.Production.Pending;
                        n_production_order_detail.quantity = 0;
                        n_production_order_detail.status = Status.Project.Pending;
                        production_order_detail.child.Add(n_production_order_detail);
                        OrderDB.production_order_detail.Add(n_production_order_detail);
                        production_orderproduction_order_detailViewSource.View.Refresh();
                        production_orderproduction_order_detailViewSource.View.MoveCurrentTo(n_production_order_detail);
                    }
                }
            }
            else
            {
                //Adding First Item
                production_order_detail n_production_order_detail = new production_order_detail();
                n_production_order_detail.status = Status.Project.Pending;
                production_order.production_order_detail.Add(n_production_order_detail);

                production_orderproduction_order_detailViewSource.View.Refresh();
                production_orderproduction_order_detailViewSource.View.MoveCurrentTo(n_production_order_detail);
                filter_task();
            }
        }

        private void btnEditTask_Click(object sender)
        {
            stpcode.IsEnabled = true;
        }

        private void btnSaveTask_Click(object sender)
        {
            try
            {
                cbxItemType.ItemsSource = Enum.GetValues(typeof(item.item_type));
                OrderDB.SaveChanges();

                production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
                production_order.State = EntityState.Modified;
                Update_request();
                toolBar.msgDone("Yay!");
                stpcode.IsEnabled = false;
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void btnDeleteTask_Click(object sender)
        {
            production_orderproduction_order_detailViewSource.View.Filter = null;
            List<production_order_detail> production_order_detailLIST = treeProject.ItemsSource.Cast<production_order_detail>().ToList();
            production_order_detailLIST = production_order_detailLIST.Where(x => x.IsSelected == true).ToList();

            foreach (production_order_detail production_order_detail in production_order_detailLIST)
            {
                production_order_detail.status = Status.Project.Rejected;
                production_order_detail.IsSelected = false;
            }

            OrderDB.SaveChanges();
            toolBar.msgDone();
            filter_task();
        }

        private void cbxItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbxItemType = (ComboBox)sender;

            if (cbxItemType.SelectedItem != null)
            {
                item.item_type Item_Type = (item.item_type)cbxItemType.SelectedItem;
                sbxItem.item_types = Item_Type;

                if (Item_Type == entity.item.item_type.Task)
                {
                    stpdate.Visibility = Visibility.Visible;
                    stpitem.Visibility = Visibility.Collapsed;
                }
                else
                {
                    stpdate.Visibility = Visibility.Collapsed;
                    stpitem.Visibility = Visibility.Visible;
                }
            }
        }

        private void toolIcon_Click(object sender)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_orderproduction_order_detailViewSource.View.Filter = null;

            List<production_order_detail> _production_order_detail = treeProject.ItemsSource.Cast<production_order_detail>().ToList();
            _production_order_detail = _production_order_detail.Where(x => x.IsSelected == true).ToList();

            foreach (production_order_detail production_order_detail in _production_order_detail)
            {
                production_order_detail.status = entity.Status.Project.Approved;
            }

            if (production_order.production_execution.Count() == 0)
            {
                production_execution production_execution = new production_execution();
                production_execution.production_order = production_order;
                production_execution.id_production_line = production_order.id_production_line;
                production_execution.trans_date = DateTime.Now;

                OrderDB.production_execution.Add(production_execution);
            }

            OrderDB.SaveChanges();
            toolBar.msgDone("Yay!");
            filter_task();
        }

        private void toolIcon_Click_1(object sender)
        {
            List<production_order_detail> _production_order_detail = treeProject.ItemsSource.Cast<production_order_detail>().ToList();
            _production_order_detail = _production_order_detail.Where(x => x.IsSelected == true).ToList();

            foreach (production_order_detail production_order_detail in _production_order_detail)
            {
                production_order_detail.status = entity.Status.Project.Rejected;

            }
            OrderDB.SaveChanges();
        }

        private void item_ServiceContractDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int _id_production_order = 0;
            _id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;
            dynamic obj = (dynamic)item_ServiceContractDataGrid.SelectedItem;
            if (obj != null)
            {
                int _id_item = obj._id_item;
                List<production_order_detail> list = OrderDB.production_order_detail.Where(
                    od => od.item.id_item_type == item.item_type.ServiceContract && 
                         (od.production_order.status != Status.Production.Pending || od.production_order.status != null) && 
                          od.id_production_order == _id_production_order && 
                          od.id_item == _id_item)
                            .ToList();
                itemDataGrid.ItemsSource = list.ToList();
            }
        }

        private void btnAddParentTask_Click(object sender)
        {
            stpcode.IsEnabled = true;

            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order_detail n_production_order_detail = new production_order_detail();
            n_production_order_detail.status = Status.Project.Pending;
            production_order.production_order_detail.Add(n_production_order_detail);

            production_orderproduction_order_detailViewSource.View.Refresh();
            production_orderproduction_order_detailViewSource.View.MoveCurrentTo(n_production_order_detail);
            filter_task();
        }

    }
}
