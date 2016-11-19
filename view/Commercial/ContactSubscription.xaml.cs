using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using entity;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cognitivo.Commercial
{
    public partial class ContactSubscription : Page
    {
        ContactDB ContactDB = new ContactDB();
        CollectionViewSource contactViewSource, contact_subscriptionViewSource;

        public ContactSubscription()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ContactDB.contacts.Where(a => a.id_company == CurrentSession.Id_Company && a.is_customer && a.contact_role.is_principal).OrderBy(a => a.name).Load();

            contactViewSource = (CollectionViewSource)FindResource("contactViewSource");
            contactViewSource.Source = ContactDB.contacts.Local;

            CollectionViewSource appContractViewSource = (CollectionViewSource)FindResource("appContractViewSource");
            appContractViewSource.Source = CurrentSession.Contracts;

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = CurrentSession.VAT_Groups;
        }

        private async void LoadChildOnthread(int ContactID)
        {
            await ContactDB.contact_subscription.Where(a => a.id_contact == ContactID).LoadAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                contact_subscriptionViewSource = (CollectionViewSource)FindResource("contact_subscriptionViewSource");
                contact_subscriptionViewSource.Source = ContactDB.contact_subscription.Local;
                FilterSubscription();
            }));
        }

        private void FilterSubscription()
        {
            try
            {
                contact contact = contactViewSource.View.CurrentItem as contact;
                if (contact != null)
                {
                    if (contact_subscriptionViewSource != null)
                    {
                        if (contact_subscriptionViewSource.View != null)
                        {
                            contact_subscriptionViewSource.View.Filter = i =>
                            {
                                contact_subscription _contact_subscription = (contact_subscription)i;
                                if (_contact_subscription.id_contact == contact.id_contact || (contact.child != null ? contact.child.Contains(_contact_subscription.contact) : false))
                                    return true;
                                else
                                    return false;
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void listContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null)
            {
                LoadChildOnthread(contact.id_contact);
                //Task Child = Task.Factory.StartNew(() => LoadChildOnthread(contact.id_contact));
            }
        }

        private void item_select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                contact contact = contactViewSource.View.CurrentItem as contact;

                if (contact != null)
                {
                    item item = ContactDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                    contact_subscription contact_subscription = new contact_subscription();
                    contact_subscription.contact = contact;
                    contact_subscription.id_contact = contact.id_contact;
                    contact_subscription.id_item = (int)item.id_item;
                    contact_subscription.item = item;
                    contact_subscription.id_vat_group = item.id_vat_group;
                    contact_subscription.id_contract = contact.app_contract == null ? 0 : (int)contact.id_contract;

                    ContactDB.contact_subscription.Add(contact_subscription);
                    contactViewSource.View.Refresh();
                    contact_subscriptionViewSource.View.Refresh();

                    FilterSubscription();
                }
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (listContacts.SelectedItem != null)
            {
                contact contact = (contact)listContacts.SelectedItem;
                contact.State = EntityState.Modified;
                contact.IsSelected = true;
            }
            else
            {
                toolBar.msgWarning("Please Select a Contact");
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            //Abhi> in Brillo, add logic to add for validations
            if (ContactDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(ContactDB.NumberOfRecords);
                contactViewSource.View.Refresh();
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<contact> ContactList = contactViewSource.View.OfType<contact>().ToList();
            DateTime InvoiceDate = (DateTime)dtpTrans_Date.SelectedDate;
            progBar.Minimum = 1;
            progBar.Value = 1;
            progBar.Maximum = ContactList.Count() + 1;
            //SyncSalesInvoice(ContactList, InvoiceDate);

            Task taskAuth = Task.Factory.StartNew(() => SyncSalesInvoice(ContactList, InvoiceDate));
        }

        private void SyncSalesInvoice(List<contact> ContactList, DateTime InvoiceDate)
        {
            app_vat_group VatGroup = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault();

            foreach (contact Contact in ContactList)
            {
                try
                {
                    using (db db = new db())
                    {
                        sales_invoice sales_invoice = new sales_invoice();
                        sales_invoice.id_contact = Contact.id_contact;

                        app_contract app_contract = db.app_contract.Find(Contact.id_contract);
                        sales_invoice.id_condition = app_contract.id_condition;
                        sales_invoice.id_contract = app_contract.id_contract;
                        sales_invoice.id_currencyfx = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;
                        sales_invoice.comment = "Subscription";
                        sales_invoice.trans_date = InvoiceDate;
                        sales_invoice.timestamp = DateTime.Now;

                        if (Contact.contact_subscription.Count > 0)
                        {
                            foreach (contact_subscription contact_subscription in Contact.contact_subscription)
                            {
                                sales_invoice_detail sales_invoice_detail = null;

                                sales_invoice_detail = new sales_invoice_detail();
                                sales_invoice_detail.id_sales_invoice = sales_invoice.id_sales_invoice;
                                sales_invoice_detail.sales_invoice = sales_invoice;
                                item item = db.items.Find(contact_subscription.id_item);

                                if (item != null)
                                {
                                    sales_invoice_detail.item = item;
                                    sales_invoice_detail.id_vat_group = contact_subscription.id_vat_group > 0 ? contact_subscription.id_vat_group : VatGroup.id_vat_group;
                                    sales_invoice_detail.id_item = contact_subscription.id_item;
                                    sales_invoice_detail.item_description = contact_subscription.item.name;
                                }
                                else
                                {
                                    continue;
                                }

                                sales_invoice_detail.quantity = contact_subscription.quantity;
                                sales_invoice_detail.UnitPrice_Vat = contact_subscription.UnitPrice_Vat;

                                sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
                            }

                            if (sales_invoice.sales_invoice_detail.Count > 0)
                            {
                                sales_invoice.State = EntityState.Added;
                                sales_invoice.IsSelected = true;

                                crm_opportunity crm_opportunity = new crm_opportunity();
                                crm_opportunity.id_contact = sales_invoice.id_contact;
                                crm_opportunity.id_currency = sales_invoice.id_currencyfx;
                                crm_opportunity.value = sales_invoice.GrandTotal;

                                crm_opportunity.sales_invoice.Add(sales_invoice);


                                db.crm_opportunity.Add(crm_opportunity);
                                //db.sales_invoice.Add(sales_invoice);
                                db.SaveChanges();


                                progBar.Value += 1;
                                // Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 1; }));
                            }
                        }
                    }

                }
                catch (Exception)
                {
                    //  Dispatcher.BeginInvoke((Action)(() => { Contact.IsSelected = true; }));
                }
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    contactViewSource.View.Filter = i =>
                    {
                        contact contact = i as contact;
                        string name = "";
                        string code = "";
                        string gov_code = "";

                        if (contact.name != null)
                        {
                            name = contact.name.ToLower();
                        }

                        if (contact.code != null)
                        {
                            code = contact.code.ToLower();
                        }

                        if (contact.gov_code != null)
                        {
                            gov_code = contact.gov_code.ToLower();
                        }

                        if (name.Contains(query.ToLower())
                            || code.Contains(query.ToLower())
                            || gov_code.Contains(query.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                else
                {
                    contactViewSource.View.Filter = null;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }
    }
}
