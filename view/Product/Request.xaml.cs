using entity;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using System;
using System.Windows.Input;
using System.ComponentModel;

namespace Cognitivo.Product
{
    public partial class Request : Page
    {
        item_requestDB dbContext = new item_requestDB();
        CollectionViewSource item_requestViewSource;
        CollectionViewSource item_requestitem_request_detailViewSource, item_request_detailitem_request_decisionViewSource;
        Configs.itemMovement itemMovement = new Configs.itemMovement();

        public Request()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            item_requestViewSource = ((CollectionViewSource)(FindResource("item_requestViewSource")));
            await dbContext.item_request.Where(x => x.id_company == CurrentSession.Id_Company).ToListAsync();
            item_requestViewSource.Source = dbContext.item_request.Local;

            item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detailitem_request_decisionViewSource = ((CollectionViewSource)(FindResource("item_request_detailitem_request_decisionViewSource")));

            CollectionViewSource app_branchViewSource = ((CollectionViewSource)(FindResource("app_branchViewSource")));
            app_branchViewSource.Source = CurrentSession.Branches;

            CollectionViewSource app_currencyViewSource = ((CollectionViewSource)(FindResource("app_currencyViewSource")));
            app_currencyViewSource.Source = CurrentSession.Currencies;

            CollectionViewSource app_departmentViewSource = ((CollectionViewSource)(FindResource("app_departmentViewSource")));
            app_departmentViewSource.Source = await dbContext.app_department.Where(x => x.id_company == CurrentSession.Id_Company).ToListAsync();
            cmburgency.ItemsSource = Enum.GetValues(typeof(item_request_detail.Urgencies));

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(dbContext, entity.App.Names.RequestManagement, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
        }



        private void toolBar_btnApprove_Click(object sender)
        {
            itemMovement = new Configs.itemMovement();
            dbContext.Approve();
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as item_request_decision != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {

                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;
                    //DeleteDetailGridRow
                    item_request_detailDataGrid.CancelEdit();
                    //item_request_detail.item_request_decision.Remove(e.Parameter as item_request_decision);
                    dbContext.item_request_decision.Remove(e.Parameter as item_request_decision);
                    item_requestitem_request_detailViewSource.View.Refresh();
                    item_request_detailitem_request_decisionViewSource.View.Refresh();
                    //calculate_total(sender, e);
                    
                    item_request_detail.item_request.GetTotalDecision();
                    item_request_detail.RaisePropertyChanged("balance");
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void item_request_detailMovementDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // entity.Properties.Settings setting = new entity.Properties.Settings();
            int id_branch = CurrentSession.Id_Branch;
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = (item_request_detail)item_requestitem_request_detailViewSource.View.CurrentItem;
            if (item_request_detail != null)
            {


                item _item = dbContext.items.Where(x => x.id_item == item_request_detail.id_item).FirstOrDefault();
                var movement =
                        (from items in dbContext.item_movement
                         where items.status == Status.Stock.InStock
                         && items.item_product.id_item == item_request_detail.id_item
                         && items.app_location.id_branch == id_branch
                         group items by new { items.app_location }
                             into last
                             select new
                             {
                                 id_location = last.Key.app_location.id_location,
                                 location = last.Key.app_location.name,
                                 quntitiy = last.Sum(x => x.credit) - last.Sum(x => x.debit),
                             }).ToList();


                List<desion> list_desion = new List<desion>();
                foreach (dynamic item in movement)
                {
                    // int id_location = (int)item.location;
                    desion desion = new desion();
                    desion.id_item = _item.id_item;
                    desion.id_location = item.id_location;
                    desion.location = item.location;
                    desion.name = _item.name;
                    desion.avlqty = item.quntitiy;
                    desion.decisionqty = 0;
                    desion.decisionState = state.added;
                    list_desion.Add(desion);
                }
                item_request_decisionmovementDataGrid.ItemsSource = list_desion;


                var transfer =
                (from items in dbContext.item_movement
                 where items.status == Status.Stock.InStock
                 && items.item_product.id_item == item_request_detail.id_item

                 group items by new { items.app_location.app_branch }
                     into last
                     select new
                     {
                         id_location = last.Key.app_branch.app_location.Where(x => x.is_default).FirstOrDefault().id_location,
                         branch = last.Key.app_branch.name,
                         quntitiy = last.Sum(x => x.credit) - last.Sum(x => x.debit),

                     }).ToList();
                List<desion> list_desion_transfer = new List<desion>();
                foreach (dynamic item in transfer)
                {
                    desion desion = new desion();
                    desion.branch = item.branch;
                    desion.id_location = item.id_location;
                    desion.id_item = _item.id_item;
                    desion.name = _item.name;
                    desion.avlqty = item.quntitiy;
                    desion.decisionqty = 0;
                    desion.decisionState = state.added;
                    list_desion_transfer.Add(desion);
                }
                item_request_decisiontransferDataGrid.ItemsSource = list_desion_transfer;

                List<desion> list_desion_purchase = new List<desion>();
                desion purchasedesion = new desion();
                purchasedesion.decisionState = state.added;
                purchasedesion.decisionqty = 0;
                list_desion_purchase.Add(purchasedesion);
                item_request_decisionpurchaseDataGrid.ItemsSource = list_desion_purchase;

                List<desion> list_desion_internal = new List<desion>();
                desion internaldesion = new desion();
                internaldesion.decisionState = state.added;
                internaldesion.decisionqty = 0;
                list_desion_internal.Add(internaldesion);
                item_request_decisioninternalDataGrid.ItemsSource = list_desion_internal;


                List<desion> list_desion_production = new List<desion>();
                desion productiondesion = new desion();
                productiondesion.name = item_request_detail.item.name;
                productiondesion.RaisePropertyChanged("name");
                productiondesion.decisionqty = 0;
                productiondesion.decisionState = state.added;
                list_desion_production.Add(productiondesion);
                item_request_decisionproductionDataGrid.ItemsSource = list_desion_production;
            }
        }

        private void item_request_decisionDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = (item_request_detail)item_requestitem_request_detailViewSource.View.CurrentItem;
            if (item_request_decisionmovementDataGrid.SelectedItem != null)
            {
                desion desion = (desion)item_request_decisionmovementDataGrid.SelectedItem;
                if (desion.avlqty < desion.decisionqty)
                {
                    toolBar.msgWarning("quantity is greater than available quantity");
                    return;
                }

                if (desion.decisionState == state.added)
                {
                    if (dbContext.items.Where(x => x.id_item == item_request_detail.id_item).FirstOrDefault().item_dimension.Count() > 0)
                    {
                        crud_modal.Children.Clear();
                        itemMovement.id_item = item_request_detail.id_item;
                        itemMovement.id_location = desion.id_location;
                        itemMovement.db = dbContext;

                        crud_modal.Visibility = Visibility.Visible;
                        crud_modal.Children.Add(itemMovement);
                    }
                    else
                    {
                        desion.decisionState = state.modified;
                        item_request_decision item_request_decision = new global::entity.item_request_decision();
                        item_request_decision.IsSelected = true;
                        item_request_decision.id_location = desion.id_location;
                        item_request_decision.quantity = desion.decisionqty;
                        item_request_decision.decision = global::entity.item_request_decision.Decisions.Movement;
                        item_request_detail.item_request_decision.Add(item_request_decision);
                    }
                }
                else
                {

                }

            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("balance");
            dbContext.SaveChanges();
            item_requestViewSource.View.MoveCurrentToLast();
            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            toolBar_btnEdit_Click(sender);

        }

        private void toolBar_btnNew_Click(object sender)
        {
            item_request item_request = new item_request();

            item_request.IsSelected = true;

            app_document_range app_document_range = entity.Brillo.Logic.Range.List_Range(dbContext, entity.App.Names.RequestManagement, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
            if (app_document_range != null)
            {
                //Gets List of Ranges avaiable for this Document.
                item_request.id_range = app_document_range.id_range;
            }

            // dbContext.sales_invoice.Local.Add(sales_invoice);
            dbContext.Entry(item_request).State = EntityState.Added;
            item_request.State = EntityState.Added;
            item_requestViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (item_requestDataGrid.SelectedItem != null)
            {
                item_request item_request_old = (item_request)item_requestDataGrid.SelectedItem;
                item_request_old.IsSelected = true;
                item_request_old.State = EntityState.Modified;
                dbContext.Entry(item_request_old).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }

        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (dbContext.SaveChanges() > 0)
            {
                item_requestViewSource.View.Refresh();
                toolBar.msgSaved(dbContext.NumberOfRecords);
            }
        }

        private void item_request_decisiontransferDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = (item_request_detail)item_requestitem_request_detailViewSource.View.CurrentItem;

            if (item_request_decisiontransferDataGrid.SelectedItem != null)
            {
                desion desion = (desion)item_request_decisiontransferDataGrid.SelectedItem;
                if (desion.avlqty < desion.decisionqty)
                {
                    toolBar.msgWarning("quantity is greater than available quantity");
                    return;
                }
                if (desion.decisionState == state.added)
                {
                    if (dbContext.items.Where(x => x.id_item == item_request_detail.id_item).FirstOrDefault().item_dimension.Count() > 0)
                    {
                        crud_modalTransfer.Children.Clear();
                        itemMovement.id_item = item_request_detail.id_item;
                        itemMovement.id_location = desion.id_location;
                        itemMovement.db = dbContext;

                        crud_modalTransfer.Visibility = Visibility.Visible;
                        crud_modalTransfer.Children.Add(itemMovement);
                    }
                    else
                    {

                        desion.decisionState = state.modified;
                        item_request_decision item_request_decision = new global::entity.item_request_decision();
                        item_request_decision.IsSelected = true;
                        item_request_decision.id_location = desion.id_location;
                        item_request_decision.quantity = desion.decisionqty;
                        item_request_decision.decision = global::entity.item_request_decision.Decisions.Transfer;
                        item_request_detail.item_request_decision.Add(item_request_decision);
                    }
                }
                else
                {
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("balance");
            dbContext.SaveChanges();
            item_requestViewSource.View.MoveCurrentToLast();
            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            toolBar_btnEdit_Click(sender);
        }

        private void item_request_decisionpurchaseDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;

            if (item_request_decisionpurchaseDataGrid.SelectedItem != null)
            {
                desion desion = (desion)item_request_decisionpurchaseDataGrid.SelectedItem;

                if (desion.decisionState == state.added)
                {
                    desion.decisionState = state.modified;
                    item_request_decision item_request_decision = new global::entity.item_request_decision();
                    item_request_decision.IsSelected = true;
                    item_request_decision.quantity = desion.decisionqty;
                    item_request_decision.decision = global::entity.item_request_decision.Decisions.Purchase;
                    item_request_detail.item_request_decision.Add(item_request_decision);
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("balance");
            dbContext.SaveChanges();
            item_requestViewSource.View.MoveCurrentToLast();
            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            toolBar_btnEdit_Click(sender);
        }

        public enum state
        {
            added, modified
        }

        public class desion : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void RaisePropertyChanged(string prop)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(prop));
                }
            }
            public int id_item { get; set; }
            public string name { get; set; }
            public int id_location { get; set; }
            public string location { get; set; }
            public string branch { get; set; }
            public decimal avlqty { get; set; }
            public decimal decisionqty { get; set; }
            public state decisionState { get; set; }
        }

        private void item_request_detailMovementDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = (item_request_detail)item_requestitem_request_detailViewSource.View.CurrentItem;
            CollectionViewSource project_task_dimensionViewSource = ((CollectionViewSource)(FindResource("project_task_dimensionViewSource")));
            if (item_request_detail != null)
            {
                if (item_request_detail.id_project_task > 0)
                {
                    project_task_dimensionViewSource.Source = dbContext.project_task_dimension.Where(x => x.id_project_task == item_request_detail.id_project_task).ToList();
                }
            }
        }

        private void item_request_decisionproductionDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;

            if (item_request_decisionproductionDataGrid.SelectedItem != null)
            {
                desion desion = (desion)item_request_decisionproductionDataGrid.SelectedItem;

                if (desion.decisionState == state.added)
                {
                    desion.decisionState = state.modified;
                    item_request_decision item_request_decision = new global::entity.item_request_decision();
                    item_request_decision.IsSelected = true;
                    item_request_decision.quantity = desion.decisionqty;
                    item_request_decision.decision = global::entity.item_request_decision.Decisions.Production;
                    item_request_detail.item_request_decision.Add(item_request_decision);
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("balance");
            dbContext.SaveChanges();
            item_requestViewSource.View.MoveCurrentToLast();
            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            toolBar_btnEdit_Click(sender);
        }

        private void toolBar_btnCancel_Click(object sender)
        {
          //  dbContext.CancelAllChanges();
            if (item_requestDataGrid.SelectedItem != null)
            {
                item_request item_request = (item_request)item_requestDataGrid.SelectedItem;

                item_request.State = EntityState.Unchanged;
                dbContext.Entry(item_request).State = EntityState.Unchanged;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Selecteditem_movement = itemMovement.item_movement; 
            item_request_detail item_request_detail = (item_request_detail)item_requestitem_request_detailViewSource.View.CurrentItem;
            if (crud_modal.Visibility == Visibility.Hidden)
            {
              
                desion desion = (desion)item_request_decisionmovementDataGrid.SelectedItem;
                desion.decisionState = state.modified;
                item_request_decision item_request_decision = new global::entity.item_request_decision();
                item_request_decision.movement_id = (int)itemMovement.item_movement.id_movement;
                item_request_decision.IsSelected = true;
                item_request_decision.id_location = desion.id_location;
                item_request_decision.quantity = desion.decisionqty;
                item_request_decision.decision = global::entity.item_request_decision.Decisions.Movement;
                item_request_detail.item_request_decision.Add(item_request_decision);


                item_request_detail.item_request.GetTotalDecision();
                item_request_detail.RaisePropertyChanged("balance");
                dbContext.SaveChanges();
                item_requestViewSource.View.MoveCurrentToLast();
                item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
                item_request_detailitem_request_decisionViewSource.View.Refresh();
                toolBar_btnEdit_Click(sender);
            }
        }

        private void crud_modalTransfer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           // Selecteditem_movement = itemMovement.item_movement;
            item_request_detail item_request_detail = (item_request_detail)item_requestitem_request_detailViewSource.View.CurrentItem;
            if (crud_modal.Visibility == Visibility.Hidden)
            {

                desion desion = (desion)item_request_decisionmovementDataGrid.SelectedItem;
                desion.decisionState = state.modified;
                item_request_decision item_request_decision = new global::entity.item_request_decision();
                item_request_decision.movement_id = (int)itemMovement.item_movement.id_movement;
                item_request_decision.IsSelected = true;
                item_request_decision.id_location = desion.id_location;
                item_request_decision.quantity = desion.decisionqty;
                item_request_decision.decision = global::entity.item_request_decision.Decisions.Transfer;
                item_request_detail.item_request_decision.Add(item_request_decision);


                item_request_detail.item_request.GetTotalDecision();
                item_request_detail.RaisePropertyChanged("balance");
                dbContext.SaveChanges();
                item_requestViewSource.View.MoveCurrentToLast();
                item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
                item_request_detailitem_request_decisionViewSource.View.Refresh();
                toolBar_btnEdit_Click(sender);
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && item_requestViewSource != null)
            {
                try
                {
                    item_requestViewSource.View.Filter = i =>
                    {
                        item_request item_request = i as item_request;

                        if (item_request != null)
                        {
                            //Protect the code against null values.
                            //string number = item_request.number != null ? item_request.number : "";
                            string name = item_request.name != null ? item_request.name : "";

                            if ((name.ToLower().Contains(query.ToLower()))
                                )
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                catch //(Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                item_requestViewSource.View.Filter = null;
            }
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            item_request item_request = (item_request)item_requestDataGrid.SelectedItem;
            if (item_request != null)
            {
                app_document_range app_document_range;

                if (item_request.id_range != null)
                {
                    app_document_range = dbContext.app_document_range.Where(x => x.id_range == item_request.id_range).FirstOrDefault();
                }
                else
                {
                    app_document app_document = new entity.app_document();
                    app_document.id_application = entity.App.Names.RequestManagement;
                    app_document.name = "RequestManagement";

                    app_document_range = new app_document_range();
                    app_document_range.use_default_printer = false;
                    app_document_range.app_document = app_document;

                }

                entity.Brillo.Document.Start.Manual(item_request, app_document_range);
            }
            else
            {
                toolBar.msgWarning("Please select");
            }
        }

        private void item_request_decisioninternalDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;

            if (item_request_decisioninternalDataGrid.SelectedItem != null)
            {
                desion desion = (desion)item_request_decisioninternalDataGrid.SelectedItem;

                if (desion.decisionState == state.added)
                {
                    desion.decisionState = state.modified;
                    item_request_decision item_request_decision = new global::entity.item_request_decision();
                    item_request_decision.IsSelected = true;
                    item_request_decision.quantity = desion.decisionqty;
                    item_request_decision.decision = global::entity.item_request_decision.Decisions.Internal;
                    item_request_detail.item_request_decision.Add(item_request_decision);
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("balance");
            dbContext.SaveChanges();
            item_requestViewSource.View.MoveCurrentToLast();
            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            toolBar_btnEdit_Click(sender);
        }

        private void chbxRowDetail_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chbx = sender as CheckBox;
            if ((bool)chbx.IsChecked)
            {
                item_request_detailMovementDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            }
            else
            {
                item_request_detailMovementDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }
    }
}