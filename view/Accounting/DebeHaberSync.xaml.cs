using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;
using System.Data.Entity;
using entity;
using System.Collections.Generic;

namespace Cognitivo.Accounting
{
    public partial class DebeHaberSync : Page, IDisposable
    {
        CollectionViewSource sales_invoiceViewSource;
        CollectionViewSource sales_returnViewSource;
        CollectionViewSource purchase_invoiceViewSource;
        CollectionViewSource purchase_returnViewSource;
        CollectionViewSource payment_detailViewSource;
        CollectionViewSource item_assetViewSource;

        db db = new db();

        string RelationshipHash = string.Empty;

        public DebeHaberSync()
        {
            InitializeComponent();

            sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
            sales_returnViewSource = ((CollectionViewSource)(FindResource("sales_returnViewSource")));
            purchase_invoiceViewSource = ((CollectionViewSource)(FindResource("purchase_invoiceViewSource")));
            purchase_returnViewSource = ((CollectionViewSource)(FindResource("purchase_returnViewSource")));
            payment_detailViewSource = ((CollectionViewSource)(FindResource("payment_detailViewSource")));
            item_assetViewSource = ((CollectionViewSource)(FindResource("item_assetViewSource")));

            RelationshipHash = db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault().hash_debehaber;

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
                Get_ItemAsset();
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
            sales_invoiceViewSource.Source = await db.sales_invoice.Where(x =>
                x.id_company == CurrentSession.Id_Company &&
                x.is_accounted == false &&
                (x.status == Status.Documents_General.Approved || x.status == Status.Documents_General.Annulled)).ToListAsync();
        }

        public async void Get_Payment()
        {
            //x.Is Head replace with Is_Accounted = True.
            payment_detailViewSource.Source = await db.payment_detail.Where(x =>
                x.payment.id_company == CurrentSession.Id_Company &&
                x.payment.is_accounted == false &&
                x.payment.status == Status.Documents_General.Approved).ToListAsync();
        }

        public async void Get_SalesReturn()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_returnViewSource.Source = await db.sales_return.Where(x =>
                x.id_company == CurrentSession.Id_Company &&
                x.is_accounted == false &&
                (x.status == Status.Documents_General.Approved || x.status == Status.Documents_General.Annulled)).ToListAsync();
        }

        public async void Get_PurchaseReturnInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_returnViewSource.Source = await db.purchase_return.Where(x =>
                x.id_company == CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == Status.Documents_General.Approved).ToListAsync();
        }

        public async void Get_PurchaseInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_invoiceViewSource.Source = await db.purchase_invoice.Where(x =>
                x.id_company == CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == Status.Documents_General.Approved).ToListAsync();
        }

        private async void Get_ItemAsset()
        {
            await db.item_asset.Where(x =>
                x.id_company == CurrentSession.Id_Company &&
                x.item.is_active == true).ToListAsync();
            item_assetViewSource.Source = db.item_asset.Local;
        }

        #endregion

        private void btnData_Sync(object sender, RoutedEventArgs e)
        {
            DebeHaber.Integration Integration = new DebeHaber.Integration();
            Integration.Key = RelationshipHash;

            DebeHaber.Transaction Transaction = new DebeHaber.Transaction();

            Sales_Sync(Transaction);
            Purchase_Sync(Transaction);
            SalesReturn_Sync(Transaction);
            PurchaseReturn_Sync(Transaction);

            PaymentSync(Transaction);
            FixedAsset(Transaction);

            Integration.Transactions.Add(Transaction);

            try
            {
                var Sales_Json = new JavaScriptSerializer().Serialize(Integration);
                Send2API(Sales_Json);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                if (MessageBox.Show("Sales Error. Would you like to Save the File for Analsys?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    MessageBox.Show(ex.Message, "Error Message");
                    file_create(new JavaScriptSerializer().Serialize(Integration).ToString(), "SalesInvoice-" + DateTime.Now);
                }
            }
            finally
            {
                fill();
            }
        }

        private void Sales_Sync(DebeHaber.Transaction Transaction)
        {
            List<sales_invoice> SalesList = db.sales_invoice.Local.Where(x => x.IsSelected).ToList();

            //Loop through
            foreach (sales_invoice sales_invoice in SalesList)
            {
                DebeHaber.Commercial_Invoice Sales = new DebeHaber.Commercial_Invoice();
                
                //Loads Data from Sales
                Sales.Fill_BySales(sales_invoice);

                ///Loop through Details.
                foreach (sales_invoice_detail Detail in sales_invoice.sales_invoice_detail)
                {
                    DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_BySales(Detail, db);
                    Sales.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (payment_schedual schedual in sales_invoice.payment_schedual)
                {
                    if (schedual.payment_detail != null && schedual.payment_detail.payment.is_accounted == false)
                    {
                        DebeHaber.Payments Payments = new DebeHaber.Payments();
                        //Fill and Add Payments
                        Payments.FillPayments(schedual);
                        Sales.Payments.Add(Payments);

                        //This will make the Sales Invoice hide from the next load.
                        schedual.payment_detail.payment.is_accounted = true;
                    }
                }

                Transaction.Commercial_Invoices.Add(Sales);
                
                sales_invoice.IsSelected = false;
                sales_invoice.is_accounted = true;
            }
        }

        private void Purchase_Sync(DebeHaber.Transaction Transaction)
        {
            //Loop through
            List<purchase_invoice>PurchaseList = db.purchase_invoice.Local.Where(x => x.IsSelected).ToList();

            foreach (purchase_invoice purchase_invoice in PurchaseList)
            {
                DebeHaber.Commercial_Invoice Purchase = new DebeHaber.Commercial_Invoice();

                //Loads Data from Sales
                Purchase.Fill_ByPurchase(purchase_invoice);

                ///Loop through Details.
                foreach (purchase_invoice_detail Detail in purchase_invoice.purchase_invoice_detail)
                {
                    DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_ByPurchase(Detail, db);
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

                        //This will make the Sales Invoice hide from the next load.
                        schedual.payment_detail.payment.is_accounted = true;
                    }
                }
                purchase_invoice.is_accounted = true;
                purchase_invoice.IsSelected = false;
                Transaction.Commercial_Invoices.Add(Purchase);
            }
        }

        private void SalesReturn_Sync(DebeHaber.Transaction Transaction)
        {
            List<sales_return> SalesReturnList = db.sales_return.Local.Where(x => x.IsSelected).ToList();

            //Loop through
            foreach (sales_return sales_return in SalesReturnList)
            {
                DebeHaber.Commercial_Invoice SalesReturn = new DebeHaber.Commercial_Invoice();

                //Loads Data from Sales
                SalesReturn.Fill_BySalesReturn(sales_return);

                ///Loop through Details.
                foreach (sales_return_detail Detail in sales_return.sales_return_detail)
                {
                    DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_BySalesReturn(Detail, db);
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

                        //This will make the Sales Invoice hide from the next load.
                        schedual.payment_detail.payment.is_accounted = true;
                    }
                }

                sales_return.is_accounted = true;
                sales_return.IsSelected = false;
                Transaction.Commercial_Invoices.Add(SalesReturn);
            }
        }

        private void PurchaseReturn_Sync(DebeHaber.Transaction Transaction)
        {
            List<purchase_return> PurchaseReturnList = db.purchase_return.Local.Where(x => x.IsSelected).ToList();

            //Loop through
            foreach (purchase_return purchase_return in PurchaseReturnList)
            {
                DebeHaber.Commercial_Invoice PurchaseReturn = new DebeHaber.Commercial_Invoice();

                //Loads Data from Sales
                PurchaseReturn.Fill_ByPurchaseReturn(purchase_return);

                ///Loop through Details.
                foreach (purchase_return_detail Detail in purchase_return.purchase_return_detail)
                {
                    DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_ByPurchaseReturn(Detail, db);
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

                        //This will make the Sales Invoice hide from the next load.
                        schedual.payment_detail.payment.is_accounted = true;
                    }
                }

                purchase_return.is_accounted = true;
                purchase_return.IsSelected = false;

                Transaction.Commercial_Invoices.Add(PurchaseReturn);
            }
        }

        private void PaymentSync(DebeHaber.Transaction Transaction)
        {
            List<payment_detail> PaymentList = db.payment_detail.Local.Where(x => x.IsSelected && x.payment.is_accounted == false).ToList();

            //Loop through
            foreach (payment_detail payment_detail in PaymentList)
            {
                DebeHaber.Payments Payment = new DebeHaber.Payments();

                //Loads Data from Sales
                payment_schedual schedual = db.payment_schedual.Where(x => x.id_payment_detail == payment_detail.id_payment_detail).FirstOrDefault();
                Payment.FillPayments(schedual);

                payment_detail.IsSelected = false;
                payment_detail.payment.is_accounted = true;
                Transaction.Payments.Add(Payment);
            }
        }

        private void FixedAsset(DebeHaber.Transaction Transaction)
        {
            List<item_asset_group> AssetGroupList = db.item_asset_group.Local.Where(x => x.id_company == CurrentSession.Id_Company && x.IsSelected).ToList();

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
        #endregion

        private void Send2API(object Json)
        {
            var webAddr = Properties.Settings.Default.DebeHaberConnString + "/api/transactions";
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
                    file_create(Json.ToString(), "DebeHaber Error File" + DateTime.Now);
                }
            }
        }

        public void file_create(string Data, string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Json.json";
            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path))
                {
                    using (var fw = new StreamWriter(fs))
                    {
                        fw.Write(Data);
                        fw.Flush();
                    }
                }
            }
            else
            {
                Console.WriteLine("File \"{0}\" already exists.", filename);
                return;
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
        #endregion
    }
}
