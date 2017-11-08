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
    public enum State
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
        public State State { get; set; }
    }

    public partial class Request : Page
    {
        private CollectionViewSource item_requestViewSource;
        private CollectionViewSource item_requestitem_request_detailViewSource, item_request_detailitem_request_decisionViewSource;

        public entity.Controller.Product.RequestController RequestController;

        public Request()
        {
            InitializeComponent();

            RequestController = FindResource("RequestController") as entity.Controller.Product.RequestController;
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                //Load Controller.
                RequestController.Initialize();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource app_departmentViewSource = FindResource("app_departmentViewSource") as CollectionViewSource;
            CollectionViewSource itemsViewSource = FindResource("itemsViewSource") as CollectionViewSource;
            CollectionViewSource security_userViewSource = FindResource("security_userViewSource") as CollectionViewSource;
            item_requestViewSource = FindResource("item_requestViewSource") as CollectionViewSource;

            RequestController.Load(dataPager.PagedSource.PageIndex);

            item_requestViewSource.Source = RequestController.db.item_request.Local.Where(x => x.is_archived == false);
            app_departmentViewSource.Source = RequestController.db.app_department.Local;
            itemsViewSource.Source = RequestController.db.items.Local;
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(RequestController.db, entity.App.Names.RequestManagement, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
            security_userViewSource.Source = RequestController.db.security_user.Local;
            cmburgency.ItemsSource = Enum.GetValues(typeof(item_request_detail.Urgencies));

            item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detailitem_request_decisionViewSource = ((CollectionViewSource)(FindResource("item_request_detailitem_request_decisionViewSource")));

            CollectionViewSource app_branchViewSource = ((CollectionViewSource)(FindResource("app_branchViewSource")));
            app_branchViewSource.Source = CurrentSession.Branches;

            CollectionViewSource app_currencyViewSource = ((CollectionViewSource)(FindResource("app_currencyViewSource")));
            app_currencyViewSource.Source = CurrentSession.Currencies;

            cbxLocation.ItemsSource = CurrentSession.Locations.ToList();
        }

        private void Approve_Click(object sender)
        {
            if (RequestController.Approve())
            {
                toolBar.msgSaved(1);
            }
        }

        private void Delete_Click(object sender)
        {
            if (RequestController.Archive())
            {
                toolBar.msgSaved(1);
            }
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
                if (MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (item_requestitem_request_detailViewSource.View.CurrentItem is item_request_detail item_request_detail)
                    {
                        //DeleteDetailGridRow
                        item_request_detailDataGrid.CancelEdit();
                        RequestController.db.item_request_decision.Remove(e.Parameter as item_request_decision);
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

            if (item_requestitem_request_detailViewSource.View != null)
            {
                if (item_requestitem_request_detailViewSource.View.CurrentItem is item_request_detail item_request_detail)
                {
                    item _item = RequestController.db.items.Where(x => x.id_item == item_request_detail.id_item).FirstOrDefault();
                    var movements =
                            (from items in RequestController.db.item_movement
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
                        list_desion.Add(new Decision()
                        {
                            id_item = _item.id_item,
                            id_location = movement.id_location,
                            location = movement.Location,
                            name = _item.name,
                            avlqty = movement.Quantity,
                            Quantity = 0,
                            State = State.Added
                        });
                    }
                    item_request_decisionmovementDataGrid.ItemsSource = list_desion;

                    var transfer =
                    (from items in RequestController.db.item_movement
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
                        list_desion_transfer.Add(new Decision()
                        {
                            branch = item.branch,
                            id_location = item.id_location,
                            id_item = _item.id_item,
                            name = _item.name,
                            avlqty = item.quntitiy,
                            Quantity = 0,
                            State = State.Added
                        });
                    }

                    item_request_decisiontransferDataGrid.ItemsSource = list_desion_transfer;

                    List<Decision> list_desion_purchase = new List<Decision>();
                    list_desion_purchase.Add(new Decision()
                    {
                        State = State.Added,
                        Quantity = 0
                    });
                    item_request_decisionpurchaseDataGrid.ItemsSource = list_desion_purchase;

                    List<Decision> list_desion_internal = new List<Decision>();
                    Decision InternalDecision = new Decision()
                    {
                        State = State.Added,
                        Quantity = 0
                    };

                    list_desion_internal.Add(InternalDecision);
                    item_request_decisioninternalDataGrid.ItemsSource = list_desion_internal;

                    List<Decision> list_desion_production = new List<Decision>();
                    Decision ProductionDecision = new Decision()
                    {
                        name = item_request_detail.item.name,
                        Quantity = 0,
                        State = State.Added
                    };

                    ProductionDecision.RaisePropertyChanged("name");

                    list_desion_production.Add(ProductionDecision);
                    item_request_decisionproductionDataGrid.ItemsSource = list_desion_production;
                }
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

                if (desion.State == State.Added)
                {
                   
                    if (item_request_detail.item != null)
                    {
                        if (item_request_detail.item.item_dimension.Count() > 0)
                        {
                            crud_modal.Children.Clear();
                            Configs.itemMovement itemMovement = new Configs.itemMovement()
                            {
                                id_item = item_request_detail.id_item,
                                id_location = desion.id_location,
                                Quantity = desion.Quantity,
                                db = RequestController.db,
                                Decision = item_request_decision.Decisions.Movement
                            };

                            itemMovement.Save += pnlMovement_SaveChanges;

                            crud_modal.Visibility = Visibility.Visible;
                            crud_modal.Children.Add(itemMovement);
                        }
                        else
                        {
                            desion.State = State.Modified;

                            item_request_detail.item_request_decision.Add(new item_request_decision()
                            {
                                IsSelected = true,
                                id_location = desion.id_location,
                                quantity = desion.Quantity,
                                decision = item_request_decision.Decisions.Movement
                            });
                        }
                    }
                    
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("Balance");
            RequestController.db.SaveChanges();

            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            Edit_Click(sender);
        }

        private void New_Click(object sender)
        {
            item_request item_request = RequestController.Create();
            item_requestViewSource.View.MoveCurrentTo(item_request);
        }

        private void Edit_Click(object sender)
        {
            if (item_requestDataGrid.SelectedItem != null)
            {
                RequestController.Edit(item_requestDataGrid.SelectedItem as item_request);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void Save_Click(object sender)
        {
            if (RequestController.db.SaveChanges() > 0)
            {
                item_requestViewSource.View.Refresh();
                toolBar.msgSaved(1);
            }
        }

        private void TransferDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            if (item_requestitem_request_detailViewSource.View!=null)
            {

            }
        
            item_request_detail item_request_detail = (item_request_detail)item_requestitem_request_detailViewSource.View.CurrentItem;

            if (item_request_decisiontransferDataGrid.SelectedItem != null)
            {
                Decision desion = (Decision)item_request_decisiontransferDataGrid.SelectedItem;
                if (desion.avlqty < desion.Quantity)
                {
                    toolBar.msgWarning("Quantity is greater than Available");
                    return;
                }
                if (desion.State == State.Added)
                {
                    if (RequestController.db.items.Where(x => x.id_item == item_request_detail.id_item).FirstOrDefault().item_dimension.Count() > 0)
                    {
                        crud_modal.Children.Clear();
                        Configs.itemMovement itemMovement = new Configs.itemMovement()
                        {
                            id_item = item_request_detail.id_item,
                            id_location = desion.id_location,
                            Quantity = desion.Quantity,
                            db = RequestController.db,
                            Decision = item_request_decision.Decisions.Transfer
                        };

                        itemMovement.Save += pnlMovement_SaveChanges;

                        crud_modal.Visibility = Visibility.Visible;
                        crud_modal.Children.Add(itemMovement);
                    }
                    else
                    {
                        desion.State = State.Modified;
                        item_request_decision item_request_decision = new item_request_decision()
                        {
                            IsSelected = true,
                            id_location = desion.id_location,
                            quantity = desion.Quantity,
                            decision = entity.item_request_decision.Decisions.Transfer
                        };
                        item_request_detail.item_request_decision.Add(item_request_decision);
                    }
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("Balance");
            RequestController.db.SaveChangesAsync();

            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            Edit_Click(sender);
        }

        private void PurchaseDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;

            if (item_request_decisionpurchaseDataGrid.SelectedItem != null)
            {
                Decision desion = item_request_decisionpurchaseDataGrid.SelectedItem as Decision;

                if (desion.State == State.Added)
                {
                    desion.State = State.Modified;
                    item_request_decision item_request_decision = new item_request_decision()
                    {
                        IsSelected = true,
                        quantity = desion.Quantity
                    };

                    item_request_decision.decision = item_request_decision.Decisions.Purchase;
                    item_request_detail.item_request_decision.Add(item_request_decision);
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("Balance");
            RequestController.db.SaveChangesAsync();

            item_requestViewSource.View.MoveCurrentToLast();
            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            Edit_Click(sender);
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
                    project_task_dimensionViewSource.Source = RequestController.db.project_task_dimension.Where(x => x.id_project_task == item_request_detail.id_project_task).ToList();
                }
            }
        }

        private void ProductionDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = FindResource("item_requestitem_request_detailViewSource") as CollectionViewSource;
            item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;

            if (item_request_decisionproductionDataGrid.SelectedItem != null)
            {
                Decision desion = (Decision)item_request_decisionproductionDataGrid.SelectedItem;

                if (desion.State == State.Added)
                {
                    desion.State = State.Modified;
                    item_request_decision item_request_decision = new item_request_decision()
                    {
                        IsSelected = true,
                        quantity = desion.Quantity,
                        decision = entity.item_request_decision.Decisions.Production
                    };
                    item_request_detail.item_request_decision.Add(item_request_decision);
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("Balance");
            RequestController.db.SaveChanges();

            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            Edit_Click(sender);
        }

        private void Cancel_Click(object sender)
        {
            if (item_requestDataGrid.SelectedItem != null)
            {
                item_request item_request = (item_request)item_requestDataGrid.SelectedItem;

                item_request.State = EntityState.Unchanged;
                RequestController.db.Entry(item_request).State = EntityState.Unchanged;
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
                Decision.State = State.Modified;

                if (itemMovement.item_movement!=null)
                {
                    item_request_decision item_request_decision = new item_request_decision()
                    {
                        movement_id = (int)itemMovement.item_movement.id_movement,
                        IsSelected = true,
                        id_location = Decision.id_location,
                        quantity = Convert.ToDecimal(itemMovement.Quantity),
                        decision = itemMovement.Decision
                    };

                    item_request_detail.item_request_decision.Add(item_request_decision);
                    item_request_detail.item_request.GetTotalDecision();
                    item_request_detail.RaisePropertyChanged("Balance");

                    RequestController.db.SaveChangesAsync();
                }
              

                item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
                item_request_detailitem_request_decisionViewSource.View.Refresh();

                Edit_Click(null);
                GenerateaMovement_Click(null, null);
            }
        }

        private void Search_Click(object sender, string query)
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
                        }

                        return false;
                    };
                }
                catch { }
            }
            else
            {
                item_requestViewSource.View.Filter = null;
            }
        }

        private void Print_Click(object sender, MouseButtonEventArgs e)
        {
            item_request item_request = (item_request)item_requestDataGrid.SelectedItem;
            if (item_request != null)
            {
                app_document_range app_document_range;

                if (item_request.id_range != null)
                {
                    app_document_range = RequestController.db.app_document_range.Where(x => x.id_range == item_request.id_range).FirstOrDefault();
                }
                else
                {
                    app_document app_document = new app_document()
                    {
                        id_application = entity.App.Names.RequestManagement,
                        name = entity.Brillo.Localize.StringText("RequestManagement")
                    };

                    app_document_range = new app_document_range()
                    {
                        use_default_printer = false,
                        app_document = app_document
                    };
                }

                entity.Brillo.Document.Start.Manual(item_request, app_document_range);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void InternalDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CollectionViewSource item_requestitem_request_detailViewSource = ((CollectionViewSource)(FindResource("item_requestitem_request_detailViewSource")));
            item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;

            if (item_request_decisioninternalDataGrid.SelectedItem != null)
            {
                Decision desion = item_request_decisioninternalDataGrid.SelectedItem as Decision;
                if (desion.State == State.Added)
                {
                    desion.State = State.Modified;
                    item_request_decision item_request_decision = new item_request_decision()
                    {
                        IsSelected = true,
                        quantity = desion.Quantity,
                        decision = entity.item_request_decision.Decisions.Internal
                    };
                    item_request_detail.item_request_decision.Add(item_request_decision);
                }
            }

            item_request_detail.item_request.GetTotalDecision();
            item_request_detail.RaisePropertyChanged("Balance");
            RequestController.db.SaveChanges();

            item_requestViewSource.View.MoveCurrentTo(item_request_detail.item_request);
            item_request_detailitem_request_decisionViewSource.View.Refresh();
            Edit_Click(sender);
        }

        private void dataPager_OnDemandLoading(object sender, Syncfusion.UI.Xaml.Controls.DataPager.OnDemandLoadingEventArgs e)
        {
            RequestController.Load(dataPager.PagedSource.PageIndex);
        }

        private void toolBar_btnSearchInSource_Click(object sender, KeyEventArgs e, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                RequestController.Load(dataPager.PagedSource.PageIndex);
                //Brings data into view.
                Search_Click(sender, query);
            }
            else
            {
                item_requestViewSource = FindResource("item_requestViewSource") as CollectionViewSource;
                item_requestViewSource.Source = RequestController.db.item_request
                    .Where
                    ( x=>
                    x.name.ToLower().Contains(query.ToLower()) ||
                             x.number.ToLower().Contains(query.ToLower()) ||
                             x.project.name.ToLower().Contains(query.ToLower())
                    ).ToList();
            }
        }

     

        private void GenerateaMovement_Click(object sender, RoutedEventArgs e)
        {
            item_request_detailMovementDataGrid_SelectionChanged(sender, null);
        }

        private void RowDetail_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox chbx)
            {
                item_request_detailMovementDataGrid.RowDetailsVisibilityMode = (bool)chbx.IsChecked ? DataGridRowDetailsVisibilityMode.VisibleWhenSelected : DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }
    }
}