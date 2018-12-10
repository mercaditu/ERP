using Cognitivo.API.Models;
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
        Cognitivo.API.Upload send;
        public WebSync()
        {
            InitializeComponent();
            db.db = new db();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task taskAuth = Task.Factory.StartNew(() => UploadData());

        }
        private void UploadData()
        {
            //Start Threading

            Dispatcher.BeginInvoke((Action)(() =>
            {
                lblInformation.Text = entity.Brillo.Localize.StringText("Items");
                //Vat_click(null, null);
                //Contract_click(null, null);
                //SyncItems();
                ////SyncPromotion();
                // SyncCustomer();
                //Sales_click(null, null);


            }));





        }

        //private void Vat_click(object sender, RoutedEventArgs e)
        //{
        //    send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, Cognitivo.API.Enums.SyncWith.Playground);

        //    List<app_vat_group> app_vat_groupList = db.db.app_vat_group
        //           .Where(x => x.id_company == CurrentSession.Id_Company).ToList();

        //    List<object> SyncVATList = new List<object>();

        //    foreach (app_vat_group app_vat_group in app_vat_groupList)
        //    {
        //        Vat syncVAT = new Vat();
        //        syncVAT.localId = app_vat_group.id_vat_group;
        //        syncVAT.cloudId = app_vat_group.cloud_id ?? 0;
        //        syncVAT.name = app_vat_group.name;
        //        syncVAT.updatedAt = app_vat_group.timestamp.ToUniversalTime();

        //        foreach (app_vat_group_details app_vat_group_details in app_vat_group.app_vat_group_details)
        //        {
        //            VatDetail vatdetail = new VatDetail();
        //            vatdetail.coefficient = app_vat_group_details.app_vat.coefficient;
        //            vatdetail.percentage = app_vat_group_details.percentage;
        //            syncVAT.details.Add(vatdetail);
        //        }

        //        SyncVATList.Add(syncVAT);
        //    }

        //    SyncVATList = send.Vats("PQR", SyncVATList).OfType<object>().ToList();
        //    List<app_vat> app_vatList = db.db.app_vat.ToList();



        //    //foreach (var item in collection)
        //    //{
        //    //    //create on server : assign cloudID
        //    //    //create on local
        //    //    //update on local
        //    //}

        //    foreach (Cognitivo.API.Models.Vat ResoponseData in SyncVATList)
        //    {
        //        if (ResoponseData.action == API.Enums.Action.CreatedOnCloud)
        //        {
        //            app_vat_group app_vat_group = db.db.app_vat_group.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
        //            app_vat_group.cloud_id = ResoponseData.cloudId;

        //        }
        //        else if (ResoponseData.action == API.Enums.Action.UpdatedOnLocal)
        //        {

        //            app_vat_group app_vat_group = db.db.app_vat_group.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
        //            app_vat_group.cloud_id = ResoponseData.cloudId;

        //            if (Convert.ToDateTime(ResoponseData.updatedAt).ToUniversalTime() > app_vat_group.timestamp)
        //            {
        //                app_vat_group.name = ResoponseData.name;
        //            }
        //        }

        //        else if (ResoponseData.action == API.Enums.Action.CreateOnLocal)
        //        {
        //            app_vat_group app_vat_group = new app_vat_group();
        //            app_vat_group.cloud_id = ResoponseData.cloudId;
        //            app_vat_group.name = ResoponseData.name;

        //            foreach (VatDetail details in ResoponseData.details)
        //            {
        //                //create group.
        //                app_vat app_vat = app_vatList.Where(x => x.coefficient == details.coefficient).FirstOrDefault();
        //                int vatId = 0;

        //                if (app_vat == null)
        //                {
        //                    vatId = 0;

        //                    using (db db = new db())
        //                    {
        //                        app_vat = new app_vat();
        //                        app_vat.coefficient = details.coefficient;
        //                        app_vat.on_product = true;

        //                        db.app_vat.Add(app_vat);
        //                        db.SaveChanges();

        //                        vatId = app_vat.id_vat;
        //                    }
        //                }
        //                else
        //                {
        //                    vatId = app_vat.id_vat;
        //                }

        //                app_vat_group_details app_vat_group_details = new app_vat_group_details
        //                {
        //                    id_vat = vatId,
        //                    percentage = details.percentage
        //                };

        //                app_vat_group.app_vat_group_details.Add(app_vat_group_details);
        //            }

        //            db.db.app_vat_group.Add(app_vat_group);
        //        }

        //    }


        //    db.db.SaveChanges();




        //}

        //private void Contract_click(object sender, RoutedEventArgs e)
        //{
        //    List<app_contract> app_contractList = db.db.app_contract
        //           .Where(x => x.id_company == CurrentSession.Id_Company).ToList();
        //    send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, Cognitivo.API.Enums.SyncWith.Playground);
        //    List<object> SyncContractList = new List<object>();
        //    foreach (app_contract app_contract in app_contractList)
        //    {
        //        PaymentContract SyncContract = new PaymentContract();
        //        SyncContract.updatedAt = app_contract.timestamp.ToUniversalTime();
        //        SyncContract.localId = app_contract.id_contract;
        //        SyncContract.cloudId = app_contract.cloud_id ?? 0;
        //        SyncContract.name = app_contract.name;

        //        foreach (app_contract_detail app_contract_detail in app_contract.app_contract_detail)
        //        {
        //            PaymentContractDetail contractdetail = new PaymentContractDetail();
        //            contractdetail.localId = app_contract_detail.id_contract_detail;
        //            contractdetail.percent = app_contract_detail.coefficient;
        //            contractdetail.offset = app_contract_detail.interval;
        //            SyncContract.details.Add(contractdetail);
        //        }
        //        SyncContractList.Add(SyncContract);
        //    }

        //    SyncContractList = send.PaymentContracts("PQR", SyncContractList).OfType<object>().ToList();

        //    foreach (Cognitivo.API.Models.PaymentContract ResoponseData in SyncContractList)
        //    {
        //        if (ResoponseData.action == API.Enums.Action.CreatedOnCloud)
        //        {
        //            app_contract app_contract = db.db.app_contract.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
        //            app_contract.cloud_id = ResoponseData.cloudId;

        //        }
        //        else if (ResoponseData.action == API.Enums.Action.UpdatedOnLocal)
        //        {

        //            app_contract app_contract = db.db.app_contract.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
        //            app_contract.cloud_id = ResoponseData.cloudId;

        //            if (Convert.ToDateTime(ResoponseData.updatedAt).ToUniversalTime() > app_contract.timestamp)
        //            {
        //                app_contract.name = ResoponseData.name;
        //            }
        //        }

        //        else if (ResoponseData.action == API.Enums.Action.CreateOnLocal)
        //        {
        //            app_contract app_contract = new app_contract();
        //            app_contract.cloud_id = ResoponseData.cloudId;
        //            app_contract.name = ResoponseData.name;
        //            app_condition app_condition = db.db.app_condition.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
        //            app_contract.id_condition = app_condition.id_condition;
        //            foreach (PaymentContractDetail details in ResoponseData.details)
        //            {
        //                app_contract_detail app_contract_detail = new app_contract_detail();
        //                app_contract_detail.id_contract = app_contract.id_contract;
        //                app_contract_detail.coefficient = details.percent;
        //                app_contract_detail.interval = (short)details.offset;
        //                app_contract.app_contract_detail.Add(app_contract_detail);
        //                // create vat group...
        //                // foreach detail
        //                // check coefficient in detail.
        //                // create if not exist app_vat.
        //            }
        //            db.db.app_contract.Add(app_contract);
        //        }

        //    }


        //    db.db.SaveChanges();



        //}

        //private void SyncItems()
        //{
        //    List<item> items = db.db.items.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).ToList();
        //    send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, Cognitivo.API.Enums.SyncWith.Playground);


        //    List<object> SyncItems = new List<object>();

        //    foreach (item item in items)
        //    {
        //        Cognitivo.API.Models.Item SyncItem = new Cognitivo.API.Models.Item
        //        {
        //            cloudId = item.cloud_id != null ? (int)item.cloud_id : 0,
        //            localId = item.id_item,
        //            name = item.name,
        //            sku = item.code,
        //            shortDescription = item.description,
        //            price = item.item_price.FirstOrDefault() != null ? item.item_price.FirstOrDefault().valuewithVAT : 0,
        //            currencyCode = CurrentSession.Currency_Default.code,
        //            updatedAt = item.timestamp.ToUniversalTime()
        //        };
        //        SyncItems.Add(SyncItem);
        //    }

        //    if (SyncItems.Count() > 0)
        //    {


        //        SyncItems = send.Item("cognitivo-1", SyncItems.Take(20).ToList()).OfType<object>().ToList();

        //        foreach (Cognitivo.API.Models.Item ResoponseData in SyncItems)
        //        {
        //            if (ResoponseData.action == API.Enums.Action.CreatedOnCloud)
        //            {
        //                item item = db.db.items.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
        //                item.cloud_id = ResoponseData.cloudId;

        //            }
        //            else if (ResoponseData.action == API.Enums.Action.UpdatedOnLocal)
        //            {

        //                item item = db.db.items.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
        //                if (item!=null)
        //                {
        //                    item.cloud_id = ResoponseData.cloudId;

        //                    if (Convert.ToDateTime(ResoponseData.updatedAt).ToUniversalTime() > item.timestamp)
        //                    {
        //                        item.name = ResoponseData.name;
        //                        app_vat_group app_vat_group = db.db.app_vat_group.Where(x => x.cloud_id == ResoponseData.vatId).FirstOrDefault();
        //                        if (app_vat_group != null)
        //                        {
        //                            item.id_vat_group = app_vat_group.id_vat_group;
        //                        }
        //                        else
        //                        {
        //                            app_vat_group = db.db.app_vat_group.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
        //                            item.id_vat_group = app_vat_group.id_vat_group;
        //                        }
        //                        item.sku = ResoponseData.sku;
        //                    }
        //                }

        //            }

        //            else if (ResoponseData.action == API.Enums.Action.CreateOnLocal)
        //            {
        //                item item = new item();
        //                item.cloud_id = ResoponseData.cloudId;
        //                item.name = ResoponseData.name;
        //                item.id_item_type = item.item_type.Product;

        //                app_vat_group app_vat_group = db.db.app_vat_group.Where(x => x.cloud_id == ResoponseData.vatId).FirstOrDefault();
        //                if (app_vat_group != null)
        //                {
        //                    item.id_vat_group = app_vat_group.id_vat_group;
        //                }
        //                else
        //                {
        //                    app_vat_group = db.db.app_vat_group.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
        //                    item.id_vat_group = app_vat_group.id_vat_group;
        //                }
        //                item.sku = ResoponseData.sku;
        //                item_price item_price = new item_price();
        //                item_price.id_price_list = (CurrentSession.PriceLists.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault() ?? CurrentSession.PriceLists.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault()).id_price_list;
        //                item_price.item = item;
        //                item_price.value = ResoponseData.price;
        //                app_currency app_currency = CurrentSession.Currencies.Where(x => x.code == ResoponseData.currencyCode).FirstOrDefault();
        //                if (app_currency != null)
        //                {
        //                    item_price.id_currency = app_currency.id_currency;

        //                }
        //                else
        //                {
        //                    item_price.id_currency = CurrentSession.Currency_Default.id_currency;
        //                }

        //                item.item_price.Add(item_price);
        //                db.db.items.Add(item);
        //            }

        //        }
        //        db.db.SaveChanges();

        //    }



        //    //run code based on timestamp.
        //    //run get to see last sync of table. check if new data must go.
        //    //if cloud.timestamp > local.timestamp then download
        //    //if cloud.timestamp < local.timestamp then upload
        //    //vat
        //    //
        //    //contract
        //    //customer
        //    //item
        //    //Promo
        //    //branch
        //    //location
        //    //company







        //}
        //private void SyncCustomer()
        //{
        //    List<object> synccustomers = new List<object>();
        //    List<contact> contacts = db.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.is_customer).ToList();
        //    send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, Cognitivo.API.Enums.SyncWith.Playground);




        //    foreach (contact contact in contacts)
        //    {

        //        Customer SyncCustomer = new Customer
        //        {
        //            cloudId = Convert.ToInt32(contact.cloud_id ?? 0),
        //            localId = contact.id_contact,
        //            name = contact.name,
        //            taxid = contact.gov_code,
        //            address = contact.address,
        //            telephone = contact.telephone,
        //            email = contact.email,
        //            updatedAt = contact.timestamp.ToUniversalTime()
        //        };
        //        synccustomers.Add(SyncCustomer);
        //    }


        //    try
        //    {

        //        synccustomers = send.Customer("PQR", synccustomers).OfType<object>().ToList();


        //        foreach (Cognitivo.API.Models.Customer ResoponseData in synccustomers)
        //        {
        //            if (ResoponseData.name != null)
        //            {
        //                if (ResoponseData.action == API.Enums.Action.CreatedOnCloud)
        //                {
        //                    contact contact = db.db.contacts.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
        //                    contact.cloud_id = ResoponseData.cloudId;

        //                }
        //                else if (ResoponseData.action == API.Enums.Action.UpdatedOnLocal)
        //                {

        //                    contact contact = db.db.contacts.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
        //                    contact.cloud_id = ResoponseData.cloudId;

        //                    if (Convert.ToDateTime(ResoponseData.updatedAt).ToUniversalTime() > contact.timestamp)
        //                    {
        //                        contact.name = ResoponseData.name;
        //                        contact.gov_code = ResoponseData.taxid;
        //                        contact.address = ResoponseData.address;
        //                        contact.telephone = ResoponseData.telephone;
        //                        contact.email = ResoponseData.email;
        //                    }
        //                }

        //                else if (ResoponseData.action == API.Enums.Action.CreateOnLocal)
        //                {
        //                    contact contact = new contact();
        //                    contact.cloud_id = ResoponseData.cloudId;
        //                    contact.name = ResoponseData.name;
        //                    contact.gov_code = ResoponseData.taxid;
        //                    contact.address = ResoponseData.address;
        //                    contact.telephone = ResoponseData.telephone;
        //                    contact.email = ResoponseData.email;
        //                    db.db.contacts.Add(contact);
        //                }

        //            }


        //        }


        //        db.db.SaveChanges();


        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }



        //}
        //private void Sales_click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        List<sales_invoice> salesinvoices = db.db.sales_invoice
        //           .Where(x => x.id_company == CurrentSession.Id_Company && x.is_archived == false).Take(1).ToList();

        //        List<object> SyncInvoices = new List<object>();
        //        send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, Cognitivo.API.Enums.SyncWith.Playground);

        //        foreach (sales_invoice sales_invoice in salesinvoices)
        //        {
        //            if (sales_invoice.contact != null)
        //            {


        //                Cognitivo.API.Models.Sales SyncInvoice = new Cognitivo.API.Models.Sales
        //                {
        //                    customerCloudId = Convert.ToInt32(sales_invoice.contact.cloud_id ?? 0),
        //                    localId = sales_invoice.id_sales_invoice,
        //                    cloudId = Convert.ToInt32(sales_invoice.cloud_id ?? 0),
        //                    invoiceNumber = sales_invoice.number,
        //                    Date = sales_invoice.trans_date,
        //                    currency = sales_invoice.app_currencyfx.app_currency.code,
        //                    currencyRate = sales_invoice.app_currencyfx.sell_value,
        //                    updatedAt = sales_invoice.timestamp
        //                };

        //                if (sales_invoice.status == Status.Documents_General.Approved)
        //                {
        //                    SyncInvoice.status = API.Enums.Status.Approved;
        //                }

        //                foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
        //                {
        //                    //SyncItems SyncItem = new SyncItems
        //                    //{
        //                    //    local_id = sales_invoice_detail.item.id_item,
        //                    //    name = sales_invoice_detail.item.name,
        //                    //    code = sales_invoice_detail.item.code,
        //                    //    comment = sales_invoice_detail.item.description,
        //                    //    unit_price = sales_invoice_detail.item.item_price.FirstOrDefault() != null ? sales_invoice_detail.item.item_price.FirstOrDefault().valuewithVAT : 0,
        //                    //};
        //                    Cognitivo.API.Models.SalesDetail Detail = new Cognitivo.API.Models.SalesDetail
        //                    {
        //                        localId = sales_invoice_detail.id_sales_invoice_detail,
        //                        cloudId = Convert.ToInt32(sales_invoice_detail.cloud_id ?? 0),
        //                        itemCloudId = Convert.ToInt32(sales_invoice_detail.item.cloud_id ?? 0),
        //                        itemDescription = sales_invoice_detail.item.name,
        //                        //product_id = sales_invoice_detail.id_item,
        //                        //cloud_id
        //                        vatCloudId = sales_invoice_detail.app_vat_group != null ? sales_invoice_detail.app_vat_group.cloud_id != null ? (int)sales_invoice_detail.app_vat_group.cloud_id : 0 : 0,
        //                        quantity = sales_invoice_detail.quantity,
        //                        price = sales_invoice_detail.unit_price
        //                    };

        //                    //  Detail.item = SyncItem;
        //                    SyncInvoice.details.Add(Detail);
        //                }
        //                SyncInvoices.Add(SyncInvoice);
        //            }

        //            //archive?
        //        }
        //        SyncInvoices = send.Transaction("PQR", SyncInvoices).OfType<object>().ToList();


        //        entity.Controller.Sales.InvoiceController SalesDB = new entity.Controller.Sales.InvoiceController();
        //        SalesDB.Initialize();
        //        List<app_document_range> app_document_rangeList = SalesDB.db.app_document_range.Where(x => x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.PointOfSale && x.is_active).ToList();

        //        foreach (Cognitivo.API.Models.Sales ResoponseData in SyncInvoices)
        //        {

        //            if (ResoponseData.action == API.Enums.Action.CreatedOnCloud)
        //            {
        //                sales_invoice sales_invoice = db.db.sales_invoice.Where(x => x.id_sales_invoice == ResoponseData.localId).FirstOrDefault();
        //                sales_invoice.cloud_id = ResoponseData.cloudId;
        //            }
        //            else if (ResoponseData.action == API.Enums.Action.CreateOnLocal)
        //            {

        //                sales_invoice sales_invoice = SalesDB.Create(0, false);
        //                sales_invoice.Location = CurrentSession.Locations.Where(x => x.id_location == Settings.Default.Location).FirstOrDefault();
        //                app_document_range app_document_range = app_document_rangeList.FirstOrDefault();
        //                if (app_document_range != null)
        //                {
        //                    sales_invoice.id_range = app_document_range.id_range;
        //                    sales_invoice.RaisePropertyChanged("id_range");
        //                    sales_invoice.app_document_range = app_document_range;
        //                }
        //                contact contact = SalesDB.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.cloud_id == ResoponseData.customerCloudId).FirstOrDefault();
        //                if (contact != null)

        //                {
        //                    sales_invoice.id_contact = contact.id_contact;
        //                    sales_invoice.contact = contact;

        //                }


        //                sales_invoice.cloud_id = ResoponseData.cloudId;

        //                foreach (Cognitivo.API.Models.SalesDetail details in ResoponseData.details)
        //                {

        //                    item item = SalesDB.db.items.Where(x => x.cloud_id == details.itemLocalId).FirstOrDefault();
        //                    if (item != null)
        //                    {
        //                        sales_invoice_detail _sales_invoice_detail = new sales_invoice_detail()
        //                        {
        //                            State = EntityState.Added,
        //                            sales_invoice = sales_invoice,
        //                            quantity = Convert.ToDecimal(details.quantity),
        //                            unit_price = Convert.ToDecimal(details.price),
        //                            Contact = sales_invoice.contact,
        //                            item_description = item.name,
        //                            item = item,
        //                            id_item = item.id_item,
        //                            id_vat_group = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault().id_vat_group,
        //                            cloud_id = details.cloudId

        //                        };
        //                        sales_invoice.sales_invoice_detail.Add(_sales_invoice_detail);

        //                    }





        //                }


        //                crm_opportunity crm_opportunity = new crm_opportunity()
        //                {
        //                    id_contact = sales_invoice.id_contact,
        //                    id_currency = sales_invoice.id_currencyfx,
        //                    value = sales_invoice.GrandTotal
        //                };

        //                crm_opportunity.sales_invoice.Add(sales_invoice);
        //                SalesDB.db.crm_opportunity.Add(crm_opportunity);

        //                SalesDB.db.sales_invoice.Add(sales_invoice);
        //                db.db.contacts.Add(contact);
        //            }
        //            else if (ResoponseData.action == API.Enums.Action.UpdatedOnLocal)
        //            {

        //                sales_invoice sales_invoice = SalesDB.Create(0, false);
        //                contact contact = SalesDB.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.cloud_id == ResoponseData.customerCloudId).FirstOrDefault();
        //                if (contact != null)

        //                {
        //                    sales_invoice.id_contact = contact.id_contact;
        //                    sales_invoice.contact = contact;

        //                }


        //                sales_invoice.cloud_id = ResoponseData.cloudId;

        //                foreach (Cognitivo.API.Models.SalesDetail details in ResoponseData.details)
        //                {

        //                    item item = SalesDB.db.items.Where(x => x.cloud_id == details.itemLocalId).FirstOrDefault();
        //                    if (item != null)
        //                    {
        //                        sales_invoice_detail _sales_invoice_detail = new sales_invoice_detail()
        //                        {
        //                            State = EntityState.Added,
        //                            sales_invoice = sales_invoice,
        //                            quantity = Convert.ToDecimal(details.quantity),
        //                            unit_price = Convert.ToDecimal(details.price),
        //                            Contact = sales_invoice.contact,
        //                            item_description = item.name,
        //                            item = item,
        //                            id_item = item.id_item,
        //                            id_vat_group = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault().id_vat_group,
        //                            cloud_id = details.cloudId

        //                        };
        //                        sales_invoice.sales_invoice_detail.Add(_sales_invoice_detail);

        //                    }





        //                }



        //            }


        //        }
        //        db.db.SaveChanges();




        //        //return and aasign ids
        //        //archive? if approved or annull and if older than one month.
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }
        //}
        ////private void SyncPromotion()
        ////{
        ////    List<SyncPromotions> SyncPromotionList = new List<SyncPromotions>();
        ////    List<sales_promotion> sales_promotionList = db.db.sales_promotion.Where(x => x.id_company == CurrentSession.Id_Company).ToList();





        ////    foreach (sales_promotion sales_promotion in sales_promotionList)
        ////    {
        ////        if (sales_promotion.type == sales_promotion.salesPromotion.BuyThis_GetThat)
        ////        {
        ////            long? input_id = db.db.items.Where(x => x.id_item == sales_promotion.reference).Select(x => x.cloud_id).FirstOrDefault();
        ////            long? output_id = db.db.items.Where(x => x.id_item == sales_promotion.reference_bonus).Select(x => x.cloud_id).FirstOrDefault();
        ////            SyncPromotions SyncPromotions = new SyncPromotions
        ////            {

        ////                type = (int)sales_promotion.type,
        ////                input_id = (int)input_id,
        ////                output_id = (int)output_id,
        ////                start_date = sales_promotion.date_start,
        ////                end_date = sales_promotion.date_end,
        ////                updated_at = sales_promotion.timestamp,

        ////            };
        ////            SyncPromotionList.Add(SyncPromotions);
        ////        }


        ////    }


        ////    try
        ////    {
        ////        var Customer_Json = new JavaScriptSerializer().Serialize(SyncPromotionList);
        ////        HttpWebResponse httpResponse = Send2API(Customer_Json, "/sync/Promotion");
        ////        if (httpResponse.StatusCode == HttpStatusCode.OK)
        ////        {
        ////            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        ////            {
        ////                var result = streamReader.ReadToEnd();
        ////                List<ResoponseData> Json = new JavaScriptSerializer().Deserialize<List<ResoponseData>>(result);
        ////                foreach (ResoponseData ResoponseData in Json)
        ////                {
        ////                    sales_promotion sales_promotion = db.db.sales_promotion.Where(x => x.id_sales_promotion == ResoponseData.ref_id).FirstOrDefault();
        ////                    sales_promotion.cloud_id = ResoponseData.id;

        ////                }
        ////                db.db.SaveChanges();
        ////            }

        ////        }

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        MessageBox.Show(ex.ToString());
        ////    }


        ////}

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {

                //DownloadVat_click(null, null);
                //DownloadContract_click(null, null);
                //DownloadItem_click(null, null);
                //DownloadCustomer_click(null, null);
                //Downloadtransaction_click(null, null);
            }));

        }
        //private void DownloadVat_click(object sender, RoutedEventArgs e)
        //{

        //    var result = Receive2API("download/saletax");
        //    List<DownloadVat> Vat_Json = new JavaScriptSerializer().Deserialize<List<DownloadVat>>(result);
        //    entity.db db = new db();
        //    db dbvat = new db();
        //    foreach (DownloadVat DownloadVat in Vat_Json)
        //    {
        //        app_vat_group app_vat_group;
        //        app_vat_group = db.app_vat_group.Where(x => x.id_vat_group == DownloadVat.ref_id).FirstOrDefault() ?? new app_vat_group();

        //        app_vat_group.cloud_id = DownloadVat.id;
        //        app_vat_group.name = DownloadVat.name;
        //        foreach (DownloadVatDetail details in DownloadVat.details)
        //        {
        //            app_vat_group_details app_vat_group_details;
        //            app_vat app_vat = db.app_vat.Where(x => x.coefficient == details.coefficient).FirstOrDefault();
        //            if (app_vat == null)
        //            {
        //                app_vat = new app_vat();
        //                app_vat.coefficient = (decimal)details.coefficient;
        //                app_vat.on_product = true;
        //                dbvat.app_vat.Add(app_vat);
        //                dbvat.SaveChanges();
        //            }

        //            app_vat_group_details = db.app_vat_group_details.Where(x => x.id_vat_group_detail == details.ref_id).FirstOrDefault();
        //            if (app_vat_group_details == null)
        //            {

        //                app_vat_group_details = new app_vat_group_details();
        //                app_vat_group_details.id_vat = app_vat.id_vat;
        //                app_vat_group.app_vat_group_details.Add(app_vat_group_details);
        //            }
        //            app_vat_group_details.percentage = details.percent;

        //        }

        //    }

        //    db.SaveChanges();


        //}
        //private void DownloadContract_click(object sender, RoutedEventArgs e)
        //{

        //    var result = Receive2API("download/contract");
        //    List<DownloadContract> Contract_Json = new JavaScriptSerializer().Deserialize<List<DownloadContract>>(result);
        //    entity.db db = new db();
        //    foreach (DownloadContract DownloadContract in Contract_Json)
        //    {
        //        app_contract app_contract;

        //        app_contract = db.app_contract.Where(x => x.id_contract == DownloadContract.ref_id).FirstOrDefault();


        //        if (app_contract == null)
        //        {


        //            app_contract = new app_contract();
        //            app_condition app_condition = db.app_condition.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
        //            app_contract.id_condition = app_condition.id_condition;
        //            db.app_contract.Add(app_contract);
        //        }
        //        app_contract.cloud_id = DownloadContract.id;
        //        app_contract.name = DownloadContract.name;
        //        foreach (DownloadContractDetail details in DownloadContract.details)
        //        {
        //            app_contract_detail app_contract_detail;

        //            app_contract_detail = db.app_contract_detail.Where(x => x.id_contract_detail == details.ref_id).FirstOrDefault();
        //            if (app_contract_detail == null)
        //            {

        //                app_contract_detail = new app_contract_detail();
        //                app_contract.app_contract_detail.Add(app_contract_detail);
        //            }
        //            app_contract_detail.coefficient = details.percent;
        //            app_contract_detail.interval = (short)details.offset;
        //        }

        //    }

        //    db.SaveChanges();


        //}

        //private void DownloadItem_click(object sender, RoutedEventArgs e)
        //{

        //    var result = Receive2API("download/item");
        //    List<DownloadItem> Item_Json = new JavaScriptSerializer().Deserialize<List<DownloadItem>>(result);
        //    entity.db db = new db();
        //    foreach (DownloadItem DownloadItem in Item_Json)
        //    {
        //        item item;

        //        item = db.items.Where(x => x.id_item == DownloadItem.ref_id).FirstOrDefault();


        //        if (item == null)
        //        {


        //            item = new item();
        //            item.id_item_type = item.item_type.Product;
        //            item_product item_product = new item_product();
        //            item.item_product.Add(item_product);
        //            app_vat_group app_vat_group = db.app_vat_group.Where(x => x.cloud_id == DownloadItem.vat_id).FirstOrDefault();
        //            if (app_vat_group != null)
        //            {
        //                item.id_vat_group = app_vat_group.id_vat_group;
        //            }
        //            else
        //            {
        //                app_vat_group = db.app_vat_group.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
        //                item.id_vat_group = app_vat_group.id_vat_group;
        //            }

        //            db.items.Add(item);
        //        }
        //        item.cloud_id = DownloadItem.id;
        //        item.name = DownloadItem.name;
        //        item.sku = DownloadItem.sku;
        //        item.description = DownloadItem.short_description;

        //    }

        //    db.SaveChanges();


        //}

        //private void DownloadCustomer_click(object sender, RoutedEventArgs e)
        //{

        //    var result = Receive2API("download/customer");
        //    List<DownloadCustomer> Customer_Json = new JavaScriptSerializer().Deserialize<List<DownloadCustomer>>(result);
        //    entity.db db = new db();
        //    foreach (DownloadCustomer DownloadCustomer in Customer_Json)
        //    {
        //        contact contact;

        //        contact = db.contacts.Where(x => x.id_contact == DownloadCustomer.ref_id).FirstOrDefault();


        //        if (contact == null)
        //        {


        //            contact = new contact();
        //            contact.id_contact_role = db.contact_role.Where(x => x.is_principal && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_contact_role;
        //            db.contacts.Add(contact);
        //        }
        //        contact.cloud_id = DownloadCustomer.id;
        //        contact.name = DownloadCustomer.customer_alias;
        //        contact.gov_code = DownloadCustomer.customer_taxid;
        //        contact.address = DownloadCustomer.customer_address;
        //        contact.telephone = DownloadCustomer.customer_telephone;
        //        contact.email = DownloadCustomer.customer_email;
        //    }

        //    db.SaveChanges();


        //}
        //private void Downloadtransaction_click(object sender, RoutedEventArgs e)
        //{
        //    var result = Receive2API("download/Order");
        //    List<DownloadInvoice> Sales_Json = new JavaScriptSerializer().Deserialize<List<DownloadInvoice>>(result);
        //    entity.Controller.Sales.InvoiceController SalesDB = new entity.Controller.Sales.InvoiceController();
        //    SalesDB.Initialize();

        //    foreach (DownloadInvoice DownloadInvoice in Sales_Json)
        //    {
        //        if (DownloadInvoice.cloud_id == null)
        //        {
        //            sales_invoice sales_invoice = SalesDB.Create(0, false);
        //            sales_invoice.Location = CurrentSession.Locations.Where(x => x.id_location == Settings.Default.Location).FirstOrDefault();
        //            app_document_range app_document_range = SalesDB.db.app_document_range.Where(x => x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.PointOfSale && x.is_active).FirstOrDefault();
        //            if (app_document_range != null)
        //            {
        //                sales_invoice.id_range = app_document_range.id_range;
        //                sales_invoice.RaisePropertyChanged("id_range");
        //                sales_invoice.app_document_range = app_document_range;
        //            }
        //            contact contact = SalesDB.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.cloud_id == DownloadInvoice.relationship_id).FirstOrDefault();
        //            if (contact == null)

        //            {
        //                using (db db = new db())
        //                {
        //                    contact = new contact();
        //                    contact.name = DownloadInvoice.customer_alias;
        //                    contact.is_customer = true;
        //                    contact.id_contact_role = SalesDB.db.contact_role.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).FirstOrDefault().id_contact_role;
        //                    contact.cloud_id = DownloadInvoice.relationship_id;

        //                    db.contacts.Add(contact);
        //                    db.SaveChanges();
        //                }
        //                contact = SalesDB.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.cloud_id == DownloadInvoice.relationship_id).FirstOrDefault();

        //            }

        //            sales_invoice.id_contact = contact.id_contact;
        //            sales_invoice.contact = contact;
        //            sales_invoice.cloud_id = DownloadInvoice.id;

        //            foreach (DownloadInvoicedetails details in DownloadInvoice.details)
        //            {

        //                item item = SalesDB.db.items.Where(x => x.cloud_id == details.item_id).FirstOrDefault();
        //                if (item == null)
        //                {
        //                    using (db db = new db())
        //                    {
        //                        item = new item();
        //                        item.name = details.item_name;
        //                        item.id_item_type = item.item_type.Product;
        //                        item.id_vat_group = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault().id_vat_group;
        //                        item.cloud_id = details.item_id;
        //                        db.items.Add(item);
        //                        db.SaveChanges();
        //                    }
        //                    item = SalesDB.db.items.Where(x => x.cloud_id == details.item_id).FirstOrDefault();
        //                }


        //                sales_invoice_detail _sales_invoice_detail = new sales_invoice_detail()
        //                {
        //                    State = EntityState.Added,
        //                    sales_invoice = sales_invoice,
        //                    quantity = Convert.ToDecimal(details.quantity),
        //                    Contact = sales_invoice.contact,
        //                    item_description = details.item_name,
        //                    item = item,
        //                    id_item = item.id_item,
        //                    id_vat_group = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault().id_vat_group,
        //                    cloud_id = details.id

        //                };

        //                sales_invoice.sales_invoice_detail.Add(_sales_invoice_detail);

        //            }


        //            crm_opportunity crm_opportunity = new crm_opportunity()
        //            {
        //                id_contact = sales_invoice.id_contact,
        //                id_currency = sales_invoice.id_currencyfx,
        //                value = sales_invoice.GrandTotal
        //            };

        //            crm_opportunity.sales_invoice.Add(sales_invoice);
        //            SalesDB.db.crm_opportunity.Add(crm_opportunity);

        //            SalesDB.db.sales_invoice.Add(sales_invoice);

        //        }
        //        else
        //        {
        //            sales_invoice sales_invoice = SalesDB.db.sales_invoice.Where(x => x.id_sales_invoice == DownloadInvoice.cloud_id).FirstOrDefault();
        //            if (sales_invoice != null)
        //            {
        //                contact contact = SalesDB.db.contacts.Where(x => x.cloud_id == DownloadInvoice.relationship_id).FirstOrDefault();
        //                if (contact == null)
        //                {
        //                    contact = new contact();
        //                    contact.name = DownloadInvoice.customer_alias;
        //                    contact.is_customer = true;

        //                }
        //                sales_invoice.id_contact = contact.id_contact;
        //                sales_invoice.contact = contact;
        //            }
        //            foreach (DownloadInvoicedetails details in DownloadInvoice.details)
        //            {
        //                sales_invoice_detail sales_invoice_detail = SalesDB.db.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == details.cloud_id).FirstOrDefault();
        //                if (sales_invoice_detail != null)
        //                {
        //                    item item = SalesDB.db.items.Where(x => x.cloud_id == details.item_id).FirstOrDefault();
        //                    if (item == null)
        //                    {
        //                        item = new item();
        //                        item.name = details.item_name;
        //                        item.id_item_type = item.item_type.Product;

        //                        sales_invoice_detail.quantity = Convert.ToDecimal(details.quantity);
        //                        sales_invoice_detail.Contact = sales_invoice.contact;
        //                        sales_invoice_detail.item_description = details.item_name;
        //                        sales_invoice_detail.item = item;
        //                        sales_invoice_detail.id_item = item.id_item;

        //                    }
        //                }

        //            }
        //        }
        //        SalesDB.db.SaveChanges();


        //    }
        //}



        private HttpWebResponse Send2API(object Json, string apiname)
        {
            try
            {
                string key = Cognitivo.Properties.Settings.Default.CognitivoKey;
                var webAddr = "";

                webAddr = txtpath.Text + apiname;


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

                return (HttpWebResponse)httpWebRequest.GetResponse();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
                return null;

            }
        }

        private string Receive2API(string apiname)
        {
            try
            {
                string key = Cognitivo.Properties.Settings.Default.CognitivoKey;
                var webAddr = txtpath.Text + "/" + apiname;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + key);

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

    }

    public class SyncVAT
    {
        public SyncVAT()
        {
            details = new List<VATDETAIL>();
        }
        public long? local_id { get; set; }
        public long? cloud_id { get; set; }
        public string name { get; set; }
        public string updated_at { get; set; }
        public List<VATDETAIL> details { get; set; }

    }
    public class VATDETAIL
    {
        public long? id { get; set; }
        public decimal percent { get; set; }
        public decimal coefficient { get; set; }

    }
    public class SyncContract
    {


        public SyncContract()
        {
            details = new List<contract_detail>();
        }
        public long? cloud_id { get; set; }
        public long? local_id { get; set; }
        public string updated_at { get; set; }
        public string name { get; set; }
        public List<contract_detail> details { get; set; }

    }
    public class contract_detail
    {
        public long? id { get; set; }
        public decimal percent { get; set; }
        public decimal offset { get; set; }

    }
    public class SyncItems
    {


        public long? localId { get; set; }
        public string name { get; set; }
        public string sku { get; set; }
        public string comment { get; set; }
        public decimal price { get; set; }
        public long? cloudId { get; set; }
        public int vatId { get; set; }
        public string currencyCode { get; set; }
        public string updatedAt { get; set; }
    }

    public class SyncLocation
    {
        public long? local_id { get; set; }
        public string name { get; set; }
        public string telephone { get; set; }
        public string address { get; set; }
        public decimal city { get; set; }
        public long? cloud_id { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public string updated_at { get; set; }
    }

    public class SyncPromotions
    {
        public long? local_id { get; set; }
        public long? cloud_id { get; set; }
        public int type { get; set; }
        public int input_id { get; set; }
        public int output_id { get; set; }
        public decimal input_value { get; set; }
        public decimal output_value { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public DateTime updated_at { get; set; }

    }
    public class SyncCustomers
    {
        public long? local_id { get; set; }
        public string name { get; set; }
        public string customer_taxid { get; set; }
        public string customer_alias { get; set; }
        public string customer_address { get; set; }
        public string customer_telephone { get; set; }
        public string customer_email { get; set; }
        public long? cloud_id { get; set; }
        public string updated_at { get; set; }
    }

    public class ResoponseVatData
    {


        public ResoponseVatData()
        {
            details = new List<ResoponseVatDetail>();
        }

        public int cloud_id { get; set; }
        public string name { get; set; }
        public int local_id { get; set; }
        public List<ResoponseVatDetail> details { get; set; }

    }
    public class ResoponseVatDetail
    {
        public int cloud_id { get; set; }
        public decimal coefficient { get; set; }
        public decimal percent { get; set; }
    }
    public class ResoponseContractData
    {


        public ResoponseContractData()
        {
            details = new List<ResoponseContractDetail>();
        }

        public int cloud_id { get; set; }
        public string name { get; set; }
        public int local_id { get; set; }
        public List<ResoponseContractDetail> details { get; set; }

    }
    public class ResoponseContractDetail
    {
        public int cloud_id { get; set; }
        public decimal offset { get; set; }
        public decimal percent { get; set; }
    }

    public class ResoponseInvoiceData
    {


        public ResoponseInvoiceData()
        {
            details = new List<ResoponseInvoiceDetail>();
        }

        public int cloud_id { get; set; }
        public int relationship_id { get; set; }
        public int local_id { get; set; }
        public List<ResoponseInvoiceDetail> details { get; set; }

    }
    public class ResoponseInvoiceDetail
    {
        public int cloud_id { get; set; }
        public decimal item_id { get; set; }
        public decimal quantity { get; set; }
        public decimal unit_price { get; set; }
    }

    public class ResoponseItemData
    {

        public int cloud_id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public decimal? price { get; set; }
        public int? vat_cloud_id { get; set; }
        public int local_id { get; set; }


    }
    public class ResoponseCustomerData
    {

        public int cloud_id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string address { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public int local_id { get; set; }


    }


    public class Invoice
    {


        public Invoice()
        {
            details = new List<Detail>();
        }

        public enum Status { Pending = 1, Invoiced = 2, RequestAnnulment = 3, Annuled = 4 }

        public long? relationship_cloud_id { get; set; }
        public int local_id { get; set; }
        public int branch_id { get; set; }
        public string branch_name { get; set; }
        public long cloud_id { get; set; }
        public SyncCustomers customer { get; set; }
        public string number { get; set; }
        public DateTime trans_date { get; set; }
        public DateTime? packing_date { get; set; }
        public int credit_days { get; set; }
        public string currency { get; set; }
        public decimal rate { get; set; }
        public string comment { get; set; }
        public Status status { get; set; }


        public List<Detail> details { get; set; }
    }

    public class Detail
    {


        public int local_id { get; set; }
        public long detail_cloud_id { get; set; }
        public long? item_cloud_id { get; set; }
        public long product_id { get; set; }
        public int vat_id { get; set; }
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public decimal discount { get; set; }
        public bool is_shipped;
        public SyncItems item { get; set; }
    }



    public class DownloadVat
    {
        public DownloadVat()
        {
            details = new List<DownloadVatDetail>();
        }
        public int id { get; set; }
        public int? ref_id { get; set; }
        public string name { get; set; }
        public List<DownloadVatDetail> details { get; set; }
    }
    public class DownloadVatDetail
    {
        public int id { get; set; }
        public int? ref_id { get; set; }
        public decimal? coefficient { get; set; }
        public decimal percent { get; set; }
    }

    public class DownloadContract
    {
        public DownloadContract()
        {
            details = new List<DownloadContractDetail>();
        }
        public int id { get; set; }
        public int? ref_id { get; set; }
        public string name { get; set; }
        public List<DownloadContractDetail> details { get; set; }
    }
    public class DownloadContractDetail
    {
        public int id { get; set; }
        public int? ref_id { get; set; }
        public short? offset { get; set; }
        public decimal percent { get; set; }
    }


    public class DownloadItem
    {

        public int id { get; set; }
        public int? ref_id { get; set; }
        public int? vat_id { get; set; }
        public string name { get; set; }
        public string sku { get; set; }
        public string short_description { get; set; }
        public string long_description { get; set; }
        public decimal unit_price { get; set; }

    }
    public class DownloadCustomer
    {

        public int id { get; set; }
        public int? ref_id { get; set; }
        public string customer_alias { get; set; }
        public string customer_taxid { get; set; }
        public string customer_address { get; set; }
        public string customer_telephone { get; set; }
        public string customer_email { get; set; }

    }

    public class DownloadInvoice
    {
        public DownloadInvoice()
        {
            details = new List<DownloadInvoicedetails>();
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
        public List<DownloadInvoicedetails> details { get; set; }
    }

    public class DownloadInvoicedetails
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
