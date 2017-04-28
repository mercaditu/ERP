using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Production
{
    public partial class FractionOrder : Page
    {
        //private ExecutionDB ExecutionDB = new ExecutionDB();

        private CollectionViewSource
            project_task_dimensionViewSource,
            projectViewSource,
            production_orderViewSource,
            production_lineViewSource,
            production_orderproduction_order_detailViewSource,
            production_order_detaillServiceViewSource,
            item_movementViewSource,
            item_movementrawViewSource,
            production_executionproduction_execustion_detailViewSource,
            production_execution_detailProductViewSource,
            production_execution_detailRawViewSource,
            production_execution_detailAssetViewSource,
            production_execution_detailServiceViewSource,
            production_execution_detailSupplyViewSource,
            production_execution_detailServiceContractViewSource,
            production_order_detaillProductViewSource,
            production_order_detaillRawViewSource,
            production_order_detaillAssetViewSource,
            production_order_detaillSupplyViewSource,
            production_order_detaillServiceContractViewSource,
            production_order_dimensionViewSource;

        private entity.Controller.Production.ExecutionController ExecutionDB;
        private entity.Controller.Production.OrderController OrderDB;

        public FractionOrder()
        {
            InitializeComponent();

            OrderDB = FindResource("") as entity.Controller.Production.OrderController;
            OrderDB.Initialize();
        }

        private void New_Click(object sender)
        {
            production_order Order = OrderDB.Create();
            production_orderViewSource.View.MoveCurrentTo(Order);
        }

        private void Delete_Click(object sender)
        {
            OrderDB.Archive();
        }

        private void Edit_Click(object sender)
        {
            if (production_orderDataGrid.SelectedItem != null)
            {
                production_order Order = production_orderDataGrid.SelectedItem as production_order;
                OrderDB.Edit(Order);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void Save_Click(object sender)
        {
            production_order _production_order = (production_order)production_orderDataGrid.SelectedItem;
            if ((_production_order.work_number == null || _production_order.work_number == string.Empty) && _production_order.id_range > 0)
            {
                if (_production_order.id_branch > 0)
                {
                    entity.Brillo.Logic.Range.branch_Code = OrderDB.db.app_branch.Where(x => x.id_branch == _production_order.id_branch).FirstOrDefault().code;
                }
                if (_production_order.id_terminal > 0)
                {
                    entity.Brillo.Logic.Range.terminal_Code = OrderDB.db.app_terminal.Where(x => x.id_terminal == _production_order.id_terminal).FirstOrDefault().code;
                }

                app_document_range app_document_range = OrderDB.db.app_document_range.Where(x => x.id_range == _production_order.id_range).FirstOrDefault();
                _production_order.work_number = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);
                _production_order.RaisePropertyChanged("work_number");
            }

            OrderDB.db.SaveChanges();
        }

        private void Cancel_Click(object sender)
        {
            //ExecutionDB.CancelAllChanges();
            production_order _production_order = (production_order)production_orderDataGrid.SelectedItem;
            _production_order.State = EntityState.Unchanged;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //projectViewSource = FindResource("projectViewSource") as CollectionViewSource;
            //projectViewSource.Source = ExecutionDB.projects.Where(a => a.id_company == CurrentSession.Id_Company).ToList();

            production_lineViewSource = FindResource("production_lineViewSource") as CollectionViewSource;
            production_lineViewSource.Source = OrderDB.db.production_line.Local;

            production_orderViewSource = FindResource("production_orderViewSource") as CollectionViewSource;
            production_orderViewSource.Source = OrderDB.db.production_order.Local;

            production_order_detaillServiceViewSource = FindResource("production_order_detaillServiceViewSource") as CollectionViewSource;

            production_executionproduction_execustion_detailViewSource = FindResource("production_executionproduction_execustion_detailViewSource") as CollectionViewSource;

            production_orderproduction_order_detailViewSource = FindResource("production_orderproduction_order_detailViewSource") as CollectionViewSource;
            production_order_dimensionViewSource = FindResource("production_order_dimensionViewSource") as CollectionViewSource;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(OrderDB.db, entity.App.Names.ProductionOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);

            item_movementViewSource = FindResource("item_movementViewSource") as CollectionViewSource;
            item_movementrawViewSource = FindResource("item_movementrawViewSource") as CollectionViewSource;

            if (production_orderproduction_order_detailViewSource.View != null)
            {
                cbxParent.ItemsSource = production_orderproduction_order_detailViewSource.View.OfType<production_order_detail>().ToList();

                production_orderproduction_order_detailViewSource.View.Filter = null;
                filter_task();
            }

            #region Execution

            CollectionViewSource hr_time_coefficientViewSource = FindResource("hr_time_coefficientViewSource") as CollectionViewSource;
            hr_time_coefficientViewSource.Source = OrderDB.db.hr_time_coefficient.Local;

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

            #endregion Execution
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

        private void Approve_Click(object sender)
        {
            Save_Click(sender);

            if (ExecutionDB.Approve(production_order.ProductionOrderTypes.Fraction) > 0)
            {
                toolBar.msgApproved(1);
            }
        }

        private void Anull_Click(object sender)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order.status = Status.Production.QA_Rejected;
            OrderDB.db.SaveChanges();
        }

        private void productionorderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter_task();

            filter_order(production_order_detaillProductViewSource, item.item_type.Product);
            filter_order(production_order_detaillRawViewSource, item.item_type.RawMaterial);
            filter_order(production_order_detaillSupplyViewSource, item.item_type.Supplies);
            filter_order(production_order_detaillServiceViewSource, item.item_type.Service);
            filter_order(production_order_detaillAssetViewSource, item.item_type.FixedAssets);
            filter_order(production_order_detaillServiceContractViewSource, item.item_type.ServiceContract);
        }

        private void Search_Click(object sender, string query)
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

                        return false;
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

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
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
                    project_task_dimensionViewSource.Source = OrderDB.db.project_task_dimension.Where(x => x.id_project_task == _id_task).ToList();
                }
            }
        }

        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
                item item = OrderDB.db.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (item != null && item.id_item > 0 && item.is_autorecepie && production_order != null)
                {
                    if (treeProject.SelectedItem is production_order_detail production_order_detail_output)
                    {
                        production_order_detail_output.id_item = item.id_item;
                        production_order_detail_output.item = item;
                        production_order_detail_output.name = item.name;
                        production_order_detail_output.RaisePropertyChanged("item");
                        production_order_detail_output.is_input = false;
                        production_order_detail_output.quantity = 1;
                        production_order_detail_output.production_order = production_order;
                        production_order_detail_output.id_production_order = production_order.id_production_order;

                        foreach (item_dimension item_dimension in item.item_dimension)
                        {
                            production_order_dimension production_order_dimension = new production_order_dimension()
                            {
                                id_dimension = item_dimension.id_app_dimension,
                                app_dimension = item_dimension.app_dimension,
                                id_measurement = item_dimension.id_measurement,
                                app_measurement = item_dimension.app_measurement,
                                value = item_dimension.value
                            };
                            production_order_detail_output.production_order_dimension.Add(production_order_dimension);
                            project_task_dimensionDataGrid.ItemsSource = production_order_detail_output.production_order_dimension.ToList();
                        }
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

                    if (treeProject.SelectedItem is production_order_detail production_order_detail_output)
                    {
                        production_order_detail_output.quantity = 1;
                        production_order_detail_output.name = item.name;
                        production_order_detail_output.id_item = item.id_item;
                        production_order_detail_output.item = item;
                        production_order_detail_output.RaisePropertyChanged("item");
                        production_order_detail_output.is_input = false;
                        production_order_detail_output.production_order = production_order;
                        production_order_detail_output.id_production_order = production_order.id_production_order;
                        foreach (item_dimension item_dimension in item.item_dimension)
                        {
                            production_order_dimension production_order_dimension = new production_order_dimension()
                            {
                                id_dimension = item_dimension.id_app_dimension,
                                id_measurement = item_dimension.id_measurement,
                                app_measurement = item_dimension.app_measurement,
                                value = item_dimension.value
                            };

                            production_order_dimension.RaisePropertyChanged("id_dimension");
                            app_dimension app_dimension = OrderDB.db.app_dimension.Where(x => x.id_dimension == item_dimension.id_app_dimension).FirstOrDefault();

                            if (app_dimension != null)
                            {
                                production_order_dimension.app_dimension = app_dimension;
                            }

                            production_order_dimension.RaisePropertyChanged("app_dimension");
                            production_order_detail_output.production_order_dimension.Add(production_order_dimension);
                            project_task_dimensionDataGrid.ItemsSource = production_order_detail_output.production_order_dimension.ToList();
                        }
                    }
                }
            }
            RefreshData();
        }

        private void btnNewTask_Click(object sender)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order_detail production_order_detail = treeProject.SelectedItem as production_order_detail;

            if (production_order_detail != null)
            {
                //Adding a Child Item.
                if (production_order_detail.item != null)
                {
                    production_order_detail n_production_order_detail = new production_order_detail()
                    {
                        is_input = false,
                        id_item = production_order_detail.id_item,
                        item = production_order_detail.item,
                        name = production_order_detail.item.name,
                        id_production_order = production_order.id_production_order,
                        production_order = production_order,
                        quantity = 0,
                        status = Status.Production.Pending
                    };

                    n_production_order_detail.production_order.status = Status.Production.Pending;
                    if (production_order_detail.item != null)
                    {
                        foreach (item_dimension item_dimension in production_order_detail.item.item_dimension)
                        {
                            production_order_dimension production_order_dimension = new production_order_dimension()
                            {
                                id_dimension = item_dimension.id_app_dimension,
                                app_dimension = item_dimension.app_dimension,
                                id_measurement = item_dimension.id_measurement,
                                app_measurement = item_dimension.app_measurement,
                                value = 0
                            };

                            n_production_order_detail.production_order_dimension.Add(production_order_dimension);
                        }
                    }

                    production_order_detail.child.Add(n_production_order_detail);

                    OrderDB.db.production_order_detail.Add(n_production_order_detail);
                    project_task_dimensionDataGrid.ItemsSource = production_order_detail.production_order_dimension.ToList();

                    production_orderproduction_order_detailViewSource.View.Refresh();
                    production_orderproduction_order_detailViewSource.View.MoveCurrentTo(n_production_order_detail);
                }
            }
            else
            {
                //Adding First Item
                production_order_detail n_production_order_detail = new production_order_detail();
                n_production_order_detail.status = Status.Production.Pending;
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
            if (OrderDB.db.SaveChanges() > 0)
            {
                production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
                production_order.State = EntityState.Modified;
                stpcode.IsEnabled = false;
            }
        }

        private void btnDeleteTask_Click(object sender)
        {
            production_orderproduction_order_detailViewSource.View.Filter = null;
            List<production_order_detail> production_order_detailLIST = treeProject.ItemsSource.Cast<production_order_detail>().ToList();
            production_order_detailLIST = production_order_detailLIST.Where(x => x.IsSelected == true).ToList();

            foreach (production_order_detail production_order_detail in production_order_detailLIST)
            {
                //if pending, simply remove.
                if (production_order_detail.status == Status.Production.Pending)
                {
                    OrderDB.db.production_order_detail.Remove(production_order_detail);
                }
                else
                {
                    production_order_detail.status = Status.Production.Anull;
                }

                production_order_detail.IsSelected = false;
            }

            if (OrderDB.db.SaveChanges() > 0)
            {
                toolBar.msgSaved(1);
                filter_task();
            }
        }

        private void toolIcon_Click(object sender)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_orderproduction_order_detailViewSource.View.Filter = null;

            List<production_order_detail> _production_order_detail = treeProject.ItemsSource.Cast<production_order_detail>().ToList();
            _production_order_detail = _production_order_detail.Where(x => x.IsSelected == true && (x.status == entity.Status.Production.Pending || x.status == null)).ToList();

            if (_production_order_detail.Count > 0)
            {
                foreach (production_order_detail production_order_detail in _production_order_detail)
                {
                    if (production_order_detail.parent != null && production_order_detail.parent.status == entity.Status.Production.Pending)
                    {
                        production_order_detail.parent.status = entity.Status.Production.Approved;
                    }

                    production_order_detail.status = entity.Status.Production.Approved;
                }

                if (OrderDB.db.SaveChanges() > 0)
                {
                    filter_task();
                    toolBar.msgSaved(1);
                }

                try
                {
                    filter_order(production_order_detaillProductViewSource, item.item_type.Product);
                    filter_order(production_order_detaillRawViewSource, item.item_type.RawMaterial);
                    filter_order(production_order_detaillSupplyViewSource, item.item_type.Supplies);
                    filter_order(production_order_detaillServiceViewSource, item.item_type.Service);
                    filter_order(production_order_detaillAssetViewSource, item.item_type.FixedAssets);
                    filter_order(production_order_detaillServiceContractViewSource, item.item_type.ServiceContract);

                    RefreshData();
                    RefreshTree();
                }
                catch { }
            }
        }

        private void toolIcon_Click_1(object sender)
        {
            List<production_order_detail> _production_order_detail = treeProject.ItemsSource.Cast<production_order_detail>().ToList();
            _production_order_detail = _production_order_detail.Where(x => x.IsSelected == true).ToList();

            foreach (production_order_detail production_order_detail in _production_order_detail)
            {
                production_order_detail.status = Status.Production.QA_Rejected;
            }

            OrderDB.db.SaveChanges();
        }

        private void btnAddParentTask_Click(object sender)
        {
            stpcode.IsEnabled = true;

            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order_detail n_production_order_detail = new production_order_detail();
            n_production_order_detail.status = Status.Production.Pending;
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
                    stpproduct.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ToggleQuantity.IsChecked = true;
                    stpproduct.Visibility = Visibility.Visible;
                }

                List<production_order_detail> production_order_detailList = OrderDB.db.production_order_detail.ToList();
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
                production_order_detail.RaisePropertyChanged("is_input");
            }
        }

        private void ToggleQuantity_Unchecked(object sender, RoutedEventArgs e)
        {
            production_order_detail production_order_detail = treeProject.SelectedItem_ as production_order_detail;
            if (production_order_detail != null)
            {
                production_order_detail.is_input = true;
                production_order_detail.RaisePropertyChanged("is_input");
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
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;

            if (treeProject.SelectedItem_ is production_order_detail production_order_detail)
            {
                if (production_order_detail.item != null && production_order_detail.is_input == false && production_order_detail.parent == null)
                {
                    production_order_detail n_production_order_detail = new production_order_detail()
                    {
                        is_input = true,
                        id_item = production_order_detail.id_item,
                        item = production_order_detail.item,
                        name = production_order_detail.item.name,
                        id_production_order = production_order.id_production_order,
                        production_order = production_order,
                        quantity = 0,
                        status = Status.Production.Pending
                    };

                    n_production_order_detail.production_order.status = Status.Production.Pending;
                    production_order_detail.parent = n_production_order_detail;

                    n_production_order_detail.child.Add(production_order_detail);
                    OrderDB.db.production_order_detail.Add(n_production_order_detail);
                    production_orderproduction_order_detailViewSource.View.Refresh();
                    production_orderproduction_order_detailViewSource.View.MoveCurrentTo(n_production_order_detail);
                }
            }
        }

        #region Production Exexustion

        private async void treeservice_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeService.SelectedItem_;
            if (production_order_detail != null)
            {
                production_execution_detailServiceViewSource.Source = await OrderDB.db.production_execution_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToListAsync();
                //production_order_detaillProductViewSource.View.MoveCurrentTo(production_order_detail);
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
                    OrderDB.db.production_order_detail.Where(a =>
                           (a.status >= Status.Production.Approved)
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
                contact contact = OrderDB.db.contacts.Where(x => x.id_contact == CmbService.ContactID).FirstOrDefault();
                if (contact != null)
                {
                    adddatacontact(contact, treeService);
                }
                production_order_detail production_order_detail = (production_order_detail)treeService.SelectedItem;
                if (production_order_detail != null)
                {
                    production_execution_detailServiceViewSource.Source = OrderDB.db.production_execution_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToList();
                }
            }
        }

        public void adddatacontact(contact Data, cntrl.ExtendedTreeView treeview)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order_detail production_order_detail = (production_order_detail)treeview.SelectedItem_;
            if (production_order_detail != null)
            {
                if (Data != null)
                {
                    //Product
                    int id = Convert.ToInt32(((contact)Data).id_contact);
                    if (id > 0)
                    {
                        //  production_execution _production_execution = (production_execution)production_executionViewSource.View.CurrentItem;
                        production_execution_detail _production_execution_detail = new entity.production_execution_detail();

                        //Check for contact
                        _production_execution_detail.id_contact = Data.id_contact;
                        _production_execution_detail.contact = Data;
                        _production_execution_detail.quantity = 1;
                        _production_execution_detail.item = production_order_detail.item;
                        _production_execution_detail.id_item = production_order_detail.item.id_item;
                        _production_execution_detail.movement_id = production_order_detail.movement_id;
                        //   _production_execution.RaisePropertyChanged("quantity");

                        hr_contract contract = OrderDB.db.hr_contract.Where(x => x.id_contact == id && x.is_active).FirstOrDefault();
                        if (contract != null)
                        {
                            _production_execution_detail.unit_cost = contract.Hourly;
                        }

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

                            _production_execution_detail.id_project_task = production_order_detail.id_project_task;
                            _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                            _production_execution_detail.production_order_detail = production_order_detail;

                            OrderDB.db.production_execution_detail.Add(_production_execution_detail);

                            production_execution_detailServiceContractViewSource.View.Refresh();
                            production_execution_detailServiceViewSource.View.MoveCurrentToLast();
                            RefreshExecution();
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

                            _production_execution_detail.id_project_task = production_order_detail.id_project_task;
                            _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                            _production_execution_detail.production_order_detail = production_order_detail;

                            OrderDB.db.production_execution_detail.Add(_production_execution_detail);
                            production_execution_detailServiceContractViewSource.View.Refresh();
                            RefreshExecution();
                        }
                    }
                }
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void CmbServicecontract_Select(object sender, RoutedEventArgs e)
        {
            if (CmbServicecontract.ContactID > 0)
            {
                contact contact = OrderDB.db.contacts.Where(x => x.id_contact == CmbServicecontract.ContactID).FirstOrDefault();
                adddatacontact(contact, treeServicecontract);
                production_order_detail production_order_detail = (production_order_detail)treeServicecontract.SelectedItem;
                if (production_order_detail != null)
                {
                    production_execution_detailServiceContractViewSource.Source = OrderDB.db.production_execution_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToList();
                }
            }
        }

        private void btnInsert_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tbx = sender as TextBox;

                Button btn = new Button()
                {
                    Name = tbx.Name
                };

                btnInsert_Click(btn, e);

                //This is to clean contents after enter.
                tbx.Text = string.Empty;
            }
        }

        private void RefreshCollection(CollectionViewSource Collection)
        {
            if (Collection != null)
            {
                if (Collection.View != null)
                {
                    Collection.View.Refresh();
                }
            }
        }

        private void RefreshCollection_Move2Current(CollectionViewSource Collection)
        {
            if (Collection != null)
            {
                if (Collection.View != null)
                {
                    Collection.View.Refresh();
                    Collection.View.MoveCurrentToLast();
                }
            }
        }

        private void RefreshExecution()
        {
            RefreshCollection(production_execution_detailProductViewSource);
            RefreshCollection(production_execution_detailRawViewSource);
            RefreshCollection(production_execution_detailSupplyViewSource);
            RefreshCollection(production_execution_detailAssetViewSource);
            RefreshCollection(production_execution_detailServiceContractViewSource);
        }

        public void RefreshData()
        {
            //RefreshCollection(production_orderViewSource);
            if (production_orderViewSource != null)
            {
                if (production_orderViewSource.View != null)
                {
                    production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
                    if (production_order != null)
                    {
                        foreach (production_order_detail production_order_detail in production_order.production_order_detail)
                        {
                            production_order_detail.CalcExecutedQty_TimerTaks();
                        }
                    }
                }
            }

            RefreshCollection(production_order_detaillRawViewSource);
            RefreshCollection(production_order_detaillProductViewSource);
            RefreshCollection(production_order_detaillSupplyViewSource);
            RefreshCollection(production_order_detaillAssetViewSource);
            RefreshCollection(production_order_detaillServiceViewSource);
            RefreshCollection(production_order_detaillServiceContractViewSource);
            RefreshCollection(production_order_dimensionViewSource);
        }

        public void RefreshTree()
        {
            treeRaw.UpdateLayout();
            treeAsset.UpdateLayout();
            treeService.UpdateLayout();
            treeProduct.UpdateLayout();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            production_order_detail production_order_detail = null;
            Button btn = sender as Button;
            decimal Quantity = 0M;
            CollectionViewSource Collection = null;

            //Assign in case of If function rejects.
            item.item_type type = item.item_type.Task;

            if (btn.Name.Contains("Prod"))
            {
                Quantity = Convert.ToDecimal(txtProduct.Text);

                production_order_detail _production_order_detail = treeProduct.SelectedItem_ as production_order_detail;
                if (_production_order_detail != null)
                {
                    production_order_detail = OrderDB.db.production_order_detail.Where(x => x.id_order_detail == _production_order_detail.id_order_detail).FirstOrDefault();
                    type = item.item_type.Product;
                    Collection = production_execution_detailProductViewSource;
                }
            }
            else if (btn.Name.Contains("Raw"))
            {
                Quantity = Convert.ToDecimal(txtRaw.Text);

                production_order_detail _production_order_detail = treeRaw.SelectedItem_ as production_order_detail;
                if (_production_order_detail != null)
                {
                    production_order_detail = OrderDB.db.production_order_detail.Where(x => x.id_order_detail == _production_order_detail.id_order_detail).FirstOrDefault();
                    type = item.item_type.RawMaterial;
                    Collection = production_execution_detailRawViewSource;
                }
            }
            else if (btn.Name.Contains("Asset"))
            {
                Quantity = Convert.ToDecimal(txtAsset.Text);
                production_order_detail _production_order_detail = treeAsset.SelectedItem_ as production_order_detail;
                if (_production_order_detail != null)
                {
                    production_order_detail = OrderDB.db.production_order_detail.Where(x => x.id_order_detail == _production_order_detail.id_order_detail).FirstOrDefault();
                    type = item.item_type.FixedAssets;
                    Collection = production_execution_detailAssetViewSource;
                }
            }
            else if (btn.Name.Contains("Supp"))
            {
                Quantity = Convert.ToDecimal(txtSupply.Text);
                production_order_detail _production_order_detail = treeSupply.SelectedItem_ as production_order_detail;
                if (_production_order_detail != null)
                {
                    production_order_detail = OrderDB.db.production_order_detail.Where(x => x.id_order_detail == _production_order_detail.id_order_detail).FirstOrDefault();
                    type = item.item_type.Supplies;
                    Collection = production_execution_detailSupplyViewSource;
                }
            }
            else if (btn.Name.Contains("ServiceContract"))
            {
                Quantity = Convert.ToDecimal(txtServicecontract.Text);
                production_order_detail _production_order_detail = treeServicecontract.SelectedItem_ as production_order_detail;
                if (_production_order_detail != null)
                {
                    production_order_detail = OrderDB.db.production_order_detail.Where(x => x.id_order_detail == _production_order_detail.id_order_detail).FirstOrDefault();
                    type = item.item_type.ServiceContract;
                    Collection = production_execution_detailServiceContractViewSource;
                }
            }

            try
            {
                if (production_order_detail != null)
                {
                    if (production_order_detail.is_input)
                    {
                        if (production_order_detail != null && Quantity > 0 && (
                            type == item.item_type.Product ||
                            type == item.item_type.RawMaterial ||
                            type == item.item_type.Supplies)
                            )
                        {
                            if (production_order_detail.item.item_dimension.Count() > 0)
                            {
                                Cognitivo.Configs.itemMovementFraction DimensionPanel = new Cognitivo.Configs.itemMovementFraction();
                                //production_execution _production_execution = production_executionViewSource.View.CurrentItem as production_execution;

                                DimensionPanel.id_item = (int)production_order_detail.id_item;
                                DimensionPanel.ExecutionDB = OrderDB.db;
                                DimensionPanel.production_order_detail = production_order_detail;
                                //  DimensionPanel._production_execution = _production_execution;
                                DimensionPanel.Quantity = Quantity;

                                crud_modal.Visibility = Visibility.Visible;
                                crud_modal.Children.Add(DimensionPanel);
                            }
                            else
                            {
                                Insert_IntoDetail(production_order_detail, Quantity);
                                RefreshExecution();
                            }
                        }
                        else
                        {
                            Insert_IntoDetail(production_order_detail, Quantity);
                            RefreshExecution();
                        }
                    }
                    else
                    {
                        Insert_IntoDetail(production_order_detail, Quantity);
                        RefreshExecution();
                    }
                    if (production_order_detail != null)
                    {
                        Collection.Source = OrderDB.db.production_execution_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToList();
                        //production_order_detaillProductViewSource.View.MoveCurrentTo(production_order_detail);
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void Insert_IntoDetail(production_order_detail production_order_detail, decimal Quantity)
        {
            // production_execution _production_execution = production_executionViewSource.View.CurrentItem as production_execution;
            production_execution_detail _production_execution_detail = new entity.production_execution_detail();

            //Adds Parent so that during approval, because it is needed for approval.
            if (production_order_detail.parent != null)
            {
                if (production_order_detail.parent.production_execution_detail != null)
                {
                    _production_execution_detail.parent = production_order_detail.parent.production_execution_detail.FirstOrDefault();
                }
            }

            // _production_execution_detail.Type = production_execution_detail.Types.Fraction;
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

            //  _production_execution_detail.production_execution = _production_execution;
            _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
            _production_execution_detail.is_input = production_order_detail.is_input;

            foreach (production_order_dimension production_order_dimension in production_order_detail.production_order_dimension)
            {
                production_execution_dimension production_execution_dimension = new production_execution_dimension()
                {
                    id_dimension = production_order_dimension.id_dimension,
                    value = production_order_dimension.value,
                    id_measurement = production_order_dimension.id_measurement
                };

                _production_execution_detail.production_execution_dimension.Add(production_execution_dimension);
            }

            production_order_detail.production_execution_detail.Add(_production_execution_detail);
            OrderDB.db.SaveChanges();
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

        private async void treeSupply_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeSupply.SelectedItem;
            if (production_order_detail != null)
            {
                production_execution_detailSupplyViewSource.Source = await OrderDB.db.production_execution_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToListAsync();
                //production_order_detaillProductViewSource.View.MoveCurrentTo(production_order_detail);
            }
        }

        private async void treeServicecontract_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeServicecontract.SelectedItem_;
            if (production_order_detail != null)
            {
                production_execution_detailServiceContractViewSource.Source = await OrderDB.db.production_execution_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToListAsync();
                //production_order_detaillProductViewSource.View.MoveCurrentTo(production_order_detail);
            }
        }

        private async void treeraw_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeRaw.SelectedItem_;
            if (production_order_detail != null)
            {
                production_execution_detailRawViewSource.Source = await OrderDB.db.production_execution_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToListAsync();
                //production_order_detaillProductViewSource.View.MoveCurrentTo(production_order_detail);
            }

            if (production_order_detail != null)
            {
                if (production_order_detail.project_task != null)
                {
                    int _id_task = production_order_detail.project_task.id_project_task;
                    project_task_dimensionViewSource = (CollectionViewSource)FindResource("project_task_dimensionViewSource");
                    project_task_dimensionViewSource.Source = OrderDB.db.project_task_dimension.Where(x => x.id_project_task == _id_task).ToList();
                }
            }
        }

        private async void treeProduct_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = treeProduct.SelectedItem_ as production_order_detail;
            if (production_order_detail != null)
            {
                production_execution_detailProductViewSource.Source = await OrderDB.db.production_execution_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToListAsync();
                //production_order_detaillProductViewSource.View.MoveCurrentTo(production_order_detail);
            }
        }

        private async void treecapital_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeAsset.SelectedItem_;
            if (production_order_detail != null)
            {
                production_execution_detailAssetViewSource.Source = await OrderDB.db.production_execution_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToListAsync();
                //production_order_detaillProductViewSource.View.MoveCurrentTo(production_order_detail);
            }
        }

        #endregion Production Exexustion

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

            RefreshData();
            RefreshExecution();
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
                    exexustiondetail.CancelEdit();
                    production_execution_detail production_execution_detail = e.Parameter as production_execution_detail;
                    production_execution_detail.State = EntityState.Deleted;
                    OrderDB.db.production_execution_detail.Remove(production_execution_detail);

                    RefreshExecution();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() == typeof(TabControl))
            {
                RefreshData();
                RefreshExecution();
            }
        }
    }
}