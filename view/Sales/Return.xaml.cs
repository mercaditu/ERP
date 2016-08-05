using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Cognitivo.Sales
{
    /// <summary>
    /// Interaction logic for Return.xaml
    /// </summary>
    public partial class Return : Page
    {
        SalesReturnDB SalesReturnDB = new SalesReturnDB();

        CollectionViewSource salesReturnViewSource, sales_returnsales_return_detailViewSource;
        cntrl.PanelAdv.pnlSalesInvoice pnlSalesInvoice;
     
        public Return()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                salesReturnViewSource = (CollectionViewSource)FindResource("sales_returnViewSource");
                SalesReturnDB.sales_return.Where(a => a.id_company == CurrentSession.Id_Company).OrderByDescending(x => x.trans_date).Load();
                salesReturnViewSource.Source = SalesReturnDB.sales_return.Local;
                sales_returnsales_return_detailViewSource = FindResource("sales_returnsales_return_detailViewSource") as CollectionViewSource;

                //sales_invoiceViewSource = (CollectionViewSource)FindResource("sales_invoiceViewSource");
                //sales_invoiceViewSource.Source = SalesReturnDB.sales_invoice.Where(a => a.status == Status.Documents_General.Approved && a.id_company == CurrentSession.Id_Company).ToList();

                CollectionViewSource currencyfxViewSource = (CollectionViewSource)FindResource("app_currencyfxViewSource");
                //SalesReturnDB.app_currencyfx.Include("app_currency").Where(x => x.app_currency.id_company == CurrentSession.Id_Company).Load();
                currencyfxViewSource.Source = CurrentSession.Get_Currency();

                CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
                app_vat_groupViewSource.Source = CurrentSession.Get_VAT_Group(); //SalesReturnDB.app_vat_group.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();

                cbxReturnType.ItemsSource = Enum.GetValues(typeof(Status.ReturnTypes));

                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.SalesReturn, CurrentSession.Id_Branch, CurrentSession.Id_Company);

                CollectionViewSource app_branchViewSource = ((CollectionViewSource)(FindResource("app_branchViewSource")));
                //SalesReturnDB.app_branch.Load();
                app_branchViewSource.Source = CurrentSession.Get_Branch(); //SalesReturnDB.app_branch.Local;
            }

            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        #region Toolbar
        private void toolBar_btnCancel_Click(object sender)
        {
            sales_return_detailDataGrid.CancelEdit();
            salesReturnViewSource.View.MoveCurrentToFirst();
            SalesReturnDB.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    SalesReturnDB.sales_return.Remove((sales_return)sales_returnDataGrid.SelectedItem);
                    salesReturnViewSource.View.MoveCurrentToFirst();
                    toolBar_btnSave_Click(sender);
                    sbxContact.Text = "";
                }
            }

            catch (Exception ex)
            {
                toolBar.msgError(ex);
                //throw ex;
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (sales_returnDataGrid.SelectedItem != null)
            {
                sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
                sales_return.IsSelected = true;
                sales_return.State = System.Data.Entity.EntityState.Modified;
                SalesReturnDB.Entry(sales_return).State = EntityState.Modified;
            }

            else
            {
                toolBar.msgWarning("Please Select A Record");
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            ReturnSetting _pref_SalesReturn = new ReturnSetting();
            sales_return objSalRtn = SalesReturnDB.New();
            objSalRtn.trans_date = DateTime.Now.AddDays(_pref_SalesReturn.TransDate_OffSet);
            SalesReturnDB.sales_return.Add(objSalRtn);
            salesReturnViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            SalesReturnDB.SaveChanges();
        }

        #endregion

        private void calculate_vat(object sender, EventArgs e)
        {
            //sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
            //sales_return.RaisePropertyChanged("GrandTotal");
            //List<sales_return_detail> sales_return_detail = sales_return.sales_return_detail.ToList();
            //dgvvat.ItemsSource = sales_return_detail
            //     .Join(SalesReturnDB.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
            //          , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_price * cfx.app_vat.coefficient, id_vat = cfx.app_vat.id_vat, ad })
            //          .GroupBy(a => new { a.name, a.id_vat, a.ad })
            //   .Select(g => new
            //   {
            //       id_vat = g.Key.id_vat,
            //       name = g.Key.name,
            //       value = g.Sum(a => a.value * a.ad.quantity)
            //   }).ToList();
        }

        private void sales_return_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void sales_returnDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
            if (sales_return != null)
            {
                calculate_vat(sender, e);
              
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    salesReturnViewSource.View.Filter = i =>
                    {
                        sales_return sales_return = i as sales_return;
                        if (sales_return.contact.name.ToLower().Contains(query.ToLower()))
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
                    salesReturnViewSource.View.Filter = null;
                }
            }
            catch (Exception)
            { }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as sales_return_detail != null)
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
                    sales_return sales_return = salesReturnViewSource.View.CurrentItem as sales_return;
                    //DeleteDetailGridRow
                    sales_return_detailDataGrid.CancelEdit();
                    SalesReturnDB.sales_return_detail.Remove(e.Parameter as sales_return_detail);
                    sales_returnsales_return_detailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void set_ContactPref(object sender, EventArgs e)
        {
            try
            {
                if (sbxContact.ContactID > 0)
                {
                    contact contact = SalesReturnDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                    sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
                    sales_return.id_contact = contact.id_contact;
                    sales_return.contact = contact;
                  
                    ///Start Thread to get Data.
                    Task thread_SecondaryData = Task.Factory.StartNew(() => set_ContactPref_Thread(contact));
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private async void set_ContactPref_Thread(contact objContact)
        {
            if (objContact != null)
            {
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    cbxCurrency.get_ActiveRateXContact(ref objContact);
                }));
            }
        }

        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                sales_return sales_return = salesReturnViewSource.View.CurrentItem as sales_return;
                item item = SalesReturnDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (item != null && item.id_item > 0 && sales_return != null)
                {
                    Task Thread = Task.Factory.StartNew(() => select_Item(sales_return, item));
                }
                sales_return.RaisePropertyChanged("GrandTotal");
            }
        }

        private void select_Item(sales_return sales_return, item item)
        {
            if (sales_return.sales_return_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null)
            {
                sales_return_detail _sales_return_detail = new sales_return_detail();
                _sales_return_detail.State = EntityState.Added;
                _sales_return_detail.sales_return = sales_return;
                _sales_return_detail.Contact = sales_return.contact;
                _sales_return_detail.item_description = item.description;
                _sales_return_detail.item = item;
                _sales_return_detail.id_item = item.id_item;
             
                sales_return.sales_return_detail.Add(_sales_return_detail);
            }
            else
            {
                sales_return_detail sales_return_detail = sales_return.sales_return_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                sales_return_detail.quantity += 1;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                
                sales_returnsales_return_detailViewSource.View.Refresh();
                calculate_vat(null, null);
            }));
        }

        private void salesinvoice_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Hyperlink Hyperlink = (Hyperlink)sender;
            sales_invoice sales_invoice = (sales_invoice)Hyperlink.Tag;
            if (sales_invoice != null)
            {
                entity.Brillo.Document.Start.Automatic(sales_invoice, sales_invoice.app_document_range);
            }
        }

        private void sales_return_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            sales_return_detail sales_return_detail = (sales_return_detail)e.NewItem;
            sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;

            sales_return_detail.sales_return = sales_return;
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            SalesReturnDB.Approve();
            foreach (sales_return sales_return in salesReturnViewSource.View.Cast<sales_return>().ToList())
            {
                sales_return.IsSelected = false;
            }
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            SalesReturnDB.Anull();
            foreach (sales_return sales_return in salesReturnViewSource.View.Cast<sales_return>().ToList())
            {
                sales_return.IsSelected = false;
            }
        }

        private void sales_return_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void btnSalesInvoice_Click(object sender, RoutedEventArgs e)
        {
            sales_return _sales_return = (sales_return)salesReturnViewSource.View.CurrentItem;

            if (_sales_return != null)
            {
                crud_modal.Visibility = Visibility.Visible;
                pnlSalesInvoice = new cntrl.PanelAdv.pnlSalesInvoice();
                pnlSalesInvoice._entity = new ImpexDB();

                if (sbxContact.ContactID > 0)
                {
                    contact contact = SalesReturnDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                    pnlSalesInvoice._contact = contact;
                }

                pnlSalesInvoice.SalesInvoice_Click += SalesInvoice_Click;
                crud_modal.Children.Add(pnlSalesInvoice);
            }
        }

        public void SalesInvoice_Click(object sender)
        {
            sales_return _sales_return = (sales_return)salesReturnViewSource.View.CurrentItem;
            if (_sales_return != null)
            {
                sbxContact.Text = pnlSalesInvoice.selected_sales_invoice.FirstOrDefault().contact.name;
                foreach (sales_invoice sales_invoice in pnlSalesInvoice.selected_sales_invoice)
                {
                    _sales_return.State = EntityState.Modified;
                    _sales_return.id_condition = sales_invoice.id_condition;
                    _sales_return.id_contract = sales_invoice.id_contract;
                    _sales_return.id_currencyfx = sales_invoice.id_currencyfx;
                    _sales_return.id_sales_invoice = sales_invoice.id_sales_invoice;

                    contact contact = SalesReturnDB.contacts.Where(x => x.id_contact == sales_invoice.id_contact).FirstOrDefault();
                    _sales_return.id_contact = contact.id_contact;
                    _sales_return.contact = contact;
                    sbxContact.Text = contact.name;

                    foreach (sales_invoice_detail _sales_invoice_detail in sales_invoice.sales_invoice_detail)
                    {
                        if (_sales_return.sales_return_detail.Where(x => x.id_item == _sales_invoice_detail.id_item).Count() == 0)
                        {
                            sales_return_detail sales_return_detail = new sales_return_detail();
                            sales_return_detail.id_sales_invoice_detail = _sales_invoice_detail.id_sales_invoice_detail;

                            if (SalesReturnDB.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == _sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault() != null)
                            {
                                sales_return_detail.sales_invoice_detail = SalesReturnDB.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == _sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();
                            }

                            sales_return_detail.sales_return = _sales_return;

                            if (SalesReturnDB.items.Where(x => x.id_item == _sales_invoice_detail.id_item).FirstOrDefault() != null)
                            {
                                sales_return_detail.item = SalesReturnDB.items.Where(x => x.id_item == _sales_invoice_detail.id_item).FirstOrDefault();
                            }

                            sales_return_detail.id_item = _sales_invoice_detail.id_item;
                            sales_return_detail.quantity = _sales_invoice_detail.quantity - SalesReturnDB.sales_return_detail
                                                                .Where(x => x.id_sales_invoice_detail == _sales_invoice_detail.id_sales_invoice_detail)
                                                                .GroupBy(x => x.id_sales_invoice_detail).Select(x => x.Sum(y => y.quantity)).FirstOrDefault();

                            sales_return_detail.unit_price = _sales_invoice_detail.unit_price;
                            sales_return_detail.CurrencyFX_ID = _sales_return.id_currencyfx;
                            _sales_return.sales_return_detail.Add(sales_return_detail);
                        }

                        SalesReturnDB.Entry(_sales_return).Entity.State = EntityState.Added;
                        crud_modal.Children.Clear();
                        crud_modal.Visibility = Visibility.Collapsed;
                        salesReturnViewSource.View.Refresh();

                        sales_returnsales_return_detailViewSource.View.Refresh();
                    }
                }
            }
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            sales_return sales_return = sales_returnDataGrid.SelectedItem as sales_return;
            if (sales_return != null)
            {
                entity.Brillo.Document.Start.Manual(sales_return, sales_return.app_document_range);
            }
            else
            {
                toolBar.msgWarning("Please select");
            }
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            ReturnSetting _pref_SalesReturn = new ReturnSetting();

            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            ReturnSetting.Default.Save();
            _pref_SalesReturn = ReturnSetting.Default;
            popupCustomize.IsOpen = false;
        }

        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }
    }
}
