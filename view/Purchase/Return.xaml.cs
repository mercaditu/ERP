using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;

namespace Cognitivo.Purchase
{
    public partial class Return : Page
    {
        PurchaseReturnDB dbContext = new PurchaseReturnDB();

        CollectionViewSource
            purchaseReturnViewSource,
            purchaseInvoiceViewSource,
            purchase_returnpurchase_return_detailViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();

        public Return()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //PurchaseReturn
            purchaseReturnViewSource = (CollectionViewSource)FindResource("purchase_returnViewSource");
            dbContext.purchase_return.Where(a => a.id_company == _entity.company_ID).OrderByDescending(x => x.trans_date).Load();
            purchaseReturnViewSource.Source = dbContext.purchase_return.Local;
            purchase_returnpurchase_return_detailViewSource = FindResource("purchase_returnpurchase_return_detailViewSource") as CollectionViewSource;

            //PurchaseInvoice
            purchaseInvoiceViewSource = (CollectionViewSource)FindResource("purchase_invoiceViewSource");
            purchaseInvoiceViewSource.Source = dbContext.purchase_invoice.Where(a => a.id_company == _entity.company_ID).ToList();



            CollectionViewSource currencyfxViewSource = (CollectionViewSource)FindResource("currencyfxViewSource");
            dbContext.app_currencyfx.Include("app_currency").Where(x => x.app_currency.id_company == _entity.company_ID).Load();
            currencyfxViewSource.Source = dbContext.app_currencyfx.Local;
            //cbxCurrency.entity = entity;



            //app_cost_centerViewSource
            CollectionViewSource app_cost_centerViewSource = (CollectionViewSource)FindResource("app_cost_centerViewSource");
            app_cost_centerViewSource.Source = dbContext.app_cost_center.Where(a => a.is_active == true && a.id_company == _entity.company_ID).ToList();

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = dbContext.app_vat_group.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).ToList();

            cbxReturnType.ItemsSource = Enum.GetValues(typeof(Status.ReturnTypes));
        }

        private void toolBar_btnDelete_Click(object sender)
        {

            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                dbContext.purchase_return.Remove((purchase_return)purchase_returnDataGrid.SelectedItem);
                purchaseReturnViewSource.View.MoveCurrentToFirst();
                toolBar_btnSave_Click(sender);
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            purchase_return purchase_return = dbContext.New();
            dbContext.purchase_return.Add(purchase_return);
            purchaseReturnViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            dbContext.SaveChanges();
            toolBar.msgSaved();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (purchase_returnDataGrid.SelectedItem != null)
            {
                purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
                purchase_return.IsSelected = true;
                purchase_return.State = EntityState.Modified;
                dbContext.Entry(purchase_return).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbContext.CancelAllChanges();
        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            if (cbxPurchaseInvoice.SelectedItem != null)
            {
                purchase_invoice purchase_invoice = cbxPurchaseInvoice.SelectedItem as purchase_invoice;
                if (purchase_returnDataGrid.SelectedItem != null)
                {
                    purchase_return purchase_return = purchase_returnDataGrid.SelectedItem as purchase_return;
                    foreach (var item in purchase_invoice.purchase_invoice_detail)
                    {
                        purchase_return_detail purchase_return_detail = new purchase_return_detail();
                        purchase_return_detail.purchase_return = purchase_return;
                        purchase_return_detail.id_cost_center = item.id_cost_center;
                        purchase_return_detail.item = item.item;
                        purchase_return_detail.id_item = item.id_item;
                        purchase_return_detail.item_description = item.item_description;
                        purchase_return_detail.unit_cost = item.unit_cost;
                        purchase_return_detail.id_vat_group = item.id_vat_group;
                        purchase_return_detail.expiration_date = item.expiration_date;
                        purchase_return_detail.id_purchase_invoice_detail = (int)item.id_purchase_invoice_detail;
                        purchase_return_detail.quantity = item.quantity;
                        purchase_return.purchase_return_detail.Add(purchase_return_detail);
                    }
                    //calculate_total(sender, e);
                    calculate_vat(sender, e);
                    purchase_returnpurchase_return_detailViewSource.View.Refresh();
                }
            }
        }

        #region Datagrid Events
        private void calculate_vat(object sender, EventArgs e)
        {
            purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
            purchase_return.RaisePropertyChanged("GrandTotal");
            //List<purchase_return_vat> deletepurchase_return_detail_vat = entity.db.purchase_return_detail_vat.Local.Where(x => x.purchase_return_detail == null).ToList();
            //List<purchase_return_vat> purchase_return_detail_vat = entity.db.purchase_return_detail_vat.Local.Where(x => x.purchase_return_detail != null && x.id_purchase_return_vat == 0).ToList();
            //entity.db.purchase_return_detail_vat.RemoveRange(deletepurchase_return_detail_vat);
            //purchase_return_detail_vat = purchase_return_detail_vat.Where(x => x.purchase_return_detail.purchase_return == purchase_return).ToList();
            //dgvvat.ItemsSource = purchase_return_detail_vat
            //                        .Join(entity.db.app_vat, ad => ad.id_vat, cfx => cfx.id_vat
            //       , (ad, cfx) => new { name = cfx.name, value = ad.unit_value, id_vat = ad.id_vat, ad.purchase_return_detail })
            //       .GroupBy(a => new { a.name, a.id_vat, a.purchase_return_detail })
            //       .Select(g => new
            //       {
            //           id_vat = g.Key.id_vat,
            //           name = g.Key.name,
            //           value = g.Sum(a => a.value * a.purchase_return_detail.quantity)
            //       }).ToList();
            List<purchase_return_detail> purchase_return_detail = purchase_return.purchase_return_detail.ToList();
            dgvvat.ItemsSource = purchase_return_detail
                 .Join(dbContext.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
                      , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_cost * cfx.app_vat.coefficient, id_vat = cfx.app_vat.id_vat, ad })
                      .GroupBy(a => new { a.name, a.id_vat, a.ad })
               .Select(g => new
               {
                   id_vat = g.Key.id_vat,
                   name = g.Key.name,
                   value = g.Sum(a => a.value * a.ad.quantity)
               }).ToList();
        }

        //private void calculate_total(object sender, EventArgs e)
        //{
        //    purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
        //    if (purchase_return != null)
        //    {
        //        purchase_return.get_Puchase_Total();
        //    }
        //}

        private void purchase_return_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }
        private void purchase_returnDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
            if (purchase_return != null)
            {
                //dgvvat.ItemsSource = entity.db.purchase_return_detail_vat
                //    .Where(x => x.purchase_return_detail.id_purchase_return == purchase_return.id_purchase_return)
                //    .Join(entity.db.app_vat, ad => ad.id_vat, cfx => cfx.id_vat
                //    , (ad, cfx) => new { name = cfx.name, value = ad.unit_value, id_vat = ad.id_vat })
                //    .GroupBy(a => new { a.name, a.id_vat })
                //    .Select(g => new
                //    {
                //        id_vat = g.Key.id_vat,
                //        name = g.Key.name,
                //        value = g.Sum(a => a.value)
                //    }).ToList();

            }
            //calculate_total(sender, e);
        }

        //private void purchase_returnDataGrid_Loaded(object sender, RoutedEventArgs e)
        //{
        //    calculate_total(sender, e);
        //}

        private void purchase_return_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            purchase_return_detail purchase_return_detail = (purchase_return_detail)e.NewItem;
            purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
            //purchase_return_detail.id_branch = (int)purchase_return.id_branch;
        }
        #endregion

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    purchaseReturnViewSource.View.Filter = i =>
                    {
                        purchase_return purchase_return = i as purchase_return;
                        if (purchase_return.contact.name.ToLower().Contains(query.ToLower()))
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
                    purchaseReturnViewSource.View.Filter = null;
                }
            }
            catch (Exception)
            { }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as purchase_return_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    purchase_return purchase_return = purchaseReturnViewSource.View.CurrentItem as purchase_return;
                    //DeleteDetailGridRow
                    purchase_return_detailDataGrid.CancelEdit();
                    dbContext.purchase_return_detail.Remove(e.Parameter as purchase_return_detail);
                    purchase_returnpurchase_return_detailViewSource.View.Refresh();
                    //calculate_total(sender, e);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
                purchase_return.id_contact = contact.id_contact;
                purchase_return.contact = contact;


            }
        }



        private void item_Select(object sender, EventArgs e)
        {
            try
            {

                if (sbxItem.ItemID > 0)
                {
                    //Product

                    item item = dbContext.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                    purchase_return purchase_return = purchase_returnDataGrid.SelectedItem as purchase_return;
                    purchase_return_detail purchase_return_detail = purchase_return.purchase_return_detail.Where(a => a.id_item == sbxItem.ItemID).FirstOrDefault();
                    int id_cost_center = 0;
                    if (purchase_return_detail == null)
                    {
                        purchase_return_detail _purchase_return_detail = new entity.purchase_return_detail();
                        //Check for contact
                        if (sbxContact.ContactID > 0)
                        {
                            contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                            if (contact.app_cost_center != null)
                            {
                                app_cost_center app_cost_center = contact.app_cost_center as app_cost_center;
                                if (app_cost_center.is_product == true)
                                {
                                    id_cost_center = app_cost_center.id_cost_center;
                                    if (id_cost_center > 0)
                                        _purchase_return_detail.id_cost_center = id_cost_center;
                                }
                                else
                                {
                                    if (dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                        id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                                    if (id_cost_center > 0)
                                        _purchase_return_detail.id_cost_center = id_cost_center;
                                }
                            }
                            else
                            {
                                if (dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                    id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                                if (id_cost_center > 0)
                                    _purchase_return_detail.id_cost_center = id_cost_center;
                            }
                        }
                        else
                        {
                            if (dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                            if (id_cost_center > 0)
                                _purchase_return_detail.id_cost_center = id_cost_center;
                        }
                        if (dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _entity.company_ID).FirstOrDefault() != null)
                        {
                            _purchase_return_detail.id_vat_group = dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _entity.company_ID).FirstOrDefault().id_vat_group;
                        }
                        _purchase_return_detail.purchase_return = purchase_return;
                        _purchase_return_detail.item = item;
                        _purchase_return_detail.id_item = sbxItem.ItemID;
                        purchase_return.purchase_return_detail.Add(_purchase_return_detail);
                    }
                    else
                    {
                        purchase_return_detail.quantity += 1;
                    }
                    purchase_returnpurchase_return_detailViewSource.View.Refresh();
                    //calculate_total(sender, e);
                    calculate_vat(sender, e);

                }
                else
                {
                    //Other
                    purchase_return purchase_return = purchase_returnDataGrid.SelectedItem as purchase_return;
                    purchase_return_detail purchase_return_detail = purchase_return.purchase_return_detail.Where(a => a.item_description == sbxItem.Text).FirstOrDefault();
                    if (purchase_return_detail == null)
                    {
                        purchase_return_detail _purchase_return_detail = new entity.purchase_return_detail();
                        int id_cost_center = 0;
                        //Check for contact
                        if (sbxContact.ContactID > 0)
                        {
                            contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                            if (contact.app_cost_center != null)
                            {
                                app_cost_center app_cost_center = contact.app_cost_center as app_cost_center;
                                if (app_cost_center.is_product == false)
                                {
                                    id_cost_center = app_cost_center.id_cost_center;
                                    if (id_cost_center > 0)
                                        _purchase_return_detail.id_cost_center = id_cost_center;
                                }
                                else
                                {
                                    if (dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                        id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                                    if (id_cost_center > 0)
                                        _purchase_return_detail.id_cost_center = id_cost_center;
                                }
                            }
                            else
                            {
                                if (dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                    id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                                if (id_cost_center > 0)
                                    _purchase_return_detail.id_cost_center = id_cost_center;
                            }
                        }
                        else
                        {
                            if (dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                            if (id_cost_center > 0)
                                _purchase_return_detail.id_cost_center = id_cost_center;
                        }
                        if (dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _entity.company_ID).FirstOrDefault() != null)
                        {
                            _purchase_return_detail.id_vat_group = dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _entity.company_ID).FirstOrDefault().id_vat_group;
                        }
                        _purchase_return_detail.purchase_return = purchase_return;
                        _purchase_return_detail.item_description = sbxItem.Text;
                        _purchase_return_detail.id_item = 0;
                        purchase_return.purchase_return_detail.Add(_purchase_return_detail);
                    }
                    else
                    {
                        purchase_return_detail.quantity += 1;

                    }
                    purchase_returnpurchase_return_detailViewSource.View.Refresh();
                    //calculate_total(sender, e);
                    calculate_vat(sender, e);

                }

            }
            catch (Exception)
            {
                //throw ex;
            }
        }

       

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            dbContext.Approve();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            dbContext.Anull();
        }

        private void cbxCurrency_MouseEnter(object sender, MouseEventArgs e)
        {
            //purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
            //if (purchase_return != null)
            //    purchase_return.is_edit = true;
        }

        private void purchase_return_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }
    }
}
