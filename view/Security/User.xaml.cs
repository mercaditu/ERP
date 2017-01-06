using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;

namespace Cognitivo.Security
{
    public partial class User : Page
    {
        UserDB UserDB = new UserDB();
        CollectionViewSource security_user_view_source, security_role_view_source;

        public User()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            security_user_view_source = FindResource("security_userViewSource") as CollectionViewSource;
            await UserDB.security_user.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).LoadAsync();
            security_user_view_source.Source = UserDB.security_user.Local;

            security_role_view_source = FindResource("securityRoleViewSource") as CollectionViewSource;
            await UserDB.security_role.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).LoadAsync();
            security_role_view_source.Source = UserDB.security_role.Local;

            //This helps create a Master Admin Role.
            if (UserDB.security_role.Local.Count() == 0)
            {
                security_role security_role = new security_role();
                security_role.name = "Master Admin";
                security_role.is_master = true;
                security_role.is_active = true;

                CurrentSession.UserRole = security_role;

                UserDB.security_role.Add(security_role);
                UserDB.SaveChanges();
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            // SetIsEnable = true;
            security_user security_user = new security_user();
            security_user.State = EntityState.Added;
            security_user.IsSelected = true;
            security_user.id_company = CurrentSession.Id_Company;

            if (CurrentSession.User != null)
            {
                security_user.id_created_user = CurrentSession.Id_User;
            }

            UserDB.security_user.Add(security_user);
            security_user_view_source.View.MoveCurrentTo(security_user);
        }

        private void toolBar_btnSave_Click(object sender)
        {

            foreach (security_user security_user in UserDB.security_user.Local.Where(x => x.IsSelected))
            {
                if (UserDB.security_user.Local.Where(x => x.name == security_user.name && x != security_user).Any() && security_user.State == EntityState.Added)
                {
                    toolBar.msgWarning("User Already Exists...");
                }
                else
                {
                    entity.Brillo.Licence Licence = new entity.Brillo.Licence();
                 
                    if (Licence.CompanyLicence!= null)
                    {
                        
                    }
                    else
                    {
                        
                    }
                    if (UserDB.SaveChanges() > 0)
                    {
                        toolBar.msgSaved(1);

                        if (CurrentSession.Id_User == 0)
                        {
                            CurrentSession.Id_User = UserDB.security_user.Local.Where(x => x.name == security_user.name).FirstOrDefault().id_user;
                        }

                        security_user_view_source.View.Refresh();
                    }
                }
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                UserDB.security_user.Remove((security_user)security_userDataGrid.SelectedItem);

                if (UserDB.SaveChanges() > 0)
                {
                    security_user_view_source.View.MoveCurrentToFirst();
                }
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            security_user_view_source.View.MoveCurrentToFirst();
            UserDB.CancelAllChanges();
            // SetIsEnable = false;
        }

        private void CreateRole_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.user_role user_role = new cntrl.Curd.user_role();
            crud_modal.Children.Add(user_role);
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modal.Visibility == Visibility.Hidden)
            {
                UserDB.security_role.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
                security_role_view_source.Source = UserDB.security_role.Local;
                security_role_view_source.View.Refresh();
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {

            if (security_userDataGrid.SelectedItem != null)
            {
                security_user security_user = (security_user)security_userDataGrid.SelectedItem;
                security_user.IsSelected = true;
                security_user.State = EntityState.Modified;
                UserDB.Entry(security_user).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select a Record");
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    security_user_view_source.View.Filter = i =>
                    {
                        security_user security_user = i as security_user;
                        if (security_user.name.ToLower().Contains(query.ToLower()))
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
                    security_user_view_source.View.Filter = null;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgWarning(ex.Message);
            }
        }
    }
}
