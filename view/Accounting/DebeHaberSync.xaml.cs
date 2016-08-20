using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Data.Entity;
using System.Timers;

namespace Cognitivo.Accounting
{
    /// <summary>
    /// Loads ERP Data and serializes into Json for DebeHaber.
    /// </summary>
    public partial class DebeHaberSync : Page
    {
        CollectionViewSource sales_invoiceViewSource;
        CollectionViewSource sales_returnViewSource;
        CollectionViewSource purchase_invoiceViewSource;
        CollectionViewSource purchase_returnViewSource;
        CollectionViewSource paymentViewSource;
        CollectionViewSource item_assetViewSource;

        entity.db db = new entity.db();

        string RelationshipHash = string.Empty;

        public DebeHaberSync()
        {
            InitializeComponent();

            sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
            sales_returnViewSource = ((CollectionViewSource)(FindResource("sales_returnViewSource")));
            purchase_invoiceViewSource = ((CollectionViewSource)(FindResource("purchase_invoiceViewSource")));
            purchase_returnViewSource = ((CollectionViewSource)(FindResource("purchase_returnViewSource")));
            paymentViewSource = ((CollectionViewSource)(FindResource("paymentViewSource")));
            item_assetViewSource = ((CollectionViewSource)(FindResource("item_assetViewSource")));

            RelationshipHash = db.app_company.Where(x => x.id_company == entity.CurrentSession.Id_Company).FirstOrDefault().hash_debehaber;

            var timer = new System.Threading.Timer(
                e => btnData_Refresh(null, null),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(15));
        }

        private void btnData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_SalesInvoice();
            Get_PurchaseInvoice();
            Get_PurchaseReturnInvoice();
            Get_SalesReturn();
            Get_Payment();
            Get_ItemAsset();
        }

        #region LoadData
        public async void Get_SalesInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_invoiceViewSource.Source = await db.sales_invoice.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToListAsync();
        }
        
        public async void Get_Payment()
        {
            //x.Is Head replace with Is_Accounted = True.
            paymentViewSource.Source = await db.payments.Where(x =>
                 x.id_company == entity.CurrentSession.Id_Company &&
                 x.is_accounted == false &&
                 x.status == entity.Status.Documents_General.Approved).ToListAsync();
        }

        public async void Get_SalesReturn()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_returnViewSource.Source = await db.sales_return.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToListAsync();
        }

        public async void Get_PurchaseReturnInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_returnViewSource.Source = await db.purchase_return.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToListAsync();
        }

        public async void Get_PurchaseInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_invoiceViewSource.Source = await db.purchase_invoice.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToListAsync();
        }

        private async void Get_ItemAsset()
        {
            await db.item_asset.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.item.is_active == true).ToListAsync();
            item_assetViewSource.Source = db.item_asset.Local;
        }

        #endregion

        private void btnData_Sync(object sender, RoutedEventArgs e)
        {
            //Sales
            Sales_Sync();
            //Purchase
            Purchase_Sync();

            SalesReturn_Sync();
            PurchaseReturn_Sync();
            
            PaymentSync();
            
        }
     
        private void Sales_Sync()
        {
            //entity.DebeHaber.Transactions Transactions = new entity.DebeHaber.Transactions();
            entity.DebeHaber.Transactions SalesError = new entity.DebeHaber.Transactions();

            //Loop through
            foreach (entity.sales_invoice sales_invoice in db.sales_invoice.Local.Where(x => x.IsSelected))// && x.is_accounted == false))
            {
                entity.DebeHaber.Transactions Transactions = new entity.DebeHaber.Transactions();
                Transactions.HashIntegration = RelationshipHash;

                entity.DebeHaber.Commercial_Invoice Sales = new entity.DebeHaber.Commercial_Invoice();

                //Loads Data from Sales
                Sales.Fill_BySales(sales_invoice);

                ///Loop through Details.
                foreach (entity.sales_invoice_detail Detail in sales_invoice.sales_invoice_detail)
                {
                    entity.DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new entity.DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_BySales(Detail, db);
                    Sales.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (entity.payment_schedual schedual in sales_invoice.payment_schedual)
                {
                    if (schedual.payment_detail != null && schedual.payment_detail.payment.is_accounted == false)
                    {
                        entity.DebeHaber.Payments Payments = new entity.DebeHaber.Payments();
                        //Fill and Add Payments
                        Payments.FillPayments(schedual);
                        Sales.Payments.Add(Payments);

                        //This will make the Sales Invoice hide from the next load.
                        schedual.payment_detail.payment.is_accounted = true;
                    }
                }

                Transactions.Commercial_Invoice.Add(Sales);

                try
                {
                    var Sales_Json = new JavaScriptSerializer().Serialize(Transactions);
                    Send2API(Sales_Json);
                    sales_invoice.is_accounted = true;
                }
                catch (Exception)
                {
                    SalesError.Commercial_Invoice.Add(Sales);
                    sales_invoice.is_accounted = false;
                }
                finally
                {
                    db.SaveChanges();
                }
            }
        }

        private void Purchase_Sync()
        {
            //entity.DebeHaber.Transactions Transactions = new entity.DebeHaber.Transactions();
            entity.DebeHaber.Transactions PurchaseError = new entity.DebeHaber.Transactions();

            //Loop through
            foreach (entity.purchase_invoice purchase_invoice in db.purchase_invoice.Local.Where(x => x.IsSelected))
            {
                entity.DebeHaber.Transactions Transactions = new entity.DebeHaber.Transactions();
                Transactions.HashIntegration = RelationshipHash;

                entity.DebeHaber.Commercial_Invoice Purchase = new entity.DebeHaber.Commercial_Invoice();

                //Loads Data from Sales
                Purchase.Fill_ByPurchase(purchase_invoice);

                ///Loop through Details.
                foreach (entity.purchase_invoice_detail Detail in purchase_invoice.purchase_invoice_detail)
                {
                    entity.DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new entity.DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_ByPurchase(Detail, db);
                    Purchase.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (entity.payment_schedual schedual in purchase_invoice.payment_schedual)
                {
                    if (schedual.payment_detail != null && schedual.payment_detail.payment.is_accounted == false)
                    {
                        entity.DebeHaber.Payments Payments = new entity.DebeHaber.Payments();
                        //Fill and Add Payments
                        Payments.FillPayments(schedual);
                        Purchase.Payments.Add(Payments);

                        //This will make the Sales Invoice hide from the next load.
                        schedual.payment_detail.payment.is_accounted = true;
                    }
                }

                Transactions.Commercial_Invoice.Add(Purchase);

                try
                {
                    var Sales_Json = new JavaScriptSerializer().Serialize(Transactions);
                    Send2API(Sales_Json);
                    purchase_invoice.is_accounted = true;
                }
                catch (Exception)
                {
                    PurchaseError.Commercial_Invoice.Add(Purchase);
                    purchase_invoice.is_accounted = false;
                }
                finally
                {
                    db.SaveChanges();
                }
            }
        }

        private void SalesReturn_Sync()
        {
            //remember to clean out those that are already accounted from SalesSync.
            //entity.DebeHaber.Transactions Transactions = new entity.DebeHaber.Transactions();
            entity.DebeHaber.Transactions SalesReturnError = new entity.DebeHaber.Transactions();

            //Loop through
            foreach (entity.sales_return sales_return in db.sales_return.Local.Where(x => x.IsSelected && x.is_accounted == false))// && x.is_accounted == false))
            {
                entity.DebeHaber.Transactions Transactions = new entity.DebeHaber.Transactions();
                Transactions.HashIntegration = RelationshipHash;

                entity.DebeHaber.Commercial_Invoice SalesReturn = new entity.DebeHaber.Commercial_Invoice();

                //Loads Data from Sales
                SalesReturn.Fill_BySalesReturn(sales_return);

                ///Loop through Details.
                foreach (entity.sales_return_detail Detail in sales_return.sales_return_detail)
                {
                    entity.DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new entity.DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_BySalesReturn(Detail, db);
                    SalesReturn.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (entity.payment_schedual schedual in sales_return.payment_schedual)
                {
                    if (schedual.payment_detail != null && schedual.payment_detail.payment.is_accounted == false)
                    {
                        entity.DebeHaber.Payments Payments = new entity.DebeHaber.Payments();
                        //Fill and Add Payments
                        Payments.FillPayments(schedual);
                        SalesReturn.Payments.Add(Payments);

                        //This will make the Sales Invoice hide from the next load.
                        schedual.payment_detail.payment.is_accounted = true;
                    }
                }

                Transactions.Commercial_Invoice.Add(SalesReturn);

                try
                {
                    var Json = new JavaScriptSerializer().Serialize(Transactions);
                    Send2API(Json);
                    sales_return.is_accounted = true;
                }
                catch (Exception)
                {
                    SalesReturnError.Commercial_Invoice.Add(SalesReturn);
                    sales_return.is_accounted = false;
                }
                finally
                {
                    db.SaveChanges();
                }
            }
        }

        private void PurchaseReturn_Sync()
        {
            //remember to clean out those that are already accounted from SalesSync.
            //entity.DebeHaber.Transactions Transactions = new entity.DebeHaber.Transactions();
            entity.DebeHaber.Transactions PurchaseReturnError = new entity.DebeHaber.Transactions();

            //Loop through
            foreach (entity.purchase_return purchase_return in db.purchase_return.Local.Where(x => x.IsSelected && x.is_accounted == false))
            {
                entity.DebeHaber.Transactions Transactions = new entity.DebeHaber.Transactions();
                Transactions.HashIntegration = RelationshipHash;

                entity.DebeHaber.Commercial_Invoice PurchaseReturn = new entity.DebeHaber.Commercial_Invoice();

                //Loads Data from Sales
                PurchaseReturn.Fill_ByPurchaseReturn(purchase_return);

                ///Loop through Details.
                foreach (entity.purchase_return_detail Detail in purchase_return.purchase_return_detail)
                {
                    entity.DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new entity.DebeHaber.CommercialInvoice_Detail();
                    //Fill and Detail SalesDetail
                    CommercialInvoice_Detail.Fill_ByPurchaseReturn(Detail, db);
                    PurchaseReturn.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (entity.payment_schedual schedual in purchase_return.payment_schedual)
                {
                    if (schedual.payment_detail != null && schedual.payment_detail.payment.is_accounted == false)
                    {
                        entity.DebeHaber.Payments Payments = new entity.DebeHaber.Payments();
                        //Fill and Add Payments
                        Payments.FillPayments(schedual);
                        PurchaseReturn.Payments.Add(Payments);

                        //This will make the Sales Invoice hide from the next load.
                        schedual.payment_detail.payment.is_accounted = true;
                    }
                }

                Transactions.Commercial_Invoice.Add(PurchaseReturn);

                try
                {
                    var Json = new JavaScriptSerializer().Serialize(Transactions);
                    Send2API(Json);
                    purchase_return.is_accounted = true;
                }
                catch (Exception)
                {
                    PurchaseReturnError.Commercial_Invoice.Add(PurchaseReturn);
                    purchase_return.is_accounted = false;
                }
                finally
                {
                    db.SaveChanges();
                }
            }
        }

        private void PaymentSync()
        {
            //entity.DebeHaber.Transactions Transactions = new entity.DebeHaber.Transactions();
            entity.DebeHaber.Transactions PaymentError = new entity.DebeHaber.Transactions();

            //Loop through
            foreach (entity.payment payments in db.payments.Local.Where(x => x.IsSelected && x.is_accounted == false))
            {
                entity.DebeHaber.Transactions Transactions = new entity.DebeHaber.Transactions();
                Transactions.HashIntegration = RelationshipHash;

                foreach (entity.payment_detail payment_detail in payments.payment_detail.ToList())
                {
                    entity.DebeHaber.Payments Payment = new entity.DebeHaber.Payments();

                    //Loads Data from Sales
                    entity.payment_schedual schedual = db.payment_schedual.Where(x => x.id_payment_detail == payment_detail.id_payment_detail).FirstOrDefault();
                    Payment.FillPayments(schedual);

                    Transactions.Payments.Add(Payment);
                }

                try
                {
                    var Json = new JavaScriptSerializer().Serialize(Transactions);
                    Send2API(Json);
                    payments.is_accounted = true;
                }
                catch (Exception)
                {
                    payments.is_accounted = false;
                }
                finally
                {
                    db.SaveChanges();
                }
            }
        }

        #region CheckBox Check/UnCheck Methods
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sales_invoiceViewSource.View != null)
            {
                foreach (entity.sales_invoice sales_invoice in sales_invoiceViewSource.View.OfType<entity.sales_invoice>().ToList())
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
                foreach (entity.sales_invoice sales_invoice in sales_invoiceViewSource.View.OfType<entity.sales_invoice>().ToList())
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
                foreach (entity.sales_return sales_return in sales_returnViewSource.View.OfType<entity.sales_return>().ToList())
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
                foreach (entity.sales_return sales_return in sales_returnViewSource.View.OfType<entity.sales_return>().ToList())
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
                foreach (entity.purchase_invoice purchase_invoice in purchase_invoiceViewSource.View.OfType<entity.purchase_invoice>().ToList())
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
                foreach (entity.purchase_invoice purchase_invoice in purchase_invoiceViewSource.View.OfType<entity.purchase_invoice>().ToList())
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
                foreach (entity.purchase_return purchase_return in purchase_returnViewSource.View.OfType<entity.purchase_return>().ToList())
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
                foreach (entity.purchase_return purchase_return in purchase_returnViewSource.View.OfType<entity.purchase_return>().ToList())
                {
                    purchase_return.IsSelected = false;
                }
                purchase_returnViewSource.View.Refresh();
            }
        }

        private void Payment_Checked(object sender, RoutedEventArgs e)
        {
            if (paymentViewSource.View != null)
            {
                foreach (entity.payment payment in paymentViewSource.View.OfType<entity.payment>().ToList())
                {
                    payment.IsSelected = true;
                }
                paymentViewSource.View.Refresh();
            }

        }

        private void Payment_UnChecked(object sender, RoutedEventArgs e)
        {
            if (paymentViewSource.View != null)
            {
                foreach (entity.payment payment in paymentViewSource.View.OfType<entity.payment>().ToList())
                {
                    payment.IsSelected = false;
                }
                paymentViewSource.View.Refresh();
            }
        }
        #endregion

        private void Send2API(object Json)
        {
            var webAddr = Cognitivo.Properties.Settings.Default.DebeHaberConnString + "/api/transactions";
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
                MessageBox.Show(result.ToString());
                if (result.ToString().Contains("Error"))
                {
                    Exception ex = new Exception();
                    throw ex;
                }
            }
        }

        public void file_create(String Data,String filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Json.json";
            if (!System.IO.File.Exists(path))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(path))
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
    }
}
