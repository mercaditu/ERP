using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Commercial
{
    public partial class ContactSubscription : Page
    {
        private ContactDB ContactDB = new ContactDB();
        private CollectionViewSource contactViewSource, contact_subscriptionViewSource;
        contact_subscription Maincontact_subscription;
        public ContactSubscription()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ContactDB.contacts.Where(a => a.is_head == true && a.id_company == CurrentSession.Id_Company && a.is_active && a.is_customer && a.contact_role.is_principal).OrderBy(a => a.name).Load();

            contactViewSource = (CollectionViewSource)FindResource("contactViewSource");
            contactViewSource.Source = ContactDB.contacts.Local;

            CollectionViewSource appContractViewSource = (CollectionViewSource)FindResource("appContractViewSource");
            appContractViewSource.Source = CurrentSession.Contracts;

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = CurrentSession.VAT_Groups;
            txtQuantity.Text = "12";
            dtpTrans_Date.SelectedDate = DateTime.Now;
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
                    contact.contact_subscription.Add(contact_subscription);
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
            MessageBoxResult msgresult = MessageBox.Show("Are You Sure To Genrate Sales Order", "Cognitivo", MessageBoxButton.YesNo);
            if (msgresult == MessageBoxResult.Yes)
            {
                List<contact> ContactList = contactViewSource.View.OfType<contact>().ToList();
                if (dtpTrans_Date.SelectedDate != null)
                {
                    DateTime InvoiceDate = (DateTime)dtpTrans_Date.SelectedDate;
                    progBar.Minimum = 1;
                    progBar.Value = 1;
                    progBar.Maximum = ContactList.Count() + 1;
                    //SyncSalesInvoice(ContactList, InvoiceDate);

                    SyncSalesOrder(ContactList, InvoiceDate,Convert.ToDecimal(txtQuantity.Text));

                    // Task taskAuth = Task.Factory.StartNew(() => SyncSalesOrder(ContactList, InvoiceDate));
                }
                else
                {
                    toolBar.msgWarning("Select Invoice Date...");
                }
            }
            

        }

        private void SyncSalesOrder(List<contact> ContactList, DateTime InvoiceDate,decimal quantity)
        {
            app_vat_group VatGroup = CurrentSession.VAT_Groups.Where(x => x.is_default).FirstOrDefault();
            entity.Controller.Sales.OrderController SalesDB = new entity.Controller.Sales.OrderController();
            SalesDB.Initialize();
            foreach (contact contact in ContactList.Where(x => x.IsSelected).ToList())
            {
                try
                {
                    // using (db db = new db())
                    // {
                    contact sales_contact = SalesDB.db.contacts.Find(contact.id_contact);
                    sales_order sales_order = new sales_order();
                    sales_order.id_contact = sales_contact.id_contact;
                    sales_order.contact = sales_contact;

                    app_contract app_contract = SalesDB.db.app_contract.Find(contact.id_contract);
                    if (app_contract != null)
                    {
                        sales_order.id_condition = app_contract.id_condition;
                        sales_order.id_contract = app_contract.id_contract;
                    }
                    else
                    {
                        app_contract = SalesDB.db.app_contract.Where(x => x.is_default && x.is_active).FirstOrDefault();
                        sales_order.id_condition = app_contract.id_condition;
                        sales_order.id_contract = app_contract.id_contract;
                    }
                    app_document_range app_document_range = SalesDB.db.app_document_range.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active == true).FirstOrDefault();
                    if (app_document_range != null)
                    {
                        sales_order.id_range = app_document_range.id_range;
                    }



                    sales_order.id_currencyfx = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;
                    sales_order.comment = "Subscription";
                    sales_order.trans_date = InvoiceDate;
                    sales_order.timestamp = DateTime.Now;

                    if (sales_contact.contact_subscription.Count > 0)
                    {
                        foreach (contact_subscription contact_subscription in sales_contact.contact_subscription.ToList())
                        {
                            sales_order_detail sales_order_detail = null;

                            sales_order_detail = new sales_order_detail();
                            sales_order_detail.id_sales_order = sales_order.id_sales_order;
                            sales_order_detail.sales_order = sales_order;
                            item item = SalesDB.db.items.Find(contact_subscription.id_item);

                            if (item != null)
                            {
                                sales_order_detail.item = item;
                                sales_order_detail.id_vat_group = contact_subscription.id_vat_group > 0 ? contact_subscription.id_vat_group : VatGroup.id_vat_group;
                                sales_order_detail.id_item = contact_subscription.id_item;
                                sales_order_detail.item_description = item.name;
                            }
                            else
                            {
                                continue;
                            }

                            sales_order_detail.quantity = quantity;
                            sales_order_detail.UnitPrice_Vat = contact_subscription.UnitPrice_Vat;

                            sales_order.sales_order_detail.Add(sales_order_detail);
                        }

                        if (sales_order.sales_order_detail.Count > 0)
                        {
                            sales_order.State = EntityState.Added;
                            sales_order.IsSelected = true;
                            SalesDB.db.sales_order.Add(sales_order);


                        }



                    }

                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            // SalesDB.SaveChanges_WithValidation();

            SalesDB.Approve();
            Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 1; }));
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            List<contact> contactList = contactViewSource.View.OfType<contact>().ToList();
            List<contact> contactRemoved = new List<contact>();

            try
            {
                using (db db = new db())
                {

                    contactRemoved.AddRange(db.contacts.Where(x => x.is_head == true && x.id_company == CurrentSession.Id_Company
                    && x.is_customer && x.contact_role.is_principal && x.is_active && x.contact_subscription.Count() == 0));
                    foreach (contact contact in contactRemoved)
                    {
                        contact parent_contact = db.contacts.Where(x => x.parent.id_contact == contact.id_contact).FirstOrDefault();
                        if (parent_contact != null)
                        {
                            parent_contact.parent = null;
                        }
                      
                    }

                    db.contacts.RemoveRange(contactRemoved);
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                contactRemoved = new List<contact>();
                contactRemoved.AddRange(ContactDB.contacts.Where(x => x.is_head == true && x.id_company == CurrentSession.Id_Company
                   && x.is_customer && x.contact_role.is_principal && x.is_active && x.contact_subscription.Count() == 0));
                foreach (contact contact in contactRemoved)
                {
                    contact.is_active = false;
                }
            }


            ContactDB.SaveChanges();

            ContactDB = new ContactDB();
            ContactDB.contacts.Where(a => a.is_head == true && a.id_company == CurrentSession.Id_Company && a.is_active && a.is_customer && a.contact_role.is_principal).OrderBy(a => a.name).Load();

            contactViewSource = (CollectionViewSource)FindResource("contactViewSource");
            contactViewSource.Source = ContactDB.contacts.Local;

            contactList = contactViewSource.View.OfType<contact>().ToList();

            foreach (contact contact in contactList)
            {
                if (contact.contact_subscription.Count() > 0)
                {
                    try
                    {
                        foreach (contact_subscription subscription in contact.contact_subscription.Where(x => x.quantity < 2))
                        {
                            subscription.quantity = subscription.quantity * 12;
                        }

                        ContactDB.SaveChanges();
                    }
                    catch { }
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