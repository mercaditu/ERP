using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity.Validation;

namespace cntrl
{
    public partial class user : UserControl
    {
        public user()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (UserDB UserDB = new UserDB())
            {
                security_user user = new security_user();
                user.name_full = txtFullName.Text;
                user.name =txtName.Text;
                user.password = txtPass.Password;

                security_role security_role = new security_role();
                security_role.name = "Master Admin";
                security_role.is_master = true;
                security_role.is_active = true;

                security_role.security_user.Add(user);

                CurrentSession.UserRole = security_role;
                CurrentSession.User = user;
                UserDB.SaveChanges();
                Class.Seeding Seeding = new Class.Seeding();

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }

           
        }
    }
}
