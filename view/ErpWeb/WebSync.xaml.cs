using Cognitivo.API;
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
using System.Windows.Input;

namespace Cognitivo.ErpWeb
{
    /// <summary>
    /// Interaction logic for WebSync.xaml
    /// </summary>
    public partial class WebSync : Page
    {
        private dbContext db = new dbContext();
        Cognitivo.API.Upload send;
        public bool serverStatus { get; set; }
        public bool apiStatus { get; set; }
        string slug = "";

        public WebSync()
        {
            InitializeComponent();
            db.db = new db();
            CheckStatus(null, null);
            cmbSyncType.ItemsSource = Enum.GetValues(typeof(Cognitivo.API.Enums.SyncWith));
            endDate.SelectedDate = DateTime.Now;
            startDate.SelectedDate = DateTime.Now.AddDays(-30);
            slug = db.db.app_company.Find(CurrentSession.Id_Company).domain;
            tbxURL.Content = slug;

        }

        private void OpenConfig(object sender, MouseButtonEventArgs e) => popConnBuilder.IsOpen = true;

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

        private void ClickInformation(object sender, EventArgs e)
        {
            Cognitivo.Properties.Settings.Default.Save();


            app_company app_company = db.db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault();
            app_company.domain = Cognitivo.Properties.Settings.Default.Slug;
            db.db.SaveChanges();
            slug = db.db.app_company.Find(CurrentSession.Id_Company).domain;
            tbxURL.Content = slug;
            popConnBuilder.IsOpen = !popConnBuilder.IsOpen;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Cognitivo.API.Enums.SyncWith SyncType = (Cognitivo.API.Enums.SyncWith)cmbSyncType.SelectedItem;
            UploadData(slug, SyncType, startDate.SelectedDate.Value, endDate.SelectedDate.Value);
        }

        private void UploadData(string slug, Cognitivo.API.Enums.SyncWith SyncType, DateTime start, DateTime end)
        {

            Task Branch_Task = Task.Factory.StartNew(() => SyncBranch(slug, SyncType));
            Task Contract_Task = Task.Factory.StartNew(() => SyncContract(slug, SyncType));
          

            Branch_Task.Wait();
            Contract_Task.Wait();
          

            //Task Customer_Task = Task.Factory.StartNew(() => SyncCustomer(slug, SyncType));
            //  Customer_Task.Wait();

            Task Item_Task = Task.Factory.StartNew(() => SyncItem(slug, SyncType));
            Item_Task.Wait();

           // Task Item_Upload_Task = Task.Factory.StartNew(() => Image_upload(slug, SyncType));


             Task Sale_Task = Task.Factory.StartNew(() => Sales_click(slug, SyncType, start, end));
               Sale_Task.Wait();

            Task Opportunity_Task = Task.Factory.StartNew(() => SyncOpportunity(slug, SyncType));


        }

        //private void SyncVat(string slug, Cognitivo.API.Enums.SyncWith SyncType)
        //{
        //    send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, SyncType);

        //    using (db db = new db())
        //    {
        //        List<app_vat_group> app_vat_groupList = db.app_vat_group
        //           .Where(x => x.id_company == CurrentSession.Id_Company).ToList();

        //        Dispatcher.BeginInvoke((Action)(() => vatMaximum.Text = app_vat_groupList.Count.ToString()));
        //        Dispatcher.BeginInvoke((Action)(() => progVat.Value = 0));
        //        int count = 0;

        //        List<object> SyncVATList = new List<object>();

        //        foreach (app_vat_group app_vat_group in app_vat_groupList)
        //        {
        //            Vat syncVAT = new Vat();
        //            syncVAT.localId = app_vat_group.id_vat_group;
        //            syncVAT.cloudId = app_vat_group.cloud_id;
        //            syncVAT.updatedAt = app_vat_group.timestamp;
        //            syncVAT.createdAt = app_vat_group.timestamp;
        //            syncVAT.name = app_vat_group.name;

        //            foreach (app_vat_group_details app_vat_group_details in app_vat_group.app_vat_group_details)
        //            {
        //                VatDetail VatDetail = new VatDetail();
        //                VatDetail.coefficient = app_vat_group_details.app_vat.coefficient;
        //                VatDetail.name = app_vat_group_details.app_vat.name;
        //                VatDetail.percentage = app_vat_group_details.percentage;
        //                VatDetail.updatedAt = app_vat_group_details.timestamp;
        //                syncVAT.details.Add(VatDetail);
        //            }

        //            SyncVATList.Add(syncVAT);

        //            count = count + 1;
        //            Dispatcher.BeginInvoke((Action)(() => progVat.Value = count));
        //            Dispatcher.BeginInvoke((Action)(() => vatValue.Text = count.ToString()));
        //        }


        //        SyncVATList = send.Vats(slug, SyncVATList).OfType<object>().ToList();
        //        List<app_vat> app_vatList = CurrentSession.VATs.ToList();


        //        Dispatcher.BeginInvoke((Action)(() => progVat.IsIndeterminate = true));

        //        foreach (Vat ResoponseData in SyncVATList)
        //        {
        //            if (ResoponseData.action == API.Enums.Action.CreateOnCloud)
        //            {
        //                app_vat_group app_vat_group = app_vat_groupList.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
        //                app_vat_group.cloud_id = ResoponseData.cloudId;

        //            }
        //            else if (ResoponseData.action == API.Enums.Action.UpdateOnLocal)
        //            {
        //                app_vat_group app_vat_group = app_vat_groupList.Where(x => x.id_vat_group == ResoponseData.localId).FirstOrDefault();
        //                app_vat_group.cloud_id = ResoponseData.cloudId;

        //                if (Convert.ToDateTime(ResoponseData.updatedAt).ToUniversalTime() > app_vat_group.timestamp)
        //                {
        //                    app_vat_group.name = ResoponseData.name;
        //                }
        //            }

        //            else if (ResoponseData.action == API.Enums.Action.CreateOnLocal)
        //            {
        //                app_vat_group app_vat_group = new app_vat_group();
        //                app_vat_group.cloud_id = ResoponseData.cloudId;
        //                app_vat_group.name = ResoponseData.name;

        //                foreach (VatDetail details in ResoponseData.details)
        //                {
        //                    create group.
        //                    app_vat app_vat = app_vatList.Where(x => x.coefficient == details.coefficient).FirstOrDefault();
        //                    int vatId = 0;

        //                    if (app_vat == null)
        //                    {
        //                        vatId = 0;

        //                        using (db db2 = new db())
        //                        {
        //                            app_vat = new app_vat();
        //                            app_vat.coefficient = details.coefficient;
        //                            app_vat.on_product = true;
        //                            app_vat.name = ResoponseData.name;
        //                            db2.app_vat.Add(app_vat);
        //                            db2.SaveChanges();

        //                            vatId = app_vat.id_vat;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        vatId = app_vat.id_vat;
        //                    }

        //                    app_vat_group_details app_vat_group_details = new app_vat_group_details
        //                    {
        //                        id_vat = vatId,
        //                        percentage = details.percentage
        //                    };

        //                    app_vat_group.app_vat_group_details.Add(app_vat_group_details);
        //                }

        //                db.app_vat_group.Add(app_vat_group);

        //                count = count + 1;
        //                Dispatcher.BeginInvoke((Action)(() => progVat.Value = count));
        //                Dispatcher.BeginInvoke((Action)(() => vatValue.Text = count.ToString()));
        //            }

        //        }

        //        Dispatcher.BeginInvoke((Action)(() => progVat.IsIndeterminate = false));

        //        db.SaveChanges();
        //    }
        //}

        private void SyncContract(string slug, Cognitivo.API.Enums.SyncWith SyncType)
        {
            send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, SyncType);

            using (db db = new db())
            {
                List<app_contract> app_contractList = db.app_contract
                   .Where(x => x.id_company == CurrentSession.Id_Company).ToList();

                Dispatcher.BeginInvoke((Action)(() => contractMaximum.Text = app_contractList.Count.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progContract.Value = 0));
                int count = 0;

                List<object> SyncContractList = new List<object>();

                foreach (app_contract app_contract in app_contractList)
                {
                    PaymentContract syncContract = new PaymentContract();
                    syncContract.localId = app_contract.id_contract;
                    syncContract.cloudId = app_contract.cloud_id != null ? (int)app_contract.cloud_id : 0;
                    syncContract.updatedAt = app_contract.timestamp;
                    syncContract.createdAt = app_contract.timestamp;
                    syncContract.name = app_contract.name;
                    foreach (app_contract_detail app_contract_detail in app_contract.app_contract_detail)
                    {
                        Cognitivo.API.Models.PaymentContractDetail PaymentContractDetail = new Cognitivo.API.Models.PaymentContractDetail();
                        PaymentContractDetail.offset = app_contract_detail.interval;
                        PaymentContractDetail.percent = app_contract_detail.coefficient;
                        PaymentContractDetail.updatedAt = app_contract_detail.timestamp;
                        syncContract.details.Add(PaymentContractDetail);
                    }

                    SyncContractList.Add(syncContract);

                    count = count + 1;
                    Dispatcher.BeginInvoke((Action)(() => progContract.Value = count));
                    Dispatcher.BeginInvoke((Action)(() => contractValue.Text = count.ToString()));
                }

                SyncContractList = send.PaymentContracts(slug, SyncContractList).OfType<object>().ToList();
                // List<app_contract> app_contracts = CurrentSession.Contracts;
                List<app_contract> app_contracts = db.app_contract.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

                Dispatcher.BeginInvoke((Action)(() => progContract.IsIndeterminate = true));
                foreach (Cognitivo.API.Models.PaymentContract ResoponseData in SyncContractList)
                {
                    if (ResoponseData.action == API.Enums.Action.CreateOnCloud)
                    {
                        app_contract app_contract = app_contracts.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
                        app_contract.cloud_id = ResoponseData.cloudId;

                    }
                    else if (ResoponseData.action == API.Enums.Action.UpdateOnLocal)
                    {

                        app_contract app_contract = app_contracts.Where(x => x.id_contract == ResoponseData.localId).FirstOrDefault();
                        app_contract.cloud_id = ResoponseData.cloudId;

                        if (Convert.ToDateTime(ResoponseData.updatedAt).ToUniversalTime() > app_contract.timestamp)
                        {
                            app_contract.name = ResoponseData.name;
                        }
                    }

                }

                Dispatcher.BeginInvoke((Action)(() => progContract.IsIndeterminate = false));
                db.SaveChanges();
            }
        }

        //private void SyncCustomer(string slug, Cognitivo.API.Enums.SyncWith SyncType)
        //{
        //    send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, SyncType);

        //    using (db db = new db())
        //    {
        //        List<object> synccustomers = new List<object>();
        //        List<contact> contacts = db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.is_customer
        //        && (x.email != null || x.email != "")).ToList();

        //        Dispatcher.BeginInvoke((Action)(() => customerMaximum.Text = contacts.Count.ToString()));
        //        Dispatcher.BeginInvoke((Action)(() => progCustomer.Value = 0));
        //        int count = 0;

        //        int id_contact_role = db.contact_role.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.is_principal).Select(x => x.id_contact_role).FirstOrDefault();
        //        int id_price_list = db.item_price_list.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.is_default).Select(x => x.id_price_list).FirstOrDefault();

        //        foreach (contact contact in contacts)
        //        {

        //            Customer SyncCustomer = new Customer
        //            {
        //                cloudId = Convert.ToInt32(contact.cloud_id ?? 0),
        //                localId = contact.id_contact,
        //                alias = contact.name,
        //                creditLimit = contact.credit_limit,
        //                taxId = contact.gov_code,
        //                address = contact.address,
        //                telephone = contact.telephone,
        //                email = contact.email,
        //                leadTime = contact.lead_time,
        //                updatedAt = contact.timestamp,
        //                createdAt = contact.timestamp,
        //            };
        //            synccustomers.Add(SyncCustomer);

        //            count = count + 1;
        //            Dispatcher.BeginInvoke((Action)(() => progCustomer.Value = count));
        //            Dispatcher.BeginInvoke((Action)(() => customerValue.Text = count.ToString()));
        //        }

        //        try
        //        {
        //            int totalcustomer = synccustomers.Count();
        //            List<object> Customers = new List<object>();
        //            for (int i = 0; i < totalcustomer; i = i + 1000)
        //            {
        //                Customers = send.Customer(slug, synccustomers.Skip(i).Take(1000).ToList()).OfType<object>().ToList();
        //                Dispatcher.BeginInvoke((Action)(() => progCustomer.IsIndeterminate = true));
        //                foreach (Cognitivo.API.Models.Customer ResoponseData in Customers)
        //                {
        //                    if (ResoponseData.name != null)
        //                    {
        //                        if (ResoponseData.action == API.Enums.Action.CreateOnCloud)
        //                        {
        //                            contact contact = contacts.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
        //                            contact.cloud_id = ResoponseData.cloudId;
        //                        }
        //                        else if (ResoponseData.action == API.Enums.Action.UpdateOnLocal)
        //                        {

        //                            contact contact = contacts.Where(x => x.id_contact == ResoponseData.localId).FirstOrDefault();
        //                            contact.cloud_id = ResoponseData.cloudId;

        //                            if (Convert.ToDateTime(ResoponseData.updatedAt).ToUniversalTime() > contact.timestamp)
        //                            {
        //                                contact.name = ResoponseData.name;
        //                                contact.gov_code = ResoponseData.taxId;
        //                                contact.address = ResoponseData.address;
        //                                contact.telephone = ResoponseData.telephone;
        //                                contact.email = ResoponseData.email;
        //                            }
        //                        }

        //                        else if (ResoponseData.action == API.Enums.Action.CreateOnLocal)
        //                        {
        //                            contact contact = new contact();
        //                            contact.id_contact_role = id_contact_role;
        //                            contact.id_price_list = id_price_list;
        //                            contact.cloud_id = ResoponseData.cloudId;
        //                            contact.name = ResoponseData.alias;
        //                            contact.gov_code = ResoponseData.taxId;
        //                            contact.address = ResoponseData.address;
        //                            contact.telephone = ResoponseData.telephone;
        //                            contact.email = ResoponseData.email;
        //                            db.contacts.Add(contact);
        //                        }

        //                    }
        //                }
        //                db.SaveChanges();
        //            }


        //            Dispatcher.BeginInvoke((Action)(() => progCustomer.IsIndeterminate = false));


        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.ToString());
        //        }
        //    }
        //}

        private void SyncItem(string slug, Cognitivo.API.Enums.SyncWith SyncType)
        {
            send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, SyncType);

            using (db db = new db())
            {
                List<item> items = db.items.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).ToList();

                Dispatcher.BeginInvoke((Action)(() => itemMaximum.Text = items.Count.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progItem.Value = 0));
                int count = 0;
                List<object> SyncItems = new List<object>();
               
                foreach (item item in items)
                {
                    Cognitivo.API.Models.Item SyncItem = new Cognitivo.API.Models.Item
                    {
                        cloudId = item.cloud_id != null ? (int)item.cloud_id : 0,
                        localId = item.id_item,
                        name = item.name,
                        sku = item.code,
                        shortDescription = item.description,
                        barCode = item.code,
                        price = item.item_price.FirstOrDefault() != null ? item.item_price.FirstOrDefault().value : 0,
                        cost = item.unit_cost,
                        currencyCode = CurrentSession.Currency_Default.code,
                        updatedAt = item.timestamp,
                        createdAt = item.timestamp,
                        isActive = item.is_active,
                        isStockable = item.id_item_type == item.item_type.Product ? true : false,
                        vatCloudId = item.app_vat_group.cloud_id,
                    };
                    //List<Cognitivo.API.Models.Attachments> Attachments = new List<Cognitivo.API.Models.Attachments>();
                    //db.app_attachment.Load();
                    //List<app_attachment> item_Attachments = db.app_attachment.Where(x => x.reference_id == item.id_item && x.application == entity.App.Names.Items).ToList();
                    //if (item_Attachments.Count() > 0)
                    //{
                    //    foreach (app_attachment app_attachment in item_Attachments)
                    //    {
                    //        Cognitivo.API.Models.Attachments attachment = new Attachments();
                    //        attachment.attachment = app_attachment.file;
                    //        Attachments.Add(attachment);
                    //    }

                    //}


                    try
                    {
                       // SyncItem.attachments.AddRange(Attachments);
                        if (db.item_tag_detail.Where(x => x.id_item == item.id_item).Count() > 0)
                        {
                            int i = 0;
                            foreach (item_tag_detail item_tag_detail in db.item_tag_detail.Where(x => x.id_item == item.id_item).ToList())
                            {
                                if (item_tag_detail.item_tag != null && item_tag_detail.item_tag.name != null)
                                {
                                    SyncItem.tags[i] = item_tag_detail.item_tag.name;
                                    i = i + 1;
                                }

                            }

                        }

                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                    SyncItem.tags =SyncItem.tags.Where(x => x != null).ToArray();
                    SyncItems.Add(SyncItem);

                   

                    
                    count = count + 1;
                    Dispatcher.BeginInvoke((Action)(() => progItem.Value = count));
                    Dispatcher.BeginInvoke((Action)(() => itemValue.Text = count.ToString()));
                }
                int totalItems = SyncItems.Count();
                List<object> Items = new List<object>();

                if (SyncItems.Count() > 0)
                {
                    //for (int i = 0; i < SyncItems.Count; i = i + 100)
                    //{
                        Items = new List<object>();
                        List<object> ListItems = new List<object>();
                        Items = send.Item(slug, SyncItems.ToList()).OfType<object>().ToList();

                        Dispatcher.BeginInvoke((Action)(() => progItem.IsIndeterminate = true));
                        foreach (Cognitivo.API.Models.Item ResoponseData in Items)
                        {

                            if (ResoponseData.action == API.Enums.Action.CreateOnCloud)
                            {
                                item clouditem = items.Where(x => x.cloud_id == ResoponseData.cloudId).FirstOrDefault();
                                clouditem.cloud_id = ResoponseData.cloudId;
                            }
                            else if (ResoponseData.action == API.Enums.Action.UpdateOnLocal)
                            {
                                item clouditem = items.Where(x => x.id_item == ResoponseData.localId).FirstOrDefault();
                                if (clouditem != null)
                                {
                                    clouditem.cloud_id = ResoponseData.cloudId;

                                    if (Convert.ToDateTime(ResoponseData.updatedAt).ToUniversalTime() > clouditem.timestamp)
                                    {
                                        clouditem.name = ResoponseData.name;
                                        app_vat_group app_vat_group = db.app_vat_group.Where(x => x.cloud_id == ResoponseData.vatCloudId).FirstOrDefault();
                                        if (app_vat_group != null)
                                        {
                                            clouditem.id_vat_group = app_vat_group.id_vat_group;
                                        }
                                        else
                                        {
                                            app_vat_group = db.app_vat_group.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
                                            clouditem.id_vat_group = app_vat_group.id_vat_group;
                                        }
                                        clouditem.sku = ResoponseData.sku;
                                    }
                                }

                            }

                            else if (ResoponseData.action == API.Enums.Action.CreateOnLocal)
                            {
                                item clouditem = new item();

                                clouditem.cloud_id = ResoponseData.cloudId;
                                clouditem.name = ResoponseData.name;
                                clouditem.id_item_type = item.item_type.Product;

                                app_vat_group app_vat_group = db.app_vat_group.Where(x => x.cloud_id == ResoponseData.vatCloudId).FirstOrDefault();
                                if (app_vat_group != null)
                                {
                                    clouditem.id_vat_group = app_vat_group.id_vat_group;
                                }
                                else
                                {
                                    app_vat_group = db.app_vat_group.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
                                    clouditem.id_vat_group = app_vat_group.id_vat_group;
                                }

                                clouditem.sku = ResoponseData.sku;
                                item_price item_price = new item_price();
                                item_price.id_price_list = (CurrentSession.PriceLists.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault() ?? CurrentSession.PriceLists.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault()).id_price_list;
                                item_price.item = clouditem;
                                item_price.value = Convert.ToDecimal(ResoponseData.price);
                                app_currency app_currency = CurrentSession.Currencies.Where(x => x.code == ResoponseData.currencyCode).FirstOrDefault();
                                if (app_currency != null)
                                {
                                    item_price.id_currency = app_currency.id_currency;

                                }
                                else
                                {
                                    item_price.id_currency = CurrentSession.Currency_Default.id_currency;
                                }

                                clouditem.item_price.Add(item_price);
                                db.items.Add(clouditem);
                            }


                        }

                        Dispatcher.BeginInvoke((Action)(() => progItem.IsIndeterminate = false));
                        db.SaveChanges();
                   // }
                }

            }
        }

        private void Image_upload(string slug, Cognitivo.API.Enums.SyncWith SyncType)
        {
            send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, SyncType);

            using (db db = new db())
            {
                List<item> items = db.items.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).ToList();
                List<int> item_attachments = db.app_attachment.Where(x => x.application == entity.App.Names.Items).Select(x=>x.reference_id).ToList();
                List<item> ItemWithAttachment = items.Where(x => item_attachments.Contains(x.id_item)).ToList();
               
                foreach (item item in ItemWithAttachment)
                {
                    try
                    {
                        List<object> Attachments = new List<object>();
                        List<app_attachment> item_Attachments = db.app_attachment.Where(x => x.application == entity.App.Names.Items && x.reference_id == item.id_item).ToList();
                        foreach (app_attachment app_attachment in item_Attachments)
                        {
                            Cognitivo.API.Models.Attachments attachment = new Attachments();
                            send.ItemImage(slug, app_attachment.file);
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                   

                   
                }
            }
        }
        private void Sales_click(string slug, Cognitivo.API.Enums.SyncWith SyncType, DateTime start, DateTime end)
        {
            try
            {



                List<sales_invoice> ErrorInvoices = new List<sales_invoice>();
                List<sales_invoice> salesinvoices = db.db.sales_invoice
                   .Where(x => x.id_company == CurrentSession.Id_Company && x.is_archived == false && x.status == Status.Documents_General.Approved
                   &&
                   (x.timestamp >= start && x.timestamp <= end)
                   &&
                 (x.contact.email != null || x.contact.email != ""))
                   .ToList();

                Dispatcher.BeginInvoke((Action)(() => salesMaximum.Text = salesinvoices.Count.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progSales.Value = 0));
                int count = 0;

                List<object> SyncInvoices = new List<object>();
                send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, SyncType);
                app_branch app_branch = CurrentSession.Branches.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault();
                string location = "";
                if (app_branch != null)
                {
                    location = app_branch.name;
                }


                foreach (sales_invoice sales_invoice in salesinvoices)
                {

                    if (sales_invoice.contact != null && sales_invoice.contact.email != "")
                    {

                        Cognitivo.API.Models.Customer customer = new Customer();
                        customer.name = sales_invoice.contact.name;
                        customer.taxId = sales_invoice.contact.gov_code;
                        customer.telephone = sales_invoice.contact.telephone;
                        customer.email = sales_invoice.contact.email;
                        customer.address = sales_invoice.contact.address;


                        Cognitivo.API.Models.Sales SyncInvoice = new Cognitivo.API.Models.Sales
                        {
                            customerCloudId = Convert.ToInt32(sales_invoice.contact.cloud_id ?? 0),
                            customer = customer,
                            localId = sales_invoice.id_sales_invoice,
                            cloudId = Convert.ToInt32(sales_invoice.cloud_id ?? 0),
                            invoiceNumber = sales_invoice.number,
                            date = sales_invoice.trans_date.ToUniversalTime(),
                            currencyCode = sales_invoice.app_currencyfx.app_currency.code,
                            rate = sales_invoice.app_currencyfx.sell_value,
                            locationName = location,
                            updatedAt = sales_invoice.timestamp,
                            createdAt = sales_invoice.timestamp,
                            sentMail = false
                        };

                        if (sales_invoice.status == Status.Documents_General.Approved)
                        {
                            SyncInvoice.status = API.Enums.Status.Approved;
                        }
                        else if (sales_invoice.status == Status.Documents_General.Annulled)
                        {
                            SyncInvoice.status = API.Enums.Status.Annulled;
                        }
                        else
                        {
                            SyncInvoice.status = API.Enums.Status.Pending;
                        }

                        foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
                        {
                            //SyncItems SyncItem = new SyncItems
                            //{
                            //    local_id = sales_invoice_detail.item.id_item,
                            //    name = sales_invoice_detail.item.name,
                            //    code = sales_invoice_detail.item.code,
                            //    comment = sales_invoice_detail.item.description,
                            //    unit_price = sales_invoice_detail.item.item_price.FirstOrDefault() != null ? sales_invoice_detail.item.item_price.FirstOrDefault().valuewithVAT : 0,
                            //};
                            Cognitivo.API.Models.SalesDetail Detail = new Cognitivo.API.Models.SalesDetail
                            {
                                localId = sales_invoice_detail.id_sales_invoice_detail,
                                cloudId = Convert.ToInt32(sales_invoice_detail.cloud_id ?? 0),
                                itemCloudId = Convert.ToInt32(sales_invoice_detail.item.cloud_id ?? 0),
                                name = sales_invoice_detail.item.name ?? "",
                                sku = sales_invoice_detail.item.sku ?? "",
                                //description = sales_invoice_detail.item.description ?? "",
                                cost = sales_invoice_detail.item.unit_cost ?? 0,
                                //product_id = sales_invoice_detail.id_item,
                                //cloud_id
                                vatCloudId = sales_invoice_detail.app_vat_group != null ? sales_invoice_detail.app_vat_group.cloud_id != null ? (int)sales_invoice_detail.app_vat_group.cloud_id : 0 : 0,
                                quantity = sales_invoice_detail.quantity,
                                price = sales_invoice_detail.unit_price
                            };

                            //  Detail.item = SyncItem;
                            SyncInvoice.details.Add(Detail);
                        }
                        if (SyncInvoice.customerCloudId > 0)
                        {
                            SyncInvoices.Add(SyncInvoice);
                        }
                        else
                        {
                            ErrorInvoices.Add(sales_invoice);
                        }

                        count = count + 1;
                        Dispatcher.BeginInvoke((Action)(() => progSales.Value = count));
                        Dispatcher.BeginInvoke((Action)(() => salesValue.Text = count.ToString()));
                    }

                    //archive?
                }


                entity.Controller.Sales.InvoiceController SalesDB = new entity.Controller.Sales.InvoiceController();
                SalesDB.Initialize();
                List<app_document_range> app_document_rangeList = SalesDB.db.app_document_range.Where(x => x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.PointOfSale && x.is_active).ToList();


                List<object> ListSyncInvoices = new List<object>();
                for (int i = 0; i < SyncInvoices.Count; i = i + 100)
                {
                    ListSyncInvoices = send.Transaction(slug, SyncInvoices.Skip(i).Take(100).ToList()).OfType<object>().ToList();
                    Dispatcher.BeginInvoke((Action)(() => progSales.IsIndeterminate = true));
                    foreach (Cognitivo.API.Models.Sales ResoponseData in ListSyncInvoices)
                    {

                        if (ResoponseData.action == API.Enums.Action.CreateOnCloud)
                        {
                            sales_invoice sales_invoice = db.db.sales_invoice.Where(x => x.id_sales_invoice == ResoponseData.localId).FirstOrDefault();
                            sales_invoice.cloud_id = ResoponseData.cloudId;
                        }
                        //else if (ResoponseData.action == API.Enums.Action.CreateOnLocal)
                        //{

                        //    sales_invoice sales_invoice = SalesDB.Create(0, false);
                        //    sales_invoice.Location = CurrentSession.Locations.Where(x => x.id_location == Settings.Default.Location).FirstOrDefault();
                        //    app_document_range app_document_range = app_document_rangeList.FirstOrDefault();
                        //    if (app_document_range != null)
                        //    {
                        //        sales_invoice.id_range = app_document_range.id_range;
                        //        sales_invoice.RaisePropertyChanged("id_range");
                        //        sales_invoice.app_document_range = app_document_range;
                        //    }
                        //    contact contact = SalesDB.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.cloud_id == ResoponseData.customerCloudId).FirstOrDefault();
                        //    if (contact != null)

                        //    {
                        //        sales_invoice.id_contact = contact.id_contact;
                        //        sales_invoice.contact = contact;

                        //    }


                        //    sales_invoice.cloud_id = ResoponseData.cloudId;

                        //    foreach (Cognitivo.API.Models.SalesDetail details in ResoponseData.details)
                        //    {

                        //        item item = SalesDB.db.items.Where(x => x.cloud_id == details.itemLocalId).FirstOrDefault();
                        //        if (item != null)
                        //        {
                        //            sales_invoice_detail _sales_invoice_detail = new sales_invoice_detail()
                        //            {
                        //                State = EntityState.Added,
                        //                sales_invoice = sales_invoice,
                        //                quantity = Convert.ToDecimal(details.quantity),
                        //                unit_price = Convert.ToDecimal(details.price),
                        //                Contact = sales_invoice.contact,
                        //                item_description = item.name,
                        //                item = item,
                        //                id_item = item.id_item,
                        //                id_vat_group = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault().id_vat_group,
                        //                cloud_id = details.cloudId

                        //            };
                        //            sales_invoice.sales_invoice_detail.Add(_sales_invoice_detail);

                        //        }





                        //    }


                        //    crm_opportunity crm_opportunity = new crm_opportunity()
                        //    {
                        //        id_contact = sales_invoice.id_contact,
                        //        id_currency = sales_invoice.id_currencyfx,
                        //        value = sales_invoice.GrandTotal
                        //    };

                        //    crm_opportunity.sales_invoice.Add(sales_invoice);
                        //    SalesDB.db.crm_opportunity.Add(crm_opportunity);

                        //    SalesDB.db.sales_invoice.Add(sales_invoice);
                        //    db.db.contacts.Add(contact);
                        //}
                        else if (ResoponseData.action == API.Enums.Action.UpdateOnLocal)
                        {

                            sales_invoice sales_invoice = db.db.sales_invoice.Where(x => x.id_sales_invoice == ResoponseData.localId).FirstOrDefault();
                            contact contact = db.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.cloud_id == ResoponseData.customerCloudId).FirstOrDefault();
                            if (contact != null)

                            {
                                sales_invoice.id_contact = contact.id_contact;
                                sales_invoice.contact = contact;

                            }

                            if (ResoponseData.status == API.Enums.Status.Approved && ResoponseData.date < DateTime.Now.AddMonths(-2))
                            {
                                sales_invoice.is_archived = true;
                            }



                            sales_invoice.cloud_id = ResoponseData.cloudId;

                            foreach (Cognitivo.API.Models.SalesDetail details in ResoponseData.details)
                            {

                                item item = db.db.items.Where(x => x.cloud_id == details.itemLocalId).FirstOrDefault();
                                if (item != null)
                                {
                                    sales_invoice_detail _sales_invoice_detail = new sales_invoice_detail()
                                    {
                                        State = EntityState.Added,
                                        sales_invoice = sales_invoice,
                                        quantity = Convert.ToDecimal(details.quantity),
                                        unit_price = Convert.ToDecimal(details.price),
                                        Contact = sales_invoice.contact,
                                        item_description = item.name,
                                        item = item,
                                        id_item = item.id_item,
                                        id_vat_group = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault().id_vat_group,
                                        cloud_id = details.cloudId

                                    };
                                    sales_invoice.sales_invoice_detail.Add(_sales_invoice_detail);

                                }





                            }



                        }


                    }
                    db.db.SaveChanges();


                }

                Dispatcher.BeginInvoke((Action)(() => progSales.IsIndeterminate = false));





                //return and aasign ids
                //archive? if approved or annull and if older than one month.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SyncBranch(string slug, Cognitivo.API.Enums.SyncWith SyncType)
        {
            send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, SyncType);

            using (db db = new db())
            {
                List<app_branch> app_branchList = db.app_branch
                   .Where(x => x.id_company == CurrentSession.Id_Company).ToList();

                Dispatcher.BeginInvoke((Action)(() => branchMaximum.Text = app_branchList.Count.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progBranch.Value = 0));
                int count = 0;

                List<object> SyncBranchList = new List<object>();

                foreach (app_branch app_branch in app_branchList)
                {
                    Location syncLocation = new Location();
                    syncLocation.localId = app_branch.id_branch;
                    syncLocation.updatedAt = app_branch.timestamp;
                    syncLocation.name = app_branch.name;


                    SyncBranchList.Add(syncLocation);

                    count = count + 1;
                    Dispatcher.BeginInvoke((Action)(() => progBranch.Value = count));
                    Dispatcher.BeginInvoke((Action)(() => branchValue.Text = count.ToString()));
                }

                Dispatcher.BeginInvoke((Action)(() => progBranch.IsIndeterminate = true));
                SyncBranchList = send.Locations(slug, SyncBranchList).OfType<object>().ToList();
                Dispatcher.BeginInvoke((Action)(() => progBranch.IsIndeterminate = false));

                db.SaveChanges();
            }
        }

        private void SyncOpportunity(string slug, Cognitivo.API.Enums.SyncWith SyncType)
        {
            send = new Cognitivo.API.Upload(Cognitivo.Properties.Settings.Default.CognitivoKey, SyncType);

            using (db db = new db())
            {
                List<project> projectList = db.projects
                   .Where(x => x.id_company == CurrentSession.Id_Company).ToList();

                Dispatcher.BeginInvoke((Action)(() => OpportunityMaximum.Text = projectList.Count.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progBranch.Value = 0));
                int count = 0;

                List<object> SyncProjectList = new List<object>();

                foreach (project project in projectList)
                {
                    Opportunity syncOpportunity = new Opportunity();
                    syncOpportunity.localId = project.id_branch;
                    syncOpportunity.updatedAt = project.timestamp;
                    syncOpportunity.name = project.name;
                    syncOpportunity.description = project.comment;
                    syncOpportunity.deadlineDate = Convert.ToDateTime(project.est_end_date);


                    SyncProjectList.Add(syncOpportunity);

                    count = count + 1;
                    Dispatcher.BeginInvoke((Action)(() => progBranch.Value = count));
                    Dispatcher.BeginInvoke((Action)(() => branchValue.Text = count.ToString()));
                }

                Dispatcher.BeginInvoke((Action)(() => progOpportunity.IsIndeterminate = true));
                SyncProjectList = send.oppo(slug, SyncProjectList).OfType<object>().ToList();
                Dispatcher.BeginInvoke((Action)(() => progOpportunity.IsIndeterminate = false));

                db.SaveChanges();
            }
        }




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
                Downloadtransaction_click(null, null);
            }));

        }
        private void DownloadVat_click(object sender, RoutedEventArgs e)
        {

            var result = Receive2API("download/saletax/");
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
            string Url = "";
            Cognitivo.API.Enums.SyncWith SyncType = (Cognitivo.API.Enums.SyncWith)cmbSyncType.SelectedItem;
            if (SyncType == Enums.SyncWith.Production)
            {
                Url = "http://www.bazaar.social/api/";
            }
            else if (SyncType == Enums.SyncWith.Playground)
            {
                Url = "https://www.test.cognitivo.in/api/";
            }
            else
            {
                Url = "http://localhost:8000/api/";
            }

            var result = Receive2API(Url + slug + "/download/order/" + startDate.SelectedDate.Value.ToString("yyyy-MM-dd") + "/" + endDate.SelectedDate.Value.ToString("yyyy-MM-dd") + "/");
            SalesData Sales_Json = new JavaScriptSerializer().Deserialize<SalesData>(result);
            entity.Controller.Sales.OrderController OrderDB = new entity.Controller.Sales.OrderController();
            OrderDB.Initialize();

            foreach (global::Sales DownloadInvoice in Sales_Json.data)
            {
                if (DownloadInvoice.localId == 0)
                {
                    sales_order sales_order = OrderDB.Create(0, false);
                    app_document_range app_document_range = OrderDB.db.app_document_range.Where(x => x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.PointOfSale && x.is_active).FirstOrDefault();
                    if (app_document_range != null)
                    {
                        sales_order.id_range = app_document_range.id_range;
                        sales_order.RaisePropertyChanged("id_range");
                        sales_order.app_document_range = app_document_range;
                    }
                    contact contact = OrderDB.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.cloud_id == DownloadInvoice.customerCloudId).FirstOrDefault();
                    //if (contact == null)

                    //{
                    //    using (db db = new db())
                    //    {
                    //        contact = new contact();
                    //        contact.name = DownloadInvoice.customer.name;
                    //        contact.is_customer = true;
                    //        contact.id_contact_role = OrderDB.db.contact_role.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).FirstOrDefault().id_contact_role;
                    //        contact.cloud_id = DownloadInvoice.customerCloudId;

                    //        db.contacts.Add(contact);
                    //        db.SaveChanges();
                    //    }
                    //    contact = OrderDB.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.cloud_id == DownloadInvoice.customerCloudId).FirstOrDefault();

                    //}

                    sales_order.id_contact = contact.id_contact;
                    sales_order.contact = contact;
                    sales_order.cloud_id = DownloadInvoice.localId;

                    foreach (SalesDetail details in DownloadInvoice.details)
                    {

                        item item = OrderDB.db.items.Where(x => x.cloud_id == details.itemCloudId).FirstOrDefault();
                        if (item == null)
                        {
                            using (db db = new db())
                            {
                                item = new item();
                                item.name = details.item.name;
                                item.id_item_type = item.item_type.Product;
                                item.id_vat_group = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault().id_vat_group;
                                item.cloud_id = details.cloudId;
                                db.items.Add(item);
                                db.SaveChanges();
                            }
                            item = OrderDB.db.items.Where(x => x.cloud_id == details.cloudId).FirstOrDefault();
                        }


                        sales_order_detail _sales_order_detail = new sales_order_detail()
                        {
                            State = EntityState.Added,
                            sales_order = sales_order,
                            quantity = Convert.ToDecimal(details.quantity),
                            Contact = sales_order.contact,
                            item_description = details.item.name,
                            item = item,
                            id_item = item.id_item,
                            id_vat_group = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault().id_vat_group,
                            cloud_id = details.cloudId

                        };

                        sales_order.sales_order_detail.Add(_sales_order_detail);

                    }


                    crm_opportunity crm_opportunity = new crm_opportunity()
                    {
                        id_contact = sales_order.id_contact,
                        id_currency = sales_order.id_currencyfx,
                        value = sales_order.GrandTotal
                    };

                    crm_opportunity.sales_order.Add(sales_order);
                    OrderDB.db.crm_opportunity.Add(crm_opportunity);

                    OrderDB.db.sales_order.Add(sales_order);

                }
                else
                {
                    sales_order sales_order = OrderDB.db.sales_order.Where(x => x.id_sales_order == DownloadInvoice.localId).FirstOrDefault();
                    if (sales_order != null)
                    {
                        contact contact = OrderDB.db.contacts.Where(x => x.cloud_id == DownloadInvoice.customerCloudId).FirstOrDefault();
                        //if (contact == null)
                        //{
                        //    contact = new contact();
                        //    contact.name = DownloadInvoice.customer_alias;
                        //    contact.is_customer = true;

                        //}
                        sales_order.id_contact = contact.id_contact;
                        sales_order.contact = contact;
                    }
                    foreach (SalesDetail details in DownloadInvoice.details)
                    {
                        sales_order_detail sales_order_detail = OrderDB.db.sales_order_detail.Where(x => x.id_sales_order_detail == details.cloudId).FirstOrDefault();
                        if (sales_order_detail != null)
                        {
                            item item = OrderDB.db.items.Where(x => x.cloud_id == details.itemCloudId).FirstOrDefault();
                            if (item == null)
                            {
                                item = new item();
                                item.name = details.item.name;
                                item.id_item_type = item.item_type.Product;

                                sales_order_detail.quantity = Convert.ToDecimal(details.quantity);
                                sales_order_detail.Contact = sales_order.contact;
                                sales_order_detail.item_description = details.item.name;
                                sales_order_detail.item = item;
                                sales_order_detail.id_item = item.id_item;

                            }
                        }

                    }
                }
                OrderDB.db.SaveChanges();


            }
        }






        private HttpWebResponse Send2API(object Json, string apiname)
        {
            try
            {
                string key = Cognitivo.Properties.Settings.Default.CognitivoKey;
                var webAddr = "";

                webAddr = tbxURL.Content + apiname;


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

        private string Receive2API(string webAddr)
        {
            try
            {
                string key = Cognitivo.Properties.Settings.Default.CognitivoKey;
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


public class Sales
{

    public Sales()
    {
        date = DateTime.Now;
        details = new List<SalesDetail>();
     
    }


   
    /// <summary>
    /// Gets or sets the local identifier.
    /// </summary>
    /// <value>The local identifier.</value>
    public int? localId { get; set; }

   
    /// <summary>
    /// Gets or sets the cloud identifier.
    /// </summary>
    /// <value>The cloud identifier.</value>
    public int? cloudId { get; set; }



   
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The status.</value>
    public Enums.Action action { get; set; }
   
    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    /// <value>The location.</value>
    public int? locationCloudId { get; set; }
   
    /// <summary>
    /// Gets or sets the location name.
    /// </summary>
    /// <value>The location.</value>
    public string locationName { get; set; }
   
    /// <summary>
    /// Gets or sets the date.
    /// </summary>
    /// <value>The date.</value>
    public DateTime date { get; set; }
   
    /// <summary>
    /// Gets or sets the contact.
    /// </summary>
    /// <value>The contact.</value>
    public int? customerCloudId { get; set; }
   
    /// <summary>
    /// Gets or sets the payment contract.
    /// </summary>
    /// <value>The payment contract.</value>
    public int? paymentContractCloudId { get; set; }
   
    /// <summary>
    /// Gets or sets the invoice number.
    /// </summary>
    /// <value>The invoice number.</value>
    public string invoiceNumber { get; set; }
   
    /// <summary>
    /// Gets or sets the invoice code.
    /// </summary>
    /// <value>The invoice code.</value>
    public string InvoiceCode { get; set; }
   
    /// <summary>
    /// Gets or sets the code expiry.
    /// </summary>
    /// <value>The code expiry.</value>
    public DateTime? codeExpiry { get; set; }
   
    /// <summary>
    /// Gets or sets the currency.
    /// </summary>
    /// <value>The currency.</value>
    public string currencyCode { get; set; }
   
    /// <summary>
    /// Gets or sets the currency rate.
    /// </summary>
    /// <value>The currency rate.</value>
    public decimal? rate { get; set; }
   
    /// <summary>
    /// Gets the interval.
    /// </summary>
    /// <value>The interval.</value>
    public TimeSpan interval { get; set; }
   
    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    /// <value>The details.</value>
    public List<SalesDetail> details { get; set; }

   
    /// <summary>
    /// Gets or sets the customer.
    /// </summary>
    /// <value>The details.</value>
    public Customer relationship { get; set; }

   
    /// <summary>
    /// Gets or sets the Contract.
    /// </summary>
    /// <value>The details.</value>
    public PaymentContract paymentContract { get; set; }

   


   
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Core.Models.Order"/> is archived.
    /// </summary>
    /// <value><c>true</c> if is archived; otherwise, <c>false</c>.</value>
    public bool sentMail { get; set; }



   
    public DateTime updatedAt { get; set; }
   
    public DateTime createdAt { get; set; }
   
    public DateTime? deletedAt { get; set; }



}

public class SalesData
{
    public List<Sales> data { get; set; }
}

public class SalesDetail
{
    public SalesDetail()
    {
        quantity = 1;
    }

    /// <summary>
    /// Gets or sets the local identifier.
    /// </summary>
    /// <value>The local identifier.</value>
    public int localId { get; set; }

    /// <summary>
    /// Gets or sets the cloud identifier.
    /// </summary>
    /// <value>The cloud identifier.</value>
    public int? cloudId { get; set; }

    /// <summary>
    /// Gets or sets the order.
    /// </summary>
    /// <value>The order.</value>
    public int? salesCloudId { get; set; }

    /// <summary>
    /// Gets or sets the vat.
    /// </summary>
    /// <value>The vat.</value>
    public int? vatCloudId { get; set; }

    /// <summary>
    /// Gets or sets the item.
    /// </summary>
    /// <value>The item.</value>
    public int? itemCloudId { get; set; }

    /// <summary>
    /// Gets or sets the item.
    /// </summary>
    /// <value>The item.</value>
    public int itemLocalId { get; set; }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    /// <value>The item description.</value>
    public string name { get; set; }

    /// <summary>
    /// Gets or sets the item sku.
    /// </summary>
    public string sku { get; set; }


    /// <summary>
    /// Gets or sets the cost.
    /// </summary>
    /// <value>The cost.</value>
    public decimal? cost { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    /// <value>The quantity.</value>
    public decimal? quantity { get; set; }

    /// <summary>
    /// Gets or sets the price.
    /// </summary>
    /// <value>The price.</value>
    public decimal? price { get; set; }

    /// <summary>
    /// Gets or sets the last updated on.
    /// </summary>
    /// <value>The last updated on.</value>
    public DateTime updatedAt { get; set; }

    public Item item { get; set; }

}


