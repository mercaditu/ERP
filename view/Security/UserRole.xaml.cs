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
        UserRoleDB UserRoleDB = new UserRoleDB();
        CollectionViewSource
            security_rolesecurity_curdViewSource,
            security_roleViewSource,
            security_privilageViewSource,
            security_rolesecurity_role_privilageViewSource;

        entity.CurrentSession.Versions CurrentVersion;

        public UserRole()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            security_roleViewSource = (CollectionViewSource)this.FindResource("security_roleViewSource");

            await UserRoleDB.security_role.Where(a =>
                                            a.id_company == CurrentSession.Id_Company)
                                            .OrderBy(a => a.name)
                                            .LoadAsync();
            security_roleViewSource.Source = UserRoleDB.security_role.Local;

            security_rolesecurity_curdViewSource = (CollectionViewSource)this.FindResource("security_rolesecurity_curdViewSource");
            security_rolesecurity_role_privilageViewSource = (CollectionViewSource)this.FindResource("security_rolesecurity_role_privilageViewSource");

            security_privilageViewSource = (CollectionViewSource)this.FindResource("security_privilageViewSource");
            security_privilageViewSource.Source = await UserRoleDB.security_privilage.OrderBy(a => a.name).ToListAsync();

            CollectionViewSource app_departmentViewSource = (CollectionViewSource)this.FindResource("app_departmentViewSource");
            app_departmentViewSource.Source = await UserRoleDB.app_department.Where(x => x.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();

            add_Privallge();
            cbxVersion.ItemsSource = Enum.GetValues(typeof(CurrentSession.Versions));
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {

        }

        private void Save_Click(object sender)
        {
            try
            {
                security_role security_role = (security_role)security_roleDataGrid.SelectedItem;
                if (security_role != null)
                {
                    CurrentSession.Versions version = (CurrentSession.Versions)Enum.Parse(typeof(CurrentSession.Versions), Convert.ToString(cbxVersion.Text));
                    security_role.Version = version;

                    UserRoleDB.SaveChanges();

                    CurrentSession.Load_Security();
                    security_roleViewSource.View.Refresh();
                }
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
            UserRoleDB.Entry(security_role).State = EntityState.Unchanged;
        }

        private void New_Click(object sender)
        {
            security_role security_role = new security_role();
            if (security_role != null)
            {
                add_Privallge();
                add_MissingRecords(security_role);
                security_role.State = EntityState.Added;
                security_role.Version = CurrentSession.Versions.Lite;

                security_role.IsSelected = true;
                UserRoleDB.security_role.Add(security_role);

                security_roleViewSource.View.Refresh();
                security_roleViewSource.View.MoveCurrentToLast();
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            security_role security_role = (security_role)security_roleDataGrid.SelectedItem;
            if (security_role != null)
            {
                add_Privallge();
                add_MissingRecords(security_role);
                security_role.IsSelected = true;
                security_role.State = EntityState.Modified;
                UserRoleDB.Entry(security_role).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select a record");
            }
        }

        private void add_Privallge()
        {
            List<entity.App.Names> Application = Enum.GetValues(typeof(entity.App.Names)).Cast<entity.App.Names>().ToList();
            List<Privilage.Privilages> Privilages = Enum.GetValues(typeof(Privilage.Privilages)).Cast<Privilage.Privilages>().ToList();
            security_role security_role = (security_role)security_roleDataGrid.SelectedItem;

            foreach (entity.App.Names Names in Application)
            {
                if (Names == entity.App.Names.SalesInvoice)
                {
                    foreach (Privilage.Privilages Privilage in Privilages)
                    {
                        if (UserRoleDB.security_privilage.Where(x => x.name == Privilage).Count() == 0)
                        {
                            if ((int)Privilage >= 3)
                            {
                                security_privilage security_privilage = new security_privilage();
                                security_privilage.id_application = entity.App.Names.ProductionExecution;
                                security_privilage.name = Privilage;
                                UserRoleDB.security_privilage.Add(security_privilage);
                            }
                            else
                            {
                                security_privilage security_privilage = new security_privilage();
                                security_privilage.id_application = Names;
                                security_privilage.name = Privilage;
                                UserRoleDB.security_privilage.Add(security_privilage);

                            }

                        }
                    }
                }
            }

            List<entity.App.Names> PreferenceList = Enum.GetValues(typeof(entity.App.Names)).Cast<entity.App.Names>().ToList();
            List<security_privilage> security_privilageList = UserRoleDB.security_privilage.ToList();

            List<security_role_privilage> security_role_privilage = new List<security_role_privilage>();
            security_role_privilage = UserRoleDB.security_role_privilage.Where(x => x.id_role == security_role.id_role).ToList();

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

                        UserRoleDB.security_role_privilage.Add(_security_role_privilage);
                    }
                }
            }
        }

        private void btnChange(object sender, RoutedEventArgs e)
        {
            security_role security_role = (security_role)security_roleDataGrid.SelectedItem;

            if (security_role != null)
            {
                add_MissingRecords(security_role);
            }
        }

        private void add_MissingRecords(security_role security_role)
        {
            AppList appList = new AppList();

            List<security_crud> security_curd = security_rolesecurity_curdViewSource.View.OfType<security_crud>().ToList().Where(x => x.id_role == security_role.id_role).ToList();

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
                CurrentSession.Versions NewVersion = (CurrentSession.Versions)Enum.Parse(typeof(CurrentSession.Versions), Convert.ToString(cbxVersion.Text));

                if (CurrentVersion < NewVersion)
                {
                    List<entity.App.Names> dtApplication = new List<entity.App.Names>();
                    string condition = "";
                    if (NewVersion==CurrentSession.Versions.Full)
                    {
                        condition = "1=1";
                    }
                    else if (NewVersion == CurrentSession.Versions.Medium)
                    {
                        condition = "Version='" + NewVersion + "' and Version ='Lite' and Version='Basic'";
                    }
                    else if (NewVersion == CurrentSession.Versions.Basic)
                    {
                        condition = "Version='" + NewVersion + "' and Version ='Lite' ";
                    }
                    else if (NewVersion == CurrentSession.Versions.Lite)
                    {
                        condition = "Version='" + NewVersion + "'";
                    }
                    foreach (DataRow item in appList.dtApp.Select(condition))
                    {
                        if (Enum.IsDefined(typeof(entity.App.Names), Convert.ToString(item["name"])) == true)
                        {
                            dtApplication.Add((entity.App.Names)Enum.Parse(typeof(entity.App.Names), Convert.ToString(item["name"])));
                        }
                    }

                    List<entity.App.Names> _security_curdApplication = security_curd.Select(x => x.id_application).ToList();
                    List<entity.App.Names> AddList = Enumerable.Except(dtApplication, _security_curdApplication).ToList();
                    foreach (entity.App.Names AppName in AddList)
                    {
                        security_crud _rolesecurity_curd = security_role.security_curd.Where(x => x.id_application == AppName).FirstOrDefault();
                        if (_rolesecurity_curd == null)
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
                }
                else if (CurrentVersion > NewVersion)
                {

                    List<entity.App.Names> dtApplication = new List<entity.App.Names>();
                    foreach (DataRow item in appList.dtApp.Select("Version = '" + NewVersion + "'"))
                    {
                        if (Enum.IsDefined(typeof(entity.App.Names), Convert.ToString(item["name"])) == true)
                        {
                            dtApplication.Add((entity.App.Names)Enum.Parse(typeof(entity.App.Names), Convert.ToString(item["name"])));
                        }
                    }

                    List<entity.App.Names> _security_curdApplication = security_curd.Select(x => x.id_application).ToList();
                    List<entity.App.Names> AddList = Enumerable.Except(_security_curdApplication, dtApplication).ToList();

                    foreach (entity.App.Names AppName in AddList)
                    {
                        security_crud _security_curd = security_role.security_curd.Where(x => x.id_application == AppName).FirstOrDefault();
                        if (_security_curd != null)
                        {
                          //  UserRoleDB.Entry(_security_curd).State = EntityState.Deleted;
                            security_role.security_curd.Remove(_security_curd);
                        }
                    }
                    
                }
                CurrentVersion = NewVersion;
            }
            security_roleViewSource.View.Refresh();
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
                            string TranslatedName = Localize.StringText(security_curd.id_application.ToString());

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
                Activation Activation = new Activation();
                CurrentSession.Versions version = Activation.VersionDecrypt(security_role);
                cbxVersion.SelectedItem = version;
                CurrentVersion = version;
            }
        }
    }
}
