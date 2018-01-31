using Cognitivo.Sales;
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

namespace Cognitivo.ErpWeb
{
    /// <summary>
    /// Interaction logic for WebSync.xaml
    /// </summary>
    public partial class WebSync : Page
    {
        private dbContext db = new dbContext();

        public WebSync()
        {
            InitializeComponent();
            db.db = new db();
        }

        private void SyncData()
        {
            //Start Threading
            lblInformation.Text = entity.Brillo.Localize.StringText("Items");
            SyncItems();
        }

        private void SyncItems()
        {
            List<item> items = db.db.items.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.cloud_id == null).ToList();

            foreach (var item in items)
            {

            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                

                List<contact> contacts = db.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.is_customer).ToList();


                List<SyncCustomers> synccustomers = new List<SyncCustomers>();
                List<SyncItems> SyncItems = new List<SyncItems>();

                foreach (item item in items)
                {
                    SyncItems SyncItem = new SyncItems
                    {
                        name = item.name,
                        code = item.code,
                        comment = item.description,
                        unit_price = item.item_price.FirstOrDefault() != null ? item.item_price.FirstOrDefault().valuewithVAT : 0,
                    };
                    SyncItems.Add(SyncItem);
                }
                foreach (contact contact in contacts)
                {

                    SyncCustomers SyncCustomer = new SyncCustomers
                    {
                        name = contact.name,
                        govcode = contact.gov_code,
                        alias = contact.alias,
                        address = contact.address,
                        telephone = contact.telephone,
                        email = contact.email,
                    };
                    synccustomers.Add(SyncCustomer);
                }
               

                try
                {
                    var Customer_Json = new JavaScriptSerializer().Serialize(synccustomers);
                    Send2API(Customer_Json, "synccustomer");

                    var Item_Json = new JavaScriptSerializer().Serialize(SyncItems);
                    Send2API(Item_Json, "syncitem");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                Sales_click(null, null);
            }));
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Task taskAuth = Task.Factory.StartNew(() => fill());
        }

        private void Send2API(object Json, string apiname)
        {
            try
            {
                var webAddr = "/" + apiname;
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
                    if (result.ToString().Contains("Sucess"))
                    {
                        MessageBox.Show(result.ToString());

                    }
                    else if (result.ToString().Contains("Error"))
                    {
                        MessageBox.Show(result.ToString());
                        Class.ErrorLog.DebeHaber(Json.ToString());
                    }
                    else
                    {
                        List<Invoice> Sales_Json = new JavaScriptSerializer().Deserialize<List<Invoice>>(result);
                        foreach (Invoice invoice in Sales_Json)
                        {
                            sales_invoice sales_invoice = db.db.sales_invoice.Where(x => x.id_sales_invoice == invoice.my_id).FirstOrDefault();

                            if (sales_invoice != null)
                            {
                                sales_invoice.cloud_id = invoice.cloud_id;
                                sales_invoice.contact.cloud_id = invoice.customer.cloud_id;
                                foreach (Detail detail in invoice.details)
                                {
                                    sales_invoice_detail sales_invoice_detail = sales_invoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == detail.my_id).FirstOrDefault();
                                    if (sales_invoice_detail != null)
                                    {
                                        sales_invoice_detail.cloud_id = detail.cloud_id;
                                        sales_invoice_detail.item.cloud_id = detail.item.cloud_id;
                                    }
                                }
                            }
                        }

                        db.db.SaveChanges();
                        MessageBox.Show("Sucess..");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private string Receive2API(string apiname)
        {
            try
            {
                var webAddr = "/" + apiname;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return "";
            }
        }

        private void Sales_click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<sales_invoice> salesinvoices = db.db.sales_invoice.Include("contact")
                   .Where(x => x.id_company == CurrentSession.Id_Company && x.status == Status.Documents_General.Approved).ToList();

                List<Invoice> SyncInvoices = new List<Invoice>();

                foreach (sales_invoice sales_invoice in salesinvoices)
                {
                    SyncCustomers SyncCustomer = new SyncCustomers
                    {
                        name = sales_invoice.contact.name,
                        govcode = sales_invoice.contact.gov_code,
                        alias = sales_invoice.contact.alias,
                        address = sales_invoice.contact.address,
                        telephone = sales_invoice.contact.telephone,
                        email = sales_invoice.contact.email
                    };

                    Invoice SyncInvoice = new Invoice
                    {
                        status = Invoice.Status.Invoiced,
                        my_id = sales_invoice.id_sales_invoice,
                        cloud_id = Convert.ToInt64(sales_invoice.cloud_id),
                        number = sales_invoice.number,
                        trans_date = sales_invoice.trans_date,
                        credit_days = 0,
                        currency_code = sales_invoice.app_currencyfx.app_currency.code,
                        currency_rate = sales_invoice.app_currencyfx.sell_value,
                        comment = sales_invoice.comment,
                        customer = SyncCustomer,
                        branch_id=sales_invoice.app_branch.id_branch,
                        branch_name = sales_invoice.app_branch.name,
                    };

                    foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
                    {
                        SyncItems SyncItem = new SyncItems
                        {
                            name = sales_invoice_detail.item.name,
                            code = sales_invoice_detail.item.code,
                            comment = sales_invoice_detail.item.description,
                            unit_price = sales_invoice_detail.item.item_price.FirstOrDefault() != null ? sales_invoice_detail.item.item_price.FirstOrDefault().valuewithVAT : 0,
                        };
                        Detail Detail = new Detail
                        {
                            my_id = sales_invoice_detail.id_sales_invoice_detail,
                            cloud_id = Convert.ToInt64(sales_invoice_detail.cloud_id),
                            product_id = sales_invoice_detail.id_item,
                            vat = sales_invoice_detail.app_vat_group.app_vat_group_details.FirstOrDefault() != null ? sales_invoice_detail.app_vat_group.app_vat_group_details.FirstOrDefault().percentage : 0,
                            quantity = sales_invoice_detail.quantity,
                            price = sales_invoice_detail.unit_price
                        };

                        Detail.item = SyncItem;
                        SyncInvoice.details.Add(Detail);
                    }
                    SyncInvoices.Add(SyncInvoice);
                }

                var Sales_Json = new JavaScriptSerializer().Serialize(SyncInvoices);
                Send2API(Sales_Json, "synctransaction");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
       
        private void Download_Click(object sender, RoutedEventArgs e)
        {

            var result = Receive2API("downloadOrder");
            List<DownloadInvoice> Sales_Json = new JavaScriptSerializer().Deserialize<List<DownloadInvoice>>(result);
            entity.Controller.Sales.InvoiceController SalesDB = new entity.Controller.Sales.InvoiceController();
            SalesDB.Initialize();

            foreach (DownloadInvoice DownloadInvoice in Sales_Json)
            {
                if (DownloadInvoice.cloud_id == null)
                {
                    sales_invoice sales_invoice = SalesDB.Create(0, false);
                    sales_invoice.Location = CurrentSession.Locations.Where(x => x.id_location == Settings.Default.Location).FirstOrDefault();
                    app_document_range app_document_range = SalesDB.db.app_document_range.Where(x => x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.PointOfSale && x.is_active).FirstOrDefault();
                    if (app_document_range != null)
                    {
                        sales_invoice.id_range = app_document_range.id_range;
                        sales_invoice.RaisePropertyChanged("id_range");
                        sales_invoice.app_document_range = app_document_range;
                    }
                    contact contact = SalesDB.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.cloud_id == DownloadInvoice.relationship_id).FirstOrDefault();
                    if (contact == null)

                    {
                        using (db db = new db())
                        {
                            contact = new contact();
                            contact.name = DownloadInvoice.customer_alias;
                            contact.is_customer = true;
                            contact.id_contact_role = SalesDB.db.contact_role.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).FirstOrDefault().id_contact_role;
                            contact.cloud_id = DownloadInvoice.relationship_id;

                            db.contacts.Add(contact);
                            db.SaveChanges();
                        }
                        contact = SalesDB.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.cloud_id == DownloadInvoice.relationship_id).FirstOrDefault();

                    }

                    sales_invoice.id_contact = contact.id_contact;
                    sales_invoice.contact = contact;
                    sales_invoice.cloud_id = DownloadInvoice.id;

                    foreach (details details in DownloadInvoice.details)
                    {

                        item item = SalesDB.db.items.Where(x => x.cloud_id == details.item_id).FirstOrDefault();
                        if (item == null)
                        {
                            using (db db = new db())
                            {
                                item = new item();
                                item.name = details.item_name;
                                item.id_item_type = item.item_type.Product;
                                item.id_vat_group = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault().id_vat_group;
                                item.cloud_id = details.item_id;
                                db.items.Add(item);
                                db.SaveChanges();
                            }
                            item = SalesDB.db.items.Where(x => x.cloud_id == details.item_id).FirstOrDefault();
                        }


                        sales_invoice_detail _sales_invoice_detail = new sales_invoice_detail()
                        {
                            State = EntityState.Added,
                            sales_invoice = sales_invoice,
                            quantity = Convert.ToDecimal(details.quantity),
                            Contact = sales_invoice.contact,
                            item_description = details.item_name,
                            item = item,
                            id_item = item.id_item,
                            id_vat_group = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault().id_vat_group,
                            cloud_id = details.id

                        };

                        sales_invoice.sales_invoice_detail.Add(_sales_invoice_detail);

                    }


                    crm_opportunity crm_opportunity = new crm_opportunity()
                    {
                        id_contact = sales_invoice.id_contact,
                        id_currency = sales_invoice.id_currencyfx,
                        value = sales_invoice.GrandTotal
                    };

                    crm_opportunity.sales_invoice.Add(sales_invoice);
                    SalesDB.db.crm_opportunity.Add(crm_opportunity);

                    SalesDB.db.sales_invoice.Add(sales_invoice);

                }
                else
                {
                    sales_invoice sales_invoice = SalesDB.db.sales_invoice.Where(x => x.id_sales_invoice == DownloadInvoice.cloud_id).FirstOrDefault();
                    if (sales_invoice != null)
                    {
                        contact contact = SalesDB.db.contacts.Where(x => x.cloud_id == DownloadInvoice.relationship_id).FirstOrDefault();
                        if (contact == null)
                        {
                            contact = new contact();
                            contact.name = DownloadInvoice.customer_alias;
                            contact.is_customer = true;

                        }
                        sales_invoice.id_contact = contact.id_contact;
                        sales_invoice.contact = contact;
                    }
                    foreach (details details in DownloadInvoice.details)
                    {
                        sales_invoice_detail sales_invoice_detail = SalesDB.db.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == details.cloud_id).FirstOrDefault();
                        if (sales_invoice_detail != null)
                        {
                            item item = SalesDB.db.items.Where(x => x.cloud_id == details.item_id).FirstOrDefault();
                            if (item == null)
                            {
                                item = new item();
                                item.name = details.item_name;
                                item.id_item_type = item.item_type.Product;

                                sales_invoice_detail.quantity = Convert.ToDecimal(details.quantity);
                                sales_invoice_detail.Contact = sales_invoice.contact;
                                sales_invoice_detail.item_description = details.item_name;
                                sales_invoice_detail.item = item;
                                sales_invoice_detail.id_item = item.id_item;

                            }
                        }

                    }
                }
                SalesDB.db.SaveChanges();


            }
        }
    }
    public class SyncItems
    {
        public string name { get; set; }
        public string code { get; set; }
        public string comment { get; set; }
        public decimal unit_price { get; set; }
        public long? cloud_id { get; set; }

    }
    public class SyncCustomers
    {
        public string name { get; set; }
        public string govcode { get; set; }
        public string alias { get; set; }
        public string address { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public long? cloud_id { get; set; }
    }

    public class Invoice
    {
        public Invoice()
        {
            details = new List<Detail>();
        }

        public enum Status { Invoiced = 3, Packed = 4 }

        public int my_id { get; set; }
        public int branch_id { get; set; }
        public string branch_name { get; set; }
        public long cloud_id { get; set; }
        public SyncCustomers customer { get; set; }
        public string number { get; set; }
        public DateTime trans_date { get; set; }
        public DateTime? packing_date { get; set; }
        public int credit_days { get; set; }
        public string currency_code { get; set; }
        public decimal currency_rate { get; set; }
        public string comment { get; set; }
        public Status status { get; set; }


        public List<Detail> details { get; set; }
    }

    public class Detail
    {
        public int my_id { get; set; }
        public long cloud_id { get; set; }
        public long product_id { get; set; }
        public decimal vat { get; set; }
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        public decimal discount { get; set; }
        public SyncItems item { get; set; }
    }


    public class DownloadInvoice
    {
        public DownloadInvoice()
        {
            details = new List<details>();
        }
        
        public int id { get; set; }
        public int? cloud_id { get; set; }
        public int? location_id { get; set; }
        public int? classification_id { get; set; }
        public int? recurring_order_id { get; set; }
        public int? buyer_profile_id { get; set; }
        public int? salesrep_profile_id { get; set; }
        public int relationship_id { get; set; }
        public string customer_alias { get; set; }
        public string customer_taxid { get; set; }
        public string customer_address { get; set; }
        public string customer_telephone { get; set; }
        public string customer_email { get; set; }
        public List<details> details { get; set; }
    }

    public class details
    {
        public int id { get; set; }
        public int? cloud_id { get; set; }
        public int item_id { get; set; }
        public string item_sku { get; set; }
        public string item_name { get; set; }
        public decimal? quantity { get; set; }
        public decimal? unit_price { get; set; }
        public decimal? unit_cost { get; set; }
    }
}
