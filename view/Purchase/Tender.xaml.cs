using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Purchase
{
    public partial class Tender : Page
    {
        //private PurchaseTenderDB PurchaseTenderDB = new PurchaseTenderDB();

        private entity.Controller.Purchase.TenderController TenderDB;

        private CollectionViewSource purchase_tenderpurchase_tender_item_detailViewSource, purchase_tenderViewSource,
            purchase_tenderpurchase_tender_itemViewSource,
            purchase_tenderpurchase_tender_contact_detailViewSource,
            app_contractViewSource;

        private CollectionViewSource app_measurementViewSource, app_dimensionViewSource;

        public Tender()
        {
            InitializeComponent();

            TenderDB = FindResource("TenderDB") as entity.Controller.Purchase.TenderController;
            TenderDB.Initialize();
        }

        private void Approve_Click(object sender)
        {
            purchase_tenderViewSource.View.Refresh();
        }

        private void Cancel_Click(object sender)
        {
            TenderDB.CancelAllChanges();
        }

        private void Anull_Click(object sender)
        {
            TenderDB.Annull();
        }

        private void New_Click(object sender)
        {
            purchase_tender purchase_tender = TenderDB.Create(new TenderSetting().TransDate_OffSet);
            purchase_tenderViewSource.View.MoveCurrentTo(purchase_tender);
        }

        private void Edit_Click(object sender)
        {
            if (purchase_tenderDataGrid.SelectedItem != null)
            {
                TenderDB.Edit(purchase_tenderDataGrid.SelectedItem as purchase_tender);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void Delete_Click(object sender)
        {
            if (MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                TenderDB.Archive();
                //purchase_tender purchase_tender = purchase_tenderDataGrid.SelectedItem as purchase_tender;
                //purchase_tender.is_head = false;
                //purchase_tender.State = EntityState.Deleted;
                //purchase_tender.IsSelected = true;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            purchase_tenderViewSource = FindResource("purchase_tenderViewSource") as CollectionViewSource;
            purchase_tenderViewSource.Source = TenderDB.db.purchase_tender.Local;

            purchase_tenderpurchase_tender_contact_detailViewSource = FindResource("purchase_tenderpurchase_tender_contact_detailViewSource") as CollectionViewSource;
            purchase_tenderpurchase_tender_itemViewSource = FindResource("purchase_tenderpurchase_tender_itemViewSource") as CollectionViewSource;
            purchase_tenderpurchase_tender_item_detailViewSource = FindResource("purchase_tenderpurchase_tender_item_detailViewSource") as CollectionViewSource;

            cbxBranch.ItemsSource = CurrentSession.Branches; //PurchaseTenderDB.app_branch.Local;

            cbxDepartment.ItemsSource = TenderDB.db.app_department.Local;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(TenderDB.db, entity.App.Names.PurchaseTender, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);

            CollectionViewSource app_conditionViewSource = FindResource("app_conditionViewSource") as CollectionViewSource;
            app_conditionViewSource.Source = CurrentSession.Conditions;

            app_contractViewSource = FindResource("app_contractViewSource") as CollectionViewSource;
            app_contractViewSource.Source = CurrentSession.Contracts;

            CollectionViewSource app_currencyfxViewSource = FindResource("app_currencyfxViewSource") as CollectionViewSource;
            app_currencyfxViewSource.Source = TenderDB.db.app_currencyfx.Local;

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = CurrentSession.VAT_Groups;

            app_dimensionViewSource = FindResource("app_dimensionViewSource") as CollectionViewSource;
            app_dimensionViewSource.Source = TenderDB.db.app_dimension.Local;

            app_measurementViewSource = FindResource("app_measurementViewSource") as CollectionViewSource;
            app_measurementViewSource.Source = TenderDB.db.app_measurement.Local;
        }

        public void Item_Select(object sender, EventArgs e)
        {
            if (purchase_tenderViewSource.View.CurrentItem is purchase_tender purchase_tender)
            {
                item item = TenderDB.db.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                if (item != null)
                {
                    //Checks if product exists.
                    if (purchase_tender.purchase_tender_item_detail.Where(x => x.id_item == item.id_item).Count() == 0)
                    {
                        purchase_tender_item purchase_tender_item = new purchase_tender_item();
                        purchase_tender_item.item = item;
                        purchase_tender_item.id_item = item.id_item;
                        purchase_tender_item.item_description = item.name;
                        purchase_tender_item.quantity = sbxItem.Quantity;

                        foreach (item_dimension item_dimension in item.item_dimension)
                        {
                            purchase_tender_dimension purchase_tender_dimension = new purchase_tender_dimension()
                            {
                                purchase_tender_item = purchase_tender_item,
                                id_dimension = item_dimension.id_app_dimension,
                                id_measurement = item_dimension.id_measurement,
                                app_measurement = item_dimension.app_measurement,
                                value = item_dimension.value
                            };

                            if (TenderDB.db.app_dimension.Where(x => x.id_dimension == item_dimension.id_app_dimension).FirstOrDefault() != null)
                            {
                                purchase_tender_dimension.app_dimension = TenderDB.db.app_dimension.Where(x => x.id_dimension == item_dimension.id_app_dimension).FirstOrDefault();
                            }
                            
                            purchase_tender_item.purchase_tender_dimension.Add(purchase_tender_dimension);
                        }

                        purchase_tender.purchase_tender_item_detail.Add(purchase_tender_item);
                    }
                    else
                    {
                        toolBar.msgWarning("Product Exists");
                    }
                }
                else
                {
                    if (sbxItem.Text != string.Empty)
                    {
                        purchase_tender_item purchase_tender_item = new purchase_tender_item()
                        {
                            item_description = sbxItem.Text,
                            quantity = sbxItem.Quantity
                        };
                        purchase_tender.purchase_tender_item_detail.Add(purchase_tender_item);
                    }
                }

                purchase_tenderViewSource.View.Refresh();
                purchase_tenderpurchase_tender_itemViewSource.View.Refresh();
            }
        }

        private void set_ContactPrefKeyStroke(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                set_ContactPref(sender, e);
            }
        }

        private void set_ContactPref(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                //Get Contact from SmartBox.
                contact contact = TenderDB.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

                if (contact == null)
                {
                    toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
                    return;
                }

                //This is required to generate the ID needed for the Item-Detail relationship.
                TenderDB.SaveChanges_Validate();

                if (purchase_tenderViewSource.View != null)
                {
                    purchase_tender purchase_tender = purchase_tenderViewSource.View.CurrentItem as purchase_tender;
                    purchase_tender_contact purchase_tender_contact = new purchase_tender_contact();

                    if (cbxContract.SelectedItem != null)
                    {
                        purchase_tender_contact.id_contract = (cbxContract.SelectedItem as app_contract).id_contract;
                        purchase_tender_contact.id_condition = (cbxCondition.SelectedItem as app_condition).id_condition;

                        purchase_tender_contact.app_contract = TenderDB.db.app_contract.Find(purchase_tender_contact.id_contract);
                        purchase_tender_contact.app_condition = TenderDB.db.app_condition.Find(purchase_tender_contact.id_condition);
                    }
                    else if (contact.app_contract != null)
                    {
                        purchase_tender_contact.id_contract = (int)contact.id_contract;
                        purchase_tender_contact.id_condition = contact.app_contract.id_condition;

                        purchase_tender_contact.app_contract = TenderDB.db.app_contract.Find(purchase_tender_contact.id_contract);
                        purchase_tender_contact.app_condition = TenderDB.db.app_condition.Find(purchase_tender_contact.id_condition);
                    }
                    else
                    {
                        toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
                        return;
                    }

                    purchase_tender_contact.contact = contact;
                    purchase_tender_contact.id_contact = contact.id_contact;

                    if (purchase_tender_contact.contact.id_currency == 0 || purchase_tender_contact.contact.id_currency == null)
                    {
                        //Contact does not have Currency, take default currency from Company.
                        if (CurrentSession.Get_Currency_Default_Rate() != null)
                        {
                            purchase_tender_contact.id_currencyfx = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;
                        }
                    }
                    else
                    {
                        //Contact has Currency, take FX Rate of Currency.
                        app_currencyfx app_currencyfx = TenderDB.db.app_currencyfx.Where(x => x.app_currency.id_currency == purchase_tender_contact.contact.id_currency && x.is_active).FirstOrDefault();
                        if (app_currencyfx != null)
                        {
                            purchase_tender_contact.id_currencyfx = app_currencyfx.id_currencyfx;
                        }
                    }

                    if (contact.lead_time != null)
                    {
                        purchase_tender_contact.recieve_date_est = DateTime.Now.AddDays((double)contact.lead_time);
                    }

                    if (purchase_tender != null)
                    {
                        List<purchase_tender_item> listtender = purchase_tender.purchase_tender_item_detail.ToList();
                        foreach (purchase_tender_item purchase_tender_item in listtender)
                        {
                            if (purchase_tender_contact.id_purchase_tender_contact == 0)
                            {
                                if (purchase_tender_contact.purchase_tender_detail.Where(x => x.purchase_tender_item.id_item == purchase_tender_item.id_item).Count() == 0)
                                {
                                    purchase_tender_detail purchase_tender_detail = new purchase_tender_detail()
                                    {
                                        id_purchase_tender_item = purchase_tender_item.id_purchase_tender_item,
                                        purchase_tender_item = purchase_tender_item,
                                        quantity = purchase_tender_item.quantity,
                                        unit_cost = 0,
                                        id_vat_group = TenderDB.db.app_vat_group.Where(x => x.is_default).FirstOrDefault().id_vat_group
                                    };

                                    foreach (purchase_tender_dimension purchase_tender_dimension in purchase_tender_item.purchase_tender_dimension)
                                    {
                                        purchase_tender_detail_dimension purchase_tender_detail_dimension = new purchase_tender_detail_dimension()
                                        {
                                            purchase_tender_detail = purchase_tender_detail,
                                            id_dimension = purchase_tender_dimension.id_dimension,
                                            app_dimension = purchase_tender_dimension.app_dimension,
                                            id_measurement = purchase_tender_dimension.id_measurement,
                                            app_measurement = purchase_tender_dimension.app_measurement,
                                            value = purchase_tender_dimension.value
                                        };

                                        purchase_tender_detail.purchase_tender_detail_dimension.Add(purchase_tender_detail_dimension);
                                    }

                                    //purchase_tender_item.purchase_tender_detail.Add(purchase_tender_detail);
                                    purchase_tender_contact.purchase_tender_detail.Add(purchase_tender_detail);
                                }
                                else
                                {
                                    purchase_tender_detail purchase_tender_detail = purchase_tender_contact.purchase_tender_detail.Where(x => x.purchase_tender_item.id_item == purchase_tender_item.id_item).FirstOrDefault();
                                    purchase_tender_detail.quantity += purchase_tender_item.quantity;
                                }
                            }
                            else
                            {
                                if (TenderDB.db.purchase_tender_detail.Where(x => x.id_purchase_tender_contact == purchase_tender_contact.id_purchase_tender_contact && x.id_purchase_tender_item == purchase_tender_item.id_purchase_tender_item) == null)
                                {
                                    purchase_tender_detail purchase_tender_detail = new purchase_tender_detail();

                                    purchase_tender_detail.id_purchase_tender_item = purchase_tender_item.id_purchase_tender_item;
                                    purchase_tender_detail.purchase_tender_item = purchase_tender_item;
                                    purchase_tender_detail.quantity = 1;
                                    purchase_tender_detail.id_vat_group = TenderDB.db.app_vat_group.Where(x => x.is_default).FirstOrDefault().id_vat_group;

                                    purchase_tender_detail.unit_cost = 0;
                                    purchase_tender_contact.purchase_tender_detail.Add(purchase_tender_detail);
                                }
                                else
                                {
                                    purchase_tender_detail purchase_tender_detail = TenderDB.db.purchase_tender_detail.Where(x => x.id_purchase_tender_contact == purchase_tender_contact.id_purchase_tender_contact && x.id_purchase_tender_item == purchase_tender_item.id_purchase_tender_item).FirstOrDefault();
                                    purchase_tender_detail.quantity += purchase_tender_detail.quantity;
                                }
                            }
                        }
                    }

                    purchase_tender.purchase_tender_contact_detail.Add(purchase_tender_contact);

                    purchase_tenderpurchase_tender_contact_detailViewSource.View.Refresh();

                    purchase_tenderpurchase_tender_contact_detailViewSource.View.MoveCurrentTo(purchase_tender_contact);
                }
            }
        }

        private void Save_Click(object sender)
        {
            if (TenderDB.SaveChanges_Validate() > 0)
            {
                purchase_tenderViewSource.View.Refresh();
                toolBar.msgSaved(TenderDB.NumberOfRecords);
            }
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DataGrid dg = (DataGrid)e.Source;
            if (dg.Name == "purchase_tender_contact_detailDataGrid")
            {
                if (e.Parameter as purchase_tender_contact != null)
                {
                    e.CanExecute = true;
                }
            }
            else if (dg.Name == "purchase_tender_itemDataGrid")
            {
                if (e.Parameter as purchase_tender_item != null)
                {
                    e.CanExecute = true;
                }
            }
            else
            {
                if (e.Parameter as purchase_tender_detail != null)
                {
                    e.CanExecute = true;
                }
            }
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                DataGrid dg = (DataGrid)e.Source;
                if (MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (dg.Name == "purchase_tender_contact_detailDataGrid")
                    {
                        purchase_tender_contact_detailDataGrid.CancelEdit();
                        purchase_tender_contact purchase_tender_contact = e.Parameter as entity.purchase_tender_contact;

                        if (purchase_tender_contact != null)
                        {
                            TenderDB.db.purchase_tender_contact_detail.Remove(purchase_tender_contact);
                            purchase_tenderpurchase_tender_contact_detailViewSource.View.Refresh();
                        }
                        else
                        {
                            toolBar.msgWarning("Please Contact Admin.");
                        }
                    }
                    else if (dg.Name == "purchase_tender_itemDataGrid")
                    {
                        purchase_tender_itemDataGrid.CancelEdit();
                        TenderDB.db.purchase_tender_item_detail.Remove(e.Parameter as purchase_tender_item);
                        purchase_tenderpurchase_tender_itemViewSource.View.Refresh();
                    }
                    else
                    {
                        //DeleteDetailGridRow
                        purchase_tender_item_detailDataGrid.CancelEdit();
                        TenderDB.db.purchase_tender_detail.Remove(e.Parameter as purchase_tender_detail);
                        purchase_tenderpurchase_tender_item_detailViewSource.View.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void purchase_tender_itemDataGrid_LoadingRowDetails(object sender, EventArgs e)
        {
            if (purchase_tenderpurchase_tender_itemViewSource != null)
            {
                if (purchase_tenderpurchase_tender_itemViewSource.View != null)
                {
                    purchase_tender_item purchase_tender_item = (purchase_tender_item)purchase_tenderpurchase_tender_itemViewSource.View.CurrentItem;
                    CollectionViewSource purchase_tender_dimensionViewSource = ((CollectionViewSource)(FindResource("purchase_tender_dimensionViewSource")));
                    if (purchase_tender_item != null)
                    {
                        if (purchase_tender_item.id_item > 0)
                        {
                            purchase_tender_dimensionViewSource.Source = purchase_tender_item.purchase_tender_dimension.ToList();
                        }
                    }
                }
            }
        }

        private void purchase_tender_contact_detailDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (purchase_tenderpurchase_tender_item_detailViewSource != null)
            {
                if (purchase_tenderpurchase_tender_item_detailViewSource.View != null)
                {
                    List<purchase_tender_detail> purchase_tender_detailList = purchase_tenderpurchase_tender_item_detailViewSource.View.OfType<purchase_tender_detail>().ToList();
                    LblTotal.Content = purchase_tender_detailList.Sum(x => x.quantity * x.UnitCost_Vat);
                }
            }
        }

        private void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (app_contractViewSource != null)
            {
                if (app_contractViewSource.View != null)
                {
                    if (app_contractViewSource.View.Cast<app_contract>().Count() > 0)
                    {
                        app_contractViewSource.View.Filter = i =>
                        {
                            app_contract app_contract = (app_contract)i;
                            int app_condition = (int)cbxCondition.SelectedValue;
                            if (app_contract.id_condition == app_condition)
                                return true;
                            else
                                return false;
                        };
                    }
                }
            }
        }

        private void purchase_tender_item_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            List<purchase_tender_detail> purchase_tender_detailList = purchase_tenderpurchase_tender_item_detailViewSource.View.OfType<purchase_tender_detail>().ToList();
            LblTotal.Content = purchase_tender_detailList.Sum(x => x.quantity * x.UnitCost_Vat);
        }

        private void EditCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as purchase_tender_contact != null)
            {
                e.CanExecute = true;
            }
        }

        private void EditCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            purchase_tender_contact purchase_tender_contact = (purchase_tender_contact)e.Parameter;

            entity.Brillo.Document.Start.Automatic(e.Parameter as purchase_tender_contact, purchase_tender_contact.purchase_tender.app_document_range);
        }

        private void purchase_tender_item_detailDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            if (purchase_tenderpurchase_tender_item_detailViewSource != null)
            {
                if (purchase_tenderpurchase_tender_item_detailViewSource.View != null)
                {
                    purchase_tender_item purchase_tender_item = (purchase_tender_item)purchase_tenderpurchase_tender_itemViewSource.View.CurrentItem;
                    CollectionViewSource purchase_tender_dimensionViewSource = ((CollectionViewSource)(FindResource("purchase_tender_dimensionViewSource")));
                    if (purchase_tender_item != null)
                    {
                        if (purchase_tender_item.id_purchase_tender_item > 0)
                        {
                            purchase_tender_dimensionViewSource.Source = TenderDB.db.purchase_tender_dimension.Where(x => x.id_purchase_tender_item == purchase_tender_item.id_purchase_tender_item).ToList();
                        }
                    }
                }
            }
        }

        private void Search_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && purchase_tenderViewSource != null)
            {
                purchase_tenderViewSource.View.Filter = i =>
                {
                    purchase_tender purchase_tender = i as purchase_tender;
                    string number = purchase_tender.number != null ? purchase_tender.number : "";
                    if (purchase_tender.name.ToLower().Contains(query.ToLower()) || number.ToLower().Contains(query.ToLower()))
                    {
                        return true;
                    }

                    return false;
                };
            }
            else
            {
                purchase_tenderViewSource.View.Filter = null;
            }
        }

        private void Mini_Approve_Click(object sender)
        {
            decimal Count = purchase_tenderpurchase_tender_item_detailViewSource.View.OfType<purchase_tender_detail>().Where(x => x.unit_cost == 0).Count();
            if (Count > 0)
            {
                if (System.Windows.Forms.MessageBox.Show("Items without Price, Do you wish to Continue?", "Cogntitivo ERP", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            TenderDB.Approve();
        }

        private void TabLogistics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem _TabItem = TabLogistics.SelectedItem as TabItem;
            if (_TabItem != null)
            {
                if (_TabItem.Header.ToString() == "Purchase Tender")
                {
                    if (TenderDB.db.app_condition.Where(x => x.is_active).FirstOrDefault() != null)
                    {
                        cbxCondition.SelectedItem = TenderDB.db.app_condition.Where(x => x.is_active).FirstOrDefault();
                        cbxCondition_SelectionChanged(null, null);
                    }
                }
            }
        }

        private void Hyperlink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            purchase_tender purchase_tender = purchase_tenderViewSource.View.CurrentItem as purchase_tender;
            foreach (purchase_tender_item purchase_tender_item_detail in purchase_tender.purchase_tender_item_detail)
            {
                purchase_tender_item_detail.Quantity_Factored = entity.Brillo.ConversionFactor.Factor_Quantity(purchase_tender_item_detail.item, purchase_tender_item_detail.quantity, purchase_tender_item_detail.GetDimensionValue());
                purchase_tender_item_detail.RaisePropertyChanged("Quantity_Factored");
            }
        }

        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            TenderSetting _pref_PurchaseTender = new TenderSetting();
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            TenderSetting.Default.Save();
            _pref_PurchaseTender = TenderSetting.Default;
            popupCustomize.IsOpen = false;
        }

        private void chbxRowDetail_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chbx = sender as CheckBox;
            if ((bool)chbx.IsChecked)
            {
                purchase_tender_item_detailDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            }
            else
            {
                purchase_tender_item_detailDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }
    }
}