using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using entity;

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

        public bool isReady { get; set; }
        public bool serverStatus { get; set; }
        public bool apiStatus { get; set; }

        public dbContext Context { get; set; }

        List<sales_invoice> sales_invoiceList { get; set; }
        List<sales_return> sales_returnList { get; set; }
        List<purchase_invoice> purchase_invoiceList { get; set; }
        List<purchase_return> purchase_returnList { get; set; }
        List<app_account_detail> app_account_detailsalespurchaseList { get; set; }


        public DebeHaber()
        {
            InitializeComponent();
            Context = new dbContext();
            isReady = false;
            RaisePropertyChanged("isReady");
            //Check KeyStatus on thread
            CheckStatus(null, null);
            // Task basic_Task = Task.Factory.StartNew(() => CheckStatus(null, null));
            LoadData(null, null);
        }

        private void CheckStatus(object sender, MouseButtonEventArgs e)
        {
            //TODO, Check if access to server is ok. Make sure to use the URL on the config file.
            //serverStatus = true;

            //TODO, Check if API Key is active (not expired). Make sure to use the URL on the config file.
            //apiStatus = true;
            string key = tbxAPI.Text;
            var obj = Send2API(null, tbxURL.Text + "/api/check-key", key);

            //If both is Ok, then we are ready to Export.
            if (serverStatus && apiStatus)
            {
                isReady = true;
                RaisePropertyChanged("isReady");
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
            Task f = Task.Factory.StartNew(() => LoadAccountsForsalespurchase());
            f.Wait();
            Task h = Task.Factory.StartNew(() => LoadProductions());
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
            sales_invoiceList = Context.db.sales_invoice.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
                && x.status == Status.Documents_General.Approved)
                  .Include(x => x.sales_invoice_detail)
                  .Include(x => x.app_currencyfx)
                  .ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progSales.IsIndeterminate = false;
                progSales.Maximum = Context.db.sales_invoice.Local.Count();
                salesMaximum.Text = progSales.Maximum.ToString();
            }));
        }

        private void LoadSalesReturn()
        {
            sales_returnList = Context.db.sales_return.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
                && x.status == Status.Documents_General.Approved)
                .Include(x => x.sales_return_detail)
                .Include(x => x.app_currencyfx)
                .ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progSalesReturn.IsIndeterminate = false;
                progSalesReturn.Maximum = Context.db.sales_return.Local.Count();
                salesReturnMaximum.Text = progSalesReturn.Maximum.ToString();
            }));
        }

        private void LoadPurchases()
        {
            purchase_invoiceList = Context.db.purchase_invoice.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
            && x.status == Status.Documents_General.Approved
            ).Include(x => x.purchase_invoice_detail).Include(x => x.app_currencyfx).ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progPurchase.IsIndeterminate = false;
                progPurchase.Maximum = Context.db.purchase_invoice.Local.Count();
                purchaseMaximum.Text = progPurchase.Maximum.ToString();
            }));
        }

        private void LoadPurchaseReturns()
        {
            purchase_returnList = Context.db.purchase_return.
                Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false
                && x.status == Status.Documents_General.Approved)
                .Include(x => x.purchase_return_detail).Include(x => x.app_currencyfx).ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progPurchaseReturn.IsIndeterminate = false;
                progPurchaseReturn.Maximum = Context.db.purchase_return.Local.Count();
                purchaseReturnMaximum.Text = progPurchaseReturn.Maximum.ToString();
            }));
        }

        private void LoadAccountsForsalespurchase()
        {
            app_account_detailsalespurchaseList = Context.db.app_account_detail.Where(x => x.id_company == CurrentSession.Id_Company &&
            x.tran_type == app_account_detail.tran_types.Transaction &&
            x.is_read == false && x.id_payment_detail != null &&
            x.status == Status.Documents_General.Approved)
                .Include(x => x.app_currencyfx)
                .Include(x => x.app_account)
                .ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progAccounts.IsIndeterminate = false;
                progAccounts.Maximum = Context.db.app_account_detail.Local.Count();
                transferMaximum.Text = progAccounts.Maximum.ToString();
            }));
        }

        private void LoadAccountsForMovement()
        {
            List<app_account_detail> app_account_detailList = Context.db.app_account_detail.Where(x => x.id_company == CurrentSession.Id_Company &&
            x.tran_type == app_account_detail.tran_types.Transaction &&
            x.is_read == false && x.id_payment_detail == null &&
            x.status == Status.Documents_General.Approved)
                .Include(x => x.app_currencyfx)
                .Include(x => x.app_account)
                .ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progAccounts.IsIndeterminate = false;
                progAccounts.Maximum = Context.db.app_account_detail.Local.Count();
                transferMaximum.Text = progAccounts.Maximum.ToString();
            }));

        }

        private void LoadProductions()
        {
            List<production_execution_detail> production_execution_detailList = Context.db.production_execution_detail.Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false)
                .Include(x => x.production_order_detail)
                .Include(x => x.production_order_detail.production_order)
                .ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progProduction.IsIndeterminate = false;
                progProduction.Maximum = Context.db.production_execution_detail.Local.Count();
                productionMaximum.Text = progProduction.Maximum.ToString();
            }));
        }

        #endregion

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //Start();
            string url = tbxURL.Text;
            string key = tbxAPI.Text;
            Task basic_Task = Task.Factory.StartNew(() => Start(url, key));
        }

        private void Start(string url, string key)
        {

            Task Sales_Task = Task.Factory.StartNew(() => Sales(url, key, sales_invoiceList));
            Sales_Task.Wait();
            Task SalesReturn_Task = Task.Factory.StartNew(() => SalesReturns(url, key, sales_returnList));
            SalesReturn_Task.Wait();
            Task Purchase_Task = Task.Factory.StartNew(() => Purchases(url, key, purchase_invoiceList));
            Purchase_Task.Wait();
            Task PurchaseReturn_Task = Task.Factory.StartNew(() => PurchaseReturns(url, key, purchase_returnList));
            PurchaseReturn_Task.Wait();

            Task AccountsForSalesPurchase_Task = Task.Factory.StartNew(() => AccountsForSalesPurchase(url, key, app_account_detailsalespurchaseList));
            //Task AccountsForMovement_Task = Task.Factory.StartNew(() => AccountsForMovement(url, key, purchase_returnList));
            //AccountsForMovement_Task.Wait();
        }

        private void Sales(string url, string key, List<sales_invoice> sales_invoiceList)
        {
            List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => salesValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progSales.Value = value));

            for (int i = 0; i < sales_invoiceList.Count(); i = i + 100)
            {
                InvoiceList.Clear();
                foreach (sales_invoice sales_invoice in sales_invoiceList.Skip(i).Take(100))
                {
                    entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                    Invoice.LoadSales(sales_invoice);
                    InvoiceList.Add(Invoice);
                }

                var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(InvoiceList);
                Send2API(Json, url + "/api/transactions", key);

                foreach (sales_invoice sales_invoice in sales_invoiceList.Skip(i).Take(100))
                {
                    sales_invoice.is_accounted = true;
                }

                Context.db.SaveChanges();
                value += 100;
                Dispatcher.BeginInvoke((Action)(() => progSales.Value = value));
                Dispatcher.BeginInvoke((Action)(() => salesValue.Text = value.ToString()));
            }

        }

        private void SalesReturns(string url, string key, List<sales_return> sales_returnList)
        {
            List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => salesReturnValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progSalesReturn.Value = value));
            for (int i = 0; i < sales_returnList.Count(); i = i + 100)
            {
                InvoiceList.Clear();
                foreach (sales_return sales_return in sales_returnList.Skip(value).Take(100))
                {
                    entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                    Invoice.LoadSalesReturn(sales_return);
                    InvoiceList.Add(Invoice);
                }
                var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(InvoiceList);
                Send2API(Json, url + "/api/transactions", key);

                foreach (sales_return sales_return in sales_returnList.Skip(value).Take(100))
                {
                    sales_return.is_accounted = true;
                }

                Context.db.SaveChanges();
                value += 100;
                Dispatcher.BeginInvoke((Action)(() => progSalesReturn.Value = value));
                Dispatcher.BeginInvoke((Action)(() => salesReturnValue.Text = value.ToString()));
            }
        }

        private void Purchases(string url, string key, List<purchase_invoice> purchase_invoiceList)
        {

            List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => purchaseValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progPurchase.Value = value));
            for (int i = 0; i < purchase_invoiceList.Count(); i = i + 100)
            {
                InvoiceList.Clear();
                foreach (purchase_invoice purchase_invoice in purchase_invoiceList.Skip(value).Take(100))
                {
                    entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                    Invoice.LoadPurchase(purchase_invoice);
                    InvoiceList.Add(Invoice);
                }
                var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(InvoiceList);
                Send2API(Json, url + "/api/transactions", key);
                foreach (purchase_invoice purchase_invoice in purchase_invoiceList.Skip(value).Take(100))
                {
                    purchase_invoice.is_accounted = true;

                }
                Context.db.SaveChanges();
                value += 100;
                Dispatcher.BeginInvoke((Action)(() => progPurchase.Value = value));
                Dispatcher.BeginInvoke((Action)(() => purchaseValue.Text = value.ToString()));
            }
        }

        private void PurchaseReturns(string url, string key, List<purchase_return> purchase_returnList)
        {
            List<entity.API.DebeHaber.Invoice> InvoiceList = new List<entity.API.DebeHaber.Invoice>();
            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => purchaseReturnValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progPurchaseReturn.Value = value));
            for (int i = 0; i < purchase_returnList.Count(); i = i + 100)
            {
                InvoiceList.Clear();
                foreach (purchase_return purchase_return in purchase_returnList.Skip(value).Take(100))
                {
                    entity.API.DebeHaber.Invoice Invoice = new entity.API.DebeHaber.Invoice();
                    Invoice.LoadPurchaseReturn(purchase_return);
                    InvoiceList.Add(Invoice);
                }
                var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(InvoiceList);
                Send2API(Json, url + "/api/transactions", key);
                foreach (purchase_return purchase_return in purchase_returnList.Skip(value).Take(100))
                {
                    purchase_return.is_accounted = true;

                }
                Context.db.SaveChanges();
                value += 100;
                Dispatcher.BeginInvoke((Action)(() => progPurchaseReturn.Value = value));
                Dispatcher.BeginInvoke((Action)(() => purchaseReturnValue.Text = value.ToString()));
            }
        }


        private void AccountsForMovement(string url, string key, List<app_account_detail> AccountMovementList)
        {
        }
        private void AccountsForSalesPurchase(string url, string key, List<app_account_detail> AccountSalePurcahseList)
        {

            List<entity.API.DebeHaber.AccountMovements> InvoiceList = new List<entity.API.DebeHaber.AccountMovements>();
            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => transferValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progAccounts.Value = value));
            for (int i = 0; i < app_account_detailsalespurchaseList.Count(); i = i + 100)
            {
                InvoiceList.Clear();
                foreach (app_account_detail app_account_detail in app_account_detailsalespurchaseList.Skip(value).Take(100))
                {
                    entity.API.DebeHaber.AccountMovements AccountMovement = new entity.API.DebeHaber.AccountMovements();
                    if (app_account_detail.payment_detail.payment_schedual.First().id_sales_invoice > 0)
                    {
                        AccountMovement.LoadPaymentsRecieved(app_account_detail);
                    }
                    else
                    {
                        AccountMovement.LoadPaymentsMade(app_account_detail);
                    }

                    InvoiceList.Add(AccountMovement);
                }
                var Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(InvoiceList);
                Send2API(Json, url + "/api/movement", key);

                Context.db.SaveChanges();
                value += 100;
                Dispatcher.BeginInvoke((Action)(() => progAccounts.Value = value));
                Dispatcher.BeginInvoke((Action)(() => transferValue.Text = value.ToString()));
            }
        }

        private void Production()
        {

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



        }
        private object Send2API(object Json, string uri, string key)
        {
            try
            {


                var webAddr = uri;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + key);

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
                    apiStatus = true;
                    serverStatus = true;
                    if (result.ToString().Contains("Error"))
                    {
                        tbxAPI.Focus();
                        apiStatus = false;
                        return null;
                    }
                    return result;
                }
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
