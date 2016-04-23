using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity;
using System.Windows.Input;

namespace Cognitivo.Purchase
{
    public partial class Tender : Page
    {
        PurchaseTenderDB PurchaseTenderDB = new PurchaseTenderDB();

        CollectionViewSource purchase_tenderpurchase_tender_item_detailViewSource, purchase_tenderViewSource, purchase_tenderpurchase_tender_itemViewSource,
            purchase_tenderpurchase_tender_contact_detailViewSource, contactViewSource, app_conditionViewSource, app_contractViewSource, app_currencyfxViewSource;

        public Tender()
        {
            InitializeComponent();
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            PurchaseTenderDB.Approve();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            PurchaseTenderDB.CancelAllChanges();

        }

        private void toolBar_btnAnull_Click(object sender)
        {
            PurchaseTenderDB.Anull();
        }

        private void toolBar_btnNew_Click(object sender)
        {
            purchase_tender purchase_tender = new purchase_tender();
            purchase_tender.State = EntityState.Added;
            purchase_tender.IsSelected = true;
            purchase_tender.trans_date = DateTime.Now;
            PurchaseTenderDB.Entry(purchase_tender).State = EntityState.Added;

            purchase_tenderViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (purchase_tenderDataGrid.SelectedItem != null)
            {
                purchase_tender purchase_tender_old = (purchase_tender)purchase_tenderDataGrid.SelectedItem;
                purchase_tender_old.IsSelected = true;
                purchase_tender_old.State = EntityState.Modified;
                PurchaseTenderDB.Entry(purchase_tender_old).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    purchase_tender purchase_tender = (purchase_tender)purchase_tenderDataGrid.SelectedItem;
                    purchase_tender.is_head = false;
                    purchase_tender.State = EntityState.Deleted;
                    purchase_tender.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            entity.Properties.Settings _setting = new entity.Properties.Settings();
            int company_ID = _setting.company_ID;

            PurchaseTenderDB.purchase_tender.Where(a => a.id_company == company_ID).Load();
            purchase_tenderViewSource = FindResource("purchase_tenderViewSource") as CollectionViewSource;
            purchase_tenderViewSource.Source = PurchaseTenderDB.purchase_tender.Local;

            purchase_tenderpurchase_tender_contact_detailViewSource = FindResource("purchase_tenderpurchase_tender_contact_detailViewSource") as CollectionViewSource;
            purchase_tenderpurchase_tender_itemViewSource = FindResource("purchase_tenderpurchase_tender_itemViewSource") as CollectionViewSource;
            purchase_tenderpurchase_tender_item_detailViewSource = FindResource("purchase_tenderpurchase_tender_item_detailViewSource") as CollectionViewSource;
            PurchaseTenderDB.app_branch.Where(b => b.can_invoice == true && b.is_active == true && b.id_company == company_ID).OrderBy(b => b.name).ToList();

            cbxBranch.ItemsSource = PurchaseTenderDB.app_branch.Local;

            PurchaseTenderDB.app_department.Where(b => b.is_active == true && b.id_company == company_ID).OrderBy(b => b.name).ToList();
            cbxDepartment.ItemsSource = PurchaseTenderDB.app_department.Local;

            PurchaseTenderDB.projects.Where(b => b.is_active == true && b.id_company == company_ID).OrderBy(b => b.name).ToList();
            cbxProject.ItemsSource = PurchaseTenderDB.projects.Local;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.PurchaseTender, CurrentSession.Id_Branch, _setting.terminal_ID);

            PurchaseTenderDB.app_condition.Where(a => a.is_active == true && a.id_company == company_ID).OrderBy(a => a.name).ToList();

            app_conditionViewSource = FindResource("app_conditionViewSource") as CollectionViewSource;
            app_conditionViewSource.Source = PurchaseTenderDB.app_condition.Local;

            PurchaseTenderDB.app_contract.Where(a => a.is_active == true && a.id_company == company_ID).OrderBy(a => a.name).ToList();

            app_contractViewSource = FindResource("app_contractViewSource") as CollectionViewSource;
            app_contractViewSource.Source = PurchaseTenderDB.app_contract.Local;

            PurchaseTenderDB.app_currencyfx.Where(a => a.is_active == true && a.id_company == company_ID).ToList();

            app_currencyfxViewSource = FindResource("app_currencyfxViewSource") as CollectionViewSource;
            app_currencyfxViewSource.Source = PurchaseTenderDB.app_currencyfx.Local;

            PurchaseTenderDB.app_vat_group.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = PurchaseTenderDB.app_vat_group.Local;

        }


        public void Item_Select(object sender, EventArgs e)
        {
            purchase_tender purchase_tender = purchase_tenderViewSource.View.CurrentItem as purchase_tender;

            if (purchase_tender != null)
            {
                item item = PurchaseTenderDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (item != null)
                {
                    purchase_tender_item purchase_tender_item = new purchase_tender_item();
                    purchase_tender_item.item = item;
                    purchase_tender_item.id_item = item.id_item;
                    purchase_tender_item.item_description = item.name;
                    purchase_tender_item.quantity = 1;
                    purchase_tender.purchase_tender_item_detail.Add(purchase_tender_item);
                    purchase_tenderViewSource.View.Refresh();
                    purchase_tenderpurchase_tender_itemViewSource.View.Refresh();
                }
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
            if (sbxContact.ContactID > 0 && purchase_tenderViewSource.View != null)
            {
                purchase_tender purchase_tender = purchase_tenderViewSource.View.CurrentItem as purchase_tender;

                contact contact = PurchaseTenderDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                purchase_tender_contact purchase_tender_contact = new purchase_tender_contact();
                purchase_tender_contact.contact = contact;
                purchase_tender_contact.id_contact = contact.id_contact;
                purchase_tender_contact.id_currencyfx = PurchaseTenderDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault().id_currencyfx;
                if (contact.lead_time != null)
                {
                    purchase_tender_contact.recieve_date_est = DateTime.Now.AddDays((double)contact.lead_time);
                }



                purchase_tender_contact.app_contract = (app_contract)cbxContract.SelectedItem;
                purchase_tender_contact.app_condition = (app_condition)cbxCondition.SelectedItem;




                if (purchase_tender != null)
                {


                    List<purchase_tender_item> listtender = purchase_tender.purchase_tender_item_detail.ToList();
                    foreach (purchase_tender_item purchase_tender_item in listtender)
                    {
                        if (purchase_tender_contact.id_purchase_tender_contact == 0)
                        {
                            if (purchase_tender_contact.purchase_tender_detail.Where(x => x.purchase_tender_item.id_item == purchase_tender_item.id_item).Count() == 0)
                            {
                                purchase_tender_detail purchase_tender_detail = new purchase_tender_detail();

                                purchase_tender_detail.id_purchase_tender_item = purchase_tender_item.id_purchase_tender_item;
                                purchase_tender_detail.purchase_tender_item = purchase_tender_item;
                                purchase_tender_detail.quantity = purchase_tender_item.quantity;
                                purchase_tender_detail.unit_cost = 0;
                                purchase_tender_detail.id_vat_group = PurchaseTenderDB.app_vat_group.Where(x => x.is_default).FirstOrDefault().id_vat_group;
                                purchase_tender_contact.purchase_tender_detail.Add(purchase_tender_detail);
                            }
                            else
                            {
                                purchase_tender_detail purchase_tender_detail = purchase_tender_contact.purchase_tender_detail.Where(x => x.purchase_tender_item.id_item == purchase_tender_item.id_item).FirstOrDefault();
                                purchase_tender_detail.quantity = purchase_tender_detail.quantity + 1;

                            }

                        }
                        else
                        {
                            if (PurchaseTenderDB.purchase_tender_detail.Where(x => x.id_purchase_tender_contact == purchase_tender_contact.id_purchase_tender_contact && x.id_purchase_tender_item == purchase_tender_item.id_purchase_tender_item) == null)
                            {
                                purchase_tender_detail purchase_tender_detail = new purchase_tender_detail();

                                purchase_tender_detail.id_purchase_tender_item = purchase_tender_item.id_purchase_tender_item;
                                purchase_tender_detail.purchase_tender_item = purchase_tender_item;
                                purchase_tender_detail.quantity = 1;
                                purchase_tender_detail.id_vat_group = PurchaseTenderDB.app_vat_group.Where(x => x.is_default).FirstOrDefault().id_vat_group;

                                purchase_tender_detail.unit_cost = 0;
                                purchase_tender_contact.purchase_tender_detail.Add(purchase_tender_detail);
                            }
                            else
                            {
                                purchase_tender_detail purchase_tender_detail = PurchaseTenderDB.purchase_tender_detail.Where(x => x.id_purchase_tender_contact == purchase_tender_contact.id_purchase_tender_contact && x.id_purchase_tender_item == purchase_tender_item.id_purchase_tender_item).FirstOrDefault();
                                purchase_tender_detail.quantity = purchase_tender_detail.quantity + 1;
                            }
                        }
                    }

                }
                purchase_tender.purchase_tender_contact_detail.Add(purchase_tender_contact);

                purchase_tenderpurchase_tender_contact_detailViewSource.View.Refresh();

                purchase_tenderpurchase_tender_contact_detailViewSource.View.MoveCurrentTo(purchase_tender_contact);
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            PurchaseTenderDB.SaveChanges();
            purchase_tenderViewSource.View.Refresh();
            toolBar.msgSaved();
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
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (dg.Name == "purchase_tender_contact_detailDataGrid")
                    {
                        purchase_tender_contact_detailDataGrid.CancelEdit();
                        PurchaseTenderDB.purchase_tender_contact_detail.Remove(e.Parameter as purchase_tender_contact);
                        purchase_tenderpurchase_tender_contact_detailViewSource.View.Refresh();
                    }
                    else
                    {
                        //DeleteDetailGridRow
                        purchase_tender_item_detailDataGrid.CancelEdit();
                        PurchaseTenderDB.purchase_tender_detail.Remove(e.Parameter as purchase_tender_detail);
                        purchase_tenderpurchase_tender_item_detailViewSource.View.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void purchase_tender_contact_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {






        }

        private void purchase_tender_itemDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            purchase_tender_item purchase_tender_item = (purchase_tender_item)purchase_tenderpurchase_tender_itemViewSource.View.CurrentItem;
            CollectionViewSource purchase_tender_dimensionViewSource = ((CollectionViewSource)(FindResource("purchase_tender_dimensionViewSource")));
            if (purchase_tender_item != null)
            {
                if (purchase_tender_item.id_item > 0)
                {
                    purchase_tender_dimensionViewSource.Source = PurchaseTenderDB.purchase_tender_dimension.Where(x => x.id_purchase_tender_item == purchase_tender_item.id_purchase_tender_item).ToList();
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


    }
}
