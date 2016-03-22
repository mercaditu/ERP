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

namespace Cognitivo.Security
{
    public partial class User : Page
    {
        //entity.dbContext dbcontext = new entity.dbContext();
        UserDB dbContext = new UserDB();
        CollectionViewSource security_user_view_source, security_ques_view_source, security_role_view_source;
        entity.Properties.Settings _entity = new entity.Properties.Settings();


        public User()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            security_user_view_source = ((CollectionViewSource)(this.FindResource("security_userViewSource")));
            dbContext.security_user.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
            security_user_view_source.Source = dbContext.security_user.Local;

            security_ques_view_source = ((CollectionViewSource)(this.FindResource("securityQuesViewSource")));
            dbContext.security_question.Load();
            security_ques_view_source.Source = dbContext.security_question.Local;

            security_role_view_source = ((CollectionViewSource)(this.FindResource("securityRoleViewSource")));
            dbContext.security_role.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
            security_role_view_source.Source = dbContext.security_role.Local;

            if (!dbContext.security_user.Where(x => x.name == "master").Any())
            {
                security_user security_user = new security_user();
                security_user.State = EntityState.Added;
                security_user.IsSelected = true;
                if (dbContext.security_role.Where(x => x.is_master).FirstOrDefault()!=null)
                {
                    security_user.security_role = dbContext.security_role.Where(x => x.is_master).FirstOrDefault();
                }
             
                security_user.name = "master";
                dbContext.security_user.Add(security_user);
                security_user_view_source.View.MoveCurrentToLast();
           

            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            // SetIsEnable = true;
            security_user security_user = new security_user();
            security_user.State = EntityState.Added;
            security_user.IsSelected = true;
            security_user.id_company = CurrentSession.Id_Company;
            if (CurrentSession.User!=null)
            {
                security_user.id_created_user =  CurrentSession.Id_User;
            }
    
            dbContext.security_user.Add(security_user);
            security_user_view_source.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            security_user security_user = security_user_view_source.View.CurrentItem as security_user;
           
            if (dbContext.security_user.Where(x => x.name == security_user.name).Any() && security_user.State==EntityState.Added)
            {
                toolBar.msgWarning("User Already Exists...");
            }
            else
            {
                dbContext.SaveChanges();
                security_user_view_source.View.Refresh();
                toolBar.msgSaved();
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                dbContext.security_user.Remove((security_user)security_userDataGrid.SelectedItem);
                security_user_view_source.View.MoveCurrentToFirst();
                toolBar_btnSave_Click(sender);
                toolBar.msgDone();
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            security_user_view_source.View.MoveCurrentToFirst();
            dbContext.CancelAllChanges();
            // SetIsEnable = false;
        }

        private void CreateRole_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.user_role user_role = new cntrl.Curd.user_role();
            crud_modal.Children.Add(user_role);
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modal.Visibility == System.Windows.Visibility.Hidden)
            {
                dbContext.security_role.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
                security_role_view_source.Source = dbContext.security_role.Local;
                security_role_view_source.View.Refresh();
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            //// SetIsEnable = true;
            //security_user objsecuser =(security_user)security_user_view_source.View.CurrentItem;
            //objsecuser.State = System.Data.Entity.EntityState.Modified;

            if (security_userDataGrid.SelectedItem != null)
            {
                security_user security_user = (security_user)security_userDataGrid.SelectedItem;
                security_user.IsSelected = true;
                security_user.State = EntityState.Modified;
                dbContext.Entry(security_user).State = EntityState.Modified;
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
