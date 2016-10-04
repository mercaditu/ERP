using Cognitivo.Menu;
using entity;
using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data;
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

        entity.CurrentSession.Versions oldversion;
        public UserRole()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            security_roleViewSource = (CollectionViewSource)this.FindResource("security_roleViewSource");

            await dbContext.security_role.Where(a =>
                                            a.is_active == true &&
                                            a.id_company == CurrentSession.Id_Company)
                                            .OrderBy(a => a.name)
                                            .LoadAsync();
            security_roleViewSource.Source = dbContext.security_role.Local;

            security_rolesecurity_curdViewSource = (CollectionViewSource)this.FindResource("security_rolesecurity_curdViewSource");
            security_rolesecurity_role_privilageViewSource = (CollectionViewSource)this.FindResource("security_rolesecurity_role_privilageViewSource");

            security_privilageViewSource = (CollectionViewSource)this.FindResource("security_privilageViewSource");
            security_privilageViewSource.Source = await dbContext.security_privilage.OrderBy(a => a.name).ToListAsync();

            CollectionViewSource app_departmentViewSource = (CollectionViewSource)this.FindResource("app_departmentViewSource");
            app_departmentViewSource.Source = await dbContext.app_department.Where(x => x.id_company == entity.Properties.Settings.Default.company_ID).OrderBy(a => a.name).ToListAsync();

            add_Privallge();
            cbxVersion.ItemsSource = Enum.GetValues(typeof(entity.CurrentSession.Versions));
           
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {

        }

        private void Save_Click(object sender)
        {
            try
            {
                security_role security_role = (security_role)security_roleDataGrid.SelectedItem;
                entity.CurrentSession.Versions version = (entity.CurrentSession.Versions)Enum.Parse(typeof(entity.CurrentSession.Versions), Convert.ToString(cbxVersion.Text));
                entity.Brillo.Activation Activation = new entity.Brillo.Activation();
                Activation.VersionEncrypt(version, security_role);
                dbContext.SaveChanges();
                entity.CurrentSession.Refresh_Security();
                security_roleViewSource.View.Refresh();
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
            security_role security_role = (security_role)security_roleDataGrid.SelectedItem;
            security_role.IsSelected = false;
            security_role.State = EntityState.Unchanged;
            dbContext.Entry(security_role).State = EntityState.Unchanged;
        }

        private void New_Click(object sender)
        {
            add_Privallge();
            add_MissingRecords();
            security_role security_role = new security_role();
            security_role.State = EntityState.Added;
            security_role.IsSelected = true;
            dbContext.security_role.Add(security_role);



            security_roleViewSource.View.Refresh();
            security_roleViewSource.View.MoveCurrentToLast();

        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (security_roleDataGrid.SelectedItem != null)
            {
                add_Privallge();
                add_MissingRecords();
                security_role security_role = (security_role)security_roleDataGrid.SelectedItem;
                security_role.IsSelected = true;
                security_role.State = EntityState.Modified;
                dbContext.Entry(security_role).State = EntityState.Modified;

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
                if (Names == entity.App.Names.SalesInvoice)
                {
                    foreach (entity.Privilage.Privilages Privilage in Privilages)
                    {
                        if (dbContext.security_privilage.Where(x => x.name == Privilage).Count() == 0)
                        {
                            if ((int)Privilage >= 3)
                            {
                                security_privilage security_privilage = new security_privilage();
                                security_privilage.id_application = entity.App.Names.ProductionExecution;
                                security_privilage.name = Privilage;
                                dbContext.security_privilage.Add(security_privilage);
                            }
                            else
                            {
                                security_privilage security_privilage = new security_privilage();
                                security_privilage.id_application = Names;
                                security_privilage.name = Privilage;
                                dbContext.security_privilage.Add(security_privilage);

                            }

                        }
                    }
                }
            }

            List<entity.App.Names> PreferenceList = Enum.GetValues(typeof(entity.App.Names)).Cast<entity.App.Names>().ToList();
            List<security_privilage> security_privilageList = dbContext.security_privilage.ToList();

            List<security_role_privilage> security_role_privilage = new List<security_role_privilage>();
            security_role_privilage = dbContext.security_role_privilage.Where(x => x.id_role == security_role.id_role).ToList();

            foreach (security_privilage security_privilage in security_privilageList)
            {
                if (security_privilage.id_application == entity.App.Names.SalesInvoice ||
                    security_privilage.id_application == entity.App.Names.ProductionExecution)
                {
                    if (security_role_privilage.Where(x => x.id_privilage == security_privilage.id_privilage).Count() == 0)
                    {
                        security_role_privilage _security_role_privilage = new security_role_privilage();
                        _security_role_privilage.id_privilage = security_privilage.id_privilage;
                        _security_role_privilage.security_privilage = security_privilage;
                        _security_role_privilage.id_role = security_role.id_role;
                        _security_role_privilage.security_role = security_role;

                        dbContext.security_role_privilage.Add(_security_role_privilage);
                    }
                }
            }
        }

      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbxVersion.SelectedIndex > 0)
            {
                add_MissingRecords();
            }
        }

        private void add_MissingRecords()
        {
            AppList appList = new AppList();
            security_role security_role = (security_role)security_roleDataGrid.SelectedItem;
            List<security_crud> security_curd = dbContext.security_curd.Where(x => x.id_role == security_role.id_role).ToList();

            if (security_curd.Count() == 0)
            {
                List<entity.App.Names> Application = Enum.GetValues(typeof(entity.App.Names)).Cast<entity.App.Names>().ToList();
                foreach (entity.App.Names AppName in Application)
                {
                    security_crud _security_curd = new security_crud();
                    _security_curd.id_application = AppName;
                    _security_curd.can_update = false;
                    _security_curd.can_read = false;
                    _security_curd.can_delete = false;
                    _security_curd.can_create = false;
                    _security_curd.can_approve = false;
                    _security_curd.can_annul = false;
                    security_role.security_curd.Add(_security_curd);
                }
            }
            else
            {
                Activation _Activation = new Activation();
                CurrentSession.Versions version = (entity.CurrentSession.Versions)Enum.Parse(typeof(entity.CurrentSession.Versions), Convert.ToString(cbxVersion.Text));
                if (oldversion < version)
                {
                    List<entity.App.Names> dtApplication = new List<entity.App.Names>();
                    foreach (DataRow item in appList.dtApp.Select("Version='" + version + "'"))
                    {
                        if (Enum.IsDefined(typeof(entity.App.Names), Convert.ToString(item["name"])) == true)
                        {
                            dtApplication.Add((entity.App.Names)Enum.Parse(typeof(entity.App.Names), Convert.ToString(item["name"])));
                        }

                    }

                    List<entity.App.Names> _security_curdApplication = security_curd.Select(x => x.id_application).ToList();
                    List<entity.App.Names> AddList = Enumerable.Except<entity.App.Names>(dtApplication, (IEnumerable<entity.App.Names>)_security_curdApplication).ToList();
                    foreach (entity.App.Names AppName in AddList)
                    {
                        security_crud _security_curd = new security_crud();
                        _security_curd.id_application = AppName;
                        _security_curd.can_update = false;
                        _security_curd.can_read = false;
                        _security_curd.can_delete = false;
                        _security_curd.can_create = false;
                        _security_curd.can_approve = false;
                        _security_curd.can_annul = false;
                        security_role.security_curd.Add(_security_curd);
                    }
                    oldversion = version;
                }
                else
                {
                    List<entity.App.Names> dtApplication = new List<entity.App.Names>();
                    foreach (DataRow item in appList.dtApp.Select("Version='" + version + "'"))
                    {
                        if (Enum.IsDefined(typeof(entity.App.Names), Convert.ToString(item["name"])) == true)
                        {
                            dtApplication.Add((entity.App.Names)Enum.Parse(typeof(entity.App.Names), Convert.ToString(item["name"])));
                        }

                    }

                    List<entity.App.Names> _security_curdApplication = security_curd.Select(x => x.id_application).ToList();
                    List<entity.App.Names> AddList = Enumerable.Except<entity.App.Names>( (IEnumerable<entity.App.Names>)_security_curdApplication, dtApplication).ToList();
                    foreach (entity.App.Names AppName in AddList)
                    {
                        security_crud _security_curd = dbContext.security_curd.Where(x => x.id_application == AppName).FirstOrDefault();
                        if (_security_curd != null)
                        {
                            security_role.security_curd.Remove(_security_curd);
                        }

                    }
                    oldversion = version;
                }





            }
            security_rolesecurity_curdViewSource.View.Refresh();
            security_rolesecurity_role_privilageViewSource.View.Refresh();

          
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            security_role security_role = security_roleViewSource.View.CurrentItem as security_role;

            if (security_role != null)
            {
                if (security_rolesecurity_curdViewSource != null)
                {
                    if (security_rolesecurity_curdViewSource.View != null)
                    {
                        security_rolesecurity_curdViewSource.View.Filter = i =>
                        {
                            security_crud security_curd = (security_crud)i;
                            string TranslatedName = entity.Brillo.Localize.StringText(security_curd.id_application.ToString());
                            if (TranslatedName.ToUpper().Contains(txtsearch.Text.ToUpper()))
                                return true;
                            else
                                return false;
                        };
                    }
                }
            }
        }



        private void security_roleDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            security_role security_role = security_roleViewSource.View.CurrentItem as security_role;
            if (security_role != null)
            {
                entity.Brillo.Activation Activation = new entity.Brillo.Activation();
                CurrentSession.Versions version = Activation.VersionDecrypt(security_role);
                cbxVersion.SelectedItem = version;
                oldversion = version;
            }
        }
    }
}
