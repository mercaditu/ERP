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
        entity.item_requestDB dbContext = new entity.item_requestDB();
        CollectionViewSource item_requestViewSource;
        CollectionViewSource item_movementViewSource;
        CollectionViewSource item_requestitem_request_detailViewSource, item_request_detailitem_request_decisionViewSource;
        
        public Request()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_requestViewSource = ((CollectionViewSource)(FindResource("item_requestViewSource")));
            dbContext.item_request.Include("item_request_detail").Where(x => x.id_company == CurrentSession.Id_Company).ToList();
            item_requestViewSource.Source = dbContext.item_request.Local;
            
            item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detailitem_request_decisionViewSource = ((CollectionViewSource)(FindResource("item_request_detailitem_request_decisionViewSource")));
            
            CollectionViewSource app_branchViewSource = ((CollectionViewSource)(FindResource("app_branchViewSource")));
            dbContext.app_branch.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).ToList();
            app_branchViewSource.Source = dbContext.app_branch.Local;

            item_movementViewSource = ((CollectionViewSource)(FindResource("item_movementViewSource")));
            dbContext.item_movement.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
            item_movementViewSource.Source = dbContext.item_movement.Local;

            CollectionViewSource app_currencyViewSource = ((CollectionViewSource)(FindResource("app_currencyViewSource")));
            app_currencyViewSource.Source = dbContext.app_currency.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

            CollectionViewSource production_orderViewSource = ((CollectionViewSource)(FindResource("production_orderViewSource")));
            production_orderViewSource.Source = dbContext.production_order.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

            CollectionViewSource app_departmentViewSource = ((CollectionViewSource)(FindResource("app_departmentViewSource")));
            app_departmentViewSource.Source = dbContext.app_department.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
            cmburgency.ItemsSource = Enum.GetValues(typeof(entity.item_request_detail.Urgencies));
        }



        private void toolBar_btnApprove_Click(object sender)
        {
            dbContext.Approve();
        }

        private void toolBar_btnAnull_Click(object sender)
        {

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
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void item_request_detailMovementDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            entity.Properties.Settings setting = new entity.Properties.Settings();
           int id_branch=setting.branch_ID;
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = (item_request_detail)item_requestitem_request_detailViewSource.View.CurrentItem;
            if (item_request_detail != null)
            {


                item _item = dbContext.items.Where(x => x.id_item == item_request_detail.id_item).FirstOrDefault();
                var movement =
                        (from items in dbContext.item_movement
                         where items.status == Status.Stock.InStock
                         && items.item_product.id_item == item_request_detail.id_item
                         && items.app_location.id_branch==id_branch
                         group items by new { items.app_location }
                             into last
                             select new
                             {
                                 id_location = last.Key.app_location.id_location,
                                 location = last.Key.app_location.name,
                                 quntitiy = last.Sum(x => x.credit != null ? x.credit : 0) - last.Sum(x => x.debit != null ? x.debit : 0),
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
                      //(from items in dbContext.item_transfer_detail
                      // where items.item_product.id_item == item_request_detail.id_item
                      // group items by new { items.item_transfer.app_branch_destination }
                      //     into last
                      //     select new
                      //     {
                      //         id_location = last.Key.app_branch_destination.app_location.Where(x => x.is_default).FirstOrDefault().id_location,
                      //         branch = last.Key.app_branch_destination.name,
                      //         quntitiy = last.Sum(x => x.quantity_destination != null ? x.quantity_destination : 0),

                      //     }).ToList();

                (from items in dbContext.item_movement
                 where items.status == Status.Stock.InStock
                 && items.item_product.id_item == item_request_detail.id_item

                 group items by new { items.app_location.app_branch }
                     into last
                     select new
                     {
                         id_location = last.Key.app_branch.app_location.Where(x => x.is_default).FirstOrDefault().id_location,
                         branch = last.Key.app_branch.name,
                         quntitiy = last.Sum(x => x.credit != null ? x.credit : 0) - last.Sum(x => x.debit != null ? x.debit : 0),

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

                    desion.decisionState = state.modified;
                    item_request_decision item_request_decision = new global::entity.item_request_decision();
                    item_request_decision.IsSelected = true;
                    item_request_decision.id_location = desion.id_location;
                    item_request_decision.quantity = desion.decisionqty;
                    item_request_decision.decision = global::entity.item_request_decision.Decisions.Movement;
                    item_request_detail.item_request_decision.Add(item_request_decision);
                }
                else
                {

                }
            }

            item_request_detail.item_request.GetTotalDecision();

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

                    desion.decisionState = state.modified;
                    item_request_decision item_request_decision = new global::entity.item_request_decision();
                    item_request_decision.IsSelected = true;
                    item_request_decision.id_location = desion.id_location;
                    item_request_decision.quantity = desion.decisionqty;
                    item_request_decision.decision = global::entity.item_request_decision.Decisions.Transfer;
                    item_request_detail.item_request_decision.Add(item_request_decision);
                }
            }

            item_request_detail.item_request.GetTotalDecision();

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
            public int decisionqty { get; set; }
            public state decisionState { get; set; }
        }

        private void item_request_detailMovementDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = (item_request_detail)item_requestitem_request_detailViewSource.View.CurrentItem;
            CollectionViewSource project_task_dimensionViewSource = ((CollectionViewSource)(FindResource("project_task_dimensionViewSource")));
            if (item_request_detail!=null)
            {
                if (item_request_detail.id_project_task>0)
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

            dbContext.SaveChanges();
            item_requestViewSource.View.MoveCurrentToLast();
            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            toolBar_btnEdit_Click(sender);
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbContext.CancelAllChanges();
            //if (item_requestDataGrid.SelectedItem != null)
            //{
            //    item_request item_request = (item_request)item_requestDataGrid.SelectedItem;
            
            //    item_request.State = EntityState.Unchanged;
            //    dbContext.Entry(item_request).State = EntityState.Unchanged;
            //}
            //else
            //{
            //    toolBar.msgWarning("Please Select an Item");
            //}
        }
    }
}