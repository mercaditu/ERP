using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using entity;
using System.Windows.Data;
using System.Windows.Controls;

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
            appContractViewSource.Source = CurrentSession.Get_Contract(); // ContactDB.app_contract.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = CurrentSession.Get_VAT_Group();//ContactDB.app_vat_group.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
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
            if (contact!=null)
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
