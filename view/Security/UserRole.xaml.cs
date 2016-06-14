using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Security
{
    public partial class UserRole : Page
    {
        UserRoleDB dbContext = new UserRoleDB();
        CollectionViewSource
            security_rolesecurity_curdViewSource,
            security_roleViewSource,
            security_privilageViewSource,
            security_rolesecurity_role_privilageViewSource;

        public UserRole()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            security_roleViewSource = (CollectionViewSource)this.FindResource("security_roleViewSource");

            dbContext.security_role.Where(a =>
                                            a.is_active == true &&
                                            a.id_company == CurrentSession.Id_Company)
                                            .OrderBy(a => a.name)
                                            .Load();
            security_roleViewSource.Source = dbContext.security_role.Local;


            security_rolesecurity_curdViewSource = (CollectionViewSource)this.FindResource("security_rolesecurity_curdViewSource");
            security_rolesecurity_role_privilageViewSource = (CollectionViewSource)this.FindResource("security_rolesecurity_role_privilageViewSource");

            security_privilageViewSource = (CollectionViewSource)this.FindResource("security_privilageViewSource");
            security_privilageViewSource.Source = dbContext.security_privilage.OrderBy(a => a.name).ToList();

            CollectionViewSource app_departmentViewSource = (CollectionViewSource)this.FindResource("app_departmentViewSource");
            app_departmentViewSource.Source = dbContext.app_department.Where(x => x.id_company == entity.Properties.Settings.Default.company_ID).OrderBy(a => a.name).ToList();
            add_Privallge();
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {

        }

        private void Save_Click(object sender)
        {
            try
            {
                dbContext.SaveChanges();
                entity.CurrentSession.Refresh_Security();
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {

        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbContext.CancelAllChanges();
        }

        private void New_Click(object sender)
        {
            add_Privallge();
            security_role security_role = new security_role();
            security_role.State = EntityState.Added;
            security_role.IsSelected = true;
            dbContext.security_role.Add(security_role);
            add_MissingRecords();
          

            security_roleViewSource.View.Refresh();
            security_roleViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (security_roleDataGrid.SelectedItem != null)
            {


                add_Privallge();
                security_role security_role = (security_role)security_roleDataGrid.SelectedItem;
                security_role.IsSelected = true;
                security_role.State = EntityState.Modified;
                dbContext.Entry(security_role).State = EntityState.Modified;
                add_MissingRecords();
            }
            else
            {
                toolBar.msgWarning("Please Select a record");
            }
        }
        private void add_Privallge()
        {
            List<entity.App.Names> Application = Enum.GetValues(typeof(entity.App.Names)).Cast<entity.App.Names>().ToList();
            List<entity.Privilage.Privilages> Privilages = Enum.GetValues(typeof(entity.Privilage.Privilages)).Cast<entity.Privilage.Privilages>().ToList();
            security_role security_role = (security_role)security_roleDataGrid.SelectedItem;
            foreach (entity.App.Names Names in Application)
            {
                foreach (entity.Privilage.Privilages Privilage in Privilages)
                {
                    if (dbContext.security_privilage.Where(x => x.name == Privilage).Count() == 0)
                    {
                        security_privilage security_privilage = new security_privilage();
                        security_privilage.id_application = Names;
                        security_privilage.name = Privilage;


                        dbContext.security_privilage.Add(security_privilage);
                    }
                }
            }
            dbContext.SaveChanges();
            List<security_privilage> security_privilageList = dbContext.security_privilage.ToList();
            foreach (security_privilage security_privilage in security_privilageList)
            {
                if (security_privilage.id_application==entity.App.Names.SalesInvoice)
                {
                    if (dbContext.security_role_privilage.Where(x => x.security_privilage.name == security_privilage.name && x.security_privilage.id_application == security_privilage.id_application).Count() == 0)
                    {
                        security_role_privilage security_role_privilage = new security_role_privilage();
                        security_role_privilage.id_privilage = security_privilage.id_privilage;
                        security_role_privilage.id_role = security_role.id_role;


                        dbContext.security_role_privilage.Add(security_role_privilage);
                    }
                }
               
                
            }
            dbContext.SaveChanges();

        }
        private void add_MissingRecords()
        {
            if (security_rolesecurity_curdViewSource.View != null)
            {
                security_role security_role = (security_role)security_roleViewSource.View.CurrentItem;

                List<security_role_privilage> security_role_privilageList = dbContext.security_role_privilage.Where(x => x.id_role == security_role.id_role).ToList();
                List<security_role_privilage> privalage = dbContext.security_role_privilage.ToList();
                List<security_role_privilage> finalprivalage = privalage.Except(security_role_privilageList).ToList();

                foreach (security_role_privilage item in finalprivalage)
                {
                    security_role_privilage _security_role_privilage = new security_role_privilage();
                    _security_role_privilage.id_privilage = item.id_privilage;
                    _security_role_privilage.has_privilage = false;
                    security_role.security_role_privilage.Add(_security_role_privilage);
                }

                List<security_curd> security_curd = dbContext.security_curd.Where(x => x.id_role == security_role.id_role).ToList();
                List<entity.App.Names> _DbApplication = security_curd.Select(x => x.id_application).ToList();
                List<entity.App.Names> Application = Enum.GetValues(typeof(entity.App.Names)).Cast<entity.App.Names>().ToList();
                List<entity.App.Names> finalapplicaiton = Enumerable.Except<entity.App.Names>(Application, (IEnumerable<entity.App.Names>)_DbApplication).ToList();

                foreach (entity.App.Names AppName in finalapplicaiton)
                {
                    security_curd _security_curd = new security_curd();
                    _security_curd.id_application = AppName;
                    _security_curd.can_update = false;
                    _security_curd.can_read = false;
                    _security_curd.can_delete = false;
                    _security_curd.can_create = false;
                    _security_curd.can_approve = false;
                    _security_curd.can_annul = false;
                    security_role.security_curd.Add(_security_curd);
                }
                security_rolesecurity_curdViewSource.View.Refresh();
                security_rolesecurity_role_privilageViewSource.View.Refresh();


             
            }
        }
    }
}
