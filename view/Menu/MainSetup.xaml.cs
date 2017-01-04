using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using entity;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;

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
        public class LicenceInfo
        {
            public string slm_action { get; set; }
            public string secret_key { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public string company_name { get; set; }

        }
        public class LicenceRegistration
        {
            public string Key { get; set; }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (txtname.Text == string.Empty || txtalias.Text == string.Empty
                || txtGovID.Text == string.Empty || txtFullName.Text == string.Empty
                || txtName.Text == string.Empty || txtPass.Password == string.Empty)
            {
                MessageBox.Show("Fill all Fields");
                return;
            }

            app_company app_company = new app_company();
            app_company.name = txtname.Text;
            app_company.alias = txtalias.Text;
            app_company.gov_code = txtGovID.Text;
            app_company.address = "Address";
            app_company.is_active = true;
            app_company.has_interest = false;
            app_company.email_port_in = 0;
            app_company.email_port_out = 0;

            using (db db = new db())
            {
                db.app_company.Add(app_company);

                //entity.Brillo.Licence Licence = new entity.Brillo.Licence();
                //app_company.version = Licence.CreateLicence(txtname.Text, txtalias.Text, txtName + "-" + txtGovID.Text, "");
                db.SaveChanges();


                entity.Properties.Settings Settings = new entity.Properties.Settings();
                Settings.company_ID = app_company.id_company;
                Settings.company_Name = app_company.name;
                Settings.Save();

                CurrentSession.Id_Company = app_company.id_company;

                security_role role = new security_role();
                role.name = "Master Admin";
                role.is_master = true;
                role.is_active = true;
                role.see_cost = true;
                db.security_role.Add(role);
                db.SaveChanges();

                security_user user = new security_user();
                user.name_full = txtFullName.Text;
                user.name = txtName.Text;
                user.id_role = role.id_role;
                user.password = txtPass.Password;
                user.email_port_in = 0;
                user.email_port_out = 0;
                user.is_active = true;
                db.security_user.Add(user);
                db.SaveChanges();

                CurrentSession.UserRole = role;
                CurrentSession.User = role.security_user.FirstOrDefault();
                CurrentSession.Id_User = role.security_user.FirstOrDefault().id_user;
            }

            Dispatcher.BeginInvoke((Action)(() => { progBar.Value = 1; }));
            Task taskAuth = Task.Factory.StartNew(() => Seeding());
        }

        private void Seeding()
        {
            using (db db = new db())
            {
                app_branch app_branch = new app_branch();
                app_branch.name = "Branch";
                app_branch.code = "001";
                app_branch.can_invoice = true;
                app_branch.can_stock = true;
                app_branch.id_user = CurrentSession.User.id_user;
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));

                app_location location = new app_location();
                location.name = app_branch.name + " Location";
                location.is_default = true;
                location.id_user = CurrentSession.User.id_user;
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));
                app_terminal app_terminal = new app_terminal();
                app_terminal.code = "001";
                app_terminal.name = CurrentSession.User.name_full + "'s Computer";
                app_terminal.id_user = CurrentSession.User.id_user;
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));
                app_branch.app_location.Add(location);
                app_branch.app_terminal.Add(app_terminal);
                db.app_branch.Add(app_branch);
                db.SaveChanges();


                //Contact Role
                contact_role role = new contact_role();
                role.name = "Main Contact";
                role.is_principal = true;
                role.can_transact = true;
                db.contact_role.Add(role);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));
                db.SaveChanges();

                app_condition conditions = new app_condition();
                conditions.name = "Cash";
                db.app_condition.Add(conditions);
                db.SaveChanges();

                app_contract contract = new app_contract();
                contract.name = "0 Days";
                contract.is_default = true;
                contract.id_condition = conditions.id_condition;

                app_contract_detail contract_detail = new app_contract_detail();
                contract_detail.coefficient = 1;
                contract_detail.interval = 0;
                contract.app_contract_detail.Add(contract_detail);
                db.app_contract.Add(contract);
                db.SaveChanges();
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));

                app_condition credit = new app_condition();
                credit.name = "Credit";
                db.app_condition.Add(credit);
                db.SaveChanges();

                app_contract thrity = new app_contract();
                thrity.name = "30 Days";
                thrity.is_default = true;
                thrity.id_condition = credit.id_condition;
                app_contract_detail thrity_detail = new app_contract_detail();
                thrity_detail.coefficient = 1;
                thrity_detail.interval = 30;
                thrity.app_contract_detail.Add(thrity_detail);
                db.app_contract.Add(thrity);
                db.SaveChanges();

                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));

                //Incoterms
                impex_incoterm_condition Shipping = new impex_incoterm_condition();
                Shipping.name = "Shipping";
                db.impex_incoterm_condition.Add(Shipping);

                impex_incoterm_condition Insurance = new impex_incoterm_condition();
                Insurance.name = "Insurance";
                db.impex_incoterm_condition.Add(Insurance);

                impex_incoterm_condition ImportDuties = new impex_incoterm_condition();
                ImportDuties.name = "Import Duties";
                db.impex_incoterm_condition.Add(ImportDuties);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));
                db.SaveChanges();

                //Department
                app_department sales = new app_department();
                sales.name = "Sales";
                db.app_department.Add(sales);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));

                app_department admin = new app_department();
                admin.name = "Accounting and Finance";
                db.app_department.Add(admin);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));

                app_department hr = new app_department();
                hr.name = "Human Resources";
                db.app_department.Add(hr);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));

                app_department purchasing = new app_department();
                purchasing.name = "Purchasing";
                db.app_department.Add(purchasing);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));
                db.SaveChanges();

                //Payment Type
                payment_type payment_type = new payment_type();
                payment_type.name = "Cash";
                payment_type.is_direct = true;
                payment_type.is_default = true;
                payment_type.payment_behavior = payment_type.payment_behaviours.Normal;
                payment_type.id_user = CurrentSession.User.id_user;
                db.payment_type.Add(payment_type);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));
                db.SaveChanges();

                //Accounts
                app_account app_account = new app_account();
                app_account.name = "CashBox";
                app_account.app_terminal = app_terminal;
                app_account.id_account_type = app_account.app_account_type.Terminal;
                app_account.id_user = CurrentSession.User.id_user;
                db.app_account.Add(app_account);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));
                db.SaveChanges();

                //Measurement Type
                app_measurement_type type = new app_measurement_type();
                type.name = "Units";
                db.app_measurement_type.Add(type);
                db.SaveChanges();

                app_measurement measurement = new app_measurement();
                measurement.name = "Units";
                measurement.code_iso = "Unts";
                measurement.id_measurement_type = type.id_measurement_type;
                db.app_measurement.Add(measurement);
                db.SaveChanges();
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));

                //Cost Center
                app_cost_center app_cost_centerProduct = new app_cost_center();
                app_cost_centerProduct.name = "Product";
                app_cost_centerProduct.is_product = true;
                app_cost_centerProduct.id_user = CurrentSession.User.id_user;
                db.app_cost_center.Add(app_cost_centerProduct);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));

                app_cost_center app_cost_centerRent = new app_cost_center();
                app_cost_centerRent.name = "Rent";
                app_cost_centerRent.is_administrative = true;
                app_cost_centerRent.id_user = CurrentSession.User.id_user;
                db.app_cost_center.Add(app_cost_centerRent);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));

                app_cost_center app_cost_centerOfficeExp = new app_cost_center();
                app_cost_centerOfficeExp.name = "Office Expense";
                app_cost_centerOfficeExp.is_administrative = true;
                app_cost_centerOfficeExp.id_user = CurrentSession.User.id_user;
                db.app_cost_center.Add(app_cost_centerOfficeExp);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));

                app_cost_center app_cost_centerMarketing = new app_cost_center();
                app_cost_centerMarketing.name = "Marketing";
                app_cost_centerMarketing.is_administrative = true;
                app_cost_centerMarketing.id_user = CurrentSession.User.id_user;
                db.app_cost_center.Add(app_cost_centerMarketing);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));
                db.SaveChanges();

                //Price List
                item_price_list item_price_list = new item_price_list();
                item_price_list.name = "Retail";
                item_price_list.is_default = true;
                item_price_list.id_user = CurrentSession.User.id_user;
                db.item_price_list.Add(item_price_list);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));

                item_price_list item_price_listWholesale = new item_price_list();
                item_price_listWholesale.name = "Wholesale";
                item_price_listWholesale.id_user = CurrentSession.User.id_user;
                db.item_price_list.Add(item_price_listWholesale);
                Dispatcher.BeginInvoke((Action)(() => { progBar.Value += 5; }));
                db.SaveChanges();

                try
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        entity.Properties.Settings Settings = new entity.Properties.Settings();
                        Settings.branch_ID = app_branch.id_branch;
                        Settings.branch_Name = app_branch.name;
                        Settings.terminal_ID = app_terminal.id_terminal;
                        Settings.terminal_Name = app_terminal.name;
                        Settings.account_ID = app_account.id_account;
                        Settings.account_Name = app_account.name;

                        Settings.Save();
                        progBar.Value = 100;

                        Countdown(5, TimeSpan.FromSeconds(1), cur => tbxCountDown.Content = cur.ToString());
                        tabRestart.IsSelected = true;
                    }));
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }


            }
        }

        void Countdown(int count, TimeSpan interval, Action<int> ts)
        {
            var dt = new System.Windows.Threading.DispatcherTimer();
            dt.Interval = interval;
            dt.Tick += (_, a) =>
            {
                if (count-- == 0)
                {
                    dt.Stop();
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
                else
                {
                    ts(count);
                }
            };
            ts(count);
            dt.Start();
        }
    }
}
