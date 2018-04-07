using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using entity;

namespace Cognitivo.Accounting
{
    /// <summary>
    /// Interaction logic for DebeHaber.xaml
    /// </summary>
    public partial class DebeHaber : Page
    {
        public bool isReady { get; set; }
        public bool serverStatus { get; set; }
        public bool apiStatus { get; set; }

        public dbContext Context { get; set; }

        public DebeHaber()
        {
            InitializeComponent();
            Context = new dbContext();

            //Check KeyStatus on thread
            Task basic_Task = Task.Factory.StartNew(() => CheckStatus(null, null));
            LoadData(null, null);
        }

        private void CheckStatus(object sender, MouseButtonEventArgs e)
        {
            //TODO, Check if access to server is ok. Make sure to use the URL on the config file.
            serverStatus = true;

            //TODO, Check if API Key is active (not expired). Make sure to use the URL on the config file.
            apiStatus = true;

            //If both is Ok, then we are ready to Export.
            if (serverStatus && apiStatus)
            {
                isReady = true;
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
            Task g = Task.Factory.StartNew(() => Load());
            Task a = Task.Factory.StartNew(() => LoadSales());
            Task b = Task.Factory.StartNew(() => LoadSalesReturn());
            Task c = Task.Factory.StartNew(() => LoadPurchases());
            Task d = Task.Factory.StartNew(() => LoadPurchaseReturns());
            Task e = Task.Factory.StartNew(() => LoadAccounts());
            Task f = Task.Factory.StartNew(() => LoadProductions());
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
            Context.db.sales_invoice.Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false)
                .Include(x => x.sales_invoice_detail)
                .Include(x => x.app_currencyfx)
                .ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progSales.IsIndeterminate = false;
                progSales.Maximum = Context.db.sales_invoice.Local.Count();
            }));
        }

        private void LoadSalesReturn()
        {
            Context.db.sales_return.Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false)
                .Include(x => x.sales_return_detail)
                .Include(x => x.app_currencyfx)
                .ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progSalesReturn.IsIndeterminate = false;
                progSalesReturn.Maximum = Context.db.sales_return.Local.Count();
            }));
        }

        private void LoadPurchases()
        {
            Context.db.purchase_invoice.Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false).Include(x => x.purchase_invoice_detail).Include(x => x.app_currencyfx).ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progPurchase.IsIndeterminate = false;
                progPurchase.Maximum = Context.db.purchase_invoice.Local.Count();
            }));
        }

        private void LoadPurchaseReturns()
        {
            Context.db.purchase_return.Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false).Include(x => x.purchase_return_detail).Include(x => x.app_currencyfx).ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progPurchaseReturn.IsIndeterminate = false;
                progPurchaseReturn.Maximum = Context.db.purchase_return.Local.Count();
            }));
        }

        private void LoadAccounts()
        {
            Context.db.app_account_detail.Where(x => x.id_company == CurrentSession.Id_Company && x.tran_type == app_account_detail.tran_types.Transaction && x.is_read == false).Include(x => x.app_currencyfx).ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progAccounts.IsIndeterminate = false;
                progAccounts.Maximum = Context.db.app_account_detail.Local.Count();
            }));

        }

        private void LoadProductions()
        {
            Context.db.production_execution_detail.Where(x => x.id_company == CurrentSession.Id_Company && x.is_accounted == false)
                .Include(x => x.production_order_detail)
                .Include(x => x.production_order_detail.production_order)
                .ToList();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                progAccounts.IsIndeterminate = false;
                progAccounts.Maximum = Context.db.production_execution_detail.Local.Count();
            }));
        }

        #endregion

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Task basic_Task = Task.Factory.StartNew(() => Start());
        }

        private void Start()
        {

        }

        private void Sales()
        {

        }

        private void SalesReturns()
        {

        }

        private void Purchases()
        {

        }

        private void PurchaseReturns()
        {

        }

        private void Accounts()
        {

        }

        private void Production()
        {

        }
    }
}
