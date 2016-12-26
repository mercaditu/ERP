using entity;

namespace cntrl.Class
{
   public class Seeding
    {
        public Seeding()
        {
            //contact_role contact_role = new contact_role();
            //contact_role.name = "Master";
            //contact_role.is_principal = true;
            //contact_role.can_transact = true;
            //db.contact_role.Add(contact_role);

            using (UserDB UserDB = new UserDB())
            {
                app_branch app_branch = new app_branch();
                app_branch.name = "Branch";
                app_branch.code = "001";
                app_branch.can_invoice = true;
                app_branch.can_stock = true;
                app_branch.id_user = CurrentSession.User.id_user;

                app_location location = new app_location();
                location.name = app_branch.name + "Location";
                location.is_default = true;
                location.id_user = CurrentSession.User.id_user;


                app_terminal app_terminal = new app_terminal();
                app_terminal.code = "001";
                app_terminal.name = CurrentSession.User.name_full + "'s Computer";
                app_terminal.id_user = CurrentSession.User.id_user;

                app_branch.app_location.Add(location);
                app_branch.app_terminal.Add(app_terminal);
                UserDB.app_branch.Add(app_branch);

                //Payment Type
                payment_type payment_type = new payment_type();
                payment_type.name = "Cash";
                payment_type.is_direct = true;
                payment_type.is_default = true;
                payment_type.payment_behavior = payment_type.payment_behaviours.Normal;
                payment_type.id_user = CurrentSession.User.id_user;
                UserDB.payment_type.Add(payment_type);

                //Accounts
                app_account app_account = new app_account();
                app_account.name = "CashBox";
                app_account.app_terminal = app_terminal;
                app_account.id_account_type = app_account.app_account_type.Terminal;
                app_account.id_user = CurrentSession.User.id_user;
                UserDB.app_account.Add(app_account);

                //Cost Center
                app_cost_center app_cost_centerProduct = new app_cost_center();
                app_cost_centerProduct.name = "Products";
                app_cost_centerProduct.is_product = true;
                app_cost_centerProduct.id_user = CurrentSession.User.id_user;
                UserDB.app_cost_center.Add(app_cost_centerProduct);

                app_cost_center app_cost_centerRent = new app_cost_center();
                app_cost_centerRent.name = "Rents";
                app_cost_centerRent.is_administrative = true;
                app_cost_centerRent.id_user = CurrentSession.User.id_user;
                UserDB.app_cost_center.Add(app_cost_centerRent);

                app_cost_center app_cost_centerOfficeExp = new app_cost_center();
                app_cost_centerOfficeExp.name = "Office Expenses";
                app_cost_centerOfficeExp.is_administrative = true;
                app_cost_centerOfficeExp.id_user = CurrentSession.User.id_user;
                UserDB.app_cost_center.Add(app_cost_centerOfficeExp);

                app_cost_center app_cost_centerMarketing = new app_cost_center();
                app_cost_centerMarketing.name = "Products";
                app_cost_centerMarketing.is_administrative = true;
                app_cost_centerMarketing.id_user = CurrentSession.User.id_user;
                UserDB.app_cost_center.Add(app_cost_centerMarketing);

                item_price_list item_price_list = new item_price_list();
                item_price_list.name = "Retail";
                item_price_list.is_default = true;
                item_price_list.id_user = CurrentSession.User.id_user;
                UserDB.item_price_list.Add(item_price_list);

                item_price_list item_price_listWholesale = new item_price_list();
                item_price_listWholesale.name = "Wholesale";
                item_price_listWholesale.id_user = CurrentSession.User.id_user;
                UserDB.item_price_list.Add(item_price_listWholesale);
                try
                {
                    entity.Properties.Settings.Default.company_ID = CurrentSession.Id_Company;
                    entity.Properties.Settings.Default.branch_ID = app_branch.id_branch;
                    entity.Properties.Settings.Default.terminal_ID = app_terminal.id_terminal;
                    entity.Properties.Settings.Default.account_ID = CurrentSession.Id_Account;
                    entity.Properties.Settings.Default.Save();
                    UserDB.SaveChanges();
                }
                catch (System.Exception ex)
                {

                    System.Windows.Forms.MessageBox.Show(ex.ToString()); 
                }
             
            }


        }
    }
}
