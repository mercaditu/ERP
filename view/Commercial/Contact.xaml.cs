using entity;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Commercial
{
    public partial class Contact : Page
    {
        private entity.Controller.Commercial.ContactController ContactDB;

        private CollectionViewSource contactChildListViewSource;
        private CollectionViewSource contactViewSource;
        private CollectionViewSource contactcontact_field_valueViewSource;
        private CollectionViewSource contactcontact_field_valueemailViewSource;
        private CollectionViewSource contactcontact_field_valuephoneViewSource;
        private CollectionViewSource app_fieldViewSource;
        private CollectionViewSource app_fieldemailViewSource;
        private CollectionViewSource app_fieldphoneViewSource;

        #region Initilize and load

        public Contact()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                ContactDB = FindResource("ContactController") as entity.Controller.Commercial.ContactController;
                //Load Controller.
                ContactDB.Initialize();
            }
        }

        private void Page_Loaded(object sender, EventArgs e)
        {
            contactChildListViewSource = (CollectionViewSource)FindResource("contactChildListViewSource");
            contactcontact_field_valueViewSource = (CollectionViewSource)FindResource("contactcontact_field_valueViewSource");
            contactcontact_field_valueemailViewSource = (CollectionViewSource)FindResource("contactcontact_field_valueemailViewSource");
            contactcontact_field_valuephoneViewSource = (CollectionViewSource)FindResource("contactcontact_field_valuephoneViewSource");

            Window Win = Window.GetWindow(this) as Window;

            if (Win.Title == entity.Brillo.Localize.StringText("Customer"))
            {
                ContactDB.LoadCustomers();
            }
            else if (Win.Title == entity.Brillo.Localize.StringText("Suppliers"))
            {
                ContactDB.LoadSuppliers();
            }
          
            contactViewSource = (CollectionViewSource)FindResource("contactViewSource");
            contactViewSource.Source = ContactDB.db.contacts.Local;

            CollectionViewSource contactParentViewSource = (CollectionViewSource)FindResource("contactParentViewSource");
            contactParentViewSource.Source = ContactDB.db.contacts.Local;

            //ContactRole
            CollectionViewSource contactRoleViewSource = (CollectionViewSource)FindResource("contactRoleViewSource");
            contactRoleViewSource.Source = ContactDB.db.contact_role.Local;

            //AppContract
            CollectionViewSource appContractViewSource = FindResource("appContractViewSource") as CollectionViewSource;
            appContractViewSource.Source = CurrentSession.Contracts.OrderBy(x => x.name);

            //AppCostCenter
            CollectionViewSource appCostCenterViewSource = FindResource("appCostCenterViewSource") as CollectionViewSource;
            appCostCenterViewSource.Source = ContactDB.db.app_cost_center.Local;

            //ItemPriceList
            CollectionViewSource itemPriceListViewSource = FindResource("itemPriceListViewSource") as CollectionViewSource;
            itemPriceListViewSource.Source = CurrentSession.PriceLists.OrderBy(a => a.name);

            CollectionViewSource salesRepViewSource = FindResource("salesRepViewSource") as CollectionViewSource;
            salesRepViewSource.Source = CurrentSession.SalesReps.OrderBy(a => a.name);

            CollectionViewSource salesRepViewSourceCollector = FindResource("salesRepViewSourceCollector") as CollectionViewSource;
            salesRepViewSourceCollector.Source = CurrentSession.SalesReps.OrderBy(a => a.name);

            //AppCurrency
            CollectionViewSource app_currencyViewSource = FindResource("app_currencyViewSource") as CollectionViewSource;
            app_currencyViewSource.Source = CurrentSession.Currencies.OrderBy(a => a.name);

            //AppBank
            CollectionViewSource bankViewSource = FindResource("bankViewSource") as CollectionViewSource;
            bankViewSource.Source = ContactDB.db.app_bank.Local;

            app_fieldemailViewSource = FindResource("app_fieldemailViewSource") as CollectionViewSource;
            app_fieldphoneViewSource = FindResource("app_fieldphoneViewSource") as CollectionViewSource;
            app_fieldViewSource = FindResource("app_fieldViewSource") as CollectionViewSource;

            app_fieldemailViewSource.Source = ContactDB.db.app_field.Local.Where(x => x.field_type == app_field.field_types.Email).ToList();
            app_fieldphoneViewSource.Source = ContactDB.db.app_field.Local.Where(x => x.field_type == app_field.field_types.Telephone).ToList();
            app_fieldViewSource.Source = ContactDB.db.app_field.Local.Where(x => x.field_type == app_field.field_types.Account).ToList();

            //Gender Type Enum
            cbxGender.ItemsSource = Enum.GetValues(typeof(contact.Genders));

            CollectionViewSource contact_tagViewSource = ((CollectionViewSource)(FindResource("contact_tagViewSource")));
            contact_tagViewSource.Source = ContactDB.db.contact_tag.Local;

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = CurrentSession.VAT_Groups.OrderBy(a => a.name);

            Filter();
        }

        #endregion Initilize and load

        #region Toolbar Events

        private void toolBar_btnNew_Click(object sender)
        {
            contact contact = ContactDB.Create();
            
            ContactDB.db.contacts.Add(contact);
            contactViewSource.View.Refresh();
            contactViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    contact contact = listContacts.SelectedItem as contact;
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
                ContactDB.Edit(ref contact);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (ContactDB.SaveChanges_WithValidation())
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

        #endregion Toolbar Events

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
                }
                else
                {
                    tabFinance.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Filter()
        {
            if (contactcontact_field_valueViewSource != null)
            {
                if (contactcontact_field_valueViewSource.View != null)
                {
                    contactcontact_field_valueViewSource.View.Filter = i =>
                    {
                        contact_field_value contact_field_value = (contact_field_value)i;
                        if (contact_field_value != null)
                        {
                            if (contact_field_value.app_field.field_type == app_field.field_types.Account)
                            {
                                return true;
                            }
                        }
                        return false;
                    };
                }
            }

            if (contactcontact_field_valueemailViewSource != null)
            {
                if (contactcontact_field_valueemailViewSource.View != null)
                {
                    contactcontact_field_valueemailViewSource.View.Filter = i =>
                    {
                        contact_field_value contact_field_value = (contact_field_value)i;
                        if (contact_field_value != null)
                        {
                            if (contact_field_value.app_field.field_type == app_field.field_types.Email)
                            {
                                return true;
                            }
                        }
                        return false;
                    };
                }
            }

            if (contactcontact_field_valuephoneViewSource != null)
            {
                if (contactcontact_field_valuephoneViewSource.View != null)
                {
                    contactcontact_field_valuephoneViewSource.View.Filter = i =>
                    {
                        contact_field_value contact_field_value = (contact_field_value)i;
                        if (contact_field_value != null)
                        {
                            if (contact_field_value.app_field.field_type == app_field.field_types.Telephone)
                            {
                                return true;
                            }
                        }
                        return false;
                    };
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
                        ContactDB.db.contact_field_value.Remove(e.Parameter as contact_field_value);
                        //contactcontact_field_valueViewSource.View.Refresh();
                    }
                    else if (e.Parameter as contact_tag_detail != null)
                    {
                        contact_tag_detailDataGrid.CancelEdit();
                        ContactDB.db.contact_tag_detail.Remove(e.Parameter as contact_tag_detail);

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

        private void LoadRelatedContactOnThread(contact ParentContact)
        {
            if (contactChildListViewSource != null)
            {
                contactChildListViewSource = (CollectionViewSource)FindResource("contactChildListViewSource");
                contactChildListViewSource.Source = ContactDB.db.contacts
                    .Where(x => x.parent.id_contact == ParentContact.id_contact || x.id_contact == ParentContact.id_contact).OrderBy(x => x.name).ToList();
                contactChildListViewSource.View.Refresh();
            }
        }

        private async void SmartBox_Geography_Select(object sender, RoutedEventArgs e)
        {
            contact contact = (contact)contactViewSource.View.CurrentItem;
            if (smtgeo.GeographyID > 0)
            {
                contact.app_geography = await ContactDB.db.app_geography.Where(p => p.id_geography == smtgeo.GeographyID).FirstOrDefaultAsync();
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

        private void Add_Tag()
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

        private async void listContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;

            if (contact != null)
            {
                LoadRelatedContactOnThread(contact);

                using (db db = new db())
                {
                    CollectionViewSource app_attachmentViewSource = ((CollectionViewSource)(FindResource("app_attachmentViewSource")));
                    app_attachmentViewSource.Source = await db.app_attachment
                        .Where(x => x.application == entity.App.Names.Contact && x.reference_id == contact.id_contact && x.mime.Contains("image")).Take(1).ToListAsync();
                }
            }
        }

        private void cbxRelation_Select(object sender, RoutedEventArgs e)
        {
            contact SelectedContact = (contact)contactViewSource.View.CurrentItem;
            if (SelectedContact != null && cbxRelation.ContactID > 0)
            {
                contact ParentContact = ContactDB.db.contacts.Where(x => x.id_contact == cbxRelation.ContactID).FirstOrDefault();
                if (ParentContact != null)
                {
                    //Clean these values to prevent Selected Contact from appearing in further reports or windows.
                    SelectedContact.is_customer = false;
                    SelectedContact.is_supplier = false;

                    ParentContact.child.Add(SelectedContact);
                    LoadRelatedContactOnThread(ParentContact);
                }
            }
        }

        private void contactcontact_subscriptionDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            // FilterSubscription();
        }

        private void MapsDropPin_Click(object sender, MouseButtonEventArgs e)
        {
            // Disables the default mouse double-click action.
            e.Handled = true;

            // Determin the location to place the pushpin at on the map.

            //Get the mouse click coordinates
            var mousePosition = e.GetPosition((UIElement)sender);

            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);

            // The pushpin to add to the map.
            Pushpin pin = new Pushpin();
            pin.Location = pinLocation;

            // Adds the pushpin to the map.
            myMap.Children.Clear();
            myMap.Children.Add(pin);

            contact contact = (contact)contactViewSource.View.CurrentItem;
            if (contact != null)
            {
                contact.geo_longlat = pinLocation.ToString();
            }
        }

        private void Add_fieldAccount(object sender, MouseButtonEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null)
            {
                app_field app_field = app_fieldViewSource.View.CurrentItem as app_field;
                if (app_field == null)
                {
                    app_field = new app_field();
                    app_field.field_type = app_field.field_types.Account;
                    app_field.name = "Account";
                    ContactDB.db.app_field.Add(app_field);

                    app_fieldViewSource.View.Refresh();
                    app_fieldViewSource.View.MoveCurrentTo(app_field);
                }
                contact_field_value contact_field_value = new contact_field_value();
                contact_field_value.app_field = app_field;
                contact.contact_field_value.Add(contact_field_value);
                contactViewSource.View.Refresh();
                contactcontact_field_valueViewSource.View.Refresh();
                contactcontact_field_valueViewSource.View.MoveCurrentToLast();
            }
        }

        private void Add_fieldEmail(object sender, MouseButtonEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;

            if (contact != null)
            {
                app_field app_field = app_fieldemailViewSource.View.CurrentItem as app_field;

                if (app_field == null)
                {
                    app_field = new app_field();
                    app_field.field_type = app_field.field_types.Email;
                    app_field.name = "Work";
                    ContactDB.db.app_field.Add(app_field);

                    app_fieldemailViewSource.View.Refresh();
                    app_fieldemailViewSource.View.MoveCurrentTo(app_field);
                }

                contact_field_value contact_field_value = new contact_field_value();
                contact_field_value.app_field = app_field;
                contact.contact_field_value.Add(contact_field_value);
                contactViewSource.View.Refresh();
                contactcontact_field_valueemailViewSource.View.Refresh();
                contactcontact_field_valueemailViewSource.View.MoveCurrentToLast();
            }
        }

        private void Add_fieldTelephone(object sender, MouseButtonEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null)
            {
                app_field app_field = app_fieldphoneViewSource.View.CurrentItem as app_field;
                if (app_field == null)
                {
                    app_field = new app_field();
                    app_field.field_type = app_field.field_types.Telephone;
                    app_field.name = "Work";
                    ContactDB.db.app_field.Add(app_field);

                    app_fieldphoneViewSource.View.Refresh();
                    app_fieldphoneViewSource.View.MoveCurrentTo(app_field);
                }

                contact_field_value contact_field_value = new contact_field_value();
                contact_field_value.app_field = app_field;
                contact.contact_field_value.Add(contact_field_value);

                contactViewSource.View.Refresh();
                contactcontact_field_valuephoneViewSource.View.Refresh();
                contactcontact_field_valuephoneViewSource.View.MoveCurrentToLast();
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

        private void Border_Drop(object sender, DragEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null && contact.id_contact > 0)
            {
                var data = e.Data as DataObject;
                entity.Brillo.Attachment Attachment = new entity.Brillo.Attachment();
                Attachment.SaveFile(data, entity.App.Names.Contact, contact.id_contact);
                listContacts_SelectionChanged(sender, null);
            }
            else
            {
                MessageBox.Show("Please save Contact before inserting an Image", "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CollectionViewSource_Account_Filter(object sender, FilterEventArgs e)
        {
            CollectionViewSource contactcontact_field_valueViewSource = FindResource("contactcontact_field_valueViewSource") as CollectionViewSource;
            if (contactcontact_field_valueViewSource.View != null)
            {
                contactcontact_field_valueViewSource.View.Filter = i =>
                {
                    contact_field_value contact_field_value = i as contact_field_value;

                    if (contact_field_value.app_field != null)
                    {
                        app_field app_field = contact_field_value.app_field;
                        if (app_field.field_type == app_field.field_types.Account)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                };
            }
        }

        private void CollectionViewSource_Email_Filter(object sender, FilterEventArgs e)
        {
            CollectionViewSource contactcontact_field_valueemailViewSource = (CollectionViewSource)FindResource("contactcontact_field_valueemailViewSource");
            if (contactcontact_field_valueemailViewSource.View != null)
            {
                contactcontact_field_valueemailViewSource.View.Filter = i =>
                {
                    contact_field_value contact_field_value = i as contact_field_value;

                    if (contact_field_value.app_field != null)
                    {
                        app_field app_field = contact_field_value.app_field;
                        if (app_field.field_type == app_field.field_types.Email)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                };
            }
        }

        private void CollectionViewSource_Telephone_Filter(object sender, FilterEventArgs e)
        {
            CollectionViewSource contactcontact_field_valuephoneViewSource = (CollectionViewSource)FindResource("contactcontact_field_valuephoneViewSource");
            if (contactcontact_field_valuephoneViewSource.View != null)
            {
                contactcontact_field_valuephoneViewSource.View.Filter = i =>
                {
                    contact_field_value contact_field_value = i as contact_field_value;

                    if (contact_field_value.app_field != null)
                    {
                        app_field app_field = contact_field_value.app_field;
                        if (app_field.field_type == app_field.field_types.Telephone)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                };
            }
        }
    }
}