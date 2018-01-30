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
        // private OrderDB OrderDB = new OrderDB();

        private CollectionViewSource
            project_task_dimensionViewSource,
            production_orderViewSource,
            production_lineViewSource,
            production_orderproduction_order_detailViewSource;

        //cntrl.Curd.ItemRequest ItemRequest;
        private entity.Controller.Production.ExecutionController ExecutionDB;
        private entity.Controller.Production.OrderController OrderDB;

        public bool ViewAll { get; set; }

        public Order()
        {
            InitializeComponent();

            OrderDB = FindResource("OrderDB") as entity.Controller.Production.OrderController;
            ExecutionDB = FindResource("ExecutionDB") as entity.Controller.Production.ExecutionController;

            OrderDB.Initialize();
            ExecutionDB.Initialize();
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
            int LineID = OrderDB.db.production_line.Local.Select(x => x.id_production_line).FirstOrDefault();

            production_order Order = OrderDB.Create_Normal(LineID);
            Order.State = EntityState.Added;

            production_orderViewSource.View.Refresh();
            production_orderViewSource.View.MoveCurrentTo(Order);

            Update_Logistics();
            filter_task();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            OrderDB.Archive();
        }

        private void toolBar_btnEdit_Click(object sender)
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

        private void toolBar_btnSave_Click(object sender)
        {
            production_order _production_order = (production_order)production_orderDataGrid.SelectedItem;
            if ((_production_order.work_number == null || _production_order.work_number == string.Empty) && _production_order.id_range > 0)
            {
                if (_production_order.id_branch > 0)
                {
                    entity.Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == _production_order.id_branch).FirstOrDefault().code;
                }
                else
                {
                    entity.Brillo.Logic.Range.branch_Code = "";
                }
                if (_production_order.id_terminal > 0)
                {
                    entity.Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == _production_order.id_terminal).FirstOrDefault().code;
                }
                else
                {
                    entity.Brillo.Logic.Range.terminal_Code = "";
                }
                if (_production_order.id_user > 0)
                {
                    entity.Brillo.Logic.Range.user_Code = OrderDB.db.security_user.Where(x => x.id_user == _production_order.id_user).Select(x => x.code).FirstOrDefault();
                }
                else
                {
                    entity.Brillo.Logic.Range.user_Code = "";
                }
                if (_production_order.id_project > 0)
                {
                    entity.Brillo.Logic.Range.project_Code = OrderDB.db.projects.Where(x => x.id_project == _production_order.id_project).Select(x => x.code).FirstOrDefault();
                }
                else
                {
                    entity.Brillo.Logic.Range.project_Code = "";
                }

                app_document_range app_document_range = OrderDB.db.app_document_range.Where(x => x.id_range == _production_order.id_range).FirstOrDefault();
                _production_order.work_number = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);
                _production_order.RaisePropertyChanged("work_number");
            }
            OrderDB.SaveChanges_WithValidation();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            production_order _production_order = (production_order)production_orderDataGrid.SelectedItem;
            _production_order.State = EntityState.Unchanged;
            production_orderViewSource.View.Refresh();
        }

        private void Page_Loaded(object sender, EventArgs e)
        {
            production_lineViewSource = (CollectionViewSource)FindResource("production_lineViewSource");
            production_lineViewSource.Source = OrderDB.db.production_line.Local;

            Load();

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            app_dimensionViewSource.Source = OrderDB.db.app_dimension.Local;

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            app_measurementViewSource.Source = OrderDB.db.app_measurement.Local;

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
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(OrderDB.db, entity.App.Names.ProductionOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
          
        }

        private void Load()
        {
            OrderDB.Load(production_order.ProductionOrderTypes.Production, dataPager.PagedSource.PageIndex);
            production_orderViewSource = ((CollectionViewSource)(FindResource("production_orderViewSource")));

            production_orderViewSource.Source = OrderDB.db.production_order.Local.Where(x => x.is_archived == false);


            if (dataPager.PageCount == 0)
            {
                dataPager.PageCount = OrderDB.PageCount;
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
            if (MessageBox.Show("FINALIZAR PROYECTO: Está seguro de finalizarlo?", "Cognitivo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
                if (production_order != null)
                {
                    production_order.status = Status.Production.Executed;
                    if (OrderDB.SaveChanges_WithValidation())
                    {
                        toolBar.msgApproved(1);
                    }
                }
            }


        }

        private void toolBar_btnAnull_Click(object sender)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_order.status = Status.Production.Anull;
            OrderDB.SaveChanges_WithValidation();
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
                    list = OrderDB.db.production_order_detail
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
                    ItemRequest.listdepartment = OrderDB.db.app_department.ToList();
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

                        item item = OrderDB.db.items.Find(idItem);
                        if (item != null)
                        {
                            item_request_detail.item = item;
                            item_request_detail.comment = item_request_detail.item.name;
                        }

                        if (data.project_task != null)
                        {
                            item_request_detail.id_project_task = data.project_task.id_project_task;

                            List<project_task_dimension> project_task_dimensionList = OrderDB.db.project_task_dimension.Where(x => x.id_project_task == data.project_task.id_project_task).ToList();
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

                    OrderDB.db.item_request.Add(item_request);
                    OrderDB.SaveChanges_WithValidation();

                    Logistics_SelectionChanged(sender, null);
                }
            }

            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;
        }

        private void btnSaveTender_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Collapsed;
            OrderDB.SaveChanges_WithValidation();
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
                item item = OrderDB.db.items.Find(sbxItem.ItemID);

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
            stpitem.IsEnabled = true;

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
                        OrderDB.db.production_order_detail.Add(n_production_order_detail);
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
                    stpitem.IsEnabled = true;
                    stpcode.IsEnabled = true;
                }
            }
        }

        private void btnSaveTask_Click(object sender)
        {
            if (OrderDB.SaveChanges_WithValidation())
            {
                production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
                if (production_order != null)
                {
                    production_order.State = EntityState.Modified;
                    Update_Logistics();
                    stpcode.IsEnabled = false;
                    stpitem.IsEnabled = false;

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
                                OrderDB.db.Entry(production_order_detail).State = EntityState.Detached;
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
                OrderDB.Initialize();
                OrderDB.Load(production_order.ProductionOrderTypes.Production, dataPager.PagedSource.PageIndex);
                OrderDB.db.production_order.Where(a =>
                        a.id_company == CurrentSession.Id_Company &&
                        a.type != production_order.ProductionOrderTypes.Fraction &&
                        a.is_archived == false &&
                        a.production_line.app_location.id_branch == CurrentSession.Id_Branch)
                    .Include(z => z.project)
                    .OrderByDescending(x => x.trans_date).Load();
                production_orderViewSource.Source = OrderDB.db.production_order.Local;

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

            foreach (production_order_detail production_order_detail in _production_order_detail.Where(x => x.IsSelected == true && x.status != Status.Production.Anull))
            {
                if (production_order_detail.item.id_item_type != item.item_type.Task)
                {
                    production_order_detail.status = Status.Production.Approved;
                }
            }

            if (OrderDB.SaveChanges_WithValidation())
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
            OrderDB.SaveChanges_WithValidation();
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

        private void slider_LostFocus(object sender, EventArgs e)
        {
            OrderDB.SaveChanges_WithValidation();
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
                    project_task_dimensionViewSource.Source = OrderDB.db.project_task_dimension.Where(x => x.id_project_task == _id_task).ToList();
                }
            }
        }

        private void btnAnull_Click(object sender)
        {

            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            production_orderproduction_order_detailViewSource.View.Filter = null;

            List<production_order_detail> _production_order_detail = treeProject.ItemsSource.Cast<production_order_detail>().ToList();

            foreach (production_order_detail production_order_detail in _production_order_detail.Where(x => x.IsSelected == true))
            {
                if (production_order_detail.item.id_item_type != item.item_type.Task)
                {
                    production_order_detail.status = Status.Production.Anull;
                }
            }

            if (OrderDB.SaveChanges_WithValidation())
            {
                filter_task();
                toolBar.msgSaved(OrderDB.NumberOfRecords);
            }
        }

        private void dataPager_OnDemandLoading(object sender, Syncfusion.UI.Xaml.Controls.DataPager.OnDemandLoadingEventArgs e)
        {
            Load();
        }

        private void SearchInSource_Click(object sender, System.Windows.Input.KeyEventArgs e, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                Load();
                //Brings data into view.
                toolBar_btnSearch_Click(sender, query);
            }
            else
            {
                production_orderViewSource = ((CollectionViewSource)(FindResource("production_orderViewSource")));
                production_orderViewSource.Source = OrderDB.db.production_order
                    .Where
                    (
                    x =>
                    x.name.Contains(query) &&
                    x.type == production_order.ProductionOrderTypes.Production
                    ).ToListAsync();
            }
        }

        private void toolBar_btnFocus_Click(object sender)
        {
            if (toolBar.ref_id > 0)
            {
                production_orderViewSource = FindResource("production_orderViewSource") as CollectionViewSource;
                production_orderViewSource.Source = OrderDB.db.production_order.Where(x => x.id_production_order == toolBar.ref_id).ToList();
            }
        }

        private void toolBar_btnClear_Click(object sender)
        {

            OrderDB.Initialize();
            Load();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void TechnicalReport_Click(object sender, RoutedEventArgs e)
        {

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