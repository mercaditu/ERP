using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using entity;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;
using Microsoft.Maps.MapControl.WPF;

namespace Cognitivo.Commercial
{
    public partial class Contact : Page
    {
        ContactDB ContactDB = new ContactDB();

        CollectionViewSource contactChildListViewSource;
        CollectionViewSource contactViewSource;
        CollectionViewSource contactcontact_field_valueViewSource;

        #region Initilize and load

        public Contact()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            contactChildListViewSource = (CollectionViewSource)FindResource("contactChildListViewSource");
            contactcontact_field_valueViewSource = (CollectionViewSource)FindResource("contactcontact_field_valueViewSource");

            //Contact
            ContactDB.contacts.Where(a => (a.id_company == CurrentSession.Id_Company || a.id_company == null) && a.is_employee == false).OrderBy(a => a.name).Load();

            contactViewSource = (CollectionViewSource)FindResource("contactViewSource");
            contactViewSource.Source = ContactDB.contacts.Local;

            CollectionViewSource contactParentViewSource = (CollectionViewSource)FindResource("contactParentViewSource");
            contactParentViewSource.Source = ContactDB.contacts.Local;

            //ContactRole
            CollectionViewSource contactRoleViewSource = (CollectionViewSource)FindResource("contactRoleViewSource");
            contactRoleViewSource.Source = ContactDB.contact_role.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();

            //AppContract
            CollectionViewSource appContractViewSource = (CollectionViewSource)FindResource("appContractViewSource");
            appContractViewSource.Source = CurrentSession.Get_Contract().OrderBy(x => x.name);

            //AppCostCenter
            CollectionViewSource appCostCenterViewSource = (CollectionViewSource)FindResource("appCostCenterViewSource");
            appCostCenterViewSource.Source = ContactDB.app_cost_center.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();

            //ItemPriceList
            CollectionViewSource itemPriceListViewSource = (CollectionViewSource)FindResource("itemPriceListViewSource");
            itemPriceListViewSource.Source = ContactDB.item_price_list.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();

            //SalesRep
            //ContactDB.sales_rep.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();
            
            CollectionViewSource salesRepViewSource = (CollectionViewSource)FindResource("salesRepViewSource");
            salesRepViewSource.Source = CurrentSession.Get_SalesRep().OrderBy(a => a.name);

            CollectionViewSource salesRepViewSourceCollector = (CollectionViewSource)FindResource("salesRepViewSourceCollector");
            salesRepViewSourceCollector.Source = CurrentSession.Get_SalesRep().OrderBy(a => a.name);

            //AppCurrency
            CollectionViewSource app_currencyViewSource = (CollectionViewSource)FindResource("app_currencyViewSource");
            app_currencyViewSource.Source = ContactDB.app_currency.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();
            
            //Fields
            CollectionViewSource app_fieldViewSource = (CollectionViewSource)FindResource("app_fieldViewSource");
            app_fieldViewSource.Source = ContactDB.app_field.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();

            //AppBank
            CollectionViewSource bankViewSource = (CollectionViewSource)FindResource("bankViewSource");
            bankViewSource.Source = ContactDB.app_bank.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();

            //Gender Type Enum
            cbxGender.ItemsSource = Enum.GetValues(typeof(contact.Genders));

            ContactDB.contact_tag
             .Where(x => x.id_company == CurrentSession.Id_Company && x.is_active == true)
             .OrderBy(x => x.name).Load();
            CollectionViewSource contact_tagViewSource = ((CollectionViewSource)(FindResource("contact_tagViewSource")));
            contact_tagViewSource.Source = ContactDB.contact_tag.Local;

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = CurrentSession.Get_VAT_Group().OrderBy(a => a.name);
        }
        #endregion

        #region Toolbar Events
        private void toolBar_btnNew_Click(object sender)
        {
            contact contact = ContactDB.New();

            contact.is_employee = false;
            contact.State = EntityState.Added;
            contact.IsSelected = true;
            ContactDB.contacts.Add(contact);
            contactViewSource.View.Refresh();
            contactViewSource.View.MoveCurrentToLast();
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
            if (ContactDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(ContactDB.NumberOfRecords);
                contactViewSource.View.Refresh();
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            ContactDB.CancelAllChanges();
            contact contact = contactViewSource.View.CurrentItem as contact;
            contact.State = EntityState.Unchanged;
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
                {
                    cbxRelation.IsEnabled = true;
                }

                if (contact_role.can_transact == true)
                {
                    tabFinance.Visibility = Visibility.Visible;
                    //tabSubscription.Visibility = Visibility.Visible;
                }
                else
                {
                    tabFinance.Visibility = Visibility.Collapsed;
                    //tabSubscription.Visibility = Visibility.Collapsed;
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
                e.CanExecute = true;
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
                contactChildListViewSource.Source = ContactDB.contacts.Where(x => x.parent.id_contact == _contact.id_contact || x.id_contact == _contact.id_contact).ToList();
            }
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
            if (contact != null)
            {
                LoadRelatedContactOnThread(contact);
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

        private void contactcontact_subscriptionDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            // FilterSubscription();
        }

        private void hrefAddCust_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Add_field();
        }

        private void MapsDropPin_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Disables the default mouse double-click action.
            e.Handled = true;

            // Determin the location to place the pushpin at on the map.

            //Get the mouse click coordinates
            Point mousePosition = e.GetPosition(this);
            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);

            // The pushpin to add to the map.
            Pushpin pin = new Pushpin();
            pin.Location = pinLocation;

            // Adds the pushpin to the map.
            myMap.Children.Add(pin);

            contact contact = (contact)contactViewSource.View.CurrentItem;
            if (contact != null)
            {
                contact.comment = pinLocation.ToString();
            }
        }

        private void Add_field()
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null)
            {
                //using (db db = new db())
                //{
                if (ContactDB.app_field.Where(x => x.field_type == app_field.field_types.Account).Count() == 0)
                {
                    app_field app_field = new app_field();
                    app_field.field_type = entity.app_field.field_types.Account;
                    app_field.name = "Account";
                    ContactDB.app_field.Add(app_field);
                    ContactDB.SaveChanges();
                }
                //}

                contact_field_value contact_field_value = new contact_field_value();
                contact.contact_field_value.Add(contact_field_value);
                contactViewSource.View.Refresh();
                contactcontact_field_valueViewSource.View.Refresh();
            }
        }

        private void lblCancel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null)
            {
                contact.id_price_list = null;
                contactViewSource.View.Refresh();
            }
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {

        }

        private void lblCancelCost_MouseUp(object sender, MouseButtonEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null)
            {
                contact.id_cost_center = null;
                contactViewSource.View.Refresh();
            }
        }

        private void lblCancelContract_MouseUp(object sender, MouseButtonEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null)
            {
                contact.id_contract = 0;
                contactViewSource.View.Refresh();
            }
        }

      
        private void lblCancelBank_MouseUp(object sender, MouseButtonEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null)
            {
                contact.id_bank = null;
                contactViewSource.View.Refresh();
            }
        }

        private void lblCancelSalesMan_MouseUp(object sender, MouseButtonEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null)
            {
                contact.id_sales_rep = null;
                contactViewSource.View.Refresh();
            }
        }

        private void lblCancelCurrency_MouseUp(object sender, MouseButtonEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null)
            {
                contact.id_currency = null;
                contactViewSource.View.Refresh();
            }
        }

        

    
    }
}
