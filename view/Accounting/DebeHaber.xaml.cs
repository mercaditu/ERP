using entity;
using entity.API.DebeHaber;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cognitivo.Accounting
{
    /// <summary>
    /// Interaction logic for DebeHaber.xaml
    /// </summary>
    public partial class DebeHaber : Page, INotifyPropertyChanged
    {

        public bool SyncSales { get => _syncSales; set => _syncSales = value; }
        public bool SyncSalesReturn { get => _syncSalesReturn; set => _syncSalesReturn = value; }
        public bool SyncPurchaseReturn { get => _syncPurchaseReturn; set => _syncPurchaseReturn = value; }
        public bool SyncPurchase { get => _syncPurchase; set => _syncPurchase = value; }
        public bool SyncPayments { get => _syncPayment; set => _syncPayment = value; }
        public bool SyncMovements { get => _syncMovement; set => _syncMovement = value; }

        public bool SyncImpex { get => _syncImpex; set => _syncImpex = value; }
        public bool SyncFixedAsset { get => _syncFixedAsset; set => _syncFixedAsset = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        //   public bool isReady { get; set; }
        public bool serverStatus { get; set; }
        public bool apiStatus { get; set; }

        public dbContext Context { get; set; }



        int countImpex = 0;
        int countPurchases = 0;
        int countPurchaseReturns = 0;
        int countSales = 0;
        int countSalesReturns = 0;
        int countAccounts = 0;
        int countTransfers = 0;
        int countFixedAssets = 0;
        private bool _syncPurchase = true;
        private bool _syncSales = true;
        private bool _syncSalesReturn = true;
        private bool _syncPurchaseReturn = true;
        private bool _syncPayment = true;
        private bool _syncMovement = true;
        private bool _syncImpex = true;
        private bool _syncFixedAsset = true;

        public DebeHaber()
        {
            InitializeComponent();
            Context = new dbContext();
            btnStart.IsEnabled = false;
            CheckStatus(null, null);
            endDate.SelectedDate = DateTime.Now;
            startDate.SelectedDate = DateTime.Now.AddDays(-30);
            //LoadData(null, null);

        }

        private void CheckStatus(object sender, MouseButtonEventArgs e)
        {
            //TODO, Check if access to server is ok. Make sure to use the URL on the config file.
            serverStatus = true;

            //TODO, Check if API Key is active (not expired). Make sure to use the URL on the config file.
            apiStatus = true;
            string key = tbxAPI.Text;
            // var obj = Send2API(null, tbxURL.Text + "/api/check-key", key);

            //If both is Ok, then we are ready to Export.
            if (serverStatus && apiStatus)
            {
                btnStart.IsEnabled = true;
                popConnBuilder.IsOpen = false;
            }
        }

        private void OpenConfig(object sender, MouseButtonEventArgs e) => popConnBuilder.IsOpen = true;

        #region Load Data

        private void LoadData(object sender, MouseButtonEventArgs e)
        {
            progSales.IsIndeterminate = true;
            progSalesReturn.IsIndeterminate = true;
            progPurchase.IsIndeterminate = true;
            progPurchaseReturn.IsIndeterminate = true;

            progAccounts.IsIndeterminate = true;

            //This is a little more code, but will allow all to be loaded at the same time for a quicker startup.
            Task g = Task.Factory.StartNew(() => Load());
            g.Wait();
            Task a = Task.Factory.StartNew(() => LoadSales());
            a.Wait();
            Task b = Task.Factory.StartNew(() => LoadSalesReturn());
            b.Wait();
            Task c = Task.Factory.StartNew(() => LoadPurchases());
            c.Wait();
            Task d = Task.Factory.StartNew(() => LoadPurchaseReturns());
            d.Wait();
            Task j = Task.Factory.StartNew(() => LoadAccountsForMovement());
            j.Wait();
            Task f = Task.Factory.StartNew(() => LoadAccountsForPayments());
            f.Wait();
            Task h = Task.Factory.StartNew(() => LoadProductions());
            h.Wait();
            //Task i = Task.Factory.StartNew(() => LoadAssets());
            //i.Wait();
            Task k = Task.Factory.StartNew(() => LoadImport());
        }

        //Load Basic Data to avoid N+1 Query Problems
        private void Load()
        {
            //Load Items
            Context.db.items.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            //Load Customers + Suppliers
            Context.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            //Load Contracts
            Context.db.app_contract.Where(x => x.id_company == CurrentSession.Id_Company).Include(x => x.app_contract_detail).LoadAsync();
        }

        private void LoadSales()
        {
            countSales = Context.db.sales_invoice.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
                && x.status == Status.Documents_General.Approved).Count();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                progSales.IsIndeterminate = false;
                progSales.Maximum = countSales;
                salesMaximum.Text = progSales.Maximum.ToString();
            }));
        }

        private void LoadSalesReturn()
        {
            countSalesReturns = Context.db.sales_return.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
                && x.status == Status.Documents_General.Approved).Count();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                progSalesReturn.IsIndeterminate = false;
                progSalesReturn.Maximum = countSalesReturns;
                salesReturnMaximum.Text = progSalesReturn.Maximum.ToString();
            }));
        }
        private void LoadImport()
        {
            countPurchases = Context.db.purchase_invoice.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false && x.is_impex == true
            && x.status == Status.Documents_General.Approved).Count();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                progPurchase.IsIndeterminate = false;
                progPurchase.Maximum = countPurchases;
                purchaseMaximum.Text = progPurchase.Maximum.ToString();
            }));
        }

        private void LoadPurchases()
        {
            countPurchases = Context.db.purchase_invoice.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
            && x.status == Status.Documents_General.Approved).Count();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                progPurchase.IsIndeterminate = false;
                progPurchase.Maximum = countPurchases;
                purchaseMaximum.Text = progPurchase.Maximum.ToString();
            }));
        }

        private void LoadPurchaseReturns()
        {
            countPurchaseReturns = Context.db.purchase_return.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
                && x.status == Status.Documents_General.Approved).Count();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                progPurchaseReturn.IsIndeterminate = false;
                progPurchaseReturn.Maximum = countPurchaseReturns;
                purchaseReturnMaximum.Text = progPurchaseReturn.Maximum.ToString();
            }));
        }

        private void LoadAccountsForPayments()
        {
            countAccounts = Context.db.payment_detail.Where(x => x.id_company == CurrentSession.Id_Company &&
           x.payment.is_accounted == false && x.payment.status == Status.Documents_General.Approved)
               .Count();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                progAccounts.IsIndeterminate = false;
                progAccounts.Maximum = countAccounts;
                paymentMaximum.Text = progAccounts.Maximum.ToString();
            }));
        }

        private void LoadAccountsForMovement()
        {
            countTransfers = Context.db.app_account_detail.Where(x => x.id_company == CurrentSession.Id_Company &&
          x.tran_type == app_account_detail.tran_types.Transaction &&
          x.is_accounted == false && x.id_payment_detail == null &&
          x.status == Status.Documents_General.Approved).Count();


            Dispatcher.BeginInvoke((Action)(() =>
            {
                progTransfer.IsIndeterminate = false;
                progTransfer.Maximum = countTransfers;
                transferMaximum.Text = progTransfer.Maximum.ToString();
            }));

        }

        private void LoadProductions()
        {
            //list<production_execution_detail> production_execution_detaillist = context.db.production_execution_detail
            //    .where(x => x.id_company == currentsession.id_company && x.is_accounted == false)
            //    .include(x => x.production_order_detail)
            //    .include(x => x.production_order_detail.production_order)
            //    .take(500)
            //    .tolist();

            //dispatcher.begininvoke((action)(() =>
            //{
            //    progproduction.isindeterminate = false;
            //    progproduction.maximum = context.db.production_execution_detail.local.count();
            //    productionmaximum.text = progproduction.maximum.tostring();
            //}));
        }

        //private void LoadAssets()
        //{
        //    ItemAssetList = Context.db.item_asset.Where(x => x.id_company == CurrentSession.Id_Company)
        //        .Include(x => x.item)
        //        .Include(x => x.app_currency)
        //        .Include(x => x.app_company)
        //        .Include(x => x.item_asset_group)
        //        .ToList();

        //    Dispatcher.BeginInvoke((Action)(() =>
        //    {
        //        progAsset.IsIndeterminate = false;
        //        progAsset.Maximum = Context.db.item_asset.Local.Count();
        //        assetMaximum.Text = progAsset.Maximum.ToString();
        //    }));
        //}
        #endregion

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //Start();
            btnStart.IsEnabled = false;
            string url = tbxURL.Text;
            string key = tbxAPI.Text;
            DateTime startdate = startDate.SelectedDate.Value;
            DateTime enddate = endDate.SelectedDate.Value;
            // int dateValue = cmbdays.SelectedIndex;
            Task basic_Task = Task.Factory.StartNew(() => Start(url, key, startdate, enddate));
        }

        private void Start(string url, string key, DateTime startdate, DateTime enddate)
        {

            app_company app_company = Context.db.app_company.Find(CurrentSession.Id_Company);
            if (SyncFixedAsset)
            {

                ////problem with migration of ref_id Column in item_asset_group
                Task FixedAssetTask = Task.Factory.StartNew(() => FixedAsset(url, key, app_company));

            }

            if (SyncMovements)
            {

                Task AccountsForMovement_Task = Task.Factory.StartNew(() => AccountsForMovement(url, key, app_company));

            }

            SalesData(url, key, app_company, startdate, enddate);
            PurchaseData(url, key, app_company, startdate, enddate);

            if (SyncPayments)
            {
                
                    Task AccountsForPayments_Task = Task.Factory.StartNew(() => AccountsForPayments(url, key, app_company, startdate, enddate));
                    AccountsForPayments_Task.Wait();
                
            }
            Dispatcher.BeginInvoke((Action)(() => btnStart.IsEnabled = true));
        }

        private void SalesData(string url, string key, app_company app_company, DateTime startdate, DateTime enddate)
        {
            if (SyncSales)
            {

                Task Sales_Task = Task.Factory.StartNew(() => Sales(url, key, app_company, startdate, enddate));
                Sales_Task.Wait();

            }

            if (SyncSalesReturn)
            {

                Task SalesReturn_Task = Task.Factory.StartNew(() => SalesReturns(url, key, app_company, startdate, enddate));

            }
        }

        private void PurchaseData(string url, string key, app_company app_company, DateTime startdate, DateTime enddate)
        {

            if (SyncPurchase)
            {
              
                    Task Purchase_Task = Task.Factory.StartNew(() => Purchases( url, key, app_company, startdate, enddate));
                    Purchase_Task.Wait();
                
            }

            if (SyncPurchaseReturn)
            {
               
                    Task PurchaseReturn_Task = Task.Factory.StartNew(() => PurchaseReturns( url, key, app_company, startdate, enddate));
                
            }

            if (SyncImpex)
            {
             
                    Task impex_Task = Task.Factory.StartNew(() => Impex( url, key, app_company, startdate, enddate));
                
            }

        }

        private void Impex( string url, string key, app_company app_company, DateTime startdate, DateTime enddate)
        {
            using (db db = new db())
            {
                List<entity.API.DebeHaber.Impex> ImpexList = new List<entity.API.DebeHaber.Impex>();
                int value = 0;
                int maincounter = 0;
                int apicounter = 0;
                int totalrecord = db.impex.Where(x => x.id_company == CurrentSession.Id_Company).Count();

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    importValue.Text = value.ToString();
                    progImport.IsIndeterminate = false;
                    progImport.Value = value;
                    progImport.Maximum = totalrecord;
                    importMaximum.Text = progImport.Maximum.ToString();
                }));


                for (maincounter = 0; maincounter < totalrecord; maincounter = maincounter + 1000)
                {

                    List<impex> impexMainList = db.impex.Where(x => x.id_company == CurrentSession.Id_Company)
                    .Include(x => x.impex_import).Include(x => x.impex_expense).OrderBy(x => x.id_impex).Skip(maincounter).Take(1000).ToList();

                    List<impex> impexAPIList;
                    for (apicounter = 0; apicounter < 1000; apicounter = apicounter + 100)
                    {

                        impexAPIList = impexMainList.Skip(apicounter).Take(100).ToList();

                        ImpexList.Clear();
                        foreach (impex impex in impexAPIList)
                        {
                            try
                            {
                                entity.API.DebeHaber.Impex Impex = new entity.API.DebeHaber.Impex();
                                Impex.loadImpex(impex, app_company);
                                ImpexList.Add(Impex);
                                value += 1;
                                Dispatcher.BeginInvoke((Action)(() => progImport.Value = value));
                                Dispatcher.BeginInvoke((Action)(() => importValue.Text = value.ToString()));
                            }
                            catch (Exception)
                            {

                                //throw;
                            }

                        }

                        Dispatcher.BeginInvoke((Action)(() => progImport.IsIndeterminate = true));

                        var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(ImpexList);
                        HttpWebResponse httpResponse = Send2API(Json, url + "/api/import", key);
                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();
                                List<entity.API.DebeHaber.Impex> ReturnJsonList = new JavaScriptSerializer().Deserialize<List<entity.API.DebeHaber.Impex>>(result);
                                foreach (entity.API.DebeHaber.Impex ReturnImpex in ReturnJsonList)
                                {
                                    foreach (entity.API.DebeHaber.Invoice ReturnJson in ReturnImpex.Invoices)
                                    {
                                        if (ReturnJson.Message == "Success")
                                        {
                                            purchase_invoice purchase_invoice = db.purchase_invoice.Where(x => x.id_purchase_invoice == ReturnJson.local_id).FirstOrDefault();
                                            if (purchase_invoice != null)
                                            {
                                                purchase_invoice.is_accounted = true;
                                                // purchase_invoice.cloud_id = ReturnJson.cloud_id;
                                            }
                                        }
                                    }
                                }
                            }
                            db.SaveChanges();
                        }

                        Dispatcher.BeginInvoke((Action)(() => progImport.IsIndeterminate = false));

                    }
                }

            }




        }

        private void Sales(string url, string key, app_company app_company, DateTime startdate, DateTime enddate)
        {
            using (db db = new db())
            {
                List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
                int value = 0;
                int maincounter = 0;
                int apicounter = 0;
                int totalrecord = db.sales_invoice.
                          Where(x => x.id_company == CurrentSession.Id_Company
                          && x.status == Status.Documents_General.Approved && (x.trans_date >= startdate && x.trans_date <= enddate)).Count();

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    salesValue.Text = value.ToString();
                    progSales.IsIndeterminate = false;
                    progSales.Value = value;
                    progSales.Maximum = totalrecord;
                    salesMaximum.Text = progSales.Maximum.ToString();
                }));


                for (maincounter = 0; maincounter < totalrecord; maincounter = maincounter + 1000)
                {
                    List<sales_invoice> salesMainList = db.sales_invoice.
                          Where(x => x.id_company == CurrentSession.Id_Company
                          && x.status == Status.Documents_General.Approved && (x.trans_date >= startdate && x.trans_date <= enddate))
                            .Include(x => x.sales_invoice_detail)
                            .Include(x => x.app_currencyfx)
                            .OrderBy(x => x.timestamp)
                            .Skip(maincounter).Take(1000)
                            .ToList();

                    List<sales_invoice> salesAPIList;
                    for (apicounter = 0; apicounter < 1000; apicounter = apicounter + 100)
                    {
                        salesAPIList = salesMainList.Skip(apicounter).Take(100).ToList();


                        InvoiceList.Clear();
                        foreach (sales_invoice sales_invoice in salesAPIList)
                        {
                            try
                            {
                                entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                                Invoice.LoadSales(sales_invoice, app_company);
                                InvoiceList.Add(Invoice);

                                value += 1;
                                Dispatcher.BeginInvoke((Action)(() => progSales.Value = value));
                                Dispatcher.BeginInvoke((Action)(() => salesValue.Text = value.ToString()));
                            }
                            catch (Exception)
                            {

                                //throw;
                            }

                        }

                        Dispatcher.BeginInvoke((Action)(() => progSales.IsIndeterminate = true));

                        var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(InvoiceList);
                        HttpWebResponse httpResponse = Send2API(Json, url + "/api/transactions", key);
                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();
                                List<entity.API.DebeHaber.Invoice> ReturnJsonList = new JavaScriptSerializer().Deserialize<List<entity.API.DebeHaber.Invoice>>(result);
                                foreach (entity.API.DebeHaber.Invoice ReturnJson in ReturnJsonList)
                                {
                                    if (ReturnJson.Message == "Success")
                                    {
                                        sales_invoice sales_invoice = salesAPIList.Where(x => x.id_sales_invoice == ReturnJson.local_id).FirstOrDefault();
                                        if (sales_invoice != null)
                                        {
                                            sales_invoice.is_accounted = true;
                                            // sales_invoice.cloud_id = ReturnJson.cloud_id;
                                        }
                                    }


                                }
                            }

                            db.SaveChanges();
                        }

                        Dispatcher.BeginInvoke((Action)(() => progSales.IsIndeterminate = false));
                    }
                }
            }
        }

        private void SalesReturns(string url, string key, app_company app_company, DateTime startdate, DateTime enddate)
        {
            using (db db = new db())
            {


                List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
                int value = 0;
                int maincounter = 0;
                int apicounter = 0;
                int totalrecord = db.sales_return.
                  Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == true
                  && x.status == Status.Documents_General.Approved
                  && (x.trans_date >= startdate && x.trans_date <= enddate)).Count();

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    salesReturnValue.Text = value.ToString();
                    progSalesReturn.IsIndeterminate = false;
                    progSalesReturn.Value = value;
                    progSalesReturn.Maximum = totalrecord;
                    salesReturnMaximum.Text = progSalesReturn.Maximum.ToString();
                }));


                for (maincounter = 0; maincounter < totalrecord; maincounter = maincounter + 1000)
                {


                    List<sales_return> salesReturnMainList = db.sales_return.
                      Where(x => x.id_company == CurrentSession.Id_Company
                      && x.status == Status.Documents_General.Approved
                      && (x.trans_date >= startdate && x.trans_date <= enddate))
                      .Include(x => x.sales_return_detail)
                      .Include(x => x.app_currencyfx)
                        .Include(x => x.app_company)
                        .OrderBy(x => x.timestamp)
                         .Skip(maincounter).Take(1000)
                      .ToList();

                    List<sales_return> salesReturnAPIList;
                    for (apicounter = 0; apicounter < 1000; apicounter = apicounter + 100)
                    {
                        salesReturnAPIList = salesReturnMainList.Skip(apicounter).Take(100).ToList();


                        InvoiceList.Clear();
                        foreach (sales_return sales_return in salesReturnAPIList)
                        {
                            try
                            {
                                entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                                Invoice.LoadSalesReturn(sales_return, app_company);
                                InvoiceList.Add(Invoice);
                                value += 1;
                                Dispatcher.BeginInvoke((Action)(() => progSalesReturn.Value = value));
                                Dispatcher.BeginInvoke((Action)(() => salesReturnValue.Text = value.ToString()));
                            }
                            catch (Exception)
                            {

                                //throw;
                            }

                        }

                        Dispatcher.BeginInvoke((Action)(() => progSalesReturn.IsIndeterminate = true));

                        var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(InvoiceList);
                        HttpWebResponse httpResponse = Send2API(Json, url + "/api/transactions", key);
                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {

                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();
                                List<entity.API.DebeHaber.Invoice> ReturnJsonList = new JavaScriptSerializer().Deserialize<List<entity.API.DebeHaber.Invoice>>(result);
                                foreach (entity.API.DebeHaber.Invoice ReturnJson in ReturnJsonList)
                                {
                                    if (ReturnJson.Message == "Success")
                                    {
                                        sales_return sales_return = salesReturnAPIList.Where(x => x.id_sales_return == ReturnJson.local_id).FirstOrDefault();
                                        if (sales_return != null)
                                        {
                                            sales_return.is_accounted = true;
                                            //sales_return.cloud_id = ReturnJson.cloud_id;
                                        }
                                    }
                                }
                            }
                            db.SaveChanges();
                        }

                        Dispatcher.BeginInvoke((Action)(() => progSalesReturn.IsIndeterminate = false));

                    }
                }
            }
        }

        private void Purchases(string url, string key, app_company app_company, DateTime startdate, DateTime enddate)
        {
            using (db db = new db())
            {

                List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
                int value = 0;
                int maincounter = 0;
                int apicounter = 0;
                int totalrecord = db.purchase_invoice.
                   Where(x => x.id_company == CurrentSession.Id_Company && x.is_impex == false
                        && x.status == Status.Documents_General.Approved
                          && (x.trans_date >= startdate && x.trans_date <= enddate)).Count();

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    purchaseValue.Text = value.ToString();
                    progPurchase.IsIndeterminate = false;
                    progPurchase.Value = value;
                    progPurchase.Maximum = totalrecord;
                    purchaseMaximum.Text = progPurchase.Maximum.ToString();
                }));


                for (maincounter = 0; maincounter < totalrecord; maincounter = maincounter + 1000)
                {

                    List<purchase_invoice> purchaseMainList = db.purchase_invoice.
                  Where(x => x.id_company == CurrentSession.Id_Company && x.is_impex == false
                        && x.status == Status.Documents_General.Approved
                          && (x.trans_date >= startdate && x.trans_date <= enddate))
                        .Include(x => x.contact)
                        .Include(x => x.purchase_invoice_detail)
                        .Include(x => x.app_currencyfx)
                        .Include(x => x.app_company)
                        .OrderBy(x => x.timestamp)
                          .Skip(maincounter).Take(1000)
                        .ToList();

                    List<purchase_invoice> purchaseAPIList;
                    for (apicounter = 0; apicounter < 1000; apicounter = apicounter + 100)
                    {
                        purchaseAPIList = purchaseMainList.Skip(apicounter).Take(100).ToList();


                        InvoiceList.Clear();
                        foreach (purchase_invoice purchase_invoice in purchaseAPIList)
                        {
                            try
                            {
                                entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                                Invoice.LoadPurchase(purchase_invoice, app_company);
                                InvoiceList.Add(Invoice);
                                value += 1;
                                Dispatcher.BeginInvoke((Action)(() => progPurchase.Value = value));
                                Dispatcher.BeginInvoke((Action)(() => purchaseValue.Text = value.ToString()));
                            }
                            catch (Exception)
                            {

                                // throw;
                            }

                        }

                        Dispatcher.BeginInvoke((Action)(() => progPurchase.IsIndeterminate = true));

                        var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(InvoiceList);
                        HttpWebResponse httpResponse = Send2API(Json, url + "/api/transactions", key);
                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();
                                List<entity.API.DebeHaber.Invoice> ReturnJsonList = new JavaScriptSerializer().Deserialize<List<entity.API.DebeHaber.Invoice>>(result);
                                foreach (entity.API.DebeHaber.Invoice ReturnJson in ReturnJsonList)
                                {
                                    if (ReturnJson.Message == "Success")
                                    {
                                        purchase_invoice purchase_invoice = purchaseAPIList.Where(x => x.id_purchase_invoice == ReturnJson.local_id).FirstOrDefault();
                                        if (purchase_invoice != null)
                                        {
                                            purchase_invoice.is_accounted = true;
                                            // purchase_invoice.cloud_id = ReturnJson.cloud_id;
                                        }
                                    }
                                }
                            }
                            db.SaveChanges();
                        }

                        Dispatcher.BeginInvoke((Action)(() => progPurchase.IsIndeterminate = false));

                    }
                }
            }
        }

        private void PurchaseReturns( string url, string key, app_company app_company, DateTime startdate, DateTime enddate)
        {
            using (db db = new db())
            {


                List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
                int value = 0;
                int maincounter = 0;
                int apicounter = 0;
                int totalrecord = db.purchase_return.
                   Where(x => x.id_company == CurrentSession.Id_Company && x.is_impex == false
                        && x.status == Status.Documents_General.Approved
                          && (x.trans_date >= startdate && x.trans_date <= enddate)).Count();

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    purchaseReturnValue.Text = value.ToString();
                    progPurchaseReturn.IsIndeterminate = false;
                    progPurchaseReturn.Value = value;
                    progPurchaseReturn.Maximum = totalrecord;
                    purchaseReturnMaximum.Text = progPurchaseReturn.Maximum.ToString();
                }));


                for (maincounter = 0; maincounter < totalrecord; maincounter = maincounter + 1000)
                {

                    List<purchase_return> purchaseReturnMainList =
                         db.purchase_return.
                    Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == true
                    && x.status == Status.Documents_General.Approved
                     && (x.trans_date >= startdate && x.trans_date <= enddate))
                    .Include(x => x.contact)
                    .Include(x => x.purchase_return_detail)
                    .Include(x => x.app_currencyfx)
                    .Include(x => x.app_company)
                    .OrderBy(x => x.timestamp)
                     .Skip(maincounter).Take(1000)
                    .ToList();



                    List<purchase_return> purchaseReturnAPIList;
                    for (apicounter = 0; apicounter < 1000; apicounter = apicounter + 100)
                    {
                        purchaseReturnAPIList = purchaseReturnMainList.Skip(apicounter).Take(100).ToList();

                        InvoiceList.Clear();
                        foreach (purchase_return purchase_return in purchaseReturnAPIList)
                        {
                            try
                            {
                                entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                                Invoice.LoadPurchaseReturn(purchase_return, app_company);
                                InvoiceList.Add(Invoice);

                                value += 1;
                                Dispatcher.BeginInvoke((Action)(() => progPurchaseReturn.Value = value));
                                Dispatcher.BeginInvoke((Action)(() => purchaseReturnValue.Text = value.ToString()));
                            }
                            catch (Exception)
                            {

                                //throw;
                            }

                        }

                        Dispatcher.BeginInvoke((Action)(() => progPurchaseReturn.IsIndeterminate = true));

                        var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(InvoiceList);
                        HttpWebResponse httpResponse = Send2API(Json, url + "/api/transactions", key);
                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();
                                List<entity.API.DebeHaber.Invoice> ReturnJsonList = new JavaScriptSerializer().Deserialize<List<entity.API.DebeHaber.Invoice>>(result);
                                foreach (entity.API.DebeHaber.Invoice ReturnJson in ReturnJsonList)
                                {
                                    if (ReturnJson.Message == "Success")
                                    {
                                        purchase_return purchase_return = purchaseReturnAPIList.Where(x => x.id_purchase_return == ReturnJson.local_id).FirstOrDefault();
                                        if (purchase_return != null)
                                        {
                                            purchase_return.is_accounted = true;
                                            //purchase_return.cloud_id = ReturnJson.cloud_id;
                                        }
                                    }
                                }
                            }
                            db.SaveChanges();
                        }

                        Dispatcher.BeginInvoke((Action)(() => progPurchaseReturn.IsIndeterminate = false));

                    }
                }
            }
        }

        private void AccountsForMovement(string url, string key, app_company app_company)
        {
            using (db db = new db())
            {

                List<entity.API.DebeHaber.AccountMovements> InvoiceList = new List<entity.API.DebeHaber.AccountMovements>();
                int value = 0;
                int maincounter = 0;
                int apicounter = 0;
                int totalrecord = db.app_account_detail.
                  Where(x => x.id_company == CurrentSession.Id_Company &&
                         x.tran_type == app_account_detail.tran_types.Transaction &&
                         x.is_accounted == false && x.id_payment_detail == null &&
                         x.status == Status.Documents_General.Approved).Count();

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    transferValue.Text = value.ToString();
                    progTransfer.IsIndeterminate = false;
                    progTransfer.Value = value;
                    progTransfer.Maximum = totalrecord;
                    transferMaximum.Text = progTransfer.Maximum.ToString();
                }));


                for (maincounter = 0; maincounter < totalrecord; maincounter = maincounter + 1000)
                {
                    List<app_account_detail> movementMainList = db.app_account_detail.Where(x => x.id_company == CurrentSession.Id_Company &&
                         x.tran_type == app_account_detail.tran_types.Transaction &&
                         x.is_accounted == false && x.id_payment_detail == null &&
                         x.status == Status.Documents_General.Approved)
                             .Include(x => x.app_currencyfx)
                             .Include(x => x.app_account)
                             .OrderBy(x => x.timestamp)
                              .Skip(maincounter).Take(1000)
                             .ToList();

                    List<app_account_detail> movementAPIList;
                    for (apicounter = 0; apicounter < 1000; apicounter = apicounter + 100)
                    {
                        movementAPIList = movementMainList.Skip(apicounter).Take(100).ToList();
                        InvoiceList.Clear();

                        foreach (app_account_detail app_account_detail in movementMainList)
                        {
                            try
                            {
                                entity.API.DebeHaber.AccountMovements AccountMovement = new entity.API.DebeHaber.AccountMovements();
                                AccountMovement.LoadTransfers(app_account_detail, app_company);
                                InvoiceList.Add(AccountMovement);

                                value += 1;

                                Dispatcher.BeginInvoke((Action)(() => progTransfer.Value = value));
                                Dispatcher.BeginInvoke((Action)(() => transferValue.Text = value.ToString()));
                            }
                            catch (Exception)
                            {

                                // throw;
                            }

                        }

                        Dispatcher.BeginInvoke((Action)(() => progTransfer.IsIndeterminate = true));

                        var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(InvoiceList);
                        HttpWebResponse httpResponse = Send2API(Json, url + "/api/movement", key);

                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();
                                List<entity.API.DebeHaber.AccountMovements> ReturnJsonList = new JavaScriptSerializer().Deserialize<List<entity.API.DebeHaber.AccountMovements>>(result);

                                foreach (entity.API.DebeHaber.AccountMovements ReturnJson in ReturnJsonList)
                                {
                                    if (ReturnJson.Message == "Success")
                                    {
                                        app_account_detail app_account_detail = movementMainList.Where(x => x.id_account_detail == ReturnJson.local_id).FirstOrDefault();
                                        if (app_account_detail != null)
                                        {
                                            app_account_detail.is_accounted = true;
                                        }
                                    }
                                }
                            }
                        }

                        Dispatcher.BeginInvoke((Action)(() => progTransfer.IsIndeterminate = false));

                        db.SaveChanges();
                    }
                }
            }
        }

        private void AccountsForPayments(string url, string key, app_company app_company, DateTime startdate, DateTime enddate)
        {
            using (db db = new db())
            {


                List<entity.API.DebeHaber.AccountMovements> InvoiceList = new List<entity.API.DebeHaber.AccountMovements>();
                int value = 0;
                int maincounter = 0;
                int apicounter = 0;
                int totalrecord = db.payment_detail.
                 Where(x => x.id_company == CurrentSession.Id_Company && x.payment.status == Status.Documents_General.Approved
                 && (x.trans_date >= startdate && x.trans_date <= enddate)).Count();

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    paymentValue.Text = value.ToString();
                    progAccounts.IsIndeterminate = false;
                    progAccounts.Value = value;
                    progAccounts.Maximum = totalrecord;
                    paymentMaximum.Text = progAccounts.Maximum.ToString();
                }));


                for (maincounter = 0; maincounter < totalrecord; maincounter = maincounter + 1000)
                {

                    List<payment_detail> paymentMainList =
                        db.payment_detail.Where(x => x.id_company == CurrentSession.Id_Company
                && x.payment.status == Status.Documents_General.Approved
                     && (x.trans_date >= startdate && x.trans_date <= enddate))
                     .Include(x => x.app_currencyfx)
                     .Include(x => x.payment_schedual)
                     .Include(x => x.app_account)
                     .OrderBy(x => x.timestamp)
                       .Skip(maincounter).Take(1000)
                     .ToList();


                    List<payment_detail> paymentAPIList;
                    for (apicounter = 0; apicounter < 1000; apicounter = apicounter + 100)
                    {
                        paymentAPIList = paymentMainList.Skip(apicounter).Take(100).ToList();
                        InvoiceList.Clear();

                        foreach (payment_detail payment_detail in paymentMainList)
                        {
                            foreach (payment_schedual schedual in payment_detail.payment_schedual
                                .AsQueryable().Include(x => x.sales_invoice).Include(x => x.purchase_invoice).ToList())
                            {
                                try
                                {
                                    entity.API.DebeHaber.AccountMovements AccountMovement = new entity.API.DebeHaber.AccountMovements();

                                    AccountMovement.LoadPaymentsRecieved(schedual, app_company);

                                    if (AccountMovement.Credit > 0 || AccountMovement.Debit > 0)
                                    {
                                        InvoiceList.Add(AccountMovement);
                                    }

                                }
                                catch (Exception)
                                {

                                    //throw;
                                }

                            }



                        }
                        value += 100;
                        Dispatcher.BeginInvoke((Action)(() => progAccounts.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => paymentValue.Text = value.ToString()));

                        Dispatcher.BeginInvoke((Action)(() => progAccounts.IsIndeterminate = true));

                        var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(InvoiceList);
                        HttpWebResponse httpResponse = Send2API(Json, url + "/api/payment", key);

                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();
                                List<entity.API.DebeHaber.AccountMovements> ReturnJsonList = new JavaScriptSerializer().Deserialize<List<entity.API.DebeHaber.AccountMovements>>(result);
                                foreach (entity.API.DebeHaber.AccountMovements ReturnJson in ReturnJsonList)
                                {
                                    if (ReturnJson.Message == "Success")
                                    {
                                        payment_detail payment_detail = db.payment_schedual.Where(x => x.id_payment_schedual == ReturnJson.local_id).Include(x => x.payment_detail).FirstOrDefault().payment_detail;
                                        if (payment_detail != null)
                                        {
                                            payment_detail.payment.is_accounted = true;
                                            // payment_detail.payment.cloud_id = ReturnJson.cloud_id;
                                        }
                                    }
                                }
                                db.SaveChanges();
                            }
                        }

                        Dispatcher.BeginInvoke((Action)(() => progAccounts.IsIndeterminate = false));
                    }
                }
            }
        }

        private void FixedAsset(string url, string key, app_company app_company)
        {
            using (db db = new db())
            {

                List<entity.API.DebeHaber.FixedAsset> AssetList = new List<entity.API.DebeHaber.FixedAsset>();
                int value = 0;
                int maincounter = 0;
                int apicounter = 0;
                int totalrecord = db.item_asset.Where(x => x.id_company == CurrentSession.Id_Company).Count();

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    assetValue.Text = value.ToString();
                    progAsset.IsIndeterminate = false;
                    progAsset.Value = value;
                    progAsset.Maximum = totalrecord;
                    assetMaximum.Text = progAsset.Maximum.ToString();
                }));


                for (maincounter = 0; maincounter < totalrecord; maincounter = maincounter + 1000)
                {

                    List<item_asset> AssetMainList =
                      db.item_asset.Where(x => x.id_company == CurrentSession.Id_Company)
                             .Include(x => x.item)
                             .Include(x => x.app_currency)
                             .Include(x => x.app_company)
                             .Include(x => x.item_asset_group)
                     .OrderBy(x => x.timestamp)
                       .Skip(maincounter).Take(1000)
                     .ToList();

                    List<item_asset> assetAPIList;
                    for (apicounter = 0; apicounter < 1000; apicounter = apicounter + 100)
                    {
                        assetAPIList = AssetMainList.Skip(apicounter).Take(100).ToList();

                        foreach (item_asset item_asset in assetAPIList)
                        {
                            try
                            {
                                entity.API.DebeHaber.FixedAsset FixedAsset = new entity.API.DebeHaber.FixedAsset();
                                FixedAsset.LoadAsset(item_asset, app_company);
                                AssetList.Add(FixedAsset);

                                value += 1;
                                Dispatcher.BeginInvoke((Action)(() => progAsset.Value = value));
                                Dispatcher.BeginInvoke((Action)(() => assetValue.Text = value.ToString()));
                                //end try
                            }
                            catch (Exception)
                            {

                                //throw;

                            }


                        }

                        if (AssetList.Count() > 0)
                        {
                            var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(AssetList);
                            HttpWebResponse httpResponse = Send2API(Json, url + "/api/fixedasset", key);
                            if (httpResponse.StatusCode == HttpStatusCode.OK)
                            {
                                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                                {
                                    var result = streamReader.ReadToEnd();
                                    List<ResoponseAssetData> ReturnJson = new JavaScriptSerializer().Deserialize<List<ResoponseAssetData>>(result);
                                    foreach (ResoponseAssetData resp in ReturnJson)
                                    {
                                        if (resp.ref_id > 0)
                                        {
                                            using (db localdb = new db())
                                            {
                                                item_asset item_asset = localdb.item_asset.Where(x => x.id_item_asset == resp.ref_id).FirstOrDefault();
                                                item_asset.purchase_date = Convert.ToDateTime(resp.purchase_date);
                                                item_asset.purchase_value = resp.purchase_value;
                                                item_asset.current_value = resp.current_value;
                                                item_asset.item.name = resp.name;
                                                item_asset.item.code = resp.serial;
                                                item_asset.quantity = Convert.ToInt32(resp.quantity);

                                                //create asset group or update values.
                                                //search asset group by name
                                                item_asset_group item_asset_group = localdb.item_asset_group.Where(x => x.ref_id == resp.chart.id).FirstOrDefault();

                                                if (item_asset_group != null)
                                                {
                                                    item_asset_group.depreciation_rate = resp.chart.asset_years;
                                                }
                                                else
                                                {
                                                    item_asset_group = localdb.item_asset_group.Where(x => x.name == resp.chart.name).FirstOrDefault() ?? new item_asset_group();
                                                    item_asset_group.ref_id = resp.chart.id;
                                                    item_asset_group.depreciation_rate = resp.chart.asset_years;
                                                    item_asset_group.name = resp.chart.name;
                                                }

                                                item_asset_group.item_asset.Add(item_asset);
                                                item_asset.item_asset_group = item_asset_group;

                                                localdb.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                            db.SaveChanges();
                        }


                    }
                }
            }
        }

        private void Production()
        {
            //tpdp
        }

        private void ClickInformation(object sender, MouseButtonEventArgs e)
        {
            Cognitivo.Properties.Settings.Default.Save();
            string key = tbxAPI.Text;
            var obj = Send2API(null, tbxURL.Text + "/api/check-key", key);
            if (obj != null)
            {
                popConnBuilder.IsOpen = false;
            }
            CheckStatus(sender, e);
        }

        private HttpWebResponse Send2API(object Json, string uri, string key)
        {
            try
            {
                var webAddr = uri;
                WebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                httpWebRequest.Headers.Add("Authorization", "Bearer " + key);

                using (var requestStream = httpWebRequest.GetRequestStream())
                {
                    using (var streamWriter = new StreamWriter(requestStream))
                    {
                        streamWriter.Write(Json);
                        streamWriter.Flush();
                        streamWriter.Close();
                        streamWriter.Dispose();

                    }
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //httpWebRequest.Abort();
                return httpResponse;
            }
            catch
            {
                apiStatus = false;
                serverStatus = false;
                return null;
            }
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            Cognitivo.Properties.Settings.Default.Save();
        }
    }
}
