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
        ContactDB dbContext = new ContactDB();
        CollectionViewSource contactChildListViewSource;
        CollectionViewSource contactViewSource;

        #region Initilize and load
        public Contact()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            entity.Properties.Settings _entity = new entity.Properties.Settings();

            //Contact
            dbContext.contacts.Where(a => a.is_active == true && a.id_company == _entity.company_ID && a.is_employee == false).OrderBy(a => a.name).Load();

            contactViewSource = (CollectionViewSource)FindResource("contactViewSource");
            contactViewSource.Source = dbContext.contacts.Local;
            CollectionViewSource contactParentViewSource = (CollectionViewSource)FindResource("contactParentViewSource");
            contactParentViewSource.Source = dbContext.contacts.Local;
            contactChildListViewSource = (CollectionViewSource)FindResource("contactChildListViewSource");
            contactChildListViewSource.Source = dbContext.contacts.Local;

            //ContactRole
            CollectionViewSource contactRoleViewSource = (CollectionViewSource)FindResource("contactRoleViewSource");
            contactRoleViewSource.Source = dbContext.contact_role.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //AppContract
            CollectionViewSource appContractViewSource = (CollectionViewSource)FindResource("appContractViewSource");
            appContractViewSource.Source = dbContext.app_contract.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //AppCostCenter
            CollectionViewSource appCostCenterViewSource = (CollectionViewSource)FindResource("appCostCenterViewSource");
            appCostCenterViewSource.Source = dbContext.app_cost_center.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //ItemPriceList
            CollectionViewSource itemPriceListViewSource = (CollectionViewSource)FindResource("itemPriceListViewSource");
            itemPriceListViewSource.Source = dbContext.item_price_list.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //SalesRep
            List<sales_rep> sales_rep = dbContext.sales_rep.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();
            CollectionViewSource salesRepViewSource = (CollectionViewSource)FindResource("salesRepViewSource");
            salesRepViewSource.Source = sales_rep.ToList();
            CollectionViewSource salesRepViewSourceCollector = (CollectionViewSource)FindResource("salesRepViewSourceCollector");
            salesRepViewSourceCollector.Source = sales_rep.ToList();

            //AppCurrency
            CollectionViewSource app_currencyViewSource = (CollectionViewSource)FindResource("app_currencyViewSource");
            app_currencyViewSource.Source = dbContext.app_currency.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //Items
            //CollectionViewSource itemViewSource = (CollectionViewSource)FindResource("itemViewSource");
            //itemViewSource.Source = dbContext.items.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

            //CollectionViewSource app_fieldViewSource = (CollectionViewSource)FindResource("app_fieldViewSource");
            //app_fieldViewSource.Source = dbContext.app_field.OrderBy(a => a.name).AsNoTracking().ToList();

            //contactcontact_field_valueViewSource = FindResource("contactcontact_field_valueViewSource") as CollectionViewSource;

            //Gender Type Enum
            cbxGender.ItemsSource = Enum.GetValues(typeof(contact.Genders));
        }
        #endregion

        #region Toolbar Events
        private void toolBar_btnNew_Click(object sender)
        {
            contact contact = new contact();
            contact.is_employee = false;
            contact.State = EntityState.Added;
            contact.IsSelected = true;
            dbContext.contacts.Add(contact);
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
                dbContext.SaveChanges();
             
             
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
            dbContext.CancelAllChanges();
        }
        #endregion

        private void cbxContactRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxContactRole.SelectedItem != null)
            {
                contact_role contact_role = cbxContactRole.SelectedItem as contact_role;
                if (contact_role.is_principal == true)
                    cbxRelation.IsEnabled = false;
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
                        dbContext.contact_field_value.Remove(e.Parameter as contact_field_value);
                        //contactcontact_field_valueViewSource.View.Refresh();
                    }
                    else if (e.Parameter as contact_subscription != null)
                    {
                        contactcontact_subscriptionDataGrid.CancelEdit();
                        dbContext.contact_subscription.Remove(e.Parameter as contact_subscription);

                        CollectionViewSource contactcontact_subscriptionViewSource = FindResource("contactcontact_subscriptionViewSource") as CollectionViewSource;
                        contactcontact_subscriptionViewSource.View.Refresh();
                    }
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }

        private void cbxRelation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contact _contact = (contact)cbxRelation.SelectedItem;
            Task taskdb = Task.Factory.StartNew(() => LoadRelatedContactOnThread(_contact));



        }

        private void LoadRelatedContactOnThread(contact _contact)
        {
            if (contactChildListViewSource != null)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (contactChildListViewSource.View != null)
                    {

                        contactChildListViewSource.View.Filter = i =>
                        {
                            contact contact = (contact)i;
                            if (contact.parent == _contact && _contact != null)
                                return true;
                            else
                                return false;
                        };

                    }
                }));

            }
        }

        private void toolIcon_Click(object sender, RoutedEventArgs e)
        {
            entity.Brillo.Logic.Document Document = new entity.Brillo.Logic.Document();
            contact contact = (contact)listContacts.SelectedItem;
            Document.Document_PrintCarnetContact(contact);
        }

        private void SmartBox_Geography_Select(object sender, RoutedEventArgs e)
        {
            contact contact = (contact)contactViewSource.View.CurrentItem;
            if (smtgeo.GeographyID>0)
            {
                contact.id_geography = smtgeo.GeographyID;
            }
        }
    }
}
