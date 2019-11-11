using entity;
using entity.Brillo;
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
    public partial class Import : Page
    {
        private ImpexDB ImpexDB = new ImpexDB();
        private CollectionViewSource impexViewSource, impeximpex_expenseViewSource, purchase_invoiceViewSource, incotermViewSource = null;
        private cntrl.PanelAdv.pnlPurchaseInvoice pnlPurchaseInvoice = new cntrl.PanelAdv.pnlPurchaseInvoice();

        private List<entity.Class.Impex_ItemDetail> Impex_ItemDetailLIST = new List<entity.Class.Impex_ItemDetail>();
        private List<entity.Class.Impex_Products> Impex_ProductsLIST = new List<entity.Class.Impex_Products>();

   

        private decimal GrandTotal;

        public Import()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //purchase_invoiceViewSource
            purchase_invoiceViewSource = FindResource("purchase_invoiceViewSource") as CollectionViewSource;

            impexViewSource = FindResource("impexViewSource") as CollectionViewSource;
            await ImpexDB.impex
                .Where(x => x.impex_type == impex.ImpexTypes.Import && x.id_company == CurrentSession.Id_Company).Include(y => y.contact)
                .LoadAsync();
            impexViewSource.Source = ImpexDB.impex.Local;
            impeximpex_expenseViewSource = FindResource("impeximpex_expenseViewSource") as CollectionViewSource;

            //incotermViewSource
            incotermViewSource = FindResource("incotermViewSource") as CollectionViewSource;
            incotermViewSource.Source = await ImpexDB.impex_incoterm.Where(x=>x.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToListAsync();

            //CurrencyFx
            CollectionViewSource currencyfxViewSource = FindResource("currencyfxViewSource") as CollectionViewSource;
            currencyfxViewSource.Source = await ImpexDB.app_currencyfx.Include("app_currency").AsNoTracking().Where(a => a.is_active == true && a.app_currency.id_company == CurrentSession.Id_Company).ToListAsync();

            //incotermconditionViewSource
            CollectionViewSource incotermconditionViewSource = FindResource("incotermconditionViewSource") as CollectionViewSource;
            incotermconditionViewSource.Source = await ImpexDB.impex_incoterm_condition.Where(x => x.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToListAsync();
        }

        private void toolBar_btnCancel_Click_1(object sender)
        {
            impeximpex_expenseDataGrid.CancelEdit();
            impexViewSource.View.MoveCurrentToFirst();
            ImpexDB.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click_1(object sender)
        {
            MessageBoxResult res = MessageBox.Show(entity.Brillo.Localize.Question_Archive, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                impex impex = impexDataGrid.SelectedItem as impex;
                impex.is_archived = true;
                toolBar_btnSave_Click_1(sender);
            }
        }

        private void toolBar_btnEdit_Click_1(object sender)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                impex impex = (impex)impexDataGrid.SelectedItem;
                if (impex.status == Status.Documents_General.Pending)
                {
                    impex.IsSelected = true;
                    impex.State = EntityState.Modified;
                    ImpexDB.Entry(impex).State = EntityState.Modified;
                }
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void toolBar_btnNew_Click_1(object sender)
        {
            purchase_invoiceViewSource.Source = ImpexDB.purchase_invoice.Where(a => a.id_company == CurrentSession.Id_Company && a.id_contact == 0 && a.is_issued == true).OrderByDescending(a => a.trans_date).ToList();
            impex impex = new impex()
            {
                impex_type = entity.impex.ImpexTypes.Import,
                eta = DateTime.Now,
                etd = DateTime.Now,
                is_active = true,
                State = EntityState.Added,
                status = Status.Documents_General.Pending,
                IsSelected = true
            };

            if (ImpexDB.impex_incoterm.Where(x => x.is_priority).FirstOrDefault() != null)
            {
                impex.id_incoterm = ImpexDB.impex_incoterm.Where(x => x.is_priority).FirstOrDefault().id_incoterm;
            }

            ImpexDB.impex.Add(impex);
            impexViewSource.View.MoveCurrentToLast();
            Impex_ItemDetailLIST.Clear();
        }

        private void toolBar_btnSave_Click_1(object sender)
        {
            impex impex = impexDataGrid.SelectedItem as impex;

            List<impex_expense> impexexpenselist = impex.impex_expense.Where(x => x.value <= 0).ToList();
            foreach (impex_expense impex_expense in impexexpenselist)
            {
                impex.impex_expense.Remove(impex_expense);
            }

            if (ImpexDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(ImpexDB.NumberOfRecords);
            }
        }

        //Gets List of Items and Shows it.
        private void btnImportInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                foreach (purchase_invoice purchase_invoice in pnlPurchaseInvoice.selected_purchase_invoice)
                {
                    if (purchase_invoice != null)
                    {
                        getProratedCostCounted(purchase_invoice, true, GrandTotal);
                    }
                }

                productDataGrid.ItemsSource = null;
                impex_importDataGrid.ItemsSource = null;
                impex_importDataGrid.ItemsSource = Impex_ItemDetailLIST;
                productDataGrid.ItemsSource = Impex_ProductsLIST;
            }
        }

        private void impexDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Impex_ItemDetailLIST.Clear();
            Impex_ProductsLIST.Clear();

            if (impexDataGrid.SelectedItem != null)
            {
                impex impex = impexDataGrid.SelectedItem as impex;

                if (impex != null)
                {
                    if (impex.impex_expense.FirstOrDefault() != null && impex.impex_expense.FirstOrDefault().purchase_invoice != null)
                    {
                        var purchase_invoice = impex.impex_expense.FirstOrDefault().purchase_invoice;
                        if (purchase_invoice != null)
                        {
                            impex.Currencyfx = purchase_invoice.app_currencyfx;
                            impex.Currency = purchase_invoice.app_currencyfx.app_currency.name;
                        }
                    }

                    GrandTotal = impex.impex_import.Sum(x => x.purchase_invoice.purchase_invoice_detail.Where(z => z.item != null && z.item.item_product != null).Sum(y => y.SubTotal));
                    foreach (impex_import impex_import in impex.impex_import)
                    {
                        getProratedCostCounted(impex_import.purchase_invoice, false, GrandTotal);
                    }
                    
                }
            }

            productDataGrid.ItemsSource = null;
            impex_importDataGrid.ItemsSource = null;
            impex_importDataGrid.ItemsSource = Impex_ItemDetailLIST;
            productDataGrid.ItemsSource = Impex_ProductsLIST;
            Calculate_Click(null, null);
            
            

        }

        private void getProratedCostCounted(purchase_invoice purchase_invoice, bool isNew, decimal GrandTotal)
        {
            impex impex = impexDataGrid.SelectedItem as impex;

            if (isNew == true)
            {
                //impex import invoice.
                if (impex.impex_import.Where(x => x.id_purchase_invoice == purchase_invoice.id_purchase_invoice).Count() > 0)
                {
                    //Update
                    impex_import impex_import = impex.impex_import.First();
                    impex_import.id_purchase_invoice = purchase_invoice.id_purchase_invoice;
                    impex_import.purchase_invoice = purchase_invoice;
                }
                else
                {
                    //Insert
                    impex_import impex_import = new impex_import()
                    {
                        id_purchase_invoice = purchase_invoice.id_purchase_invoice,
                        purchase_invoice = purchase_invoice
                    };

                    impex.impex_import.Add(impex_import);
                }
            }
            else
            {
                //Impex datagrid selection change.
                if (purchase_invoice == null)
                {
                    impex_import impex_import = impex.impex_import.First();
                    purchase_invoice = impex_import.purchase_invoice;
                }
            }

            //Get expences
            List<impex_expense> impex_expense = impex.impex_expense.ToList();
            decimal totalExpense = 0;

            foreach (var item in impex_expense)
            {
                if (item.value != null)
                {
                    totalExpense += (decimal)item.value;
                }
            }

            if (purchase_invoice != null)
            {
                //Insert Purchase Invoice Detail
                List<purchase_invoice_detail> purchase_invoice_detail = purchase_invoice.purchase_invoice_detail.ToList();

                decimal TotalInvoiceAmount = 0;

                foreach (var item in purchase_invoice_detail)
                {
                    TotalInvoiceAmount += (item.quantity * item.UnitCost_Vat);
                }

                if (Impex_ProductsLIST.Where(x => x.id_item == 0).Count() == 0)
                {
                    entity.Class.Impex_Products ImpexImportProductDetails = new entity.Class.Impex_Products()
                    {
                        id_item = 0,
                        item = entity.Brillo.Localize.StringText("General")
                    };

                    Impex_ProductsLIST.Add(ImpexImportProductDetails);
                }

                foreach (purchase_invoice_detail _purchase_invoice_detail in purchase_invoice_detail.Where(x => x.item != null && x.item.item_product.Count()>0))
                {
                    int id_item = (int)_purchase_invoice_detail.id_item;

                    if (Impex_ProductsLIST.Where(x => x.id_item == id_item).Count() == 0)
                    {
                        entity.Class.Impex_Products ImpexImportProductDetails = new entity.Class.Impex_Products()
                        {
                            id_item = (int)_purchase_invoice_detail.id_item,
                            item = ImpexDB.items.Where(a => a.id_item == _purchase_invoice_detail.id_item).FirstOrDefault().name
                            
                        };

                        Impex_ProductsLIST.Add(ImpexImportProductDetails);
                    }

                    entity.Class.Impex_ItemDetail ImpexImportDetails = new entity.Class.Impex_ItemDetail()
                    {
                        number = _purchase_invoice_detail.purchase_invoice.number,
                        id_item = (int)_purchase_invoice_detail.id_item,
                        item_code = ImpexDB.items.Where(a => a.id_item == _purchase_invoice_detail.id_item).FirstOrDefault().code,
                        item = ImpexDB.items.Where(a => a.id_item == _purchase_invoice_detail.id_item).FirstOrDefault().name,
                     
                        quantity = _purchase_invoice_detail.quantity,
                        unit_cost = _purchase_invoice_detail.unit_cost,
                        sub_total = _purchase_invoice_detail.SubTotal,
                        id_invoice = _purchase_invoice_detail.id_purchase_invoice,
                        id_invoice_detail = _purchase_invoice_detail.id_purchase_invoice_detail
                    };

                    if (totalExpense > 0)
                    {
                        ImpexImportDetails.unit_Importcost = Math.Round(((_purchase_invoice_detail.SubTotal / GrandTotal) * totalExpense) / _purchase_invoice_detail.quantity, 2);
                        ImpexImportDetails.prorated_cost = _purchase_invoice_detail.unit_cost + ImpexImportDetails.unit_Importcost;
                    }

                    decimal SubTotal = (_purchase_invoice_detail.quantity * ImpexImportDetails.prorated_cost);
                    ImpexImportDetails.sub_total = Math.Round(SubTotal, 2);

                    Impex_ItemDetailLIST.Add(ImpexImportDetails);
                }
            }
        }

        private void GetExpenses_PreviewMouseUp(object sender, EventArgs e)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                impex impex = impexDataGrid.SelectedItem as impex;
                if (impex != null && id_incotermComboBox.SelectedItem != null && impex.impex_import.FirstOrDefault() != null)
                {
                    purchase_invoice PurchaseInvoice = impex.impex_import.FirstOrDefault().purchase_invoice as purchase_invoice;
                    impex_incoterm Incoterm = id_incotermComboBox.SelectedItem as impex_incoterm;
                    List<impex_incoterm_detail> IncotermDetail = null;

                    if (impex.impex_type == impex.ImpexTypes.Import)
                    {
                        //Only fetch buyer expense
                        IncotermDetail = ImpexDB.impex_incoterm_detail.Where(i => i.id_incoterm == Incoterm.id_incoterm && i.buyer == true).ToList();
                    }
                    if (impex.impex_type == impex.ImpexTypes.Export)
                    {
                        //Only fetch seller expense
                        IncotermDetail = ImpexDB.impex_incoterm_detail.Where(i => i.id_incoterm == Incoterm.id_incoterm && i.seller == true).ToList();
                    }

                    foreach (entity.Class.Impex_Products product in Impex_ProductsLIST)
                    {
                        foreach (var item in IncotermDetail)
                        {
                            impex_expense impex_expense = new impex_expense()
                            {
                                State = EntityState.Added,
                                value = 0,
                                id_incoterm_condition = item.id_incoterm_condition,
                                id_currency = PurchaseInvoice.app_currencyfx.id_currency,
                                id_currencyfx = PurchaseInvoice.id_currencyfx,
                                id_purchase_invoice = PurchaseInvoice.id_purchase_invoice,
                                id_item = (int)product.id_item
                            };

                            if (ImpexDB.impex_incoterm_condition.Where(x => x.id_incoterm_condition == item.id_incoterm_condition).FirstOrDefault() != null)
                            {
                                impex_expense.impex_incoterm_condition = ImpexDB.impex_incoterm_condition.Where(x => x.id_incoterm_condition == item.id_incoterm_condition).FirstOrDefault();
                            }

                            impex.impex_expense.Add(impex_expense);
                        }
                    }
                    impeximpex_expenseViewSource.View.Refresh();
                    productDataGrid.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show(entity.Brillo.Localize.PleaseSelect + " Incoterm & " + entity.Brillo.Localize.StringText("PurchaseInvoice"), "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            ImpexDB.ApproveImport();
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as impex_expense != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            impeximpex_expenseDataGrid.CancelEdit();
            impex impex = impexDataGrid.SelectedItem as impex;

            if (impex != null)
            {
                MessageBoxResult result = MessageBox.Show(entity.Brillo.Localize.Question_Archive, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //DeleteDetailGridRow
                    impex.is_archived = true;
                    impeximpex_expenseViewSource.View.Refresh();

                    impexDataGrid_SelectionChanged(null, null);
                }
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            impex impex = impexDataGrid.SelectedItem as impex;
            List<entity.Class.Impex_ItemDetail> Impex_ItemDetails = null;

            decimal totalExpense = 0;

            if (impex_importDataGrid.ItemsSource != null)
            {
                Impex_ItemDetails = impex_importDataGrid.ItemsSource.OfType<entity.Class.Impex_ItemDetail>().ToList();
                //Total of General expenses asigned to no item.
                totalExpense = (decimal)impex.impex_expense.Where(x => x.id_item == 0).Sum(x => x.value);

                foreach (entity.Class.Impex_ItemDetail Detail in Impex_ItemDetails)
                {
                    //Adds extra expenses asigend to this product.
                    totalExpense += (decimal)impex.impex_expense.Where(x => x.id_item == Detail.id_item).Sum(x => x.value);

                    if (totalExpense > 0)
                    {
                        decimal percentage = ((Detail.unit_cost * Detail.quantity) / GrandTotal);
                        decimal participation = percentage * totalExpense;
                        Detail.unit_Importcost = participation / Detail.quantity;
                        Detail.prorated_cost = Detail.unit_cost + Detail.unit_Importcost;

                        decimal SubTotal = (Detail.quantity * Detail.prorated_cost);
                        Detail.sub_total = SubTotal;
                    }
                }
            }
        }

        private void set_ContactPref(object sender, RoutedEventArgs e)
        {
            contact contact = ImpexDB.contacts.Find(sbxContact.ContactID);

            if (contact != null)
            {
                impex impex = impexViewSource.View.CurrentItem as impex;
                if (impex != null)
                {
                    impex.contact = contact;

                    if (contact != null)
                    {
                        purchase_invoiceViewSource.Source =
                            ImpexDB.purchase_invoice
                            .Where(a =>
                                   a.id_company == CurrentSession.Id_Company
                                && a.is_impex
                                && a.id_contact == contact.id_contact
                                && a.status == Status.Documents_General.Approved)
                            .OrderByDescending(a => a.trans_date)
                            .ToList();
                    }
                }
                else
                {
                    toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
                }
            }
        }

        private void Hyperlink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlPurchaseInvoice = new cntrl.PanelAdv.pnlPurchaseInvoice();
            pnlPurchaseInvoice._entity = ImpexDB;
            impex impex = impexViewSource.View.CurrentItem as impex;

            if (sbxContact.ContactID > 0 || impex.id_contact > 0)
            {
                int id_contact = 0;

                if (sbxContact.ContactID > 0)
                {
                    id_contact = sbxContact.ContactID;
                }
                else
                {
                    id_contact = impex.id_contact;
                }

                contact contact = ImpexDB.contacts.Find(id_contact);

                if (contact != null)
                {
                    pnlPurchaseInvoice._contact = contact;
                }

                pnlPurchaseInvoice.IsImpex = true;
            }

            pnlPurchaseInvoice.PurchaseInvoice_Click += PurchaseInvoice_Click;
            crud_modal.Children.Add(pnlPurchaseInvoice);
        }

        public void PurchaseInvoice_Click(object sender)
        {
            if (pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault() != null)
            {
                if (pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault().contact != null)
                {
                    contact contact = pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault().contact;

                    impex impex = impexViewSource.View.CurrentItem as impex;
                    impex.contact = contact;
                    impex.Currencyfx = pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault().app_currencyfx;
                    sbxContact.Text = contact.name;

                    foreach (purchase_invoice purchase_invoice in pnlPurchaseInvoice.selected_purchase_invoice)
                    {
                        GrandTotal += purchase_invoice.purchase_invoice_detail.Where(z => z.item != null && z.item.item_product != null).Sum(y => y.SubTotal);
                    }

                    purchase_invoiceViewSource.Source =
                    pnlPurchaseInvoice.selected_purchase_invoice;
                    btnImportInvoice_Click(sender, null);

                    crud_modal.Children.Clear();
                    crud_modal.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Calculate_Click(sender, e);
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            impex impex = impexDataGrid.SelectedItem as impex;
            if (impex != null && impex.status == Status.Documents_General.Approved)
            {
                entity.Brillo.Document.Start.Automatic(impex, "Import");
            }
            else
            {
                toolBar.msgWarning("Please Approve the Document..");
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query) && impexViewSource != null)
                {
                    impexViewSource.View.Filter = i =>
                    {
                        impex impex = i as impex;
                        string number = impex.number != null ? impex.number : "";
                        string contact = impex.contact != null ? impex.contact.name : "";
                        if (contact.ToLower().Contains(query.ToLower()) || number.ToLower().Contains(query.ToLower()))
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
                    impexViewSource.View.Filter = null;
                }
            }
            catch (Exception)
            { }
        }

        private void ToolBar_btnAnull_Click(object sender)
        {
            impex impex = impexDataGrid.SelectedItem as impex;
            foreach (impex_import impex_import in impex.impex_import)
            {
                purchase_invoice purchase_invoice = impex_import.purchase_invoice;
                int ID_CurrencyFX_Default = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;

                if (purchase_invoice!=null)
                {

                    foreach (purchase_invoice_detail purchase_invoice_detail in purchase_invoice.purchase_invoice_detail.
                   Where(x => (x.item.id_item_type == item.item_type.Product) || (x.item.id_item_type == item.item_type.RawMaterial)).ToList())
                    {

                        foreach (item_movement item_movement in purchase_invoice_detail.item_movement)
                        {
                            item_movement_value_detail item_movement_value_detail = item_movement.item_movement_value_rel.item_movement_value_detail.FirstOrDefault();
                            decimal defaultcurrency_cost = entity.Brillo.Currency.convert_Values(purchase_invoice_detail.unit_cost, purchase_invoice.id_currencyfx, ID_CurrencyFX_Default, null);

                            item_movement_value_detail.unit_value = defaultcurrency_cost;

                            item_movement_value_detail.comment = entity.Brillo.Localize.StringText("directcost");

                        }

                    }
                }
               
            }
            impex.is_active = false;
            ImpexDB.SaveChanges();
        }

        private void impeximpex_expenseDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            impexDataGrid_SelectionChanged(null, null);

        }

        private void productDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            entity.Class.Impex_Products clsProductDetail = productDataGrid.SelectedItem as entity.Class.Impex_Products;
            if (impeximpex_expenseViewSource != null && clsProductDetail != null)
            {
                if (impeximpex_expenseViewSource.View != null)
                {
                    impeximpex_expenseViewSource.View.Filter = i =>
                    {
                        impex_expense impex_expense = (impex_expense)i;
                        if (clsProductDetail != null)
                        {
                            if (impex_expense.id_item == clsProductDetail.id_item)
                                return true;
                            else
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Impex_ItemDetailLIST != null)
            {
                if (txtsearch.Text != "")
                {
                    impex_importDataGrid.ItemsSource = null;
                    impex_importDataGrid.ItemsSource = Impex_ItemDetailLIST.Where(x => x.item.ToUpper().Contains(txtsearch.Text.ToUpper()));
                }
            }
        }
    }
}