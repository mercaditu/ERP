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

            RelationshipHash = db.app_company.Where(x => x.id_company == entity.CurrentSession.Id_Company).FirstOrDefault().domain;
        }

        private void btnData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_SalesInvoice();
            Get_PurchaseInvoice();
            Get_PurchaseReturnInvoice();
            Get_SalesReturn();
            Get_Payment();
        }

        #region LoadData
        public void Get_SalesInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_invoiceViewSource.Source = db.sales_invoice.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }
        
        public void Get_Payment()
        {
            //x.Is Head replace with Is_Accounted = True.
            paymentViewSource.Source = db.payments.Where(x =>
                 x.id_company == entity.CurrentSession.Id_Company &&
                 x.is_accounted == false &&
                 x.status == entity.Status.Documents_General.Approved).ToList();
        }

        public void Get_SalesReturn()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_returnViewSource.Source = db.sales_return.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }

        public void Get_PurchaseReturnInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_returnViewSource.Source = db.purchase_return.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }

        public void Get_PurchaseInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_invoiceViewSource.Source = db.purchase_invoice.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }
        #endregion

        private void btnData_Sync(object sender, RoutedEventArgs e)
        {
            SalesInvoice_Sync();
        }
     
        private void SalesInvoice_Sync()
        {
            List<entity.DebeHaber.Commercial_Invoice> SalesInvoiceLIST = new List<entity.DebeHaber.Commercial_Invoice>();

            //Loop through
            foreach (entity.sales_invoice sales_invoice in db.sales_invoice.Local.Where(x => x.IsSelected && x.is_accounted == false))
            {
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
                foreach (entity.payment_schedual schedual in sales_invoice.payment_schedual.Where(x => 
                    x.id_payment_detail > 0 
                    && x.parent != null 
                    && x.payment_detail.payment.is_accounted == false))
                {         
                    if (schedual.parent.sales_invoice != null && schedual.payment_detail != null)
                    {
                        entity.DebeHaber.Payments Payments = new entity.DebeHaber.Payments();
                        //Fill and Add Payments
                        Payments.FillPayments(schedual);
                        Sales.Payments.Add(Payments);

                        //This will make the Sales Invoice hide from the next load.
                        schedual.payment_detail.payment.is_accounted = true;
                    }
                }

                SalesInvoiceLIST.Add(Sales);

                //This will make the Sales Invoice hide from the next load.
                sales_invoice.is_accounted = true;
            }

            try
            {
                ///Serealize SalesInvoiceLIST into Json
                var Sales_Json = new JavaScriptSerializer().Serialize(SalesInvoiceLIST);

                Send2API(Sales_Json);
                file_create(Sales_Json as string, "sales_invoice");
                //Send Sales_Json send it to Server Address specified.

                //If all success, then SaveChanges.
                db.SaveChanges();
                Get_SalesInvoice();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Error: " + ex.Message);
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

        private void Send2API(string Json)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Cognitivo.Properties.Settings.Default.DebeHaberConnString + "/api_transactions/" + RelationshipHash + "/");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(Json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            
            //var webAddr = Cognitivo.Properties.Settings.Default.DebeHaberConnString + "/api_transactions/" + RelationshipHash + "/";
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            //httpWebRequest.ContentType = "application/json; charset=utf-8";
            //httpWebRequest.Method = "POST";

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(Json);
            //    streamWriter.Flush();
            //}
        }

        public void file_create(String Data,String filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + filename + ".json";
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
