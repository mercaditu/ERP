using entity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.HumanResource
{
    public partial class Employee : Page
    {
        private CollectionViewSource employeeViewSource,
            app_departmentViewSource,
            app_locationViewSource,
            contacthr_contractViewSource,
            contacthr_familyViewSource,
            hr_talentViewSource,
            contacthr_talent_detailViewSource,
        contacthr_educationViewSource;

        private ContactDB dbContext = new ContactDB();

        public Employee()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            employeeViewSource = (CollectionViewSource)FindResource("contactViewSource");
            await dbContext.contacts.Where(i => i.is_employee && i.is_active && i.id_company == CurrentSession.Id_Company).OrderBy(i => i.name).ToListAsync();
            employeeViewSource.Source = dbContext.contacts.Local;
            contacthr_contractViewSource = (CollectionViewSource)FindResource("contacthr_contractViewSource");
            contacthr_familyViewSource = (CollectionViewSource)FindResource("contacthr_familyViewSource");
            contacthr_talent_detailViewSource = (CollectionViewSource)FindResource("contacthr_talent_detailViewSource");
            contacthr_educationViewSource = (CollectionViewSource)FindResource("contacthr_educationViewSource");

            app_departmentViewSource = (CollectionViewSource)FindResource("app_departmentViewSource");
            await dbContext.app_department.ToListAsync();
            app_departmentViewSource.Source = dbContext.app_department.Local;

            hr_talentViewSource = (CollectionViewSource)FindResource("hr_talentViewSource");
            await dbContext.hr_talent.ToListAsync();
            hr_talentViewSource.Source = dbContext.hr_talent.Local;

            app_locationViewSource = (CollectionViewSource)FindResource("app_locationViewSource");
            //await dbContext.app_location.ToListAsync();
            app_locationViewSource.Source = CurrentSession.Locations.ToList(); // dbContext.app_location.Local;

            CollectionViewSource hr_positionViewSource = FindResource("hr_positionViewSource") as CollectionViewSource;
            hr_positionViewSource.Source = dbContext.hr_position.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

            cbxBloodtype.ItemsSource = Enum.GetValues(typeof(contact.BloodTypes));
            cbxmaritialstatus.ItemsSource = Enum.GetValues(typeof(contact.CivilStatus));
            cbxGender.ItemsSource = Enum.GetValues(typeof(contact.Genders));
            cbxRelation.ItemsSource = Enum.GetValues(typeof(hr_family.Relationship));

            cbxContractType.ItemsSource = Enum.GetValues(typeof(hr_contract.WorkTypes));
        }

        #region Contract Buttons

        private void btnNewTask_Click(object sender, EventArgs e)
        {
            contact contact = (contact)employeeViewSource.View.CurrentItem;
            foreach (hr_contract _hr_contract in contact.hr_contract)
            {
                _hr_contract.is_active = false;
            }

            hr_contract hr_contract = new hr_contract();
            hr_contract.start_date = DateTime.Now;
            hr_contract.end_date = DateTime.Now.AddYears(1);
            hr_contract.id_department = dbContext.app_department.FirstOrDefault().id_department;
            hr_contract.app_department = dbContext.app_department.FirstOrDefault();
            hr_contract.is_active = true;
            contact.hr_contract.Add(hr_contract);
            contacthr_contractViewSource.View.MoveCurrentToLast();
            contacthr_contractViewSource.View.Refresh();
        }

        private void btnDeleteTask_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question)
                          == MessageBoxResult.Yes)
            {
                dbContext.hr_contract.Remove((hr_contract)contacthr_contractViewSource.View.CurrentItem);
                contacthr_contractViewSource.View.MoveCurrentToFirst();
            }
        }

        #endregion Contract Buttons

        private void btnNew_Click(object sender)
        {
            contact contact = new contact();
            contact.is_employee = true;
            contact.id_contact_role = dbContext.contact_role.Where(x => x.is_principal).FirstOrDefault().id_contact_role;
            contact.State = EntityState.Added;
            contact.IsSelected = true;

            dbContext.contacts.Add(contact);
            employeeViewSource.View.MoveCurrentToLast();
        }

        private void btnSave_Click(object sender)
        {
            if (dbContext.SaveChanges() > 0)
            {
                toolBar.msgSaved(dbContext.NumberOfRecords);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbContext.CancelAllChanges();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (dgemployee.SelectedItem != null)
            {
                contact Employee_old = (contact)dgemployee.SelectedItem;
                Employee_old.IsSelected = true;
                Employee_old.State = EntityState.Modified;
                dbContext.Entry(Employee_old).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void btnadd_Click(object sender, RoutedEventArgs e)
        {
            contact contact = (contact)employeeViewSource.View.CurrentItem;
            hr_talent_detail hr_talent_detail = new hr_talent_detail();
            hr_talent_detail.hr_talent = (hr_talent)cmbtalent.SelectionBoxItem;
            hr_talent_detail.experience = 0;
            contact.hr_talent_detail.Add(hr_talent_detail);
            contacthr_talent_detailViewSource.View.Refresh();
            contacthr_talent_detailViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && employeeViewSource != null)
            {
                try
                {
                    employeeViewSource.View.Filter = i =>
                    {
                        contact contact = i as contact;
                        if ((contact.name.ToLower().Contains(query.ToLower())
                            || contact.code.ToLower().Contains(query.ToLower()) && contact.is_employee)
                           )
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                catch (Exception ex)
                {
                    toolBar.msgError(ex);
                }
            }
            else
            {
                employeeViewSource.View.Filter = null;
            }
        }

        private async void SmartBox_Geography_Select(object sender, RoutedEventArgs e)
        {
            contact contact = (contact)employeeViewSource.View.CurrentItem;
            if (smtgeo.GeographyID > 0)
            {
                contact.app_geography = await dbContext.app_geography.Where(p => p.id_geography == smtgeo.GeographyID).FirstOrDefaultAsync();
            }
        }

        private void DeleteCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as hr_family != null)
            {
                e.CanExecute = true;
            }
            if (e.Parameter as hr_talent_detail != null)
            {
                e.CanExecute = true;
            }
            if (e.Parameter as hr_education != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (e.Parameter as hr_family != null)
                    {
                        hr_familyDataGrid.CancelEdit();
                        dbContext.hr_family.Remove(e.Parameter as hr_family);
                        contacthr_familyViewSource.View.Refresh();
                    }
                    if (e.Parameter as hr_talent_detail != null)
                    {
                        hr_talent_detailDataGrid.CancelEdit();
                        dbContext.hr_talent_detail.Remove(e.Parameter as hr_talent_detail);
                        contacthr_talent_detailViewSource.View.Refresh();
                    }
                    if (e.Parameter as hr_education != null)
                    {
                        //DeleteDetailGridRow
                        hr_educationDataGrid.CancelEdit();
                        dbContext.hr_education.Remove(e.Parameter as hr_education);
                        contacthr_educationViewSource.View.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }
    }
}