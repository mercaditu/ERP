using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using entity;

namespace Cognitivo.Menu
{
    /// <summary>
    /// Interaction logic for StartUpComapny.xaml
    /// </summary>
    public partial class MainSetup : Page
    {
        public MainSetup()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (txtname.Text == string.Empty || txtalias.Text == string.Empty 
                || txtGovID.Text == string.Empty || txtAddress.Text == string.Empty ||
             txtFullName.Text == string.Empty || txtName.Text == string.Empty || txtPassS.Text == string.Empty )
            {
                return;
            }

            app_company app_company = new app_company();
            app_company.name = txtname.Text;
            app_company.alias = txtalias.Text;
            app_company.gov_code = txtGovID.Text;
            app_company.address = txtAddress.Text;

            using (db db = new db())
            {
                db.app_company.Add(app_company);
                CurrentSession.Id_Company = app_company.id_company;
            }

            security_role security_role = new security_role();
            security_role.name = "Master Admin";
            security_role.is_master = true;
            security_role.is_active = true;
            
            security_user user = new security_user();
            user.name_full = txtFullName.Text;
            user.name = txtName.Text;
            user.password = txtPass.Password;
            user.id_role = security_role.id_role;
            user.IsSelected = true;

            security_role.security_user.Add(user);

            Task taskAuth = Task.Factory.StartNew(() => Start(app_company, security_role));
            
        }
        private void Start(app_company app_company, security_role security_role)
        {
            Dispatcher.BeginInvoke((Action)(() => { progBar.IsIndeterminate = true; }));
            using (UserDB UserDB = new UserDB())
            {

                UserDB.security_role.Add(security_role);
                UserDB.SaveChanges();

                CurrentSession.UserRole = security_role;
                CurrentSession.User = security_role.security_user.FirstOrDefault();
                CurrentSession.Id_User = security_role.security_user.FirstOrDefault().id_user;

                cntrl.Class.Seeding Seeding = new cntrl.Class.Seeding();
                

                Dispatcher.BeginInvoke((Action)(() => { progBar.IsIndeterminate = false; }));
            }
        }


    }
}
