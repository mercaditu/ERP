using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;

namespace Cognitivo.Sales
{
    public partial class Salesman : Page
    {
        //dbContextContext entity = new dbContextContext();
        SalesmanDB dbContext = new SalesmanDB();
        CollectionViewSource sales_repViewSource, contactViewSource = null;
        ContactDB ContactdbContext = new ContactDB();
        contact _contact = new contact();

        public Salesman()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            sales_repViewSource = FindResource("sales_repViewSource") as CollectionViewSource;
            dbContext.sales_rep.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
            sales_repViewSource.Source = dbContext.sales_rep.Local;

            contactViewSource = ((CollectionViewSource)(FindResource("contactViewSource")));
            dbContext.contacts.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
            contactViewSource.Source = dbContext.contacts.Local;

            cbxSalesRepType.ItemsSource = Enum.GetValues(typeof(sales_rep.SalesRepType));
        }

        private void toolBar_btnNew_Click(object sender)
        {
            sales_rep sales_rep = new sales_rep();
            sales_rep.State = EntityState.Added;
            sales_rep.IsSelected = true;
            sales_rep.timestamp = DateTime.Now.AddDays(0);

            dbContext.Entry(sales_rep).State = EntityState.Added;
            sales_rep.State = EntityState.Added;
            sales_repViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (dbContext.SaveChanges() > 0)
            {
                sales_repViewSource.View.Refresh();
                toolBar.msgSaved(dbContext.NumberOfRecords);
                CurrentSession.Load_BasicData(null, null);
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    dbContext.sales_rep.Remove((sales_rep)sales_repDataGrid.SelectedItem);
                    //sales_repViewSource.View.MoveCurrentToFirst();
                    toolBar_btnSave_Click(sender);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            sales_repViewSource.View.MoveCurrentToFirst();
            dbContext.CancelAllChanges();
            stackMain.IsEnabled = false;
            sales_repDataGrid.IsEnabled = true;
            stackExisting.Visibility = Visibility.Visible;
            stackContact.Visibility = Visibility.Visible;
            txtblkAddContact.Visibility = Visibility.Collapsed;
        }

        private void cbxSalesRepType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stackContact.Visibility = Visibility.Visible;
            if (cbxSalesRepType.SelectedItem != null)
            {
                sales_rep sales_rep = new sales_rep();
                contactViewSource.View.Filter = i =>
                {
                    contact contact = i as contact;
                    int SelectedIndex = (cbxSalesRepType.SelectedIndex) + 1;
                    if (SelectedIndex == (int)sales_rep.SalesRepType.Salesman ||
                        SelectedIndex == (int)sales_rep.SalesRepType.Collector)
                    {
                        if (contact.is_employee == true || contact.is_supplier == true)
                            return true;
                        else
                            return false;
                    }
                    else if (SelectedIndex == (int)sales_rep.SalesRepType.PurchaseAgent)
                    {
                        if (contact.is_customer == true)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                };
            }
        }

        private void cbxContact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stackExisting.Visibility = Visibility.Visible;
        }

        private void Hyperlink_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.contact contact = new cntrl.Curd.contact();
            crud_modal.Children.Add(contact);
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (sales_repDataGrid.SelectedItem != null)
            {
                sales_rep sales_rep_rep = (sales_rep)sales_repDataGrid.SelectedItem;
                sales_rep_rep.IsSelected = true;
                sales_rep_rep.State = EntityState.Modified;
                dbContext.Entry(sales_rep_rep).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select a Record");
            }
        }

        #region Filter Data
        private void set_ContactPrefKeyStroke(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                set_ContactPref(sender, e);
            }
        }

        private void set_ContactPref(object sender, EventArgs e)
        {
            try
            {
                if (contactComboBox.Data != null)
                {
                    contact contact = (contact)contactComboBox.Data;
                                    contactComboBox.focusGrid = false;
                    contactComboBox.Text = contact.name;
                 
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        #endregion

        public void Save_Click(object sender)
        {
            if (_contact.State == EntityState.Added)
            {
                ContactdbContext.contacts.Add(_contact);
            }
            _contact.IsSelected = true;


            IEnumerable<DbEntityValidationResult> validationresult = ContactdbContext.GetValidationErrors();
            if (validationresult.Count() == 0)
            {
                    ContactdbContext.SaveChanges();

                crud_modal.Children.Clear();
                crud_modal.Visibility = System.Windows.Visibility.Collapsed;
                contactComboBox.Text = _contact.name;
                
            }
            else
            {
                MessageBox.Show("error");
            }

        }
    }
}
