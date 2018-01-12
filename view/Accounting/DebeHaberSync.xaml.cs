using cntrl;
using Cognitivo.Menu;
using Cognitivo.Properties;
using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Accounting
{
    public partial class DebeHaberSync : Page, IDisposable
    {
        private CollectionViewSource sales_invoiceViewSource;
        private CollectionViewSource sales_returnViewSource;
        private CollectionViewSource purchase_invoiceViewSource;
        private CollectionViewSource purchase_returnViewSource;
        private CollectionViewSource payment_detailViewSource;
        private CollectionViewSource item_assetViewSource;
        private CollectionViewSource production_order_detailViewSource;

        //private db db = new db();
        private dbContext db = new dbContext();

        private string RelationshipHash = string.Empty;
        private string GovCode = string.Empty;

        public DateTime StartDate
        {
            get { return AbsoluteDate.Start(_StartDate); }
            set { _StartDate = value; } //fill(); }
        }

        private DateTime _StartDate = AbsoluteDate.Start(DateTime.Now.AddMonths(-1));

        public DateTime EndDate
        {
            get { return AbsoluteDate.End(_EndDate); }
            set { _EndDate = value; } // fill(); }
        }

        private DateTime _EndDate = AbsoluteDate.End(DateTime.Now);

        public DebeHaberSync()
        {
            InitializeComponent();

            db.db = new db();

            DatePanel.StartDate = DateTime.Now.AddMonths(-1);
            DatePanel.EndDate = DateTime.Now.Date.AddDays(1).AddTicks(-1);

            sales_invoiceViewSource = FindResource("sales_invoiceViewSource") as CollectionViewSource;
            sales_returnViewSource = FindResource("sales_returnViewSource") as CollectionViewSource;
            purchase_invoiceViewSource = FindResource("purchase_invoiceViewSource") as CollectionViewSource;
            purchase_returnViewSource = FindResource("purchase_returnViewSource") as CollectionViewSource;
            payment_detailViewSource = FindResource("payment_detailViewSource") as CollectionViewSource;
            item_assetViewSource = FindResource("item_assetViewSource") as CollectionViewSource;
            production_order_detailViewSource = FindResource("production_order_detailViewSource") as CollectionViewSource;

            RelationshipHash = db.db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault().hash_debehaber;
            GovCode = db.db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault().gov_code;
            var timer = new System.Threading.Timer(
                e => btnData_Refresh(null, null),
                null,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(15));
        }

        private void fill()
        {
            //Dispatcher
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Get_SalesInvoice();
                Get_PurchaseInvoice();
                Get_PurchaseReturnInvoice();
                Get_SalesReturn();
                Get_Payment();
                Get_ProductionExecution();
            }));
            Dispatcher.BeginInvoke((Action)(() => { progBar.IsIndeterminate = false; }));
        }

        private void btnData_Refresh(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => { progBar.IsIndeterminate = true; }));
            Task taskAuth = Task.Factory.StartNew(() => fill());
        }

        #region LoadData

        public async void Get_SalesInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_invoiceViewSource.Source = await db.db.sales_invoice.Where(x =>
                x.id_company == CurrentSession.Id_Company &&
                x.trans_date >= DatePanel.StartDate && x.trans_date <= DatePanel.EndDate &&
                x.is_accounted == false &&
                (x.status == Status.Documents_General.Approved || x.status == Status.Documents_General.Annulled)).Include(x => x.sales_invoice_detail).Include(x => x.contact).ToListAsync();
        }

        public async void Get_Payment()
        {
            //x.Is Head replace with Is_Accounted = True.
            payment_detailViewSource.Source = await db.db.payment_detail.Where(x =>
                x.payment.id_company == CurrentSession.Id_Company &&
                x.trans_date >= DatePanel.StartDate && x.trans_date <= DatePanel.EndDate &&
                x.payment.is_accounted == false &&
                x.payment.status == Status.Documents_General.Approved).Include(x => x.payment).ToListAsync();
        }

        public async void Get_SalesReturn()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_returnViewSource.Source = await db.db.sales_return.Where(x =>
                x.id_company == CurrentSession.Id_Company &&
                x.trans_date >= DatePanel.StartDate && x.trans_date <= DatePanel.EndDate &&
                x.is_accounted == false &&
                (x.status == Status.Documents_General.Approved || x.status == Status.Documents_General.Annulled)).Include(x => x.contact).ToListAsync();
        }

        public async void Get_PurchaseReturnInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_returnViewSource.Source = await db.db.purchase_return.Where(x =>
                x.id_company == CurrentSession.Id_Company &&
                x.trans_date >= DatePanel.StartDate && x.trans_date <= DatePanel.EndDate &&
                x.is_accounted == false &&
                x.status == Status.Documents_General.Approved).Include(x => x.contact).ToListAsync();
        }

        public async void Get_PurchaseInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_invoiceViewSource.Source = await db.db.purchase_invoice.Where(x =>
                x.id_company == CurrentSession.Id_Company &&
                x.trans_date >= DatePanel.StartDate && x.trans_date <= DatePanel.EndDate &&
                x.is_accounted == false &&
                x.status == Status.Documents_General.Approved).Include(x => x.contact).ToListAsync();
        }

        private async void Get_ItemAsset()
        {
            await db.db.item_asset.Where(x =>
                x.id_company == CurrentSession.Id_Company &&
                x.item.is_active == true).ToListAsync();
            item_assetViewSource.Source = db.db.item_asset.Local;
        }

        private async void Get_ProductionExecution()
        {
            //If we bring only low level items, it's easy to calculate the higher level items.
            await db.db.production_order.Where
                (x =>
                x.id_company == CurrentSession.Id_Company
                //x.production_order_detail.Where(y => y.product == false).Count() > 0
                //x.child.Count() == 0
                )
                .Include(z => z.production_order_detail)
                //.Include(a => a.production_execution_detail)
                .ToListAsync();

            production_order_detailViewSource.Source = db.db.production_order.Local;
        }

        #endregion LoadData

        private  void btnData_Sync(object sender, RoutedEventArgs e)
        {
            DebeHaberLogIn DebeHaberLogIn = new DebeHaberLogIn();
            try
            {
                DebeHaberLogIn.check_api(RelationshipHash, GovCode);
            }
            catch (Exception)
            {

                return;
            }
            
            

            //Loops through each set of data and syncs each record individually.
            Sales_Sync();
            Purchase_Sync();
            SalesReturn_Sync();
            PurchaseReturn_Sync();
            PaymentSync();
            Production_Sync();
        }

        #region Sales Sync

        private void Sales_Sync()
        {
            List<sales_invoice> SalesList = db.db.sales_invoice.Local.Where(x => x.IsSelected).ToList();

            //Loop through
            foreach (sales_invoice sales_invoice in SalesList)
            {
                DebeHaber.Integration Integration = new DebeHaber.Integration()
                {
                    Key = RelationshipHash,
                    GovCode = GovCode
                };

                DebeHaber.Transaction Transaction = new DebeHaber.Transaction();
                DebeHaber.Commercial_Invoice Sales = new DebeHaber.Commercial_Invoice();

                //Loads Data from Sales
                Sales.Fill_BySales(sales_invoice);

                ///Loop through Details.
                foreach (sales_invoice_detail Detail in sales_invoice.sales_invoice_detail)
                {
                    DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_BySales(Detail, db.db);
                    Sales.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (payment_schedual schedual in sales_invoice.payment_schedual)
                {
                    ///If Detail exists but not Header, then create the header (payment)
                    ///This will help fix payment details that don't have header.
                    if (schedual.payment_detail != null && schedual.payment_detail.payment == null)
                    {
                        payment payment = new payment()
                        {
                            id_contact = sales_invoice.id_contact,
                            id_company = CurrentSession.Id_Company,
                            status = Status.Documents_General.Approved,
                            trans_date = schedual.expire_date,
                            id_user = schedual.payment_detail.id_user,
                            is_archived = false,
                            is_read = false,
                            is_accounted = false,
                            timestamp = DateTime.Now
                        };

                        db.db.payments.Add(payment);
                        db.db.SaveChanges();
                        schedual.payment_detail.id_payment = payment.id_payment;
                        db.db.SaveChanges();

                        schedual.payment_detail.payment = payment;
                    }

                    if (schedual.payment_detail != null && schedual.payment_detail.payment != null && schedual.payment_detail.payment.is_accounted == false)
                    {
                        DebeHaber.Payments Payments = new DebeHaber.Payments();
                        //Fill and Add Payments
                        Payments.FillPayments(schedual);
                        Sales.Payments.Add(Payments);
                    }
                }

                Transaction.Commercial_Invoices.Add(Sales);
                Integration.Transactions.Add(Transaction);

                try
                {
                    var Sales_Json = new JavaScriptSerializer().Serialize(Integration);
                    Send2API(Sales_Json);

                    sales_invoice.IsSelected = false;
                    sales_invoice.is_accounted = true;
                    foreach (payment_schedual schedual in sales_invoice.payment_schedual)
                    {
                        if (schedual.payment_detail != null && schedual.payment_detail.payment != null && schedual.payment_detail.payment.is_accounted == false)
                        {
                            //This will make the Sales Invoice hide from the next load.
                            schedual.payment_detail.payment.is_accounted = true;
                        }
                    }
                    db.db.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("403"))
                    {


                        DebeHaberLogIn page = new DebeHaberLogIn();
                        MainWindow rootWindow = Window.GetWindow(this) as MainWindow;

                        rootWindow.mainFrame.Navigate(page);


                    }
                    else
                    {
                        if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        {
                            MessageBox.Show(ex.Message, "Error Message");
                            Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                        }
                    }

                    //Error Sales Invoice keep Is Accounted to False.
                    sales_invoice.is_accounted = false;
                }
                finally
                {
                    db.db.SaveChanges();
                    fill();
                }
            }
        }

        #endregion Sales Sync

        #region Purchase Sync

        private void Purchase_Sync()
        {
            //Loop through
            List<purchase_invoice> PurchaseList = db.db.purchase_invoice.Local.Where(x => x.IsSelected).ToList();

            foreach (purchase_invoice purchase_invoice in PurchaseList)
            {
                DebeHaber.Integration Integration = new DebeHaber.Integration()
                {
                    Key = RelationshipHash,
                    GovCode = GovCode
                };

                DebeHaber.Transaction Transaction = new DebeHaber.Transaction();

                DebeHaber.Commercial_Invoice Purchase = new DebeHaber.Commercial_Invoice();

                //Loads Data from Sales
                Purchase.Fill_ByPurchase(purchase_invoice);

                ///Loop through Details.
                foreach (purchase_invoice_detail Detail in purchase_invoice.purchase_invoice_detail)
                {
                    DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_ByPurchase(Detail, db.db);
                    Purchase.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (payment_schedual schedual in purchase_invoice.payment_schedual)
                {
                    if (schedual.payment_detail != null && schedual.payment_detail.payment.is_accounted == false)
                    {
                        DebeHaber.Payments Payments = new DebeHaber.Payments();
                        //Fill and Add Payments
                        Payments.FillPayments(schedual);
                        Purchase.Payments.Add(Payments);
                    }
                }

                Transaction.Commercial_Invoices.Add(Purchase);
                Integration.Transactions.Add(Transaction);

                try
                {
                    var Sales_Json = new JavaScriptSerializer().Serialize(Integration);
                    Send2API(Sales_Json);

                    purchase_invoice.IsSelected = false;
                    purchase_invoice.is_accounted = true;
                    foreach (payment_schedual schedual in purchase_invoice.payment_schedual)
                    {
                        if (schedual.payment_detail != null && schedual.payment_detail.payment.is_accounted == false)
                        {
                            //This will make the Sales Invoice hide from the next load.
                            schedual.payment_detail.payment.is_accounted = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("403"))
                    {

                       
                        DebeHaberLogIn page = new DebeHaberLogIn();
                        MainWindow rootWindow = Window.GetWindow(this) as MainWindow;
                       
                        rootWindow.mainFrame.Navigate(page);


                    }
                    else
                    {
                        if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        {
                            MessageBox.Show(ex.Message, "Error Message");
                            Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                        }
                    }

                    //Error Sales Invoice keep Is Accounted to False.
                    purchase_invoice.is_accounted = false;
                }
                finally
                {
                    db.db.SaveChanges();
                    fill();
                }
            }
        }

        #endregion Purchase Sync

        #region SalesReturn Sync

        private void SalesReturn_Sync()
        {
            List<sales_return> SalesReturnList = db.db.sales_return.Local.Where(x => x.IsSelected).ToList();

            //Loop through
            foreach (sales_return sales_return in SalesReturnList)
            {
                DebeHaber.Integration Integration = new DebeHaber.Integration();
                Integration.Key = RelationshipHash;
                Integration.GovCode = GovCode;

                DebeHaber.Transaction Transaction = new DebeHaber.Transaction();

                DebeHaber.Commercial_Invoice SalesReturn = new DebeHaber.Commercial_Invoice();

                //Loads Data from Sales
                SalesReturn.Fill_BySalesReturn(sales_return);

                ///Loop through Details.
                foreach (sales_return_detail Detail in sales_return.sales_return_detail)
                {
                    DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_BySalesReturn(Detail, db.db);
                    SalesReturn.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (payment_schedual schedual in sales_return.payment_schedual)
                {
                    if (schedual.payment_detail != null && schedual.payment_detail.payment.is_accounted == false)
                    {
                        DebeHaber.Payments Payments = new DebeHaber.Payments();
                        //Fill and Add Payments
                        Payments.FillPayments(schedual);
                        SalesReturn.Payments.Add(Payments);
                    }
                }

                Transaction.Commercial_Invoices.Add(SalesReturn);
                Integration.Transactions.Add(Transaction);

                try
                {
                    var Sales_Json = new JavaScriptSerializer().Serialize(Integration);
                    Send2API(Sales_Json);

                    //This will make the Sales Invoice hide from the next load.
                    sales_return.IsSelected = false;
                    sales_return.is_accounted = true;
                    //This will make the Payment hide from the next load.
                    foreach (payment_schedual schedual in sales_return.payment_schedual)
                    {
                        if (schedual.payment_detail != null && schedual.payment_detail.payment.is_accounted == false)
                        {
                            schedual.payment_detail.payment.is_accounted = true;
                        }
                    }
                }
                catch (Exception ex)
                {

                    if (ex.Message.Contains("403"))
                    {


                        DebeHaberLogIn page = new DebeHaberLogIn();
                        MainWindow rootWindow = Window.GetWindow(this) as MainWindow;

                        rootWindow.mainFrame.Navigate(page);


                    }
                    else
                    {
                        if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        {
                            MessageBox.Show(ex.Message, "Error Message");
                            Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                        }
                    }

                    //Error Sales Invoice keep Is Accounted to False.
                    sales_return.is_accounted = false;
                }
                finally
                {
                    db.db.SaveChanges();
                    fill();
                }
            }
        }

        #endregion SalesReturn Sync

        #region PurchaseReturn Sync

        private void PurchaseReturn_Sync()
        {
            List<purchase_return> PurchaseReturnList = db.db.purchase_return.Local.Where(x => x.IsSelected).ToList();

            //Loop through
            foreach (purchase_return purchase_return in PurchaseReturnList)
            {
                DebeHaber.Integration Integration = new DebeHaber.Integration();
                Integration.Key = RelationshipHash;
                Integration.GovCode = GovCode;
                DebeHaber.Transaction Transaction = new DebeHaber.Transaction();

                DebeHaber.Commercial_Invoice PurchaseReturn = new DebeHaber.Commercial_Invoice();

                //Loads Data from Sales
                PurchaseReturn.Fill_ByPurchaseReturn(purchase_return);

                ///Loop through Details.
                foreach (purchase_return_detail Detail in purchase_return.purchase_return_detail)
                {
                    DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_ByPurchaseReturn(Detail, db.db);
                    PurchaseReturn.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (payment_schedual schedual in purchase_return.payment_schedual)
                {
                    if (schedual.payment_detail != null && schedual.payment_detail.payment.is_accounted == false)
                    {
                        DebeHaber.Payments Payments = new DebeHaber.Payments();
                        //Fill and Add Payments
                        Payments.FillPayments(schedual);
                        PurchaseReturn.Payments.Add(Payments);
                    }
                }

                Transaction.Commercial_Invoices.Add(PurchaseReturn);
                Integration.Transactions.Add(Transaction);

                try
                {
                    var Sales_Json = new JavaScriptSerializer().Serialize(Integration);
                    Send2API(Sales_Json);

                    //Marks Is Accounted True so that it does not appear again on next load.
                    purchase_return.IsSelected = false;
                    purchase_return.is_accounted = true;

                    //Loop through payments to mark Is Accounted. Same code as above, but required for control.
                    foreach (payment_schedual schedual in purchase_return.payment_schedual)
                    {
                        if (schedual.payment_detail != null && schedual.payment_detail.payment.is_accounted == false)
                        {
                            schedual.payment_detail.payment.is_accounted = true;
                        }
                    }
                }
                catch (Exception ex)
                {

                    if (ex.Message.Contains("403"))
                    {


                        DebeHaberLogIn page = new DebeHaberLogIn();
                        MainWindow rootWindow = Window.GetWindow(this) as MainWindow;

                        rootWindow.mainFrame.Navigate(page);


                    }
                    else
                    {
                        if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        {
                            MessageBox.Show(ex.Message, "Error Message");
                            Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                        }
                    }
                    //Error Sales Invoice keep Is Accounted to False.
                    purchase_return.is_accounted = false;
                }
                finally
                {
                    db.db.SaveChanges();
                    fill();
                }
            }
        }

        #endregion PurchaseReturn Sync

        #region Payment Sync

        private void PaymentSync()
        {
            List<payment_detail> PaymentList = db.db.payment_detail.Local.Where(x => x.IsSelected && x.payment.is_accounted == false).ToList();

            //Loop through
            foreach (payment_detail payment_detail in PaymentList)
            {
                DebeHaber.Integration Integration = new DebeHaber.Integration();
                Integration.Key = RelationshipHash;
                Integration.GovCode = GovCode;
                DebeHaber.Transaction Transaction = new DebeHaber.Transaction();

                DebeHaber.Payments Payment = new DebeHaber.Payments();

                //Loads Data from Sales
                payment_schedual schedual = db.db.payment_schedual.Where(x => x.id_payment_detail == payment_detail.id_payment_detail).FirstOrDefault();
                Payment.FillPayments(schedual);

                Transaction.Payments.Add(Payment);
                Integration.Transactions.Add(Transaction);

                try
                {
                    var Sales_Json = new JavaScriptSerializer().Serialize(Integration);
                    Send2API(Sales_Json);

                    payment_detail.IsSelected = false;
                    payment_detail.payment.is_accounted = true;
                }
                catch (Exception ex)
                {

                    if (ex.Message.Contains("403"))
                    {


                        DebeHaberLogIn page = new DebeHaberLogIn();
                        MainWindow rootWindow = Window.GetWindow(this) as MainWindow;

                        rootWindow.mainFrame.Navigate(page);


                    }
                    else
                    {
                        if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        {
                            MessageBox.Show(ex.Message, "Error Message");
                            Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                        }
                    }

                    //Error Sales Invoice keep Is Accounted to False.
                    payment_detail.payment.is_accounted = false;
                }
                finally
                {
                    db.db.SaveChanges();
                    fill();
                }
            }
        }

        #endregion Payment Sync

        #region Production_Sync

        private void Production_Sync()
        {
            List<production_order> OrderList = db.db.production_order.Local.Where(x => x.IsSelected).ToList();

            foreach (production_order ProductionOrder in OrderList)
            {
                DebeHaber.Integration Integration = new DebeHaber.Integration();
                Integration.Key = RelationshipHash;
                Integration.GovCode = GovCode;
                DebeHaber.Transaction Transaction = new DebeHaber.Transaction();
                DebeHaber.Production Production = new DebeHaber.Production();

                Production.branch = ProductionOrder.id_branch > 0 ? db.db.app_branch.Find(ProductionOrder.id_branch).name : "";
                Production.name = ProductionOrder.name;

                DateTime productionDate = ProductionOrder.trans_date;

                IQueryable<production_order_detail> ProductionOrder2 = ProductionOrder.production_order_detail.Where(x => x.item.id_item_type != item.item_type.Task).AsQueryable().Include(x => x.production_execution_detail);

                int DetailCount = 0;

                foreach (production_order_detail Detail in ProductionOrder2)
                {
                    if (Detail.production_execution_detail.Where(x => x.is_accounted == false && x.status == Status.Production.Executed).Count() > 0)
                    {
                        DebeHaber.Production_Detail Production_Detail = new DebeHaber.Production_Detail();
                        Production_Detail.Fill_ByExecution(Detail, db.db);
                        Production.Production_Detail.Add(Production_Detail);
                        productionDate = Production_Detail.trans_date;
                        DetailCount += 1;
                    }

                }

                Production.trans_date = productionDate;

                if (DetailCount > 0)
                {
                    Transaction.Production.Add(Production);
                    Integration.Transactions.Add(Transaction);



                    try
                    {
                        var Sales_Json = new JavaScriptSerializer().Serialize(Integration);
                        Send2API(Sales_Json);

                        ProductionOrder.IsSelected = false;

                        foreach (production_order_detail Detail in ProductionOrder.production_order_detail.Where(x => x.item.id_item_type != item.item_type.Task))
                        {
                            foreach (production_execution_detail Exe in Detail.production_execution_detail.Where(x => x.is_accounted == false && x.status == Status.Production.Executed))
                            {
                                Exe.is_accounted = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        if (ex.Message.Contains("403"))
                        {


                            DebeHaberLogIn page = new DebeHaberLogIn();
                            MainWindow rootWindow = Window.GetWindow(this) as MainWindow;

                            rootWindow.mainFrame.Navigate(page);


                        }
                        else
                        {
                            if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                            {
                                MessageBox.Show(ex.Message, "Error Message");
                                Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                            }
                        }
                    }
                    finally
                    {
                        db.db.SaveChanges();
                        fill();
                    }
                }
            }
        }

        #endregion Production_Sync

        private void FixedAsset(DebeHaber.Transaction Transaction)
        {
            List<item_asset_group> AssetGroupList = db.db.item_asset_group.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

            foreach (item_asset_group item_asset_group in AssetGroupList)
            {
                DebeHaber.FixedAssetGroup FixedAssetGroup = new DebeHaber.FixedAssetGroup();
                FixedAssetGroup.Name = item_asset_group.name;
                FixedAssetGroup.LifespanYears = (decimal)item_asset_group.depreciation_rate;

                //Loop through
                foreach (item_asset item_asset in item_asset_group.item_asset.Where(x => x.IsSelected))
                {
                    DebeHaber.FixedAsset FixedAsset = new DebeHaber.FixedAsset();
                    FixedAsset.Name = item_asset.item.name;
                    FixedAsset.Code = item_asset.item.code;
                    FixedAsset.CurrentCost = (decimal)item_asset.current_value;
                    FixedAsset.PurchaseCost = (decimal)item_asset.purchase_value;
                    FixedAsset.PurchaseDate = (DateTime)item_asset.purchase_date;
                    FixedAsset.Quantity = 1;
                    FixedAsset.CurrencyName = CurrentSession.Currency_Default.name;

                    item_asset.IsSelected = false;
                    FixedAssetGroup.FixedAssets.Add(FixedAsset);
                }

                Transaction.FixedAssetGroups.Add(FixedAssetGroup);
            }
        }

        #region CheckBox Check/UnCheck Methods

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sales_invoiceViewSource.View != null)
            {
                foreach (sales_invoice sales_invoice in sales_invoiceViewSource.View.OfType<sales_invoice>().ToList())
                {
                    sales_invoice.IsSelected = true;
                }
                sales_invoiceViewSource.View.Refresh();
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sales_invoiceViewSource.View != null)
            {
                foreach (sales_invoice sales_invoice in sales_invoiceViewSource.View.OfType<sales_invoice>().ToList())
                {
                    sales_invoice.IsSelected = false;
                }
                sales_invoiceViewSource.View.Refresh();
            }
        }

        private void SalesReturn_Checked(object sender, RoutedEventArgs e)
        {
            if (sales_returnViewSource.View != null)
            {
                foreach (sales_return sales_return in sales_returnViewSource.View.OfType<sales_return>().ToList())
                {
                    sales_return.IsSelected = true;
                }
                sales_returnViewSource.View.Refresh();
            }
        }

        private void SalesReturn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sales_returnViewSource.View != null)
            {
                foreach (sales_return sales_return in sales_returnViewSource.View.OfType<sales_return>().ToList())
                {
                    sales_return.IsSelected = false;
                }
                sales_returnViewSource.View.Refresh();
            }
        }

        private void Purchase_Checked(object sender, RoutedEventArgs e)
        {
            if (purchase_invoiceViewSource.View != null)
            {
                foreach (purchase_invoice purchase_invoice in purchase_invoiceViewSource.View.OfType<purchase_invoice>().ToList())
                {
                    purchase_invoice.IsSelected = true;
                }
                purchase_invoiceViewSource.View.Refresh();
            }
        }

        private void Purchase_UnChecked(object sender, RoutedEventArgs e)
        {
            if (purchase_invoiceViewSource.View != null)
            {
                foreach (purchase_invoice purchase_invoice in purchase_invoiceViewSource.View.OfType<purchase_invoice>().ToList())
                {
                    purchase_invoice.IsSelected = false;
                }
                purchase_invoiceViewSource.View.Refresh();
            }
        }

        private void PurchaseRetuen_Checked(object sender, RoutedEventArgs e)
        {
            if (purchase_returnViewSource.View != null)
            {
                foreach (purchase_return purchase_return in purchase_returnViewSource.View.OfType<purchase_return>().ToList())
                {
                    purchase_return.IsSelected = true;
                }
                purchase_returnViewSource.View.Refresh();
            }
        }

        private void PurchaseReturn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (purchase_returnViewSource.View != null)
            {
                foreach (purchase_return purchase_return in purchase_returnViewSource.View.OfType<purchase_return>().ToList())
                {
                    purchase_return.IsSelected = false;
                }
                purchase_returnViewSource.View.Refresh();
            }
        }

        private void Payment_Checked(object sender, RoutedEventArgs e)
        {
            if (payment_detailViewSource.View != null)
            {
                foreach (payment_detail payment_detail in payment_detailViewSource.View.OfType<payment_detail>().ToList())
                {
                    payment_detail.IsSelected = true;
                }
                payment_detailViewSource.View.Refresh();
            }
        }

        private void Payment_UnChecked(object sender, RoutedEventArgs e)
        {
            if (payment_detailViewSource.View != null)
            {
                foreach (payment_detail payment_detail in payment_detailViewSource.View.OfType<payment_detail>().ToList())
                {
                    payment_detail.IsSelected = false;
                }
                payment_detailViewSource.View.Refresh();
            }
        }

        #endregion CheckBox Check/UnCheck Methods

        private void Send2API(object Json)
        {
            var webAddr = Settings.Default.DebeHaberConnString + "/api/transactionsV2";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(Json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                if (result.ToString().Contains("Error"))
                {
                    MessageBox.Show(result.ToString());
                    Class.ErrorLog.DebeHaber(Json.ToString());
                }
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DebeHaberSync() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support

        private void tbxServer_LostFocus(object sender, RoutedEventArgs e)
        {
            Settings SalesSettings = new Settings();

            SalesSettings.DebeHaberConnString = tbxServer.Content as string;
            Settings.Default.Save();
            SalesSettings = Settings.Default;
        }

        private void btnSalesData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_SalesInvoice();

        }

        private void btnSalesReturnData_Refresh(object sender, RoutedEventArgs e)
        {
         
            Get_SalesReturn();
           
        }

        private void btnPurchaseData_Refresh(object sender, RoutedEventArgs e)
        {
         
            Get_PurchaseInvoice();
           
        }

        private void btnPurchaseReturnData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_PurchaseReturnInvoice();
        }

        private void btnPaymentData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_Payment();
        }

        private void btnProductionData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_ProductionExecution();
        }
    }
}