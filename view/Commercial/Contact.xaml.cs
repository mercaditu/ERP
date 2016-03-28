using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using entity;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace Cognitivo.Commercial
{
    public partial class Contact : Page
    {
        ContactDB ContactDB = new ContactDB();
        CollectionViewSource contactChildListViewSource;
        CollectionViewSource contactViewSource;
        CollectionViewSource contact_subscriptionViewSource;
        #region Initilize and load
        public Contact()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            entity.Properties.Settings _entity = new entity.Properties.Settings();

            //Contact
            ContactDB.contacts.Where(a => a.is_active == true && a.id_company == _entity.company_ID && a.is_employee == false).OrderBy(a => a.code).Load();

            contactViewSource = (CollectionViewSource)FindResource("contactViewSource");
            contactViewSource.Source = ContactDB.contacts.Local;
            CollectionViewSource contactParentViewSource = (CollectionViewSource)FindResource("contactParentViewSource");
            contactParentViewSource.Source = ContactDB.contacts.Local;
            contactChildListViewSource = (CollectionViewSource)FindResource("contactChildListViewSource");

            contactChildListViewSource.Source = ContactDB.contacts.Local;


            contact_subscriptionViewSource = (CollectionViewSource)FindResource("contact_subscriptionViewSource");
            ContactDB.contact_subscription.Where(a => a.is_active == true && a.id_company == _entity.company_ID).Load();
            contact_subscriptionViewSource.Source = ContactDB.contact_subscription.Local;
            //ContactRole
            CollectionViewSource contactRoleViewSource = (CollectionViewSource)FindResource("contactRoleViewSource");
            contactRoleViewSource.Source = ContactDB.contact_role.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //AppContract
            CollectionViewSource appContractViewSource = (CollectionViewSource)FindResource("appContractViewSource");
            appContractViewSource.Source = ContactDB.app_contract.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //AppCostCenter
            CollectionViewSource appCostCenterViewSource = (CollectionViewSource)FindResource("appCostCenterViewSource");
            appCostCenterViewSource.Source = ContactDB.app_cost_center.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //ItemPriceList
            CollectionViewSource itemPriceListViewSource = (CollectionViewSource)FindResource("itemPriceListViewSource");
            itemPriceListViewSource.Source = ContactDB.item_price_list.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //SalesRep
            List<sales_rep> sales_rep = ContactDB.sales_rep.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();
            CollectionViewSource salesRepViewSource = (CollectionViewSource)FindResource("salesRepViewSource");
            salesRepViewSource.Source = sales_rep.ToList();
            CollectionViewSource salesRepViewSourceCollector = (CollectionViewSource)FindResource("salesRepViewSourceCollector");
            salesRepViewSourceCollector.Source = sales_rep.ToList();

            //AppCurrency
            CollectionViewSource app_currencyViewSource = (CollectionViewSource)FindResource("app_currencyViewSource");
            app_currencyViewSource.Source = ContactDB.app_currency.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //AppBank
            CollectionViewSource bankViewSource = (CollectionViewSource)FindResource("bankViewSource");
            bankViewSource.Source = ContactDB.app_bank.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //Gender Type Enum
            cbxGender.ItemsSource = Enum.GetValues(typeof(contact.Genders));


            ContactDB.contact_tag
             .Where(x => x.id_company == _entity.company_ID && x.is_active == true)
             .OrderBy(x => x.name).Load();

            CollectionViewSource contact_tagViewSource = ((CollectionViewSource)(FindResource("contact_tagViewSource")));
            contact_tagViewSource.Source = ContactDB.contact_tag.Local;

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = ContactDB.app_vat_group.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).ToList();
            contact_subscriptionViewSource.View.Refresh();
            FilterSubscription();
        }
        #endregion

        #region Toolbar Events
        private void toolBar_btnNew_Click(object sender)
        {
            contact contact = new contact();

            ContactDB.New(contact);

            contact.is_employee = false;
            contact.State = EntityState.Added;
            contact.IsSelected = true;
            ContactDB.contacts.Add(contact);
            contactViewSource.View.Refresh();
            contactViewSource.View.MoveCurrentToLast();
            txtName.Focus();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    contact contact = (contact)listContacts.SelectedItem;
                    contact.is_head = false;
                    contact.State = EntityState.Deleted;
                    contact.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
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
            try
            {
                ContactDB.SaveChanges();


                contactViewSource.View.Refresh();
                toolBar.msgSaved();
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            ContactDB.CancelAllChanges();
        }
        #endregion

        private void cbxContactRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxContactRole.SelectedItem != null)
            {
                contact_role contact_role = cbxContactRole.SelectedItem as contact_role;
                if (contact_role.is_principal == true)
                {
                    cbxRelation.IsEnabled = false;
                }
                else
                    cbxRelation.IsEnabled = true;
                if (contact_role.can_transact == true)
                {
                    tabFinance.Visibility = Visibility.Visible;
                    tabSubscription.Visibility = Visibility.Visible;
                }
                else
                {
                    tabFinance.Visibility = Visibility.Collapsed;
                    tabSubscription.Visibility = Visibility.Collapsed;
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

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as contact_field_value != null)
            {
                //contact_field_value contact_field_value = e.Parameter as contact_field_value;
                //if (string.IsNullOrEmpty(contact_field_value.Error))
                //{
                e.CanExecute = true;
                //}
            }
            else if (e.Parameter as contact_subscription != null)
            {
                e.CanExecute = true;
            }
            else if (e.Parameter as contact_tag_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {

                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //DeleteDetailGridRow
                    if (e.Parameter as contact_field_value != null)
                    {
                        //ontact_field_valueDataGrid.CancelEdit();
                        ContactDB.contact_field_value.Remove(e.Parameter as contact_field_value);
                        //contactcontact_field_valueViewSource.View.Refresh();
                    }
                    else if (e.Parameter as contact_subscription != null)
                    {
                        contactcontact_subscriptionDataGrid.CancelEdit();
                        ContactDB.contact_subscription.Remove(e.Parameter as contact_subscription);

                        contact_subscriptionViewSource.View.Refresh();
                        FilterSubscription();
                    }
                    else if (e.Parameter as contact_tag_detail != null)
                    {
                        contact_tag_detailDataGrid.CancelEdit();
                        ContactDB.contact_tag_detail.Remove(e.Parameter as contact_tag_detail);

                        CollectionViewSource contactcontact_tag_detailViewSource = FindResource("contactcontact_tag_detailViewSource") as CollectionViewSource;
                        contactcontact_tag_detailViewSource.View.Refresh();
                    }
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }

        private void LoadRelatedContactOnThread(contact _contact)
        {

            if (contactChildListViewSource != null)
            {
               
                    if (contactChildListViewSource.View != null)
                    {
                        
                        contactChildListViewSource.View.Filter = i =>
                        {
                            contact contact = (contact)i;
                            if (_contact != null)
                            {
                                if (contact.parent == _contact || contact.id_contact == _contact.id_contact)
                                    return true;
                                else
                                    return false;
                            }
                            else
                                return false;
                        };

                    }
               

            }
        }

        private void FilterSubscription()
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
                           if (_contact_subscription.id_contact == contact.id_contact || contact.child!=null?contact.child.Contains(_contact_subscription.contact):false)
                                return true;
                            else
                                return false;
                        };

                    }


                }
            }
        }

        private void toolIcon_Click(object sender, RoutedEventArgs e)
        {
            entity.Brillo.Logic.Document Document = new entity.Brillo.Logic.Document();
            contact contact = (contact)listContacts.SelectedItem;
            Document.Document_PrintCarnetContact(contact);
        }

        private async void SmartBox_Geography_Select(object sender, RoutedEventArgs e)
        {
            contact contact = (contact)contactViewSource.View.CurrentItem;
            if (smtgeo.GeographyID > 0)
            {
                contact.app_geography = await ContactDB.app_geography.Where(p => p.id_geography == smtgeo.GeographyID).FirstOrDefaultAsync();
            }
        }
        private void cbxTag_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Add_Tag();

            }
        }

        private void cbxTag_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Add_Tag();
        }
        void Add_Tag()
        {
            // CollectionViewSource item_tagViewSource = ((CollectionViewSource)(FindResource("item_tagViewSource")));
            if (cbxTag.Data != null)
            {
                int id = Convert.ToInt32(((contact_tag)cbxTag.Data).id_tag);
                if (id > 0)
                {
                    contact contact = contactViewSource.View.CurrentItem as contact;
                    if (contact != null)
                    {
                        contact_tag_detail contact_tag_detail = new contact_tag_detail();
                        contact_tag_detail.id_tag = ((contact_tag)cbxTag.Data).id_tag;
                        contact_tag_detail.contact_tag = ((contact_tag)cbxTag.Data);
                        contact.contact_tag_detail.Add(contact_tag_detail);
                        CollectionViewSource contactcontact_tag_detailViewSource = FindResource("contactcontact_tag_detailViewSource") as CollectionViewSource;
                        contactcontact_tag_detailViewSource.View.Refresh();

                    }
                }
            }
        }

        private void listContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            contact_role contact_role = cbxContactRole.SelectedItem as contact_role;
            if (contact_role != null)
            {
                if (contact_role.is_principal == true)
                {

                   LoadRelatedContactOnThread(contact);
                }
            }
            if (contact_subscriptionViewSource != null)
            {
                if (contact_subscriptionViewSource.View != null)
                {
                    contact_subscriptionViewSource.View.Refresh();
                    FilterSubscription();
                }
            }
        }
        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                contact contact = contactViewSource.View.CurrentItem as contact;

                if (contact != null)
                {

                    item item = ContactDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                    contact_subscription contact_subscription = new contact_subscription();
                    contact_subscription.contact = contact;
                    contact_subscription.id_item = (int)item.id_item;
                    contact_subscription.item = item;
                    ContactDB.contact_subscription.Add(contact_subscription);
                    contactViewSource.View.Refresh();
                    contact_subscriptionViewSource.View.Refresh();
                    FilterSubscription();


                }

            }
        }

        private async void cbxRelation_Select(object sender, RoutedEventArgs e)
        {
            contact contact = (contact)contactViewSource.View.CurrentItem;
            if (contact != null && cbxRelation.ContactID > 0)
            {
                contact relatedto_contact = await ContactDB.contacts.Where(x => x.id_contact == cbxRelation.ContactID).FirstOrDefaultAsync();
                contact.parent = relatedto_contact;

                LoadRelatedContactOnThread(relatedto_contact);
            }
        }




    }
}
