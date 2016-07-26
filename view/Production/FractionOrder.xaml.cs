using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System;
using System.ComponentModel;
using cntrl.Panels;
using System.Windows.Media;
using System.Windows.Input;

namespace Cognitivo.Production
{
    public partial class FractionOrder : Page
    {
        OrderDB OrderDB = new OrderDB();

        CollectionViewSource
            project_task_dimensionViewSource,
            projectViewSource,
            production_orderViewSource,
            production_lineViewSource,
            production_orderproduction_order_detailViewSource,
            production_order_detaillServiceViewSource,
            item_movementViewSource,
             item_movementrawViewSource,
            production_executionViewSource,
            production_executionproduction_execustion_detailViewSource,
            item_movementitem_movement_dimensionViewSource;


        CollectionViewSource
         production_execution_detailProductViewSource,
         production_execution_detailRawViewSource,
         production_execution_detailAssetViewSource,
         production_execution_detailServiceViewSource,
         production_execution_detailSupplyViewSource,
         production_execution_detailServiceContractViewSource;

        CollectionViewSource
         production_order_detaillProductViewSource,
            production_order_detaillRawViewSource,
            production_order_detaillAssetViewSource,
            production_order_detaillSupplyViewSource,
            production_order_detaillServiceContractViewSource,
            production_order_dimensionViewSource;

        //cntrl.Curd.ItemRequest ItemRequest;
        Cognitivo.Configs.itemMovementFraction itemMovementFraction;

        //pnl_FractionOrder objpnl_FractionOrder;
        public FractionOrder()
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

            production_orderViewSource = ((CollectionViewSource)(FindResource("production_orderViewSource")));
            OrderDB.production_order.Where(a => a.id_company == CurrentSession.Id_Company && a.types == production_order.ProductionOrderTypes.Fraction).Load();
            production_orderViewSource.Source = OrderDB.production_order.Local;

            production_order_detaillServiceViewSource = ((CollectionViewSource)(FindResource("production_order_detaillServiceViewSource")));

            production_executionViewSource = ((CollectionViewSource)(FindResource("production_executionViewSource")));
            OrderDB.production_execution.Where(a => a.id_company == CurrentSession.Id_Company && a.production_order.types == production_order.ProductionOrderTypes.Fraction).Load();
            production_executionViewSource.Source = OrderDB.production_execution.Local;
            production_executionproduction_execustion_detailViewSource = ((CollectionViewSource)(FindResource("production_executionproduction_execustion_detailViewSource")));

            production_orderproduction_order_detailViewSource = ((CollectionViewSource)(FindResource("production_orderproduction_order_detailViewSource")));
            production_order_dimensionViewSource = ((CollectionViewSource)(FindResource("production_order_dimensionViewSource")));

            cmbtype.ItemsSource = Enum.GetValues(typeof(production_order.ProductionOrderTypes)).Cast<production_order.ProductionOrderTypes>().ToList();
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.ProductionOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);

            item_movementViewSource = ((CollectionViewSource)(FindResource("item_movementViewSource")));
            item_movementrawViewSource = ((CollectionViewSource)(FindResource("item_movementrawViewSource")));

            if (production_orderproduction_order_detailViewSource.View != null)
            {
                cbxParent.ItemsSource = production_orderproduction_order_detailViewSource.View.OfType<production_order_detail>().ToList();

                production_orderproduction_order_detailViewSource.View.Filter = null;
                filter_task();
            }


            #region execustion

            CollectionViewSource hr_time_coefficientViewSource = FindResource("hr_time_coefficientViewSource") as CollectionViewSource;
            OrderDB.hr_time_coefficient.Where(x => x.id_company == CurrentSession.Id_Company).Load();
            hr_time_coefficientViewSource.Source = OrderDB.hr_time_coefficient.Local;

            production_execution_detailProductViewSource = FindResource("production_execution_detailProductViewSource") as CollectionViewSource;
            production_execution_detailRawViewSource = FindResource("production_execution_detailRawViewSource") as CollectionViewSource;
            production_execution_detailServiceViewSource = FindResource("production_execution_detailServiceViewSource") as CollectionViewSource;
            production_execution_detailAssetViewSource = FindResource("production_execution_detailAssetViewSource") as CollectionViewSource;
            production_execution_detailSupplyViewSource = FindResource("production_execution_detailSupplyViewSource") as CollectionViewSource;
            production_execution_detailServiceContractViewSource = FindResource("production_execution_detailServiceContractViewSource") as CollectionViewSource;

            production_order_detaillProductViewSource = FindResource("production_order_detaillProductViewSource") as CollectionViewSource;
            production_order_detaillServiceViewSource = FindResource("production_order_detaillServiceViewSource") as CollectionViewSource;
            production_order_detaillRawViewSource = FindResource("production_order_detaillRawViewSource") as CollectionViewSource;
            production_order_detaillAssetViewSource = FindResource("production_order_detaillAssetViewSource") as CollectionViewSource;
            production_order_detaillSupplyViewSource = FindResource("production_order_detaillSupplyViewSource") as CollectionViewSource;
            production_order_detaillServiceContractViewSource = FindResource("production_order_detaillServiceContractViewSource") as CollectionViewSource;

            filter_order(production_order_detaillProductViewSource, item.item_type.Product);
            filter_order(production_order_detaillRawViewSource, item.item_type.RawMaterial);
            filter_order(production_order_detaillSupplyViewSource, item.item_type.Supplies);
            filter_order(production_order_detaillServiceViewSource, item.item_type.Service);
            filter_order(production_order_detaillAssetViewSource, item.item_type.FixedAssets);
            filter_order(production_order_detaillServiceContractViewSource, item.item_type.ServiceContract);


            filter_execution(production_execution_detailProductViewSource, item.item_type.Product);
            filter_execution(production_execution_detailRawViewSource, item.item_type.RawMaterial);
            filter_execution(production_execution_detailSupplyViewSource, item.item_type.Supplies);
            filter_execution(production_execution_detailServiceViewSource, item.item_type.Service);
            filter_execution(production_execution_detailAssetViewSource, item.item_type.FixedAssets);
            filter_execution(production_execution_detailServiceContractViewSource, item.item_type.ServiceContract);
            #endregion
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

            production_execution production_execution = production_executionViewSource.View.CurrentItem as production_execution;
            if (production_execution.id_production_execution == 0)
            {
                toolBar_btnSave_Click(sender);
            }
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order.status = Status.Production.Executed;
            production_order.State = EntityState.Modified;

            if (production_execution != null)
            {
                entity.Brillo.Logic.Stock _Stock = new entity.Brillo.Logic.Stock();
                List<item_movement> item_movementList = new List<item_movement>();
                item_movementList = _Stock.insert_Stock(OrderDB, production_execution);

                if (item_movementList != null && item_movementList.Count > 0)
                {
                    OrderDB.item_movement.AddRange(item_movementList);
                }
                production_execution.State = EntityState.Modified;
                production_execution.status = Status.Documents_General.Approved;

            }
            if (OrderDB.SaveChanges() > 0)
            {
                toolBar.msgApproved(1);
            }

        }

        private void toolBar_btnAnull_Click(object sender)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order.status = Status.Production.QA_Rejected;
            OrderDB.SaveChanges();
        }

        private void productionorderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Update_request();
            filter_task();


            filter_order(production_order_detaillProductViewSource, item.item_type.Product);
            filter_order(production_order_detaillRawViewSource, item.item_type.RawMaterial);
            filter_order(production_order_detaillSupplyViewSource, item.item_type.Supplies);
            filter_order(production_order_detaillServiceViewSource, item.item_type.Service);
            filter_order(production_order_detaillAssetViewSource, item.item_type.FixedAssets);
            filter_order(production_order_detaillServiceContractViewSource, item.item_type.ServiceContract);


            filter_execution(production_execution_detailProductViewSource, item.item_type.Product);
            filter_execution(production_execution_detailRawViewSource, item.item_type.RawMaterial);
            filter_execution(production_execution_detailSupplyViewSource, item.item_type.Supplies);
            filter_execution(production_execution_detailServiceViewSource, item.item_type.Service);
            filter_execution(production_execution_detailAssetViewSource, item.item_type.FixedAssets);
            filter_execution(production_execution_detailServiceContractViewSource, item.item_type.ServiceContract);


            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            if (production_order.production_execution.FirstOrDefault() != null)
            {
                if (production_executionViewSource != null)
                {
                    if (production_executionViewSource.View != null)
                    {
                        production_executionViewSource.View.MoveCurrentTo(production_order.production_execution.FirstOrDefault());
                    }
                }
            }
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

            try
            {
                production_execution_detailAssetViewSource.View.Refresh();
                production_execution_detailProductViewSource.View.Refresh();
                production_execution_detailServiceViewSource.View.Refresh();
                production_execution_detailRawViewSource.View.Refresh();
            }
            catch
            {
                
            }

        }

        private void btnNewTask_Click(object sender)
        {
            //            stpcode.IsEnabled = true;

            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order_detail production_order_detail = treeProject.SelectedItem as production_order_detail;

            if (production_order_detail != null)
            {
                //Adding a Child Item.
                if (production_order_detail.item != null)
                {
                    //if (production_order_detail.item.id_item_type == entity.item.item_type.Task)
                    //{
                    production_order_detail n_production_order_detail = new production_order_detail();
                    n_production_order_detail.is_input = false;
                    n_production_order_detail.id_item = production_order_detail.id_item;
                    n_production_order_detail.item = production_order_detail.item;
                    n_production_order_detail.name = production_order_detail.item.name;
                    n_production_order_detail.id_production_order = production_order.id_production_order;
                    n_production_order_detail.production_order = production_order;
                    n_production_order_detail.production_order.status = Status.Production.Pending;
                    n_production_order_detail.quantity = 0;
                    n_production_order_detail.status = Status.Project.Pending;
                    if (production_order_detail.item != null)
                    {
                        foreach (item_dimension item_dimension in production_order_detail.item.item_dimension)
                        {
                            production_order_dimension production_order_dimension = new production_order_dimension();
                            production_order_dimension.id_dimension = item_dimension.id_app_dimension;
                            production_order_dimension.app_dimension = item_dimension.app_dimension;
                            production_order_dimension.id_measurement = item_dimension.id_measurement;
                            production_order_dimension.app_measurement = item_dimension.app_measurement;
                            production_order_dimension.value = 0;
                            n_production_order_detail.production_order_dimension.Add(production_order_dimension);

                        }
                    }
                    production_order_detail.child.Add(n_production_order_detail);

                    OrderDB.production_order_detail.Add(n_production_order_detail);
                    project_task_dimensionDataGrid.ItemsSource = production_order_detail.production_order_dimension.ToList();

                    production_orderproduction_order_detailViewSource.View.Refresh();
                    production_orderproduction_order_detailViewSource.View.MoveCurrentTo(n_production_order_detail);
                    //  }
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
            if (OrderDB.SaveChanges() > 0)
            {
                production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
                production_order.State = EntityState.Modified;
                // Update_request();
                stpcode.IsEnabled = false;
                //StpMovement.Visibility = System.Windows.Visibility.Collapsed;
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

            if (OrderDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(OrderDB.NumberOfRecords);
                filter_task();
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
                if (production_order_detail.parent != null)
                {
                    production_order_detail.parent.status = entity.Status.Project.Approved;
                }

                production_order_detail.status = entity.Status.Project.Approved;
            }

            if (production_order.production_execution.Count() == 0)
            {
                production_execution production_execution = new production_execution();
                production_execution.production_order = production_order;
                production_execution.id_production_line = production_order.id_production_line;
                production_execution.trans_date = DateTime.Now;
                OrderDB.production_execution.Add(production_execution);
                production_executionViewSource.View.Refresh();
                production_executionViewSource.View.MoveCurrentToLast();
            }

            if (OrderDB.SaveChanges() > 0)
            {
                filter_task();
                toolBar.msgSaved(OrderDB.NumberOfRecords);
            }

            try
            {
                production_orderViewSource.View.Refresh();
                production_orderViewSource.View.MoveCurrentToFirst();
                production_orderViewSource.View.MoveCurrentTo(production_order);
                production_order_detaillRawViewSource.View.Refresh();
                production_order_detaillProductViewSource.View.Refresh();
                production_order_detaillServiceViewSource.View.Refresh();
                production_order_detaillRawViewSource.View.Refresh();
                treeRaw.UpdateLayout();
                treeAsset.UpdateLayout();
                treeService.UpdateLayout();
                treeProduct.UpdateLayout();
            }
            catch
            {

            }
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

        private void treeProject_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = treeProject.SelectedItem_ as production_order_detail;
            if (production_order_detail != null)
            {
                if (production_order_detail.is_input)
                {
                    ToggleQuantity.IsChecked = false;
                    stpproduct.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    ToggleQuantity.IsChecked = true;
                    stpproduct.Visibility = System.Windows.Visibility.Visible;
                }

                List<production_order_detail> production_order_detailList = OrderDB.production_order_detail.ToList();
                cbxParent.ItemsSource = production_order_detailList.Where(x => x.id_production_order == production_order_detail.id_production_order && x != production_order_detail).ToList().ToList();
            }

            production_orderproduction_order_detailViewSource.View.MoveCurrentTo(production_order_detail);
            project_task_dimensionDataGrid.ItemsSource = production_order_detail.production_order_dimension.ToList();
        }

        private void item_movement_detailDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            item_movement item_movement = item_movementViewSource.View.CurrentItem as item_movement;
            production_execution_detail production_execution_detail = (production_execution_detail)production_execution_detailProductViewSource.View.CurrentItem;
            if (production_execution_detail != null)
            {
                if (item_movement != null)
                {
                    production_execution_detail.movement_id = (int)item_movement.id_movement;
                }

            }
        }

        private void ToggleQuantity_Checked(object sender, RoutedEventArgs e)
        {
            production_order_detail production_order_detail = treeProject.SelectedItem_ as production_order_detail;
            if (production_order_detail != null)
            {

                production_order_detail.is_input = false;


            }
        }

        private void ToggleQuantity_Unchecked(object sender, RoutedEventArgs e)
        {
            production_order_detail production_order_detail = treeProject.SelectedItem_ as production_order_detail;
            if (production_order_detail != null)
            {

                production_order_detail.is_input = true;


            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            production_order_detail production_order_detail = treeProject.SelectedItem_ as production_order_detail;
            if (production_order_detail != null)
            {

                production_order_detail.parent = null;

                production_orderproduction_order_detailViewSource.View.Refresh();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //StpMovement.Visibility = System.Windows.Visibility.Visible;
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order_detail production_order_detail = treeProject.SelectedItem_ as production_order_detail;
            if (production_order_detail != null)
            {


                if (production_order_detail.item != null && production_order_detail.is_input == false && production_order_detail.parent == null)
                {
                    //if (production_order_detail.item.id_item_type == entity.item.item_type.Task)
                    //{
                    production_order_detail n_production_order_detail = new production_order_detail();
                    n_production_order_detail.is_input = true;
                    n_production_order_detail.id_item = production_order_detail.id_item;
                    n_production_order_detail.item = production_order_detail.item;
                    n_production_order_detail.name = production_order_detail.item.name;
                    n_production_order_detail.id_production_order = production_order.id_production_order;
                    n_production_order_detail.production_order = production_order;
                    n_production_order_detail.production_order.status = Status.Production.Pending;
                    n_production_order_detail.quantity = 0;
                    n_production_order_detail.status = Status.Project.Pending;
                    n_production_order_detail.child.Add(production_order_detail);
                    //   production_order_detail.child.Add(n_production_order_detail);
                    OrderDB.production_order_detail.Add(n_production_order_detail);
                    production_orderproduction_order_detailViewSource.View.Refresh();
                    production_orderproduction_order_detailViewSource.View.MoveCurrentTo(n_production_order_detail);
                    //  }
                }
            }

        }

        #region Production Exexustion
        private void treeservice_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }



        public void filter_execution(CollectionViewSource CollectionViewSource, item.item_type item_type)
        {
            production_order production_order = (production_order)production_orderViewSource.View.CurrentItem;
            if (CollectionViewSource != null)
            {
                if (CollectionViewSource.View != null)
                {
                    CollectionViewSource.View.Filter = i =>
                    {
                        production_execution_detail objproduction_execution_detail = (production_execution_detail)i;
                        if (objproduction_execution_detail.item != null)
                        {
                            if (objproduction_execution_detail.item.id_item_type == item_type)
                            {
                                if (production_order != null)
                                {
                                    if (objproduction_execution_detail.production_order_detail.production_order == production_order)
                                    {
                                        return true;
                                    }
                                    return false;
                                }
                                else
                                {
                                    return false;
                                }

                            }
                            else { return false; }
                        }
                        else { return false; }
                    };
                }
            }

        }

        public void filter_order(CollectionViewSource CollectionViewSource, item.item_type item_type)
        {
            int id_production_order = 0;
            if (production_orderViewSource != null)
            {
                if (production_orderViewSource.View != null)
                {
                    if (production_orderViewSource.View.CurrentItem != null)
                    {
                        id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;
                    }
                }
            }


            if (CollectionViewSource != null)
            {

                List<production_order_detail> _production_order_detail =
                    OrderDB.production_order_detail.Where(a =>
                           a.status == Status.Project.Approved
                        && (a.item.id_item_type == item_type || a.item.id_item_type == item.item_type.Task)
                        && a.id_production_order == id_production_order)
                         .ToList();

                if (_production_order_detail.Count() > 0)
                {
                    CollectionViewSource.Source = _production_order_detail;
                }
                else
                {
                    CollectionViewSource.Source = null;
                }
            }

            if (CollectionViewSource != null)
            {
                if (CollectionViewSource.View != null)
                {
                    CollectionViewSource.View.Filter = i =>
                    {

                        production_order_detail production_order_detail = (production_order_detail)i;
                        if (production_order_detail.parent == null)
                        {

                            return true;

                        }
                        else { return false; }

                    };
                }
            }

        }
        private void itemserviceComboBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (CmbService.ContactID > 0)
            {
                contact contact = OrderDB.contacts.Where(x => x.id_contact == CmbService.ContactID).FirstOrDefault();
                if (contact != null)
                {
                    adddatacontact(contact, treeService);
                }
            }
        }
        public void adddatacontact(contact Data, cntrl.ExtendedTreeView treeview)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            if (production_order.production_execution.Count() == 0)
            {
                production_execution production_execution = new production_execution();
                production_execution.production_order = production_order;
                production_execution.id_production_line = production_order.id_production_line;
                production_execution.trans_date = DateTime.Now;
                OrderDB.production_execution.Add(production_execution);
                production_executionViewSource.View.Refresh();
                production_executionViewSource.View.MoveCurrentToLast();
            }

            production_order_detail production_order_detail = (production_order_detail)treeview.SelectedItem_;
            if (production_order_detail != null)
            {
                if (Data != null)
                {

                    //Product
                    int id = Convert.ToInt32(((contact)Data).id_contact);
                    if (id > 0)
                    {
                        production_execution _production_execution = (production_execution)production_executionViewSource.View.CurrentItem;
                        production_execution_detail _production_execution_detail = new entity.production_execution_detail();

                        //Check for contact
                        _production_execution_detail.id_contact = ((contact)Data).id_contact;
                        _production_execution_detail.contact = Data;
                        _production_execution_detail.quantity = 1;
                        _production_execution_detail.item = production_order_detail.item;
                        _production_execution_detail.id_item = production_order_detail.item.id_item;
                        _production_execution_detail.movement_id = production_order_detail.movement_id;
                        _production_execution.RaisePropertyChanged("quantity");

                        if (production_order_detail.item.id_item_type == item.item_type.Service)
                        {
                            if (cmbcoefficient.SelectedValue != null)
                            {
                                _production_execution_detail.id_time_coefficient = (int)cmbcoefficient.SelectedValue;
                            }

                            string start_date = string.Format("{0} {1}", dtpstartdate.Text, dtpstarttime.Text);
                            _production_execution_detail.start_date = Convert.ToDateTime(start_date);
                            string end_date = string.Format("{0} {1}", dtpenddate.Text, dtpendtime.Text);
                            _production_execution_detail.end_date = Convert.ToDateTime(end_date);

                            _production_execution_detail.id_production_execution = _production_execution.id_production_execution;
                            _production_execution_detail.production_execution = _production_execution;
                            _production_execution_detail.id_project_task = production_order_detail.id_project_task;
                            _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                            _production_execution_detail.production_order_detail = production_order_detail;

                            OrderDB.production_execution_detail.Add(_production_execution_detail);


                            production_execution_detailServiceContractViewSource.View.Refresh();
                            production_execution_detailServiceViewSource.View.MoveCurrentToLast();

                            loadServiceTotal(production_order_detail);
                          
                        }
                        else if (production_order_detail.item.id_item_type == item.item_type.ServiceContract)
                        {
                            if (cmbsccoefficient.SelectedValue != null)
                            {
                                _production_execution_detail.id_time_coefficient = (int)cmbsccoefficient.SelectedValue;
                            }

                            string start_date = string.Format("{0} {1}", dtpscstartdate.Text, dtpscstarttime.Text);
                            _production_execution_detail.start_date = Convert.ToDateTime(start_date);
                            string end_date = string.Format("{0} {1}", dtpscenddate.Text, dtpscendtime.Text);
                            _production_execution_detail.end_date = Convert.ToDateTime(end_date);

                            _production_execution_detail.id_production_execution = _production_execution.id_production_execution;
                            _production_execution_detail.production_execution = _production_execution;
                            _production_execution_detail.id_project_task = production_order_detail.id_project_task;
                            _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                            _production_execution_detail.production_order_detail = production_order_detail;

                            OrderDB.production_execution_detail.Add(_production_execution_detail);


                            production_execution_detailServiceContractViewSource.View.Refresh();
                          
                            loadServiceContractTotal(production_order_detail);
                        }


                    }
                }
            }
            else
            {
                toolBar.msgWarning("select Production order for insert");
            }
        }
        private void CmbServicecontract_Select(object sender, RoutedEventArgs e)
        {
            if (CmbServicecontract.ContactID > 0)
            {

                contact contact = OrderDB.contacts.Where(x => x.id_contact == CmbServicecontract.ContactID).FirstOrDefault();
                adddatacontact(contact, treeServicecontract);

            }

        }
        
        private void btnInsert_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tbx = sender as TextBox;
                Button btn = new Button();
                btn.Name = tbx.Name;
                btnInsert_Click(btn, e);

                //This is to clean contents after enter.
                tbx.Text = string.Empty;
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            itemMovementFraction = new Cognitivo.Configs.itemMovementFraction();

            production_order_detail production_order_detail = null;
            Button btn = sender as Button;
            decimal Quantity = 0M;

            if (btn.Name.Contains("Prod"))
            {
                Quantity = Convert.ToDecimal(txtProduct.Text);
                production_order_detail = treeProduct.SelectedItem_ as production_order_detail;
                itemMovementFraction.type = Configs.itemMovementFraction.Types.Product;
            }
            else if (btn.Name.Contains("Raw"))
            {
                Quantity = Convert.ToDecimal(txtRaw.Text);
                production_order_detail = treeRaw.SelectedItem_ as production_order_detail;
                itemMovementFraction.type = Configs.itemMovementFraction.Types.RawMaterial;
            }
            else if (btn.Name.Contains("Asset"))
            {
                Quantity = Convert.ToDecimal(txtAsset.Text);
                production_order_detail = treeAsset.SelectedItem_ as production_order_detail;
                itemMovementFraction.type = Configs.itemMovementFraction.Types.Asset;
            }
            else if (btn.Name.Contains("Supp"))
            {
                Quantity = Convert.ToDecimal(txtSupply.Text);
                production_order_detail = treeSupply.SelectedItem_ as production_order_detail;
                itemMovementFraction.type = Configs.itemMovementFraction.Types.Supplier;
            }
            else if (btn.Name.Contains("ServiceContract"))
            {
                Quantity = Convert.ToDecimal(txtServicecontract.Text);
                production_order_detail = treeServicecontract.SelectedItem_ as production_order_detail;
                itemMovementFraction.type = Configs.itemMovementFraction.Types.ServiceContract;
            }

            try
            {
                if (production_order_detail.is_input)
                {
                    if (production_order_detail != null && Quantity > 0 && (
                        itemMovementFraction.type == Configs.itemMovementFraction.Types.Product || 
                        itemMovementFraction.type == Configs.itemMovementFraction.Types.RawMaterial || 
                        itemMovementFraction.type == Configs.itemMovementFraction.Types.Supplier)
                        )
                    {
                        production_execution _production_execution = production_executionViewSource.View.CurrentItem as production_execution;

                        itemMovementFraction.id_item = (int)production_order_detail.id_item;
                        itemMovementFraction.OrderDB = OrderDB;
                        itemMovementFraction.production_order_detail = production_order_detail;
                        itemMovementFraction._production_execution = _production_execution;
                        itemMovementFraction.Quantity = Quantity;

                        crud_modal.Visibility = Visibility.Visible;
                        crud_modal.Children.Add(itemMovementFraction);
                    }
                }
                else
                {
                    Insert_IntoDetail(production_order_detail, Quantity);
                    itemMovementFraction.production_order_detail = production_order_detail;
                    RefeshData();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        public void RefeshData()
        {
            production_execution_detailRawViewSource.View.Refresh();
            production_execution_detailRawViewSource.View.MoveCurrentToLast();

            production_execution_detailSupplyViewSource.View.Refresh();
            production_execution_detailSupplyViewSource.View.MoveCurrentToLast();

            production_execution_detailProductViewSource.View.Refresh();
            production_execution_detailProductViewSource.View.MoveCurrentToLast();

            production_execution_detailAssetViewSource.View.Refresh();
            production_execution_detailAssetViewSource.View.MoveCurrentToLast();
            production_execution_detailServiceContractViewSource.View.Refresh();
            production_execution_detailServiceContractViewSource.View.MoveCurrentToLast();

            if (itemMovementFraction.type == Configs.itemMovementFraction.Types.Product)
            {
                loadProductTotal(itemMovementFraction.production_order_detail);
            }
            else if (itemMovementFraction.type == Configs.itemMovementFraction.Types.RawMaterial)
            {
                loadRawTotal(itemMovementFraction.production_order_detail);
            }
            else if (itemMovementFraction.type == Configs.itemMovementFraction.Types.Asset)
            {
                loadAssetTotal(itemMovementFraction.production_order_detail);
            }
            else if (itemMovementFraction.type == Configs.itemMovementFraction.Types.Supplier)
            {
                loadSupplierTotal(itemMovementFraction.production_order_detail);
            }
            else if (itemMovementFraction.type == Configs.itemMovementFraction.Types.ServiceContract)
            {
                loadServiceContractTotal(itemMovementFraction.production_order_detail);
            }
        }

        private void Insert_IntoDetail(production_order_detail production_order_detail, decimal Quantity)
        {
            production_execution _production_execution = production_executionViewSource.View.CurrentItem as production_execution;
            production_execution_detail _production_execution_detail = new entity.production_execution_detail();

            _production_execution_detail.State = EntityState.Added;
            _production_execution_detail.id_item = production_order_detail.id_item;
            _production_execution_detail.item = production_order_detail.item;
            _production_execution_detail.quantity = Quantity;
            _production_execution_detail.id_project_task = production_order_detail.id_project_task;
            _production_execution_detail.movement_id = production_order_detail.movement_id;

            if (production_order_detail.item.unit_cost != null)
            {
                _production_execution_detail.unit_cost = (decimal)production_order_detail.item.unit_cost;
            }

            _production_execution_detail.production_execution = _production_execution;
            _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;

            if (production_order_detail.item.is_autorecepie)
            {
                _production_execution_detail.is_input = false;
            }
            else
            {
                _production_execution_detail.is_input = true;
            }
            _production_execution.production_execution_detail.Add(_production_execution_detail);

        }

        private void dgServicecontract_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailServiceContractViewSource != null)
            {
                if (production_execution_detailServiceContractViewSource.View != null)
                {
                    production_execution_detail obj = production_execution_detailServiceContractViewSource.View.CurrentItem as production_execution_detail;

                    if (obj != null)
                    {
                        if (obj.id_item != null)
                        {
                            int _id_item = (int)obj.id_item;
                        }
                    }
                }
            }
        }



        private void dgSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailSupplyViewSource != null)
            {
                if (production_execution_detailSupplyViewSource.View != null)
                {
                    production_execution_detail obj = production_execution_detailSupplyViewSource.View.CurrentItem as production_execution_detail;

                    if (obj != null)
                    {
                        if (obj.id_item != null)
                        {
                            int _id_item = (int)obj.id_item;
                        }

                    }
                }
            }
        }

        private void dgproduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (production_execution_detailProductViewSource != null)
            {
                if (production_execution_detailProductViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailProductViewSource.View.CurrentItem;
                    if (obj != null)
                    {
                        if (obj.id_item != null)
                        {
                            int _id_item = (int)obj.id_item;
                        }
                    }

                }
            }


        }

        private void dgRaw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (production_execution_detailRawViewSource != null)
            {
                if (production_execution_detailRawViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailRawViewSource.View.CurrentItem;

                    if (obj != null)
                    {
                        if (obj.id_item != null)
                        {
                            int _id_item = (int)obj.id_item;
                        }
                    }
                }
            }
        }

        private void dgCapital_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailAssetViewSource != null)
            {
                if (production_execution_detailAssetViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailAssetViewSource.View.CurrentItem;
                    if (obj != null)
                    {
                        if (obj.id_item != null)
                        {
                            int _id_item = (int)obj.id_item;
                        }
                    }
                }
            }
        }
        private void treeSupply_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeSupply.SelectedItem;
            if (production_order_detail != null)
            {
                production_execution_detailSupplyViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;
                    if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail)
                    {
                        return true;
                    }
                    else { return false; }
                };


                loadSupplierTotal(production_order_detail);

            }
        }
        private void treeServicecontract_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeServicecontract.SelectedItem_;
            if (production_order_detail != null)
            {
                production_execution_detailServiceContractViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;

                    if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail)
                    {
                        return true;
                    }
                    else { return false; }
                };

                loadServiceContractTotal(production_order_detail);
            }

        }

        private void treeraw_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeRaw.SelectedItem_;
            if (production_order_detail != null)
            {
                production_execution_detailRawViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;
                    if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail && production_execution_detail.item.id_item_type == item.item_type.RawMaterial)
                    {
                        return true;
                    }
                    else { return false; }
                };



                loadRawTotal(production_order_detail);



                if (production_order_detail != null)
                {
                    if (production_order_detail.project_task != null)
                    {
                        int _id_task = production_order_detail.project_task.id_project_task;
                        project_task_dimensionViewSource = (CollectionViewSource)FindResource("project_task_dimensionViewSource");
                        project_task_dimensionViewSource.Source = OrderDB.project_task_dimension.Where(x => x.id_project_task == _id_task).ToList();
                    }
                }

            }
        }

        private void treeProduct_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeProduct.SelectedItem_;
            if (production_order_detail != null)
            {
                production_execution_detailProductViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;
                    if (production_execution_detail.item != null)
                    {
                        if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail && production_execution_detail.item.id_item_type == item.item_type.Product)
                        {
                            return true;
                        }
                        else { return false; }
                    }
                    else { return false; }
                };

                loadProductTotal(production_order_detail);

            }
        }

        private void treecapital_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeAsset.SelectedItem_;
            if (production_order_detail != null)
            {
                production_execution_detailAssetViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;
                    if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail && production_execution_detail.item.id_item_type == item.item_type.FixedAssets)
                    {
                        return true;
                    }
                    else { return false; }
                };


                loadAssetTotal(production_order_detail);

                production_execution_detailProductViewSource.View.Filter = null;
            }
        }

        public void loadProductTotal(production_order_detail production_order_detail)
        {
            production_execution _production_execution = (production_execution)production_executionViewSource.View.CurrentItem as production_execution;
            if (_production_execution != null)
            {
                decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.Product && x.id_order_detail == production_order_detail.id_order_detail).Sum(x => x.quantity);
                decimal projectedqty = production_order_detail.quantity;
                lblProjectedProductQty.Content = "Total:-" + projectedqty.ToString();
                lblTotalProduct.Content = "Total:-" + actuallqty.ToString();
                if (actuallqty > projectedqty)
                {
                    lblTotalProduct.Foreground = Brushes.Red;
                }
            }
        }
        public void loadRawTotal(production_order_detail production_order_detail)
        {
            production_execution _production_execution = (production_execution)production_executionViewSource.View.CurrentItem as production_execution;
            if (_production_execution != null)
            {
                decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.RawMaterial && x.id_order_detail == production_order_detail.id_order_detail).Sum(x => x.quantity);
                decimal projectedqty = production_order_detail.quantity;
                lblProjectedRawQty.Content = "Total:-" + projectedqty.ToString();
                lblTotalRaw.Content = "Total:-" + actuallqty.ToString();
                if (actuallqty > projectedqty)
                {
                    lblTotalRaw.Foreground = Brushes.Red;
                }
            }
        }
        public void loadAssetTotal(production_order_detail production_order_detail)
        {
            production_execution _production_execution = (production_execution)production_executionViewSource.View.CurrentItem as production_execution;
            if (_production_execution != null)
            {
                decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.FixedAssets && x.id_order_detail == production_order_detail.id_order_detail).Sum(x => x.quantity);
                decimal projectedqty = production_order_detail.quantity;
                lblProjectedassetqty.Content = "Total:-" + projectedqty.ToString();
                lblTotalasset.Content = "Total:-" + actuallqty.ToString();
                if (actuallqty > projectedqty)
                {
                    lblTotalasset.Foreground = Brushes.Red;
                }
            }
        }
        public void loadSupplierTotal(production_order_detail production_order_detail)
        {
            production_execution _production_execution = (production_execution)production_executionViewSource.View.CurrentItem as production_execution;
            if (_production_execution != null)
            {
                decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.Supplies && x.id_order_detail == production_order_detail.id_order_detail).Sum(x => x.quantity);
                decimal projectedqty = production_order_detail.quantity;
                lblProjectedSuppliesqty.Content = "Total:-" + projectedqty.ToString();
                lblTotalsupplies.Content = "Total:-" + actuallqty.ToString();
                if (actuallqty > projectedqty)
                {
                    lblTotalsupplies.Foreground = Brushes.Red;
                }
            }
        }
        public void loadServiceContractTotal(production_order_detail production_order_detail)
        {
            production_execution _production_execution = (production_execution)production_executionViewSource.View.CurrentItem as production_execution;
            if (_production_execution != null)
            {
                decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.ServiceContract && x.id_order_detail == production_order_detail.id_order_detail).Sum(x => x.quantity);
                decimal projectedqty = production_order_detail.quantity;
                lblProjectedservicecontrcthourqty.Content = "Total : " + projectedqty.ToString();
                lblProjectedServicecontractqty.Content = "Total : " + projectedqty.ToString();
                lblTotalServicecontract.Content = "Total : " + actuallqty.ToString();
                lblTotalservicecontracthour.Content = "Total : " + actuallqty.ToString();
                if (actuallqty > projectedqty)
                {
                    lblTotalServicecontract.Foreground = Brushes.Red;
                    lblTotalservicecontracthour.Foreground = Brushes.Red;
                }
            }
        }

        public void loadServiceTotal(production_order_detail production_order_detail)
        {
            production_execution _production_execution = (production_execution)production_executionViewSource.View.CurrentItem as production_execution;
            if (_production_execution != null)
            {
                decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.Service && x.id_order_detail == production_order_detail.id_order_detail).Sum(x => x.quantity);
                decimal projectedqty = production_order_detail.quantity;
                lblProjectedempqty.Content = "Total:-" + projectedqty.ToString();
                lblTotalemp.Content = "Total:-" + actuallqty.ToString();
                if (actuallqty > projectedqty)
                {
                    lblTotalemp.Foreground = Brushes.Red;
                }
            }

        }
        #endregion

        private void btnapprove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnanull_Click(object sender, RoutedEventArgs e)
        {

        }

        private void item_movement_detailDataGridraw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            item_movement item_movement = item_movementrawViewSource.View.CurrentItem as item_movement;
            production_execution_detail production_execution_detail = (production_execution_detail)production_execution_detailRawViewSource.View.CurrentItem;
            if (production_execution_detail != null)
            {
                if (item_movement != null)
                {
                    production_execution_detail.movement_id = (int)item_movement.id_movement;
                }

            }
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modal.Children != null)
            {
                crud_modal.Children.Clear();
            }

            RefeshData();

        }

        private void btnItemSelect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as production_execution_detail != null)
            {
                e.CanExecute = true;
            }

        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                DataGrid exexustiondetail = (DataGrid)e.Source;
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (production_executionViewSource.View.CurrentItem as production_execution != null)
                    {
                        production_execution production_execution = production_executionViewSource.View.CurrentItem as production_execution;

                        //DeleteDetailGridRow
                        exexustiondetail.CancelEdit();
                        production_execution_detail production_execution_detail = e.Parameter as production_execution_detail;
                        production_execution_detail.State = EntityState.Deleted;
                        OrderDB.production_execution_detail.Remove(production_execution_detail);
                        production_execution_detailAssetViewSource.View.Refresh();
                        production_execution_detailProductViewSource.View.Refresh();
                        production_execution_detailServiceViewSource.View.Refresh();
                        production_execution_detailRawViewSource.View.Refresh();
                    }

                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_executionViewSource != null)
            {


                if (production_executionViewSource.View != null)
                {
                    if (production_execution_detailAssetViewSource!=null)
                    {
                        if (production_execution_detailAssetViewSource.View != null)
                        {
                            production_execution_detailAssetViewSource.View.Refresh();
                        }
                    }
                    if (production_execution_detailProductViewSource != null)
                    {
                        if (production_execution_detailProductViewSource.View != null)
                        {
                            production_execution_detailProductViewSource.View.Refresh();
                        }
                    }
                    if (production_execution_detailServiceViewSource!=null)
                    {
                        if (production_execution_detailServiceViewSource.View != null)
                        {
                            production_execution_detailServiceViewSource.View.Refresh();

                        }
                    }

                    if (production_execution_detailRawViewSource!=null)
                    {
                        if (production_execution_detailRawViewSource.View != null)
                        {
                            production_execution_detailRawViewSource.View.Refresh();
                        }
                    }
                }
            }
        }
    }
}
