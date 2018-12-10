using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using entity;
using entity.API.DebeHaber;

namespace Cognitivo.Accounting
{
    /// <summary>
    /// Interaction logic for DebeHaber.xaml
    /// </summary>
    public partial class DebeHaber : Page, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        //   public bool isReady { get; set; }
        public bool serverStatus { get; set; }
        public bool apiStatus { get; set; }

        public dbContext Context { get; set; }

        List<sales_invoice> sales_invoiceList { get; set; }
        List<sales_return> sales_returnList { get; set; }
        List<purchase_invoice> purchase_invoiceList { get; set; }
        List<purchase_return> purchase_returnList { get; set; }
        List<payment_detail> app_account_detailsalespurchaseList { get; set; }
        List<item_asset> ItemAssetList { get; set; }
        List<app_account_detail> AccountMovementList { get; set; }

        public DebeHaber()
        {
            InitializeComponent();
            Context = new dbContext();
            btnStart.IsEnabled = false;
            // RaisePropertyChanged("isReady");
            //Check KeyStatus on thread
            CheckStatus(null, null);
            // Task basic_Task = Task.Factory.StartNew(() => CheckStatus(null, null));
            LoadData(null, null);
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
            progProduction.IsIndeterminate = true;

            //This is a little more code, but will allow all to be loaded at the same time for a quicker startup.
            //Load();
            //LoadSales();
            //LoadSalesReturn();
            //LoadPurchases();
            //LoadPurchaseReturns();
            //LoadAccounts();
            //LoadProductions();

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
            Task f = Task.Factory.StartNew(() => LoadAccountsForsalespurchase());
            f.Wait();
            Task h = Task.Factory.StartNew(() => LoadProductions());
            h.Wait();
            Task i = Task.Factory.StartNew(() => LoadAssets());
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
            int count = Context.db.sales_invoice.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
                && x.status == Status.Documents_General.Approved).Count();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                progSales.IsIndeterminate = false;
                progSales.Maximum = count;
                salesMaximum.Text = progSales.Maximum.ToString();
            }));
        }

        private void LoadSalesReturn()
        {
            int count = Context.db.sales_return.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
                && x.status == Status.Documents_General.Approved).Count();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                progSalesReturn.IsIndeterminate = false;
                progSalesReturn.Maximum = count;
                salesReturnMaximum.Text = progSalesReturn.Maximum.ToString();
            }));
        }

        private void LoadPurchases()
        {
            int count = Context.db.purchase_invoice.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
            && x.status == Status.Documents_General.Approved).Count();



            Dispatcher.BeginInvoke((Action)(() =>
            {
                progPurchase.IsIndeterminate = false;
                progPurchase.Maximum = count;
                purchaseMaximum.Text = progPurchase.Maximum.ToString();
            }));
        }

        private void LoadPurchaseReturns()
        {
            int count = Context.db.purchase_return.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
                && x.status == Status.Documents_General.Approved).Count();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                progPurchaseReturn.IsIndeterminate = false;
                progPurchaseReturn.Maximum = count;
                purchaseReturnMaximum.Text = progPurchaseReturn.Maximum.ToString();
            }));
        }

        private void LoadAccountsForsalespurchase()
        {
            int count = Context.db.payment_detail.Where(x => x.id_company == CurrentSession.Id_Company &&
           x.payment.is_accounted == false)
               .Count();



            Dispatcher.BeginInvoke((Action)(() =>
            {
                progAccounts.IsIndeterminate = false;
                progAccounts.Maximum = count;
                paymentMaximum.Text = progAccounts.Maximum.ToString();
            }));
        }

        private void LoadAccountsForMovement()
        {
            int count = Context.db.app_account_detail.Where(x => x.id_company == CurrentSession.Id_Company &&
          x.tran_type == app_account_detail.tran_types.Transaction &&
          x.is_accounted == false && x.id_payment_detail == null &&
          x.status == Status.Documents_General.Approved).Count();


            Dispatcher.BeginInvoke((Action)(() =>
            {
                progTransfer.IsIndeterminate = false;
                progTransfer.Maximum = count;
                transferMaximum.Text = progTransfer.Maximum.ToString();
            }));

        }

        private void LoadProductions()
        {
            List<production_execution_detail> production_execution_detailList = Context.db.production_execution_detail
                .Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false)
                .Include(x => x.production_order_detail)
                .Include(x => x.production_order_detail.production_order)
                .Take(500)
                .ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progProduction.IsIndeterminate = false;
                progProduction.Maximum = Context.db.production_execution_detail.Local.Count();
                productionMaximum.Text = progProduction.Maximum.ToString();
            }));
        }

        private void LoadAssets()
        {
            ItemAssetList = Context.db.item_asset.Where(x => x.id_company == CurrentSession.Id_Company)
                .Include(x => x.item)
                .Include(x => x.app_currency)
                .Include(x => x.app_company)
                .ToList();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                progAsset.IsIndeterminate = false;
                progAsset.Maximum = Context.db.item_asset.Local.Count();
                assetMaximum.Text = progAsset.Maximum.ToString();
            }));
        }
        #endregion

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //Start();
            btnStart.IsEnabled = false;
            string url = tbxURL.Text;
            string key = tbxAPI.Text;
            Task basic_Task = Task.Factory.StartNew(() => Start(url, key));
        }

        private void Start(string url, string key)
        {

            Task Sales_Task = Task.Factory.StartNew(() => Sales(url, key));
            Sales_Task.Wait();
            Task SalesReturn_Task = Task.Factory.StartNew(() => SalesReturns(url, key));
            SalesReturn_Task.Wait();
            Task Purchase_Task = Task.Factory.StartNew(() => Purchases(url, key));
            Purchase_Task.Wait();
            Task PurchaseReturn_Task = Task.Factory.StartNew(() => PurchaseReturns(url, key));
            PurchaseReturn_Task.Wait();

            Task AccountsForSalesPurchase_Task = Task.Factory.StartNew(() => AccountsForSalesPurchase(url, key));
            AccountsForSalesPurchase_Task.Wait();

            Task AccountsForMovement_Task = Task.Factory.StartNew(() => AccountsForMovement(url, key));
            AccountsForMovement_Task.Wait();

            Task FixedAssetTask = Task.Factory.StartNew(() => FixedAsset(url, key, ItemAssetList));
            FixedAssetTask.Wait();

            Dispatcher.BeginInvoke((Action)(() => btnStart.IsEnabled = true));
        }

        private void Sales(string url, string key)
        {



            for (int i = 0; i < progSales.Maximum; i = i + 100)
            {
                sales_invoiceList = Context.db.sales_invoice.
              Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
              && x.status == Status.Documents_General.Approved)
                .Include(x => x.sales_invoice_detail)
                .Include(x => x.app_currencyfx)
                .Include(x => x.app_company)
                .Skip(i)
                .Take(100)
                .ToList();

                List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
                int value = 0;
                Dispatcher.BeginInvoke((Action)(() => salesValue.Text = value.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progSales.Value = value));

                InvoiceList.Clear();
                foreach (sales_invoice sales_invoice in sales_invoiceList.Skip(i).Take(100))
                {
                    entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                    Invoice.LoadSales(sales_invoice);
                    InvoiceList.Add(Invoice);
                }

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

                            sales_invoice sales_invoice = sales_invoiceList.Where(x => x.id_sales_invoice == ReturnJson.local_id).FirstOrDefault();
                            if (sales_invoice != null)
                            {
                                sales_invoice.is_accounted = true;
                                sales_invoice.cloud_id = ReturnJson.cloud_id;
                            }

                        }
                    }
                    Context.db.SaveChanges();
                }

                value += 100;
                Dispatcher.BeginInvoke((Action)(() => progSales.Value = value));
                Dispatcher.BeginInvoke((Action)(() => salesValue.Text = value.ToString()));
            }

        }

        private void SalesReturns(string url, string key)
        {
           
            for (int i = 0; i < progSalesReturn.Maximum; i = i + 100)
            {
                sales_returnList = Context.db.sales_return.
               Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
               && x.status == Status.Documents_General.Approved)
               .Include(x => x.sales_return_detail)
               .Include(x => x.app_currencyfx)
                 .Include(x => x.app_company)
                 .Skip(i)
                 .Take(100)
               .ToList();

                List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
                int value = 0;
                Dispatcher.BeginInvoke((Action)(() => salesReturnValue.Text = value.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progSalesReturn.Value = value));

                InvoiceList.Clear();
                foreach (sales_return sales_return in sales_returnList.Skip(value).Take(100))
                {
                    entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                    Invoice.LoadSalesReturn(sales_return);
                    InvoiceList.Add(Invoice);
                }
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

                            sales_return sales_return = sales_returnList.Where(x => x.id_sales_return == ReturnJson.local_id).FirstOrDefault();
                            if (sales_return != null)
                            {
                                sales_return.is_accounted = true;
                                sales_return.cloud_id = ReturnJson.cloud_id;
                            }


                        }
                    }

                    Context.db.SaveChanges();
                }

                value += 100;
                Dispatcher.BeginInvoke((Action)(() => progSalesReturn.Value = value));
                Dispatcher.BeginInvoke((Action)(() => salesReturnValue.Text = value.ToString()));
            }
        }

        private void Purchases(string url, string key)
        {


            for (int i = 0; i < progPurchase.Maximum; i = i + 100)
            {
                purchase_invoiceList = Context.db.purchase_invoice.
              Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
          && x.status == Status.Documents_General.Approved)
          .Include(x => x.contact)
          .Include(x => x.purchase_invoice_detail)
          .Include(x => x.app_currencyfx)
          .Include(x => x.app_company)
          .Skip(i)
          .Take(100)
          .ToList();

                List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
                int value = 0;
                Dispatcher.BeginInvoke((Action)(() => purchaseValue.Text = value.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progPurchase.Value = value));

                InvoiceList.Clear();
                foreach (purchase_invoice purchase_invoice in purchase_invoiceList.Skip(value).Take(100))
                {
                    entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                    Invoice.LoadPurchase(purchase_invoice);
                    InvoiceList.Add(Invoice);
                }
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

                            purchase_invoice purchase_invoice = purchase_invoiceList.Where(x => x.id_purchase_invoice == ReturnJson.local_id).FirstOrDefault();
                            if (purchase_invoice != null)
                            {
                                purchase_invoice.is_accounted = true;
                                purchase_invoice.cloud_id = ReturnJson.cloud_id;
                            }


                        }
                    }
                    Context.db.SaveChanges();
                }
                value += 100;
                Dispatcher.BeginInvoke((Action)(() => progPurchase.Value = value));
                Dispatcher.BeginInvoke((Action)(() => purchaseValue.Text = value.ToString()));
            }
        }

        private void PurchaseReturns(string url, string key)
        {
           
            for (int i = 0; i < purchase_returnList.Count(); i = i + 100)
            {
                purchase_returnList = Context.db.purchase_return.
              Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
              && x.status == Status.Documents_General.Approved).Include(x => x.contact)
              .Include(x => x.purchase_return_detail)
              .Include(x => x.app_currencyfx)
              .Include(x => x.app_company)
              .Skip(i)
              .Take(100)
              .ToList();

                List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
                int value = 0;
                Dispatcher.BeginInvoke((Action)(() => purchaseReturnValue.Text = value.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progPurchaseReturn.Value = value));

                InvoiceList.Clear();
                foreach (purchase_return purchase_return in purchase_returnList.Skip(value).Take(100))
                {
                    entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                    Invoice.LoadPurchaseReturn(purchase_return);
                    InvoiceList.Add(Invoice);
                }
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

                            purchase_return purchase_return = purchase_returnList.Where(x => x.id_purchase_return == ReturnJson.local_id).FirstOrDefault();
                            if (purchase_return != null)
                            {
                                purchase_return.is_accounted = true;
                                purchase_return.cloud_id = ReturnJson.cloud_id;
                            }


                        }
                    }
                    Context.db.SaveChanges();
                }
                value += 100;
                Dispatcher.BeginInvoke((Action)(() => progPurchaseReturn.Value = value));
                Dispatcher.BeginInvoke((Action)(() => purchaseReturnValue.Text = value.ToString()));
            }
        }


        private void AccountsForMovement(string url, string key)
        {

          
            for (int i = 0; i < progTransfer.Maximum; i = i + 100)
            {
                AccountMovementList = Context.db.app_account_detail.Where(x => x.id_company == CurrentSession.Id_Company &&
       x.tran_type == app_account_detail.tran_types.Transaction &&
       x.is_accounted == false && x.id_payment_detail == null &&
       x.status == Status.Documents_General.Approved)
           .Include(x => x.app_currencyfx)
           .Include(x => x.app_account)
           .Skip(i)
           .Take(100)
           .ToList();

                List<entity.API.DebeHaber.AccountMovements> InvoiceList = new List<entity.API.DebeHaber.AccountMovements>();

                Dispatcher.BeginInvoke((Action)(() => transferValue.Text = ""));
                Dispatcher.BeginInvoke((Action)(() => progTransfer.Value = 0));

                InvoiceList.Clear();



                foreach (app_account_detail app_account_detail in AccountMovementList.Skip(i).Take(100))
                {
                    entity.API.DebeHaber.AccountMovements AccountMovement = new entity.API.DebeHaber.AccountMovements();
                    AccountMovement.LoadTransfers(app_account_detail);
                    InvoiceList.Add(AccountMovement);
                }




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

                            app_account_detail app_account_detail = AccountMovementList.Where(x => x.id_account_detail == ReturnJson.local_id).FirstOrDefault();
                            if (app_account_detail != null)
                            {

                                app_account_detail.is_accounted = true;

                            }

                        }
                    }
                }

                Context.db.SaveChanges();
                Dispatcher.BeginInvoke((Action)(() => progTransfer.Value = i));
                Dispatcher.BeginInvoke((Action)(() => transferValue.Text = i.ToString()));
            }
        }

        private void AccountsForSalesPurchase(string url, string key)
        {
            
            for (int i = 0; i < progAccounts.Maximum; i = i + 100)
            {
                //get records with skip and take.
                app_account_detailsalespurchaseList = Context.db.payment_detail.Where(x => x.id_company == CurrentSession.Id_Company &&
        x.payment.is_accounted == false)
             .Include(x => x.app_currencyfx)
             .Include(x => x.app_account)
             .Include(x => x.payment_schedual)
             .Include(x => x.app_company)
             .Include(x => x.payment)
             .Include(x => x.app_account)
             .Skip(i)
             .Take(100)
             .ToList();

                List<entity.API.DebeHaber.AccountMovements> InvoiceList = new List<entity.API.DebeHaber.AccountMovements>();
                int value = 0;

                Dispatcher.BeginInvoke((Action)(() => paymentValue.Text = value.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progAccounts.Value = value));

                InvoiceList.Clear();
                //run for each on payments table where is_accounted == false.

                foreach (payment_detail payment_detail in app_account_detailsalespurchaseList.Skip(value).Take(100))
                {


                    foreach (payment_schedual schedual in payment_detail.payment_schedual
                        .AsQueryable().Include(x => x.sales_invoice).Include(x => x.purchase_invoice).ToList())
                    {
                        entity.API.DebeHaber.AccountMovements AccountMovement = new entity.API.DebeHaber.AccountMovements();



                        AccountMovement.LoadPaymentsRecieved(schedual);

                        if (AccountMovement.Credit > 0 || AccountMovement.Debit > 0)
                        {
                            InvoiceList.Add(AccountMovement);
                        }



                    }



                }
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
                            payment_detail payment_detail = Context.db.payment_schedual.Where(x => x.id_payment_schedual == ReturnJson.local_id).Include(x => x.payment_detail).FirstOrDefault().payment_detail;
                            if (payment_detail != null)
                            {
                                payment_detail.payment.is_accounted = true;
                                payment_detail.payment.cloud_id = ReturnJson.cloud_id;
                            }


                        }
                        Context.db.SaveChanges();
                    }

                }
                value += 100;
                Dispatcher.BeginInvoke((Action)(() => progAccounts.Value = value));
                Dispatcher.BeginInvoke((Action)(() => paymentValue.Text = value.ToString()));
            }
        }

        private void FixedAsset(string url, string key, List<item_asset> ItemAssetList)
        {
            List<entity.API.DebeHaber.FixedAsset> AssetList = new List<entity.API.DebeHaber.FixedAsset>();

            Dispatcher.BeginInvoke((Action)(() => assetMaximum.Text = ItemAssetList.Count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progAsset.Value = 0));
            for (int value = 0; value < ItemAssetList.Count() + 1; value = value + 100)
            {
                AssetList.Clear();
                foreach (item_asset item_asset in ItemAssetList.Skip(value).Take(100))
                {



                    entity.API.DebeHaber.FixedAsset FixedAsset = new entity.API.DebeHaber.FixedAsset();
                    FixedAsset.LoadAsset(item_asset);


                    AssetList.Add(FixedAsset);



                }
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
                                using (db db = new db())
                                {

                                    item_asset item_asset = db.item_asset.Where(x => x.id_item_asset == resp.ref_id).FirstOrDefault();
                                    item_asset.purchase_date = Convert.ToDateTime(resp.purchase_date);
                                    item_asset.purchase_value = resp.purchase_value;
                                    item_asset.current_value = resp.current_value;
                                    item_asset.item.name = resp.name;
                                    item_asset.item.code = resp.serial;
                                    item_asset.quantity = Convert.ToInt32(resp.quantity);

                                    //create asset group or update values.
                                    //search asset group by name
                                    item_asset_group item_asset_group = db.item_asset_group.Where(x => x.ref_id == resp.chart.id).FirstOrDefault();

                                    if (item_asset_group != null)
                                    {
                                        item_asset_group.depreciation_rate = resp.chart.asset_years;
                                    }
                                    else
                                    {
                                        item_asset_group = db.item_asset_group.Where(x => x.name == resp.chart.name).FirstOrDefault() ?? new item_asset_group();
                                        item_asset_group.ref_id = resp.chart.id;
                                        item_asset_group.depreciation_rate = resp.chart.asset_years;
                                        item_asset_group.name = resp.chart.name;
                                    }

                                    item_asset_group.item_asset.Add(item_asset);
                                    item_asset.item_asset_group = item_asset_group;

                                    db.SaveChanges();
                                }

                            }

                        }
                    }
                }


                Context.db.SaveChanges();

                Dispatcher.BeginInvoke((Action)(() => progAsset.Value = value));
                Dispatcher.BeginInvoke((Action)(() => assetValue.Text = value.ToString()));
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
            catch (Exception ex)
            {
                apiStatus = false;
                serverStatus = false;
                return null;
            }
        }
    }
}
