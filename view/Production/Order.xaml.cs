using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Production
{
    public partial class Order : Page, INotifyPropertyChanged
    {
        private OrderDB OrderDB = new OrderDB();

        private CollectionViewSource
            project_task_dimensionViewSource,
            production_orderViewSource,
            production_lineViewSource,
            production_orderproduction_order_detailViewSource;

        //cntrl.Curd.ItemRequest ItemRequest;

        public bool ViewAll { get; set; }

        public Order()
        {
            InitializeComponent();
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
                        if (production_order.name.ToLower().Contains(query.ToLower()) && production_order.type == entity.production_order.ProductionOrderTypes.Production)
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

            filter_task();
        }

        private void toolBar_btnNew_Click(object sender)
        {
            int LineID = OrderDB.production_line.Local.Select(x => x.id_production_line).FirstOrDefault();
            production_order production_order = OrderDB.New("", production_order.ProductionOrderTypes.Production, LineID);
            production_order.State = EntityState.Added;
            OrderDB.production_order.Add(production_order);
           OrderDB.SaveChanges();
            production_orderViewSource.View.Refresh();
            production_orderViewSource.View.MoveCurrentTo(production_order);
            Update_Logistics();
            filter_task();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Archive?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                foreach (production_order production_order in OrderDB.production_order.Local.Where(x => x.IsSelected))
                {
                    production_order.is_archived = true;
                }

                toolBar_btnSave_Click(sender);
                Load();
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
                    entity.Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == _production_order.id_branch).FirstOrDefault().code;
                }
                if (_production_order.id_terminal > 0)
                {
                    entity.Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == _production_order.id_terminal).FirstOrDefault().code;
                }

                app_document_range app_document_range = OrderDB.app_document_range.Find(_production_order.id_range);
                _production_order.work_number = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);
                _production_order.RaisePropertyChanged("work_number");
            }
            OrderDB.SaveChanges();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            production_order _production_order = (production_order)production_orderDataGrid.SelectedItem;
            _production_order.State = EntityState.Unchanged;
            production_orderViewSource.View.Refresh();
        }

        private async void Page_Loaded(object sender, EventArgs e)
        {
            production_lineViewSource = (CollectionViewSource)FindResource("production_lineViewSource");
            production_lineViewSource.Source = await OrderDB.production_line.Where(x =>
                    x.id_company == CurrentSession.Id_Company &&
                    x.app_location.id_branch == CurrentSession.Id_Branch).ToListAsync();

            Load();

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            app_dimensionViewSource.Source = await OrderDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).ToListAsync();

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            app_measurementViewSource.Source = await OrderDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).ToListAsync();

            production_orderproduction_order_detailViewSource = ((CollectionViewSource)(FindResource("production_orderproduction_order_detailViewSource")));

            if (production_orderproduction_order_detailViewSource.View != null)
            {
                production_orderproduction_order_detailViewSource.View.Filter = null;
                filter_task();
            }

            if (!CurrentSession.User.security_role.see_cost)
            {
                btncost.Visibility = Visibility.Collapsed;
            }

            cmbtype.ItemsSource = Enum.GetValues(typeof(production_order.ProductionOrderTypes)).Cast<production_order.ProductionOrderTypes>().ToList();
            cbxItemType.ItemsSource = Enum.GetValues(typeof(item.item_type)).Cast<item.item_type>().ToList();
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(OrderDB, entity.App.Names.ProductionOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
        }

        private async void Load()
        {
            production_orderViewSource = ((CollectionViewSource)(FindResource("production_orderViewSource")));
            await OrderDB.production_order.Where(a =>
                    a.id_company == CurrentSession.Id_Company &&
                    a.type != production_order.ProductionOrderTypes.Fraction &&
                    a.is_archived == false &&
                    a.production_line.app_location.id_branch == CurrentSession.Id_Branch)
                .Include(z => z.project)
                .OrderByDescending(x => x.trans_date)
                .LoadAsync();
            production_orderViewSource.Source = OrderDB.production_order.Local.Where(x => x.is_archived == false);
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
            if (production_order != null)
            {
                production_order.status = Status.Production.Executed;
                if (OrderDB.SaveChanges() > 0)
                {
                    toolBar.msgApproved(1);
                }
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
            Update_Logistics();
            filter_task();
        }

        private void Update_Logistics()
        {
            //Task taskdb = Task.Factory.StartNew(() =>
            Calculate_Logistics();//);
        }

        private void Calculate_Logistics()
        {
            if (production_orderViewSource != null)
            {
                production_order production_order = production_orderViewSource.View.CurrentItem as production_order;

                if (production_order != null)
                {
                    int _id_production_order = production_order.id_production_order;
                    List<Class.Logistics> LogisticsList = new List<Class.Logistics>();

                    if (_id_production_order > 0)
                    {
                        Class.Production Production = new Class.Production();
                        LogisticsList.AddRange(Production.Return_OrderLogistics(_id_production_order));
                    }

                    item_ProductDataGrid.ItemsSource = LogisticsList.Where(x => x.Type == item.item_type.Product).ToList();
                    item_RawDataGrid.ItemsSource = LogisticsList.Where(x => x.Type == item.item_type.RawMaterial).ToList();
                    item_SupplierDataGrid.ItemsSource = LogisticsList.Where(x => x.Type == item.item_type.Supplies).ToList();
                    item_CapitalDataGrid.ItemsSource = LogisticsList.Where(x => x.Type == item.item_type.FixedAssets).ToList();
                    item_ServiceContractDataGrid.ItemsSource = LogisticsList.Where(x => x.Type == item.item_type.ServiceContract).ToList();
                }
            }
        }

        private void Logistics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int _id_production_order = 0;
            _id_production_order = ((production_order)production_orderViewSource.View.CurrentItem).id_production_order;

            Class.Logistics Logistics = new Class.Logistics();

            if (item_ProductDataGrid.SelectedItem != null)
            {
                Logistics = (Class.Logistics)item_ProductDataGrid.SelectedItem;
            }
            else if (item_RawDataGrid.SelectedItem != null)
            {
                Logistics = (Class.Logistics)item_RawDataGrid.SelectedItem;
            }
            else if (item_ServiceContractDataGrid.SelectedItem != null)
            {
                Logistics = (Class.Logistics)item_ServiceContractDataGrid.SelectedItem;
            }
            else if (item_CapitalDataGrid.SelectedItem != null)
            {
                Logistics = (Class.Logistics)item_CapitalDataGrid.SelectedItem;
            }
            else if (item_SupplierDataGrid.SelectedItem != null)
            {
                Logistics = (Class.Logistics)item_SupplierDataGrid.SelectedItem;
            }

            List<production_order_detail> list = new List<production_order_detail>();

            if (Logistics != null)
            {
                int ItemID = Logistics.ItemID;

                if (ItemID > 0)
                {
                    list = OrderDB.production_order_detail
                        .Where(p =>
                        (
                        p.production_order.status != Status.Production.Pending || p.production_order.status != null) &&
                        p.id_production_order == _id_production_order &&
                        p.id_item == ItemID
                        )
                        .ToList();

                    itemDataGrid.ItemsSource = list.ToList();
                }
            }
            else
            {
                itemDataGrid.ItemsSource = null;
            }
        }

        private void btnRequestResource_Click(object sender, RoutedEventArgs e)
        {
            if (itemDataGrid.ItemsSource != null)
            {
                production_order production_order = ((production_order)production_orderViewSource.View.CurrentItem);
                List<production_order_detail> DetailList = production_order.production_order_detail.Where(x => x.IsSelected == true).ToList();

                if (DetailList.Count() > 0)
                {
                    cntrl.Curd.ItemRequest ItemRequest = new cntrl.Curd.ItemRequest();
                    crud_modal.Visibility = Visibility.Visible;
                    ItemRequest.listdepartment = OrderDB.app_department.ToList();
                    ItemRequest.item_request_Click += item_request_Click;

                    if (production_order.id_project > 0 && production_order.project != null)
                    {
                        ItemRequest.name = production_order.project.name;
                    }

                    crud_modal.Children.Add(ItemRequest);
                }
                else
                {
                    toolBar.msgWarning("Select a Task");
                }
            }
        }

        public void item_request_Click(object sender)
        {
            cntrl.Curd.ItemRequest ItemRequest = crud_modal.Children.Cast<cntrl.Curd.ItemRequest>().First();

            production_order production_order = ((production_order)production_orderViewSource.View.CurrentItem);

            if (production_order != null)
            {
                int id_production_order = production_order.id_production_order;

                List<production_order_detail> production_order_detaillist = new List<production_order_detail>();
                production_order_detaillist = production_order.production_order_detail.Where(x => x.IsSelected == true).ToList();

                if (production_order_detaillist.Count() > 0)
                {
                    item_request item_request = new item_request();
                    item_request.name = ItemRequest.name;
                    item_request.comment = ItemRequest.comment;

                    item_request.id_department = ItemRequest.id_department;
                    item_request.id_production_order = id_production_order;

                    if (production_order.id_project != null)
                    {
                        item_request.id_project = production_order.id_project;
                    }
                    if (production_order.production_line != null)
                    {
                        if (production_order.production_line.app_location != null)
                        {
                            item_request.id_branch = production_order.production_line.app_location.id_branch;
                        }
                    }
                    if (item_request.id_branch == null || item_request.id_branch <= 0)
                    {
                        item_request.id_branch = CurrentSession.Id_Branch;
                    }

                    item_request.request_date = DateTime.Now;

                    foreach (production_order_detail data in production_order_detaillist.Where(x => x.IsSelected == true))
                    {
                        data.IsSelected = false;

                        item_request_detail item_request_detail = new item_request_detail();

                        item_request_detail.date_needed_by = ItemRequest.neededDate;
                        item_request_detail.id_order_detail = data.id_order_detail;
                        item_request_detail.urgency = ItemRequest.Urgencies;
                        int idItem = data.item.id_item;
                        item_request_detail.id_item = idItem;

                        item item = OrderDB.items.Find(idItem);
                        if (item != null)
                        {
                            item_request_detail.item = item;
                            item_request_detail.comment = item_request_detail.item.name;
                        }

                        if (data.project_task != null)
                        {
                            item_request_detail.id_project_task = data.project_task.id_project_task;

                            List<project_task_dimension> project_task_dimensionList = OrderDB.project_task_dimension.Where(x => x.id_project_task == data.project_task.id_project_task).ToList();
                            foreach (project_task_dimension project_task_dimension in project_task_dimensionList)
                            {
                                item_request_dimension item_request_dimension = new item_request_dimension();
                                item_request_dimension.id_dimension = project_task_dimension.id_dimension;
                                item_request_dimension.id_measurement = project_task_dimension.id_measurement;
                                item_request_dimension.value = project_task_dimension.value;
                                string comment = item_request_detail.item.name;

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

                    Logistics_SelectionChanged(sender, null);
                }
            }

            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;
        }

        private void btnSaveTender_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Collapsed;
            OrderDB.SaveChanges();
        }

        private void lblCancel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Collapsed;
        }


        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
                item item = OrderDB.items.Find(sbxItem.ItemID);

                if (item != null && item.id_item > 0 && item.item_recepie.Count > 0 && production_order != null)
                {
                    production_order_detail production_order_detail_output = treeProject.SelectedItem_ as production_order_detail;
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
                        //treeProject.SelectedItem_ = production_order_detail_output;
                    }
                }
                else
                {
                    production_order_detail production_order_detail_output = treeProject.SelectedItem_ as production_order_detail;

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
                        // treeProject.SelectedItem_ = production_order_detail_output;
                    }
                }
            }
        }

        private void btnNewTask_Click(object sender)
        {
            stpcode.IsEnabled = true;
            stackQuantity.IsEnabled = true;

            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order_detail production_order_detail = treeProject.SelectedItem as production_order_detail;

            if (production_order_detail != null)
            {
                //Adding a Child Item.
                if (production_order_detail.item != null)
                {
                    if (production_order_detail.item.id_item_type == entity.item.item_type.Task)
                    {
                        production_order_detail n_production_order_detail = new production_order_detail()
                        {
                            id_production_order = production_order.id_production_order,
                            production_order = production_order,
                            quantity = 0,
                            status = Status.Production.Pending
                        };

                        n_production_order_detail.production_order.status = Status.Production.Pending;

                        production_order_detail.child.Add(n_production_order_detail);
                        OrderDB.production_order_detail.Add(n_production_order_detail);
                        production_orderproduction_order_detailViewSource.View.Refresh();
                        production_orderproduction_order_detailViewSource.View.MoveCurrentTo(n_production_order_detail);
                        treeProject.SelectedItem_ = n_production_order_detail;
                    }
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
                treeProject.SelectedItem_ = n_production_order_detail;
            }
        }

        private void btnEditTask_Click(object sender)
        {
            production_order_detail production_order_detail = treeProject.SelectedItem as production_order_detail;
            if (production_order_detail != null)
            {
                if (production_order_detail.project_task != null)
                {
                    toolBar.msgWarning("Access Denied. Order linked to Approved Task.");
                }
                else
                {
                    stackQuantity.IsEnabled = true;
                    stpcode.IsEnabled = true;
                }
            }
        }

        private void btnSaveTask_Click(object sender)
        {
            if (OrderDB.SaveChanges() > 0)
            {
                production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
                if (production_order != null)
                {
                    production_order.State = EntityState.Modified;
                    Update_Logistics();
                    stpcode.IsEnabled = false;
                    stackQuantity.IsEnabled = false;

                    toolBar.msgSaved(OrderDB.NumberOfRecords);
                }
            }
        }

        private void btnDeleteTask_Click(object sender)
        {
            if (production_orderproduction_order_detailViewSource.View != null)
            {
                production_orderproduction_order_detailViewSource.View.Filter = null;
                List<production_order_detail> _production_order_detail = treeProject.ItemsSource.Cast<production_order_detail>().ToList();

                OrderDB.NumberOfRecords = 0;
                foreach (production_order_detail production_order_detail in _production_order_detail.Where(x => x.IsSelected == true))
                {
                    if (production_order_detail.status == Status.Production.Pending)
                    {
                        using (db db = new db())
                        {
                            if (production_order_detail.id_project_task != 0)
                            {
                                production_order_detail deltproduction_order_detail = db.production_order_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).FirstOrDefault();
                                List<production_order_detail> production_order_detaillist = deltproduction_order_detail.child.ToList();
                                foreach (production_order_detail childproduction_order_detail in production_order_detaillist)
                                {
                                    db.production_order_detail.Remove(childproduction_order_detail);
                                }
                                db.production_order_detail.Remove(deltproduction_order_detail);
                                db.SaveChanges();
                            }
                            else
                            {
                                OrderDB.Entry(production_order_detail).State = EntityState.Detached;
                            }
                        }

                        OrderDB.NumberOfRecords += 1;
                    }
                    else
                    {
                        //ProjectTaskDB.SaveChanges();
                        toolBar_btnAnull_Click(sender);
                    }
                }
                OrderDB = new OrderDB();
                OrderDB.production_order.Where(a =>
                        a.id_company == CurrentSession.Id_Company &&
                        a.type != production_order.ProductionOrderTypes.Fraction &&
                        a.is_archived == false &&
                        a.production_line.app_location.id_branch == CurrentSession.Id_Branch)
                    .Include(z => z.project)
                    .OrderByDescending(x => x.trans_date).Load();
                production_orderViewSource.Source = OrderDB.production_order.Local;

                toolBar.msgSaved(OrderDB.NumberOfRecords);
                filter_task();

                treeProject.UpdateLayout();
            }
        }

        private void cbxItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbxItemType = (ComboBox)sender;

            if (cbxItemType.SelectedItem != null)
            {
                item.item_type Item_Type = (item.item_type)cbxItemType.SelectedItem;
                sbxItem.item_types = Item_Type;

                if (Item_Type == item.item_type.Task)
                {
                    stpdate.Visibility = Visibility.Visible;
                    stackQuantity.Visibility = Visibility.Collapsed;
                }
                else
                {
                    stpdate.Visibility = Visibility.Collapsed;
                    stackQuantity.Visibility = Visibility.Visible;
                }
            }
        }

        private void toolIcon_Click(object sender)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_orderproduction_order_detailViewSource.View.Filter = null;

            List<production_order_detail> _production_order_detail = treeProject.ItemsSource.Cast<production_order_detail>().ToList();

            foreach (production_order_detail production_order_detail in _production_order_detail.Where(x => x.IsSelected == true))
            {
                if (production_order_detail.item.id_item_type != item.item_type.Task)
                {
                    production_order_detail.status = Status.Production.Approved;
                }
            }

            if (OrderDB.SaveChanges() > 0)
            {
                filter_task();
                toolBar.msgSaved(OrderDB.NumberOfRecords);
            }
        }

        private void toolIcon_Click_1(object sender)
        {
            List<production_order_detail> _production_order_detail = treeProject.ItemsSource.Cast<production_order_detail>().ToList();
            _production_order_detail = _production_order_detail.Where(x => x.IsSelected == true).ToList();

            foreach (production_order_detail production_order_detail in _production_order_detail)
            {
                if (production_order_detail.item.id_item_type != item.item_type.Task)
                {
                    production_order_detail.status = entity.Status.Production.QA_Rejected;
                }
            }
            OrderDB.SaveChanges();
        }

        private void btnAddParentTask_Click(object sender)
        {
            stpcode.IsEnabled = true;

            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            if (production_order != null)
            {
                production_order_detail n_production_order_detail = new production_order_detail();
                n_production_order_detail.status = Status.Production.Pending;
                production_order.production_order_detail.Add(n_production_order_detail);

                production_orderproduction_order_detailViewSource.View.Refresh();
                production_orderproduction_order_detailViewSource.View.MoveCurrentTo(n_production_order_detail);
                filter_task();
                treeProject.SelectedItem_ = n_production_order_detail;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            if (production_order != null)
            {
                List<production_order_detail> production_order_detailList = production_order.production_order_detail.Where(x => x.is_input).ToList();
                List<production_order_detail> production_order_detailOutputList = production_order.production_order_detail.Where(x => x.is_input == false).ToList();
                if (production_order_detailList.Count > 0)
                {
                    cntrl.PanelAdv.pnlCostCalculation pnlCostCalculation = new cntrl.PanelAdv.pnlCostCalculation();
                    pnlCostCalculation.Inputproduction_order_detailList = production_order_detailList;
                    pnlCostCalculation.Outputproduction_order_detailList = production_order_detailOutputList;
                    crud_modal_cost.Visibility = Visibility.Visible;
                    crud_modal_cost.Children.Add(pnlCostCalculation);
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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update_Logistics();
        }

        private async void slider_LostFocus(object sender, EventArgs e)
        {
            await OrderDB.SaveChangesAsync();
        }

        private void btnExpandAll_Checked(object sender, RoutedEventArgs e)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            ViewAll = !ViewAll;

            foreach (production_order_detail production_order_detail in production_order.production_order_detail)
            {
                production_order_detail.is_read = ViewAll;
                production_order_detail.RaisePropertyChanged("is_read");
            }
        }

        private void itemDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemDataGrid.SelectedItem is production_order_detail obj)
            {
                if (obj.project_task != null)
                {
                    int _id_task = obj.project_task.id_project_task;
                    project_task_dimensionViewSource = (CollectionViewSource)FindResource("project_task_dimensionViewSource");
                    project_task_dimensionViewSource.Source = OrderDB.project_task_dimension.Where(x => x.id_project_task == _id_task).ToList();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void Slider_LostFocus(object sender, RoutedEventArgs e)
        {
            if (production_orderproduction_order_detailViewSource.View.CurrentItem is production_order_detail production_order_detail)
            {
                using (db db = new db())
                {
                    production_order_detail _production_order_detail =
                        db.production_order_detail
                        .Where(x => x.id_production_order == production_order_detail.id_production_order)
                        .FirstOrDefault();
                    if (_production_order_detail != null)
                    {
                        _production_order_detail.completed = production_order_detail.completed;
                        _production_order_detail.importance = production_order_detail.importance;
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}