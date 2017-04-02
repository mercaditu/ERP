using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Product
{
    public enum state
    {
        Added, Modified
    }

    public class Decision : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public int id_item { get; set; }
        public string name { get; set; }
        public int id_location { get; set; }
        public string location { get; set; }
        public string branch { get; set; }
        public decimal avlqty { get; set; }
        public decimal Quantity { get; set; }
        public state State { get; set; }
    }

    public partial class Request : Page
    {
        private item_requestDB RequestDB = new item_requestDB();
        private CollectionViewSource item_requestViewSource;
        private CollectionViewSource item_requestitem_request_detailViewSource, item_request_detailitem_request_decisionViewSource;

        public Request()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource app_departmentViewSource = ((CollectionViewSource)(FindResource("app_departmentViewSource")));
            app_departmentViewSource.Source = await RequestDB.app_department.Where(x => x.id_company == CurrentSession.Id_Company).ToListAsync();
            cmburgency.ItemsSource = Enum.GetValues(typeof(item_request_detail.Urgencies));

            Load();

            item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detailitem_request_decisionViewSource = ((CollectionViewSource)(FindResource("item_request_detailitem_request_decisionViewSource")));

            CollectionViewSource app_branchViewSource = ((CollectionViewSource)(FindResource("app_branchViewSource")));
            app_branchViewSource.Source = CurrentSession.Branches;

            CollectionViewSource app_currencyViewSource = ((CollectionViewSource)(FindResource("app_currencyViewSource")));
            app_currencyViewSource.Source = CurrentSession.Currencies;

            CollectionViewSource security_userViewSource = ((CollectionViewSource)(FindResource("security_userViewSource")));
            await RequestDB.security_user.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).ToListAsync();
            security_userViewSource.Source = RequestDB.security_user.Local;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(RequestDB, entity.App.Names.RequestManagement, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
        }

        private async void Load()
        {
            await RequestDB.item_request
                .Where(x => x.id_company == CurrentSession.Id_Company && x.is_archived == false)
                .Include(x => x.production_order)
                .Include(x => x.sales_order)
                .Include(x => x.project)
                .Include(x => x.security_user)
                .Include(x => x.item_request_detail)
                .OrderByDescending(x => x.request_date)
                .LoadAsync();

            item_requestViewSource = FindResource("item_requestViewSource") as CollectionViewSource;
            item_requestViewSource.Source = RequestDB.item_request.Local.Where(x => x.is_archived == false);
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            RequestDB.Approve();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            int i = 0;

            foreach (item_request item_request in RequestDB.item_request.Local.Where(x => x.IsSelected))
            {
                item_request.is_archived = true;
                i += 1;
            }

            toolBar.msgSaved(i);
            RequestDB.SaveChangesAsync();
            Load();
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
                MessageBoxResult result = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;

                    if (item_request_detail != null)
                    {
                        //DeleteDetailGridRow
                        item_request_detailDataGrid.CancelEdit();
                        RequestDB.item_request_decision.Remove(e.Parameter as item_request_decision);
                        item_requestitem_request_detailViewSource.View.Refresh();
                        item_request_detailitem_request_decisionViewSource.View.Refresh();

                        item_request_detail.item_request.GetTotalDecision();
                        item_request_detail.RaisePropertyChanged("Balance");
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void item_request_detailMovementDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = FindResource("item_requestitem_request_detailViewSource") as CollectionViewSource;
            item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;

            if (item_request_detail != null)
            {
                item _item = RequestDB.items.Where(x => x.id_item == item_request_detail.id_item).FirstOrDefault();
                var movements =
                        (from items in RequestDB.item_movement
                         where items.status == Status.Stock.InStock
                         && items.item_product.id_item == item_request_detail.id_item
                         && items.app_location.id_branch == CurrentSession.Id_Branch
                         group items by new { items.app_location }
                             into last
                         select new
                         {
                             id_location = last.Key.app_location.id_location,
                             Location = last.Key.app_location.name,
                             Quantity = last.Sum(x => x.credit) - last.Sum(x => x.debit),
                         }).ToList();

                List<Decision> list_desion = new List<Decision>();

                foreach (dynamic movement in movements)
                {
                    Decision desion = new Decision();
                    desion.id_item = _item.id_item;
                    desion.id_location = movement.id_location;
                    desion.location = movement.Location;
                    desion.name = _item.name;
                    desion.avlqty = movement.Quantity;
                    desion.Quantity = 0;
                    desion.State = state.Added;
                    list_desion.Add(desion);
                }
                item_request_decisionmovementDataGrid.ItemsSource = list_desion;

                var transfer =
                (from items in RequestDB.item_movement
                 where items.status == Status.Stock.InStock
                 && items.item_product.id_item == item_request_detail.id_item

                 group items by new { items.app_location.app_branch }
                     into last
                 select new
                 {
                     id_location = last.Key.app_branch.app_location.Where(x => x.is_default).Select(x => x.id_location).FirstOrDefault(),
                     branch = last.Key.app_branch.name,
                     quntitiy = last.Sum(x => x.credit) - last.Sum(x => x.debit),
                 }).ToList();

                List<Decision> list_desion_transfer = new List<Decision>();
                foreach (dynamic item in transfer)
                {
                    Decision desion = new Decision();
                    desion.branch = item.branch;
                    desion.id_location = item.id_location;
                    desion.id_item = _item.id_item;
                    desion.name = _item.name;
                    desion.avlqty = item.quntitiy;
                    desion.Quantity = 0;
                    desion.State = state.Added;
                    list_desion_transfer.Add(desion);
                }

                item_request_decisiontransferDataGrid.ItemsSource = list_desion_transfer;

                List<Decision> list_desion_purchase = new List<Decision>();
                Decision PurchaseDecision = new Decision();
                PurchaseDecision.State = state.Added;
                PurchaseDecision.Quantity = 0;
                list_desion_purchase.Add(PurchaseDecision);
                item_request_decisionpurchaseDataGrid.ItemsSource = list_desion_purchase;

                List<Decision> list_desion_internal = new List<Decision>();
                Decision InternalDecision = new Decision();
                InternalDecision.State = state.Added;
                InternalDecision.Quantity = 0;
                list_desion_internal.Add(InternalDecision);
                item_request_decisioninternalDataGrid.ItemsSource = list_desion_internal;

                List<Decision> list_desion_production = new List<Decision>();
                Decision ProductionDecision = new Decision();
                ProductionDecision.name = item_request_detail.item.name;
                ProductionDecision.RaisePropertyChanged("name");
                ProductionDecision.Quantity = 0;
                ProductionDecision.State = state.Added;
                list_desion_production.Add(ProductionDecision);
                item_request_decisionproductionDataGrid.ItemsSource = list_desion_production;
            }
        }

        private void item_request_decisionDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = (item_request_detail)item_requestitem_request_detailViewSource.View.CurrentItem;
            if (item_request_decisionmovementDataGrid.SelectedItem != null)
            {
                Decision desion = (Decision)item_request_decisionmovementDataGrid.SelectedItem;
                if (desion.avlqty < desion.Quantity)
                {
                    toolBar.msgWarning("Quantity is greater than Available");
                    return;
                }

                if (desion.State == state.Added)
                {
                    if (RequestDB.items.Where(x => x.id_item == item_request_detail.id_item).FirstOrDefault().item_dimension.Count() > 0)
                    {
                        crud_modal.Children.Clear();
                        Configs.itemMovement itemMovement = new Configs.itemMovement();
                        itemMovement.id_item = item_request_detail.id_item;
                        itemMovement.id_location = desion.id_location;
                        itemMovement.db = RequestDB;
                        itemMovement.Decision = item_request_decision.Decisions.Movement;
                        itemMovement.Save += pnlMovement_SaveChanges;

                        crud_modal.Visibility = Visibility.Visible;
                        crud_modal.Children.Add(itemMovement);
                    }
                    else
                    {
                        desion.State = state.Modified;
                        item_request_decision item_request_decision = new item_request_decision();
                        item_request_decision.IsSelected = true;
                        item_request_decision.id_location = desion.id_location;
                        item_request_decision.quantity = desion.Quantity;
                        item_request_decision.decision = item_request_decision.Decisions.Movement;
                        item_request_detail.item_request_decision.Add(item_request_decision);
                    }
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("Balance");
            RequestDB.SaveChanges();
            item_requestViewSource.View.MoveCurrentToLast();
            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            toolBar_btnEdit_Click(sender);
        }

        private void toolBar_btnNew_Click(object sender)
        {
            item_request item_request = new item_request();

            item_request.IsSelected = true;

            app_document_range app_document_range = entity.Brillo.Logic.Range.List_Range(RequestDB, entity.App.Names.RequestManagement, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
            if (app_document_range != null)
            {
                //Gets List of Ranges avaiable for this Document.
                item_request.id_range = app_document_range.id_range;
            }

            RequestDB.Entry(item_request).State = EntityState.Added;
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
                RequestDB.Entry(item_request_old).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (RequestDB.SaveChanges() > 0)
            {
                item_requestViewSource.View.Refresh();
                toolBar.msgSaved(RequestDB.NumberOfRecords);
            }
        }

        private void item_request_decisiontransferDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = (item_request_detail)item_requestitem_request_detailViewSource.View.CurrentItem;

            if (item_request_decisiontransferDataGrid.SelectedItem != null)
            {
                Decision desion = (Decision)item_request_decisiontransferDataGrid.SelectedItem;
                if (desion.avlqty < desion.Quantity)
                {
                    toolBar.msgWarning("Quantity is greater than Available");
                    return;
                }
                if (desion.State == state.Added)
                {
                    if (RequestDB.items.Where(x => x.id_item == item_request_detail.id_item).FirstOrDefault().item_dimension.Count() > 0)
                    {
                        crud_modal.Children.Clear();
                        Configs.itemMovement itemMovement = new Configs.itemMovement();
                        itemMovement.id_item = item_request_detail.id_item;
                        itemMovement.id_location = desion.id_location;
                        itemMovement.db = RequestDB;
                        itemMovement.Decision = item_request_decision.Decisions.Transfer;
                        itemMovement.Save += pnlMovement_SaveChanges;

                        crud_modal.Visibility = Visibility.Visible;
                        crud_modal.Children.Add(itemMovement);
                    }
                    else
                    {
                        desion.State = state.Modified;
                        item_request_decision item_request_decision = new item_request_decision();
                        item_request_decision.IsSelected = true;
                        item_request_decision.id_location = desion.id_location;
                        item_request_decision.quantity = desion.Quantity;
                        item_request_decision.decision = entity.item_request_decision.Decisions.Transfer;
                        item_request_detail.item_request_decision.Add(item_request_decision);
                    }
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("Balance");
            RequestDB.SaveChangesAsync();

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
                Decision desion = item_request_decisionpurchaseDataGrid.SelectedItem as Decision;

                if (desion.State == state.Added)
                {
                    desion.State = state.Modified;
                    item_request_decision item_request_decision = new item_request_decision();
                    item_request_decision.IsSelected = true;
                    item_request_decision.quantity = desion.Quantity;
                    item_request_decision.decision = item_request_decision.Decisions.Purchase;
                    item_request_detail.item_request_decision.Add(item_request_decision);
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("Balance");
            RequestDB.SaveChangesAsync();

            item_requestViewSource.View.MoveCurrentToLast();
            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            toolBar_btnEdit_Click(sender);
        }

        private void item_request_detailMovementDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = FindResource("item_requestitem_request_detailViewSource") as CollectionViewSource;
            item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;
            CollectionViewSource project_task_dimensionViewSource = FindResource("project_task_dimensionViewSource") as CollectionViewSource;
            if (item_request_detail != null)
            {
                if (item_request_detail.id_project_task > 0)
                {
                    project_task_dimensionViewSource.Source = RequestDB.project_task_dimension.Where(x => x.id_project_task == item_request_detail.id_project_task).ToList();
                }
            }
        }

        private void item_request_decisionproductionDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = FindResource("item_requestitem_request_detailViewSource") as CollectionViewSource;
            item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;

            if (item_request_decisionproductionDataGrid.SelectedItem != null)
            {
                Decision desion = (Decision)item_request_decisionproductionDataGrid.SelectedItem;

                if (desion.State == state.Added)
                {
                    desion.State = state.Modified;
                    item_request_decision item_request_decision = new item_request_decision();
                    item_request_decision.IsSelected = true;
                    item_request_decision.quantity = desion.Quantity;
                    item_request_decision.decision = entity.item_request_decision.Decisions.Production;
                    item_request_detail.item_request_decision.Add(item_request_decision);
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("Balance");
            RequestDB.SaveChanges();
            item_requestViewSource.View.MoveCurrentToLast();
            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            toolBar_btnEdit_Click(sender);
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            if (item_requestDataGrid.SelectedItem != null)
            {
                item_request item_request = (item_request)item_requestDataGrid.SelectedItem;

                item_request.State = EntityState.Unchanged;
                RequestDB.Entry(item_request).State = EntityState.Unchanged;
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void pnlMovement_SaveChanges(object sender, RoutedEventArgs e)
        {
            item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;
            Configs.itemMovement itemMovement = sender as Configs.itemMovement;

            if (item_request_detail != null && itemMovement != null)
            {
                Decision Decision = item_request_decisionmovementDataGrid.SelectedItem as Decision;
                Decision.State = state.Modified;

                item_request_decision item_request_decision = new item_request_decision();
                item_request_decision.movement_id = (int)itemMovement.item_movement.id_movement;
                item_request_decision.IsSelected = true;
                item_request_decision.id_location = Decision.id_location;
                item_request_decision.quantity = Decision.Quantity;
                item_request_decision.decision = itemMovement.Decision;
                item_request_detail.item_request_decision.Add(item_request_decision);

                item_request_detail.item_request.GetTotalDecision();
                item_request_detail.RaisePropertyChanged("Balance");

                RequestDB.SaveChangesAsync();

                item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
                item_request_detailitem_request_decisionViewSource.View.Refresh();

                toolBar_btnEdit_Click(null);
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
                            string name = item_request.name != null ? item_request.name : "";
                            string number = item_request.number != null ? item_request.number : "";
                            string project = item_request.project != null ? item_request.project.name : "";
                            if (
                            (
                             name.ToLower().Contains(query.ToLower())) ||
                             number.ToLower().Contains(query.ToLower()) ||
                             project.ToLower().Contains(query.ToLower())
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
                catch { }
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
                    app_document_range = RequestDB.app_document_range.Where(x => x.id_range == item_request.id_range).FirstOrDefault();
                }
                else
                {
                    app_document app_document = new entity.app_document();
                    app_document.id_application = entity.App.Names.RequestManagement;
                    app_document.name = entity.Brillo.Localize.StringText("RequestManagement");

                    app_document_range = new app_document_range();
                    app_document_range.use_default_printer = false;
                    app_document_range.app_document = app_document;
                }

                entity.Brillo.Document.Start.Manual(item_request, app_document_range);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void item_request_decisioninternalDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;

            if (item_request_decisioninternalDataGrid.SelectedItem != null)
            {
                Decision desion = (Decision)item_request_decisioninternalDataGrid.SelectedItem;

                if (desion.State == state.Added)
                {
                    desion.State = state.Modified;
                    item_request_decision item_request_decision = new global::entity.item_request_decision();
                    item_request_decision.IsSelected = true;
                    item_request_decision.quantity = desion.Quantity;
                    item_request_decision.decision = global::entity.item_request_decision.Decisions.Internal;
                    item_request_detail.item_request_decision.Add(item_request_decision);
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("Balance");
            RequestDB.SaveChanges();
            item_requestViewSource.View.MoveCurrentToLast();
            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            toolBar_btnEdit_Click(sender);
        }

        private void chbxRowDetail_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chbx = sender as CheckBox;
            if (chbx != null)
            {
                item_request_detailMovementDataGrid.RowDetailsVisibilityMode = (bool)chbx.IsChecked ? DataGridRowDetailsVisibilityMode.VisibleWhenSelected : DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }
    }
}