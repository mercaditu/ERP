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
                Vat_click(null, null);
                Contract_click(null, null);
                SyncItems();
                //SyncPromotion();
                SyncCustomer();
                Sales_click(null, null);


            }));





        }

        private void Vat_click(object sender, RoutedEventArgs e)
        {
            db dbvat = new db();
            List<app_vat_group> app_vat_groupList = db.db.app_vat_group
                   .Where(x => x.id_company == CurrentSession.Id_Company).ToList();
            List<SyncVAT> SyncVATList = new List<SyncVAT>();
            foreach (app_vat_group app_vat_group in app_vat_groupList)
            {
                SyncVAT syncVAT = new SyncVAT();
                syncVAT.local_id = app_vat_group.id_vat_group;
                syncVAT.cloud_id = app_vat_group.cloud_id;
                syncVAT.name = app_vat_group.name;
                syncVAT.updated_at = app_vat_group.timestamp.ToString("yyyy-MM-dd hh:mm:ss");

                foreach (app_vat_group_details app_vat_group_details in app_vat_group.app_vat_group_details)
                {
                    VATDETAIL vatdetail = new VATDETAIL();
                    vatdetail.id = app_vat_group_details.id_vat_group_detail;
                    vatdetail.coefficient = app_vat_group_details.app_vat.coefficient;
                    vatdetail.percent = app_vat_group_details.percentage;
                    syncVAT.details.Add(vatdetail);
                }
                SyncVATList.Add(syncVAT);
            }


            var Vat_Json = new JavaScriptSerializer().Serialize(SyncVATList);
            HttpWebResponse httpResponse = Send2API(Vat_Json, "/sync/saletax");
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                List<app_vat> app_vatList = db.db.app_vat.ToList();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    List<ResoponseVatData> Json = new JavaScriptSerializer().Deserialize<List<ResoponseVatData>>(result);
                    foreach (ResoponseVatData ResoponseData in Json)
                    {
                        if (ResoponseData.local_id > 0)
                        {
                            app_vat_group app_vat_group = db.db.app_vat_group.Where(x => x.id_vat_group == ResoponseData.local_id).FirstOrDefault();
                            app_vat_group.cloud_id = ResoponseData.cloud_id;
                        }
                        else
                        {
                            app_vat_group app_vat_group = new app_vat_group();
                            app_vat_group.cloud_id = ResoponseData.cloud_id;
                            app_vat_group.name = ResoponseData.name;
                            foreach (ResoponseVatDetail details in ResoponseData.details)
                            {
                                app_vat_group_details app_vat_group_details;
                                app_vat app_vat = app_vatList.Where(x => x.coefficient == details.coefficient).FirstOrDefault();
                                if (app_vat == null)
                                {
                                    app_vat = new app_vat();
                                    app_vat.coefficient = (decimal)details.coefficient;
                                    app_vat.on_product = true;
                                    dbvat.app_vat.Add(app_vat);
                                    dbvat.SaveChanges();
                                }
                                app_vat_group_details = new app_vat_group_details();
                                app_vat_group_details.id_vat = app_vat.id_vat;
                                app_vat_group_details.percentage = details.percent;
                                app_vat_group.app_vat_group_details.Add(app_vat_group_details);
                                // create vat group...
                                // foreach detail
                                // check coefficient in detail.
                                // create if not exist app_vat.
                            }
                            db.db.app_vat_group.Add(app_vat_group);
                        }

                        db.db.SaveChanges();
                    }

                }
            }

        }

        private void Contract_click(object sender, RoutedEventArgs e)
        {
            List<app_contract> app_contractList = db.db.app_contract
                   .Where(x => x.id_company == CurrentSession.Id_Company).ToList();
            List<SyncContract> SyncContractList = new List<SyncContract>();
            foreach (app_contract app_contract in app_contractList)
            {
                SyncContract SyncContract = new SyncContract();
                SyncContract.updated_at = app_contract.timestamp.ToString("yyyy-MM-dd hh:mm:ss");
                SyncContract.local_id = app_contract.id_contract;
                SyncContract.cloud_id = app_contract.cloud_id;
                SyncContract.name = app_contract.name;

                foreach (app_contract_detail app_contract_detail in app_contract.app_contract_detail)
                {
                    contract_detail contractdetail = new contract_detail();
                    contractdetail.id = app_contract_detail.id_contract_detail;
                    contractdetail.percent = app_contract_detail.coefficient;
                    contractdetail.offset = app_contract_detail.interval;
                    SyncContract.details.Add(contractdetail);
                }
                SyncContractList.Add(SyncContract);
            }


            var Contract_Json = new JavaScriptSerializer().Serialize(SyncContractList);
            HttpWebResponse httpResponse = Send2API(Contract_Json, "/sync/contract");

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    List<ResoponseContractData> Json = new JavaScriptSerializer().Deserialize<List<ResoponseContractData>>(result);
                    foreach (ResoponseContractData ResoponseData in Json)
                    {
                        if (ResoponseData.local_id > 0)
                        {
                            app_contract app_contract = db.db.app_contract.Where(x => x.id_contract == ResoponseData.local_id).FirstOrDefault();
                            app_contract.cloud_id = ResoponseData.cloud_id;
                        }
                        else
                        {
                            app_contract app_contract = new app_contract();
                            app_contract.cloud_id = ResoponseData.cloud_id;
                            app_contract.name = ResoponseData.name;
                            app_condition app_condition = db.db.app_condition.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
                            app_contract.id_condition = app_condition.id_condition;
                            foreach (ResoponseContractDetail details in ResoponseData.details)
                            {
                                app_contract_detail app_contract_detail = new app_contract_detail();
                                app_contract_detail.id_contract = app_contract.id_contract;
                                app_contract_detail.coefficient = details.percent;
                                app_contract_detail.interval = (short)details.offset;
                                app_contract.app_contract_detail.Add(app_contract_detail);
                                // create vat group...
                                // foreach detail
                                // check coefficient in detail.
                                // create if not exist app_vat.
                            }
                            db.db.app_contract.Add(app_contract);
                        }

                    }
                    db.db.SaveChanges();
                }

            }
        }

        private void SyncItems()
        {
            List<item> items = db.db.items.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.cloud_id == null).ToList();



            List<SyncItems> SyncItems = new List<SyncItems>();

            foreach (item item in items)
            {
                SyncItems SyncItem = new SyncItems
                {
                    cloud_id = item.cloud_id,
                    local_id = item.id_item,
                    name = item.name,
                    code = item.code,
                    comment = item.description,
                    unit_price = item.item_price.FirstOrDefault() != null ? item.item_price.FirstOrDefault().valuewithVAT : 0,
                    currency_code = CurrentSession.Currency_Default.code,
                    updated_at = item.timestamp.ToString("yyyy-MM-dd hh:mm:ss")
                };
                SyncItems.Add(SyncItem);
            }

            for (int i = 0; i <100; i++)
            {
                var Item_Json = new JavaScriptSerializer().Serialize(SyncItems.Skip(i).Take(100));
                HttpWebResponse httpResponse = Send2API(Item_Json, "/sync/item");
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        List<ResoponseItemData> Json = new JavaScriptSerializer().Deserialize<List<ResoponseItemData>>(result);
                        foreach (ResoponseItemData ResoponseData in Json)
                        {
                            if (ResoponseData.local_id > 0)
                            {
                                item item = db.db.items.Where(x => x.id_item == ResoponseData.local_id).FirstOrDefault();
                                item.cloud_id = ResoponseData.cloud_id;
                            }
                            else
                            {
                                item item = new item();
                                item.cloud_id = ResoponseData.cloud_id;
                                item.name = ResoponseData.name;
                                app_vat_group app_vat_group = db.db.app_vat_group.Where(x => x.cloud_id == ResoponseData.vat_cloud_id).FirstOrDefault();
                                if (app_vat_group != null)
                                {
                                    item.id_vat_group = app_vat_group.id_vat_group;
                                }
                                else
                                {
                                    app_vat_group = db.db.app_vat_group.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
                                    item.id_vat_group = app_vat_group.id_vat_group;
                                }
                                item.sku = ResoponseData.code;
                                item.description = ResoponseData.description;
                                item_price item_price = new item_price();
                                item_price.item = item;
                                item_price.value = ResoponseData.price;
                                item.item_price.Add(item_price);
                                db.db.items.Add(item);
                            }

                        }
                        db.db.SaveChanges();
                    }

                }
            }
          

            //run code based on timestamp.
            //run get to see last sync of table. check if new data must go.
            //if cloud.timestamp > local.timestamp then download
            //if cloud.timestamp < local.timestamp then upload
            //vat
            //
            //contract
            //customer
            //item
            //Promo
            //branch
            //location
            //company







        }
        private void SyncCustomer()
        {
            List<SyncCustomers> synccustomers = new List<SyncCustomers>();
            List<contact> contacts = db.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.is_customer).ToList();





            foreach (contact contact in contacts)
            {

                SyncCustomers SyncCustomer = new SyncCustomers
                {
                    cloud_id = contact.cloud_id,
                    local_id = contact.id_contact,
                    name = contact.name,
                    govcode = contact.gov_code,
                    alias = contact.alias,
                    address = contact.address,
                    telephone = contact.telephone,
                    email = contact.email,
                    updated_at = contact.timestamp.ToString("yyyy-MM-dd hh:mm:ss")
                };
                synccustomers.Add(SyncCustomer);
            }


            try
            {
                var Customer_Json = new JavaScriptSerializer().Serialize(synccustomers);
                HttpWebResponse httpResponse = Send2API(Customer_Json, "/sync/customer");
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        List<ResoponseCustomerData> Json = new JavaScriptSerializer().Deserialize<List<ResoponseCustomerData>>(result);
                        foreach (ResoponseCustomerData ResoponseData in Json)
                        {
                            if (ResoponseData.local_id > 0)
                            {
                                contact contact = db.db.contacts.Where(x => x.id_contact == ResoponseData.local_id).FirstOrDefault();
                                contact.cloud_id = ResoponseData.cloud_id;
                            }
                            else
                            {
                                contact contact = new contact();
                                contact.cloud_id = ResoponseData.cloud_id;
                                contact.name = ResoponseData.name;
                                contact.gov_code = ResoponseData.code;
                                contact.address = ResoponseData.address;
                                contact.telephone = ResoponseData.telephone;
                                contact.email = ResoponseData.email;
                                db.db.contacts.Add(contact);
                            }

                        }
                        db.db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }



        }
        private void Sales_click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<sales_invoice> salesinvoices = db.db.sales_invoice
                   .Where(x => x.id_company == CurrentSession.Id_Company && x.is_archived == false).Take(500).ToList();

                List<Invoice> SyncInvoices = new List<Invoice>();

                foreach (sales_invoice sales_invoice in salesinvoices)
                {
                    if (sales_invoice.contact != null)
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
                            relationship_cloud_id = sales_invoice.contact.cloud_id,
                            local_id = sales_invoice.id_sales_invoice,
                            cloud_id = Convert.ToInt64(sales_invoice.cloud_id),
                            number = sales_invoice.number,
                            trans_date = sales_invoice.trans_date,
                            credit_days = 0,
                            currency = sales_invoice.app_currencyfx.app_currency.code,
                            rate = sales_invoice.app_currencyfx.sell_value,
                            comment = sales_invoice.comment,
                            customer = SyncCustomer,
                            branch_id = sales_invoice.app_branch.id_branch,
                            branch_name = sales_invoice.app_branch.name,
                        };

                        if (sales_invoice.status == Status.Documents_General.Approved)
                        {
                            SyncInvoice.status = Invoice.Status.Invoiced;
                        }

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
                                local_id = sales_invoice_detail.id_sales_invoice_detail,
                                detail_cloud_id = Convert.ToInt64(sales_invoice_detail.cloud_id),
                                item_cloud_id = sales_invoice_detail.item.cloud_id,
                                name = sales_invoice_detail.item.name,
                                code = sales_invoice_detail.item.code,
                                //product_id = sales_invoice_detail.id_item,
                                //cloud_id
                                vat = sales_invoice_detail.app_vat_group.app_vat_group_details.FirstOrDefault() != null ? sales_invoice_detail.app_vat_group.app_vat_group_details.FirstOrDefault().percentage : 0,
                                quantity = sales_invoice_detail.quantity,
                                price = sales_invoice_detail.unit_price
                            };

                            Detail.item = SyncItem;
                            SyncInvoice.details.Add(Detail);
                        }
                        SyncInvoices.Add(SyncInvoice);
                    }

                    //archive?
                }

                var Sales_Json = new JavaScriptSerializer().Serialize(SyncInvoices);
                HttpWebResponse httpResponse = Send2API(Sales_Json, "/sync/transaction");
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        List<Invoice> Json = new JavaScriptSerializer().Deserialize<List<Invoice>>(result);
                        foreach (Invoice invoice in Json)
                        {
                            sales_invoice sales_invoice = db.db.sales_invoice.Where(x => x.id_sales_invoice == invoice.local_id).FirstOrDefault();

                            if (sales_invoice != null)
                            {
                                sales_invoice.cloud_id = invoice.cloud_id;
                                sales_invoice.contact.cloud_id = invoice.customer.cloud_id;
                                foreach (Detail detail in invoice.details)
                                {
                                    sales_invoice_detail sales_invoice_detail = sales_invoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == detail.local_id).FirstOrDefault();
                                    if (sales_invoice_detail != null)
                                    {
                                        sales_invoice_detail.cloud_id = detail.detail_cloud_id;
                                        sales_invoice_detail.item.cloud_id = detail.item.cloud_id;
                                    }
                                }
                            }

                        }
                        db.db.SaveChanges();
                    }

                }
               
                //return and aasign ids
                //archive? if approved or annull and if older than one month.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //private void SyncPromotion()
        //{
        //    List<SyncPromotions> SyncPromotionList = new List<SyncPromotions>();
        //    List<sales_promotion> sales_promotionList = db.db.sales_promotion.Where(x => x.id_company == CurrentSession.Id_Company).ToList();





        //    foreach (sales_promotion sales_promotion in sales_promotionList)
        //    {
        //        if (sales_promotion.type == sales_promotion.salesPromotion.BuyThis_GetThat)
        //        {
        //            long? input_id = db.db.items.Where(x => x.id_item == sales_promotion.reference).Select(x => x.cloud_id).FirstOrDefault();
        //            long? output_id = db.db.items.Where(x => x.id_item == sales_promotion.reference_bonus).Select(x => x.cloud_id).FirstOrDefault();
        //            SyncPromotions SyncPromotions = new SyncPromotions
        //            {

        //                type = (int)sales_promotion.type,
        //                input_id = (int)input_id,
        //                output_id = (int)output_id,
        //                start_date = sales_promotion.date_start,
        //                end_date = sales_promotion.date_end,
        //                updated_at = sales_promotion.timestamp,

        //            };
        //            SyncPromotionList.Add(SyncPromotions);
        //        }


        //    }


        //    try
        //    {
        //        var Customer_Json = new JavaScriptSerializer().Serialize(SyncPromotionList);
        //        HttpWebResponse httpResponse = Send2API(Customer_Json, "/sync/Promotion");
        //        if (httpResponse.StatusCode == HttpStatusCode.OK)
        //        {
        //            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //            {
        //                var result = streamReader.ReadToEnd();
        //                List<ResoponseData> Json = new JavaScriptSerializer().Deserialize<List<ResoponseData>>(result);
        //                foreach (ResoponseData ResoponseData in Json)
        //                {
        //                    sales_promotion sales_promotion = db.db.sales_promotion.Where(x => x.id_sales_promotion == ResoponseData.ref_id).FirstOrDefault();
        //                    sales_promotion.cloud_id = ResoponseData.id;

        //                }
        //                db.db.SaveChanges();
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }


        //}

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {

                DownloadVat_click(null, null);
                DownloadContract_click(null, null);
                DownloadItem_click(null, null);
                DownloadCustomer_click(null, null);
                Downloadtransaction_click(null, null);
            }));

        }
        private void DownloadVat_click(object sender, RoutedEventArgs e)
        {

            var result = Receive2API("download/saletax");
            List<DownloadVat> Vat_Json = new JavaScriptSerializer().Deserialize<List<DownloadVat>>(result);
            entity.db db = new db();
            db dbvat = new db();
            foreach (DownloadVat DownloadVat in Vat_Json)
            {
                app_vat_group app_vat_group;
                app_vat_group = db.app_vat_group.Where(x => x.id_vat_group == DownloadVat.ref_id).FirstOrDefault() ?? new app_vat_group();

                app_vat_group.cloud_id = DownloadVat.id;
                app_vat_group.name = DownloadVat.name;
                foreach (DownloadVatDetail details in DownloadVat.details)
                {
                    app_vat_group_details app_vat_group_details;
                    app_vat app_vat = db.app_vat.Where(x => x.coefficient == details.coefficient).FirstOrDefault();
                    if (app_vat == null)
                    {
                        app_vat = new app_vat();
                        app_vat.coefficient = (decimal)details.coefficient;
                        app_vat.on_product = true;
                        dbvat.app_vat.Add(app_vat);
                        dbvat.SaveChanges();
                    }

                    app_vat_group_details = db.app_vat_group_details.Where(x => x.id_vat_group_detail == details.ref_id).FirstOrDefault();
                    if (app_vat_group_details == null)
                    {

                        app_vat_group_details = new app_vat_group_details();
                        app_vat_group_details.id_vat = app_vat.id_vat;
                        app_vat_group.app_vat_group_details.Add(app_vat_group_details);
                    }
                    app_vat_group_details.percentage = details.percent;

                }

            }

            db.SaveChanges();


        }
        private void DownloadContract_click(object sender, RoutedEventArgs e)
        {

            var result = Receive2API("download/contract");
            List<DownloadContract> Contract_Json = new JavaScriptSerializer().Deserialize<List<DownloadContract>>(result);
            entity.db db = new db();
            foreach (DownloadContract DownloadContract in Contract_Json)
            {
                app_contract app_contract;

                app_contract = db.app_contract.Where(x => x.id_contract == DownloadContract.ref_id).FirstOrDefault();


                if (app_contract == null)
                {


                    app_contract = new app_contract();
                    app_condition app_condition = db.app_condition.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
                    app_contract.id_condition = app_condition.id_condition;
                    db.app_contract.Add(app_contract);
                }
                app_contract.cloud_id = DownloadContract.id;
                app_contract.name = DownloadContract.name;
                foreach (DownloadContractDetail details in DownloadContract.details)
                {
                    app_contract_detail app_contract_detail;

                    app_contract_detail = db.app_contract_detail.Where(x => x.id_contract_detail == details.ref_id).FirstOrDefault();
                    if (app_contract_detail == null)
                    {

                        app_contract_detail = new app_contract_detail();
                        app_contract.app_contract_detail.Add(app_contract_detail);
                    }
                    app_contract_detail.coefficient = details.percent;
                    app_contract_detail.interval = (short)details.offset;
                }

            }

            db.SaveChanges();


        }

        private void DownloadItem_click(object sender, RoutedEventArgs e)
        {

            var result = Receive2API("download/item");
            List<DownloadItem> Item_Json = new JavaScriptSerializer().Deserialize<List<DownloadItem>>(result);
            entity.db db = new db();
            foreach (DownloadItem DownloadItem in Item_Json)
            {
                item item;

                item = db.items.Where(x => x.id_item == DownloadItem.ref_id).FirstOrDefault();


                if (item == null)
                {


                    item = new item();
                    item.id_item_type = item.item_type.Product;
                    item_product item_product = new item_product();
                    item.item_product.Add(item_product);
                    app_vat_group app_vat_group = db.app_vat_group.Where(x => x.cloud_id == DownloadItem.vat_id).FirstOrDefault();
                    if (app_vat_group != null)
                    {
                        item.id_vat_group = app_vat_group.id_vat_group;
                    }
                    else
                    {
                        app_vat_group = db.app_vat_group.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
                        item.id_vat_group = app_vat_group.id_vat_group;
                    }

                    db.items.Add(item);
                }
                item.cloud_id = DownloadItem.id;
                item.name = DownloadItem.name;
                item.sku = DownloadItem.sku;
                item.description = DownloadItem.short_description;

            }

            db.SaveChanges();


        }

        private void DownloadCustomer_click(object sender, RoutedEventArgs e)
        {

            var result = Receive2API("download/customer");
            List<DownloadCustomer> Customer_Json = new JavaScriptSerializer().Deserialize<List<DownloadCustomer>>(result);
            entity.db db = new db();
            foreach (DownloadCustomer DownloadCustomer in Customer_Json)
            {
                contact contact;

                contact = db.contacts.Where(x => x.id_contact == DownloadCustomer.ref_id).FirstOrDefault();


                if (contact == null)
                {


                    contact = new contact();
                    contact.id_contact_role = db.contact_role.Where(x => x.is_principal && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_contact_role;
                    db.contacts.Add(contact);
                }
                contact.cloud_id = DownloadCustomer.id;
                contact.name = DownloadCustomer.customer_alias;
                contact.gov_code = DownloadCustomer.customer_taxid;
                contact.address = DownloadCustomer.customer_address;
                contact.telephone = DownloadCustomer.customer_telephone;
                contact.email = DownloadCustomer.customer_email;
            }

            db.SaveChanges();


        }
        private void Downloadtransaction_click(object sender, RoutedEventArgs e)
        {
            var result = Receive2API("download/Order");
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

                    foreach (DownloadInvoicedetails details in DownloadInvoice.details)
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
                    foreach (DownloadInvoicedetails details in DownloadInvoice.details)
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
        public long? local_id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string comment { get; set; }
        public decimal unit_price { get; set; }
        public long? cloud_id { get; set; }
        public string currency_code { get; set; }
        public string updated_at { get; set; }
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
        public string govcode { get; set; }
        public string alias { get; set; }
        public string address { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
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

    public class ResoponseItemData
    {

        public int cloud_id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public int vat_cloud_id { get; set; }
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

        public enum Status { Invoiced = 3, Packed = 4 }

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
        public decimal vat { get; set; }
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public decimal discount { get; set; }
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
