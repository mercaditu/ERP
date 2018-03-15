using cntrl;
using Cognitivo.Menu;
using Cognitivo.Properties;
using entity;
using entity.BrilloQuery;
using System;
using System.Collections.Generic;
using System.Data;
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
    public partial class DebeHaberSyncLatest : Page, IDisposable
    {
        DataTable salesdt;
        DataTable salesdetaildt;
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

        public DebeHaberSyncLatest()
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

        public void Get_SalesInvoice()
        {
            string query = @" 
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select
	   
       sales_invoice.id_sales_invoice ,contacts.gov_code as customerTaxID,contacts.name as customerName,app_company.name as supplierName,app_company.gov_code as supplierTaxID,
        app_currency.code as currencyCode,app_contract_detail.interval as paymentCondition,Date(sales_invoice.trans_date) as date,sales_invoice.number as number
        ,sales_invoice.comment as comment
											
												
												from sales_invoice 
                                                inner join app_company on sales_invoice.id_company = app_company.id_company
												inner join contacts on sales_invoice.id_contact = contacts.id_contact
												left join app_terminal on sales_invoice.id_terminal = app_terminal.id_terminal
																inner join app_branch on app_branch.id_branch=sales_invoice.id_branch
																inner join app_currencyfx on app_currencyfx.id_currencyfx=sales_invoice.id_currencyfx
																inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency
																inner join app_contract on app_contract.id_contract=sales_invoice.id_contract
                                                                left join app_contract_detail on app_contract_detail.id_contract=app_contract.id_contract
																inner join app_condition on app_condition.id_condition=sales_invoice.id_condition
														 	  where sales_invoice.id_company = @CompanyID
                                              order by sales_invoice.trans_date";
            query = query.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            salesdt = QueryExecutor.DT(query);

            sales_invoiceViewSource.Source = salesdt;

            string querydetail = @" 
   
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select
	  
sales_invoice_detail.id_sales_invoice_detail as id,items.name as chart,sales_invoice_detail.id_sales_invoice,
												
												sales_invoice_detail.unit_cost as UnitCost,vatco.vat as vat,
												sales_invoice_detail.unit_price as UnitPrice,
												round(( (sales_invoice_detail.unit_price) * vatco.coef),4) as UnitPriceVat,
												round((sales_invoice_detail.quantity * sales_invoice_detail.unit_price),4) as SubTotal,
												round((sales_invoice_detail.quantity * sales_invoice_detail.unit_price * vatco.coef),4) as value,
												round(sales_invoice_detail.discount, 4) as Discount,
												round((sales_invoice_detail.quantity * (sales_invoice_detail.discount * vatco.coef)),4) as DiscountVat,
												(sales_invoice_detail.quantity * sales_invoice_detail.unit_cost) as SubTotalCost
                                             
                                                
												from sales_invoice_detail
												inner join sales_invoice on sales_invoice_detail.id_sales_invoice=sales_invoice.id_sales_invoice
												left join sales_rep on sales_invoice.id_sales_rep = sales_rep.id_sales_rep
												inner join contacts on sales_invoice.id_contact = contacts.id_contact
												left join app_geography on app_geography.id_geography=contacts.id_geography
												inner join items on sales_invoice_detail.id_item = items.id_item
												left join app_terminal on sales_invoice.id_terminal = app_terminal.id_terminal
													 LEFT OUTER JOIN
															 (SELECT app_vat_group.id_vat_group, SUM(app_vat.coefficient * app_vat_group_details.percentage) + 1 AS coef, app_vat_group.name as VAT
																FROM  app_vat_group
																	LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group
																	LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
																GROUP BY app_vat_group.id_vat_group)

																vatco ON vatco.id_vat_group = sales_invoice_detail.id_vat_group
																inner join app_branch on app_branch.id_branch=sales_invoice.id_branch
																inner join app_currencyfx on app_currencyfx.id_currencyfx=sales_invoice.id_currencyfx
																inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency
																inner join app_contract on app_contract.id_contract=sales_invoice.id_contract
																inner join app_condition on app_condition.id_condition=sales_invoice.id_condition
																left join projects on projects.id_project=sales_invoice.id_project
											  where sales_invoice.id_company = @CompanyID
                                              order by sales_invoice.trans_date";
            querydetail = querydetail.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            salesdetaildt = QueryExecutor.DT(querydetail);

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

        private void btnData_Sync(object sender, RoutedEventArgs e)
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
            DebeHaber.SyncLatest.Integration Integration = new DebeHaber.SyncLatest.Integration()
            {
                Key = RelationshipHash,
                GovCode = GovCode
            };

            DebeHaber.SyncLatest.Transaction Transaction = new DebeHaber.SyncLatest.Transaction();
            DebeHaber.SyncLatest.Commercial_Invoice Sales = new DebeHaber.SyncLatest.Commercial_Invoice();

            //Loop through
            foreach (DataRow sales_invoice in salesdt.Rows)
            {
                

                //Loads Data from Sales
                Sales.Fill_BySales(sales_invoice);

            
                if (salesdetaildt.Select("id_sales_invoice=" + sales_invoice["id_sales_invoice"].ToString()).Count()>0)
                {
                    DataTable dtdetail = salesdetaildt.Select("id_sales_invoice=" + sales_invoice["id_sales_invoice"].ToString()).CopyToDataTable();
                    foreach (DataRow Detail in dtdetail.Rows)
                    {
                        DebeHaber.SyncLatest.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.SyncLatest.CommercialInvoice_Detail();
                        //Fill and Detail SalesDetail
                        CommercialInvoice_Detail.Fill_BySales(Detail);
                        Sales.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                    }

                }

                //Loop through payments made.


                Transaction.Commercial_Invoices.Add(Sales);
                Integration.Transactions.Add(Transaction);

               
            }
            try
            {
                var Sales_Json = new JavaScriptSerializer().Serialize(Integration);

                var obj = Send2API(Sales_Json);
                DebeHaber.SyncLatest.Web_Data[] sales_json = new JavaScriptSerializer().Deserialize<DebeHaber.SyncLatest.Web_Data[]>(obj.ToString());
                using (db db = new db())
                {
                    foreach (DebeHaber.SyncLatest.Web_Data data in sales_json)
                    {

                        sales_invoice sales = db.sales_invoice.Where(x => x.id_sales_invoice == data.ref_id).FirstOrDefault();
                        sales.cloud_id = data.id;
                    }
                    db.SaveChanges();
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

            }
            finally
            {
                db.db.SaveChanges();
                fill();
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

        private object Send2API(object Json)
        {
            var webAddr = Settings.Default.DebeHaberConnString + "/api/syncData";
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
                return result;
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