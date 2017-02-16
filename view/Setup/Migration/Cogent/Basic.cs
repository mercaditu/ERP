using entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Cognitivo.Setup.Migration.Cogent
{
    partial class MigrationGUI
    {
        public int id_company { get; set; }

        public void startTask()
        {
            Task basic_task = Task.Factory.StartNew(() => basic());
            basic_task.Wait();
            Task employees = Task.Factory.StartNew(() => employee());

            Task contact = Task.Factory.StartNew(() => contacts());
            Task item = Task.Factory.StartNew(() => items());

            employees.Wait();
            contact.Wait();
            item.Wait();
            Task talent = Task.Factory.StartNew(() => Talent());
            talent.Wait();
            Task item_dimension = Task.Factory.StartNew(() => items_dimension());
            item_dimension.Wait();
            Task item_factor = Task.Factory.StartNew(() => items_conversion_factor());
            item_factor.Wait();
            Task item_price = Task.Factory.StartNew(() => items_price());
            item_price.Wait();
            //Task project_task = Task.Factory.StartNew(() => project());
            //project_task.Wait();
            //Task productions = Task.Factory.StartNew(() => production());
            //productions.Wait();
            //Task productionsExecution = Task.Factory.StartNew(() => production_Exec());
            //productionsExecution.Wait();
            //Task PurchaseInvoice = Task.Factory.StartNew(() => Purchase_Invoice());
            //PurchaseInvoice.Wait();
            //  Task SalesInvocie = Task.Factory.StartNew(() => Sales_Invoice());
        }

        private void basic()
        {
            Dispatcher.BeginInvoke((Action)(() => progBasic.IsIndeterminate = true));
            //Company, Branch, Terminal, & Location

            sync_Company();
            //Users;

            //Contact Role
            sync_ContactRole();
            //CostCenter
            sync_CostCenter();
            //PriceList
            sync_PriceList();
            //Tags for Products
            sync_TagsList();
            //VAT
            sync_VatList();
            //Currency
            sync_Currency();
            //??Accounts??
            sync_Accounts();
            //Condition
            sync_Condition();
            //??Contract??
            sync_Contracts();
            sync_SalesRep();
            sync_measure();
            sync_Dimension();
            sync_Department();
            Dispatcher.BeginInvoke((Action)(() => progBasic.IsIndeterminate = false));
        }

        private void sync_Company()
        {
            using (db dbContext = new db())
            {
                //  MySqlDataReader readeritem = cmd.ExecuteReader();
                DataTable dtcompany = exeDTMysql("select * from app_company");
                DataTable dt_Branch = exeDTMysql("select * from app_branch");
                DataTable dt_Terminal = exeDTMysql("select * from app_terminal");

                foreach (DataRow row in dtcompany.Rows)
                {
                    app_company _app_company = new app_company();
                    _app_company.name = row["company_full"].ToString();

                    _app_company.alias = row["company_short"].ToString();

                    if (_app_company.name != null && _app_company.alias != null)
                    {
                        _app_company.name = _app_company.alias;
                    }
                    else
                    {
                        continue;
                    }

                    _app_company.address = "Address Placeholder";
                    _app_company.gov_code = (row["tax_code"].ToString() == "") ? "GovID Placeholder" : row["tax_code"].ToString();

                    try
                    {
                        dbContext.app_company.Add(_app_company);
                        dbContext.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var e in ex.EntityValidationErrors)
                        {
                            MessageBox.Show(e.Entry.ToString());
                        }
                    }

                    id_company = dbContext.app_company.Where(i => i.name == _app_company.name).FirstOrDefault().id_company;
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        entity.Properties.Settings.Default.company_ID = id_company;
                        entity.Properties.Settings.Default.Save();
                    }
                    ));

                    sync_Users();

                    CurrentSession.Id_User = dbContext.security_user.Where(i => i.id_company == id_company).FirstOrDefault().id_user;

                    foreach (DataRow row_Branch in dt_Branch.Rows)
                    {
                        app_branch _app_branch = new app_branch();
                        _app_branch.id_company = id_company;
                        _app_branch.name = row_Branch["branch"].ToString();
                        _app_branch.code = row_Branch["branch_code"].ToString();
                        _app_branch.can_invoice = Convert.ToBoolean(row_Branch["can_invoice"]);
                        _app_branch.can_stock = Convert.ToBoolean(row_Branch["can_stock"]);

                        if (_app_branch.can_stock)
                        {
                            app_location app_location = new app_location();
                            app_location.is_active = true;
                            app_location.is_default = true;
                            app_location.name = "Deposito";
                            _app_branch.app_location.Add(app_location);
                        }

                        string id_branch = row_Branch["id"].ToString();
                        foreach (DataRow row_Terminal in dt_Terminal.Select("id_branch = " + id_branch))
                        {
                            app_terminal app_terminal = new app_terminal();
                            app_terminal.is_active = true;
                            app_terminal.code = row_Terminal["terminal_code"].ToString();
                            app_terminal.name = row_Terminal["terminal"].ToString();
                            _app_branch.app_terminal.Add(app_terminal);
                        }
                        if (_app_branch.Error == null)
                        {
                            dbContext.app_branch.Add(_app_branch);
                            try
                            {
                                dbContext.SaveChanges();
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                    //Dispatcher.BeginInvoke((Action)(() =>
                    //{
                    entity.Properties.Settings.Default.branch_ID = dbContext.app_branch.Where(i => i.id_company == id_company).FirstOrDefault().id_branch;
                    entity.Properties.Settings.Default.terminal_ID = dbContext.app_terminal.Where(i => i.id_company == id_company).FirstOrDefault().id_terminal;
                    entity.Properties.Settings.Default.Save();
                    //                    }
                    //));
                    dt_Branch.Clear();
                    dt_Terminal.Clear();
                }
                dtcompany.Clear();
            }
        }

        private void sync_ContactRole()
        {
            using (entity.db db = new db())
            {
                contact_role contact_role = new contact_role();
                contact_role.id_company = id_company;
                contact_role.is_active = true;
                contact_role.is_principal = true;
                contact_role.can_transact = true;
                contact_role.name = "Contacto Principal";
                db.contact_role.Add(contact_role);
                db.SaveChanges();
            }
        }

        private void sync_CostCenter()
        {
            using (db dbContext = new db())
            {
                DataTable dt = exeDTMysql("select * from app_cost_center");
                foreach (DataRow row in dt.Rows)
                {
                    app_cost_center app_cost_center = new app_cost_center();
                    app_cost_center.name = (string)row["cost_center"];
                    app_cost_center.is_product = Convert.ToBoolean(row["can_stock"]);
                    app_cost_center.is_administrative = Convert.ToBoolean(row["is_direct"]);
                    app_cost_center.id_company = id_company;
                    app_cost_center.is_active = true;
                    dbContext.app_cost_center.Add(app_cost_center);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        private void sync_Currency()
        {
            using (db dbContext = new db())
            {
                DataTable dt = exeDTMysql("SELECT * FROM app_currency");
                foreach (DataRow row in dt.Rows)
                {
                    app_currency app_currency = new app_currency();
                    app_currency.name = (string)row["currency"];
                    app_currency.id_company = id_company;
                    app_currency.is_priority = Convert.ToBoolean(row["is_priority"]);
                    if (app_currency.is_priority)
                    {
                        app_currencyfx app_currencyfx = new app_currencyfx();
                        app_currencyfx.buy_value = 1;
                        app_currencyfx.sell_value = 1;
                        app_currencyfx.is_active = true;
                        app_currency.app_currencyfx.Add(app_currencyfx);
                    }
                    app_currency.is_active = true;
                    //app_currency.id_country = id_country;
                    dbContext.app_currency.Add(app_currency);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        private void sync_Accounts()
        {
            using (db dbContext = new db())
            {
                DataTable dt = exeDTMysql("SELECT * FROM app_account;");
                foreach (DataRow row in dt.Rows)
                {
                    app_account app_account = new app_account();
                    app_account.name = (string)row["account"];
                    app_account.id_company = id_company;
                    app_account.is_active = true;
                    dbContext.app_account.Add(app_account);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        private void sync_Condition()
        {
            using (db dbContext = new db())
            {
                app_condition app_condition = new app_condition();
                app_condition.name = "Contado";
                app_condition.id_company = id_company;
                app_condition.is_active = true;
                dbContext.app_condition.Add(app_condition);

                app_condition app_conditionCred = new app_condition();
                app_conditionCred.name = "Crédito";
                app_conditionCred.id_company = id_company;
                app_conditionCred.is_active = true;
                dbContext.app_condition.Add(app_conditionCred);
                dbContext.SaveChanges();
            }
        }

        private void sync_Contracts()
        {
            using (db dbContext = new db())
            {
                string sql = "select * from app_contract inner join app_contract_detail on app_contract.id=app_contract_detail.id";
                DataTable dt = exeDTMysql(sql);
                foreach (DataRow row in dt.Rows)
                {
                    int _dias = Convert.ToInt32((row["interval"] is DBNull) ? 0 : row["interval"]);
                    app_contract app_contract = new app_contract();
                    app_contract.name = Convert.ToString(row["contract"]);
                    app_contract.id_company = id_company;
                    app_contract.is_active = true;

                    if (_dias == 0)
                    {
                        app_contract.id_condition = dbContext.app_condition.Where(x => x.name == "Contado" && x.id_company == id_company).FirstOrDefault().id_condition;
                    }
                    else
                    {
                        app_contract.id_condition = dbContext.app_condition.Where(x => x.name == "Crédito" && x.id_company == id_company).FirstOrDefault().id_condition;
                    }

                    app_contract_detail app_contract_detail = new app_contract_detail();
                    app_contract_detail.coefficient = 1;
                    app_contract_detail.interval = (short)_dias;
                    app_contract_detail.is_order = false;

                    app_contract.app_contract_detail.Add(app_contract_detail);
                    dbContext.app_contract.Add(app_contract);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        private void sync_PriceList()
        {
            using (db dbContext = new db())
            {
                DataTable dt = exeDTMysql("SELECT * FROM app_price_list");
                foreach (DataRow row in dt.Rows)
                {
                    item_price_list price = new item_price_list();
                    price.name = (string)row["price_list"];
                    price.id_company = id_company;
                    price.is_active = true;

                    dbContext.item_price_list.Add(price);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        private void sync_VatList()
        {
            using (db dbContext = new db())
            {
                DataTable dt = exeDTMysql("SELECT * FROM app_tax");
                foreach (DataRow row in dt.Rows)
                {
                    app_vat_group_details app_vat_group_details = new app_vat_group_details();

                    app_vat vat = new app_vat();
                    vat.name = (string)row["tax"];
                    vat.id_company = id_company;
                    vat.coefficient = (decimal)row["coeficient"];
                    dbContext.app_vat.Add(vat);
                    dbContext.SaveChanges();

                    app_vat_group vat_group = new app_vat_group();
                    vat_group.name = (string)row["tax"];
                    vat_group.id_company = id_company;
                    dbContext.app_vat_group.Add(vat_group);
                    dbContext.SaveChanges();

                    app_vat_group_details.app_vat = dbContext.app_vat.Where(x => x.name == vat.name && x.id_company == id_company).FirstOrDefault();
                    app_vat_group_details.app_vat_group = dbContext.app_vat_group.Where(x => x.name == vat_group.name && x.id_company == id_company).FirstOrDefault();
                    dbContext.app_vat_group_details.Add(app_vat_group_details);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        private void sync_MeasureList()
        {
            using (db dbContext = new db())
            {
                DataTable dt = exeDTMysql("SELECT * FROM app_measure");
                foreach (DataRow row in dt.Rows)
                {
                    app_measurement app_measurement = new app_measurement();
                    //measure.id_measurement = (int)row["CODMEDIDA"];
                    app_measurement.name = (string)row["measure"];
                    app_measurement.id_company = id_company;
                    app_measurement.code_iso = (row["symbol"] == null) ? string.Empty : row["symbol"].ToString();

                    app_measurement_type measure_type = new app_measurement_type();
                    measure_type.name = "Unit";
                    measure_type.app_measurement.Add(app_measurement);
                    dbContext.app_measurement_type.Add(measure_type);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        private void sync_TagsList()
        {
            using (db dbContext = new db())
            {
                DataTable dt = new DataTable();

                dt = exeDTMysql("SELECT * FROM item_tag");
                foreach (DataRow row in dt.Rows)
                {
                    using (db db = new db())
                    {
                        string _tag = row["tag"].ToString();
                        if (db.item_tag.Any(x => x.name == _tag && x.id_company == id_company))
                        {
                            continue;
                        }
                        item_tag tag = new item_tag();
                        tag.id_company = id_company;
                        tag.name = (string)row["tag"];
                        db.item_tag.Add(tag);
                        db.SaveChanges();
                    }
                }
                dt.Clear();
            }
        }

        private void sync_SalesRep()
        {
            using (db dbContext = new db())
            {
                DataTable dt = exeDTMysql("SELECT * FROM sales_representative");
                foreach (DataRow row in dt.Rows)
                {
                    sales_rep sales_rep = new sales_rep();
                    sales_rep.name = (string)row["salesman"];
                    sales_rep.id_company = id_company;
                    sales_rep.is_active = true;
                    dbContext.sales_rep.Add(sales_rep);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        private void sync_Dimension()
        {
            using (db dbContext = new db())
            {
                DataTable dt = exeDTMysql("SELECT * FROM app_dimension");
                foreach (DataRow row in dt.Rows)
                {
                    app_dimension app_dimension = new app_dimension();
                    app_dimension.name = (string)row["dimension"];
                    dbContext.app_dimension.Add(app_dimension);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        private void sync_Department()
        {
            using (db dbContext = new db())
            {
                DataTable dt = exeDTMysql("SELECT * FROM app_department");
                foreach (DataRow row in dt.Rows)
                {
                    app_department app_department = new app_department();
                    app_department.name = (string)row["department"];
                    dbContext.app_department.Add(app_department);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        private void sync_measure()
        {
            using (db dbContext = new db())
            {
                DataTable dt = exeDTMysql("SELECT * FROM app_measure_type");
                foreach (DataRow row in dt.Rows)
                {
                    app_measurement_type app_measurement_type = new app_measurement_type();
                    app_measurement_type.name = (string)row["measure_type"];
                    dbContext.app_measurement_type.Add(app_measurement_type);
                }
                dt.Clear();
                dbContext.SaveChanges();

                DataTable dtmeasure = exeDTMysql("SELECT *,(select measure_type from app_measure_type where app_measure_type.id=app_measure.id_measure_type) as measure_type  FROM app_measure");
                foreach (DataRow row in dtmeasure.Rows)
                {
                    app_measurement app_measurement = new app_measurement();
                    app_measurement.name = (string)row["measure"];
                    app_measurement.code_iso = (string)row["symbol"];
                    if (!(row["measure_type"] is DBNull))
                    {
                        string name = (string)row["measure_type"];
                        if (name != "")
                        {
                            app_measurement.id_measurement_type = dbContext.app_measurement_type.Where(x => x.name == name).FirstOrDefault().id_measurement_type;
                        }
                        else
                        {
                            app_measurement.id_measurement_type = dbContext.app_measurement_type.FirstOrDefault().id_measurement_type;
                        }
                    }
                    else
                    {
                        app_measurement.id_measurement_type = dbContext.app_measurement_type.FirstOrDefault().id_measurement_type;
                    }

                    dbContext.app_measurement.Add(app_measurement);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        private void sync_Users()
        {
            using (db dbContext = new db())
            {
                security_role security_role = new security_role();
                security_role.is_active = true;
                security_role.name = "Administrador";
                security_role.id_company = id_company;
                dbContext.security_role.Add(security_role);
                dbContext.SaveChanges();
                DataTable dt = exeDTMysql("SELECT * FROM user");
                foreach (DataRow row in dt.Rows)
                {
                    security_user security_user = new security_user();
                    security_user.name = row["user"].ToString();
                    security_user.password = row["password"].ToString();
                    security_user.name_full = row["name"].ToString();
                    security_user.id_company = id_company;
                    security_user.is_active = true;
                    security_user.id_role = security_role.id_role;
                    security_user.security_role = security_role;

                    dbContext.security_user.Add(security_user);
                }
                dt.Clear();
                dbContext.SaveChanges();
            }
        }

        public void items()
        {
            using (db dbContext = new db())
            {
                MySqlConnection conn = new MySqlConnection(_connString);
                //Counts Total number of Rows we have to process
                MySqlCommand cmd = new MySqlCommand();
                string sql_item = " SELECT item_sku.*, item_type.type, (select tax from app_tax where id=item_tax.id_tax) as vat FROM item_sku "
                                   + " inner join item_type on item_sku.id_type = item_type.id "
                                   + " left outer join item_tax on item_sku.id = item_tax.id_sku";
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = sql_item;
                cmd.CommandType = CommandType.Text;

                //MySqlDataReader readeritem = cmd.ExecuteReader();
                DataTable dtitem = exeDTMysql(sql_item);
                int count = dtitem.Rows.Count;

                int value = 0;
                Dispatcher.BeginInvoke((Action)(() => itemMaximum.Text = count.ToString()));
                Dispatcher.BeginInvoke((Action)(() => itemValue.Text = value.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progitem.Maximum = count));
                Dispatcher.BeginInvoke((Action)(() => progitem.Value = value));

                //while (readeritem.Read())
                foreach (DataRow row in dtitem.Rows)
                {
                    item item = new item();

                    if (!(row["product"] is DBNull))
                    {
                        item.name = (string)row["product"];
                    }

                    if (!(row["sku_code"] is DBNull) && (string)row["sku_code"] != "")
                    {
                        item.code = (string)row["sku_code"];
                    }
                    else
                    {
                        item.code = "code";
                    }

                    item.variation = "variation";

                    if (!(row["details"] is DBNull))
                    {
                        item.description = (string)row["details"];
                    }
                    else
                    {
                        item.description = "";
                    }

                    if (!(row["is_auto_recepie"] is DBNull))
                    {
                        item.is_autorecepie = (bool)row["is_auto_recepie"];
                    }
                    else
                    {
                        item.is_autorecepie = false;
                    }

                    //if (!(row["has_promo"] is DBNull))
                    //{
                    //    item.has_promo = (bool)row["has_promo"];
                    //}
                    //else
                    //{
                    //    item.has_promo = false;
                    //}

                    if (!(row["is_active"] is DBNull))
                    {
                        item.is_active = (bool)row["is_active"];
                    }
                    else
                    {
                        item.is_active = false;
                    }

                    //item.is_substitute = false;

                    if (row["type"].ToString() != string.Empty)
                    {
                        string type = row["type"].ToString();
                        if (type == "MATERIA PRIMA")
                        {
                            item.id_item_type = item.item_type.RawMaterial;
                            item_product item_product = new item_product();
                            item_product.can_expire = false;
                            item_product.is_weigted = false;
                            item.item_product.Add(item_product);
                        }
                        else if (type == "SERVICIOS"
                                || type == "SUBCONTRATADO"
                                || type == "HORAS HOMBRE"
                                || type == "Hs. Hs. TERCERIZADO"
                                || type == "TARIFARIO"
                                || type == "MECANIZADO")
                        {
                            item.id_item_type = item.item_type.Service;
                            item_service item_service = new item_service();
                            item.item_service.Add(item_service);
                        }
                        else if (type == "BIEN DE USO")
                        {
                            item.id_item_type = item.item_type.FixedAssets;
                            item_asset item_asset = new item_asset();
                            item.item_asset.Add(item_asset);
                        }
                        else if (type == "LIMPIEZA"
                                || type == "ELECTRICIDAD"
                                || type == "LIBRERIA"
                                || type == "INSUMO"
                                || type == "RPTO. EQUIPOS/MAQUINAS"
                                || type == "RPTO. AUTOMOTRIZ"
                                || type == "HERRAMIENTAS"
                                || type == " EPI")
                        {
                            item.id_item_type = item.item_type.Supplies;
                            item_product item_product = new item_product();
                            item_product.can_expire = false;
                            item_product.is_weigted = false;
                            item.item_product.Add(item_product);
                        }
                        else if (type == "TITULO")
                        {
                            item.id_item_type = item.item_type.Task;
                        }
                        else
                        {
                            item.id_item_type = item.item_type.Product;
                            item_product item_product = new item_product();
                            item_product.can_expire = false;
                            item_product.is_weigted = false;
                            item.item_product.Add(item_product);
                        }
                    }

                    if (!(row["vat"] is DBNull))
                    {
                        string tax = (string)row["vat"];
                        app_vat_group app_vat_group = dbContext.app_vat_group.Where(x => x.name == tax).FirstOrDefault();
                        if (app_vat_group != null)
                        {
                            item.id_vat_group = app_vat_group.id_vat_group;
                        }
                    }
                    else
                    {
                        item.id_vat_group = dbContext.app_vat_group.FirstOrDefault().id_vat_group;
                    }

                    if (item.Error == null)
                    {
                        using (db db = new db())
                        {
                            db.Configuration.AutoDetectChangesEnabled = false;
                            db.items.Add(item);
                            IEnumerable<DbEntityValidationResult> validationresult = db.GetValidationErrors();
                            if (validationresult.Count() == 0)
                            {
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch
                                {
                                    throw;
                                }
                            }
                        }
                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progitem.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => itemValue.Text = value.ToString()));
                    }
                }

                dtitem.Clear();
                cmd.Dispose();
                conn.Close();
            }
        }

        public void items_dimension()
        {
            MySqlConnection conn = new MySqlConnection(_connString);
            //Counts Total number of Rows we have to process
            MySqlCommand cmd = new MySqlCommand();
            string sql_item = " SELECT item_dimension.*,(select dimension from app_dimension where id=id_dimension) as dimension,item_sku.product FROM item_dimension "
                               + "   inner join item_sku on item_sku.id = item_dimension.id_sku";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_item;
            cmd.CommandType = CommandType.Text;

            //MySqlDataReader readeritem = cmd.ExecuteReader();
            DataTable dtitem = exeDTMysql(sql_item);
            int count = dtitem.Rows.Count;

            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => itemDimensionMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => itemDimensionValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progitemDimension.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progitemDimension.Value = value));

            //while (readeritem.Read())
            foreach (DataRow row in dtitem.Rows)
            {
                item_dimension item_dimension = new item_dimension();
                using (db db = new db())
                {
                    string name = row["dimension"].ToString();
                    item_dimension.id_app_dimension = db.app_dimension.Where(x => x.name == name).FirstOrDefault().id_dimension;

                    item_dimension.id_measurement = db.app_measurement.FirstOrDefault().id_measurement;
                    string product = row["product"].ToString();
                    if (db.items.Where(x => x.name == product).FirstOrDefault() != null)
                    {
                        item_dimension.id_item = db.items.Where(x => x.name == product).FirstOrDefault().id_item;
                    }
                    else
                    {
                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progitemDimension.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => itemDimensionValue.Text = value.ToString()));
                        continue;
                    }
                }
                if (row["value"] is DBNull)
                {
                    item_dimension.value = 0;
                }
                else
                {
                    item_dimension.value = Convert.ToInt32(row["value"]);
                }

                if (item_dimension.Error == null)
                {
                    using (db db = new db())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.item_dimension.Add(item_dimension);
                        IEnumerable<DbEntityValidationResult> validationresult = db.GetValidationErrors();
                        if (validationresult.Count() == 0)
                        {
                            try
                            {
                                db.SaveChanges();
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                    value += 1;
                    Dispatcher.BeginInvoke((Action)(() => progitemDimension.Value = value));
                    Dispatcher.BeginInvoke((Action)(() => itemDimensionValue.Text = value.ToString()));
                }
            }

            dtitem.Clear();
            cmd.Dispose();
            conn.Close();
        }

        public void items_conversion_factor()
        {
            MySqlConnection conn = new MySqlConnection(_connString);
            //Counts Total number of Rows we have to process
            MySqlCommand cmd = new MySqlCommand();
            string sql_item = " SELECT item_factor.*,(select measure from app_measure where id=item_factor.id_measure) as measure,item_sku.product FROM item_factor "
                               + "   inner join item_sku on item_sku.id = item_factor.id_sku";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_item;
            cmd.CommandType = CommandType.Text;

            //MySqlDataReader readeritem = cmd.ExecuteReader();
            DataTable dtitem = exeDTMysql(sql_item);
            int count = dtitem.Rows.Count;

            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => itemConversionMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => itemConversionValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progitemConversion.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progitemConversion.Value = value));

            //while (readeritem.Read())
            foreach (DataRow row in dtitem.Rows)
            {
                item_conversion_factor item_conversion_factor = new item_conversion_factor();
                using (db db = new db())
                {
                    string name = row["measure"].ToString();
                    if (db.app_measurement.Where(x => x.name == name).FirstOrDefault() != null)
                    {
                        item_conversion_factor.id_measurement = db.app_measurement.Where(x => x.name == name).FirstOrDefault().id_measurement;
                    }
                    else
                    {
                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progitemConversion.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => itemConversionValue.Text = value.ToString()));
                        continue;
                    }

                    string product = row["product"].ToString();
                    if (db.items.Where(x => x.name == product).FirstOrDefault() != null)
                    {
                        int id = db.items.Where(x => x.name == product).FirstOrDefault().id_item;
                        if (db.item_product.Where(x => x.id_item == id).FirstOrDefault() != null)
                        {
                            item_conversion_factor.id_item_product = db.item_product.Where(x => x.id_item == id).FirstOrDefault().id_item_product;
                        }
                        else
                        {
                            value += 1;
                            Dispatcher.BeginInvoke((Action)(() => progitemConversion.Value = value));
                            Dispatcher.BeginInvoke((Action)(() => itemConversionValue.Text = value.ToString()));
                            continue;
                        }
                    }
                    else
                    {
                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progitemConversion.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => itemConversionValue.Text = value.ToString()));
                        continue;
                    }
                }
                if (row["factor"] is DBNull)
                {
                    item_conversion_factor.value = 0;
                }
                else
                {
                    item_conversion_factor.value = Convert.ToInt32(row["factor"]);
                }

                if (item_conversion_factor.Error == null)
                {
                    using (db db = new db())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.item_conversion_factor.Add(item_conversion_factor);
                        IEnumerable<DbEntityValidationResult> validationresult = db.GetValidationErrors();
                        if (validationresult.Count() == 0)
                        {
                            try
                            {
                                db.SaveChanges();
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                    value += 1;
                    Dispatcher.BeginInvoke((Action)(() => progitemConversion.Value = value));
                    Dispatcher.BeginInvoke((Action)(() => itemConversionValue.Text = value.ToString()));
                }
            }

            dtitem.Clear();
            cmd.Dispose();
            conn.Close();
        }

        public void items_price()
        {
            MySqlConnection conn = new MySqlConnection(_connString);
            //Counts Total number of Rows we have to process
            MySqlCommand cmd = new MySqlCommand();
            string sql_item = " SELECT item_price.*,(select price_list from app_price_list where id=id_price_list) as price_list,(select currency from app_currency where id=id_currency) as currency,item_sku.product FROM item_price "
                               + "   inner join item_sku on item_sku.id = item_price.id_sku";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_item;
            cmd.CommandType = CommandType.Text;

            //MySqlDataReader readeritem = cmd.ExecuteReader();
            DataTable dtitem = exeDTMysql(sql_item);
            int count = dtitem.Rows.Count;

            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => itemPriceMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => itemPriceValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progitemPrice.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progitemPrice.Value = value));

            //while (readeritem.Read())
            foreach (DataRow row in dtitem.Rows)
            {
                item_price item_price = new item_price();
                using (db db = new db())
                {
                    string currency = row["currency"].ToString();
                    if (currency != "")
                    {
                        item_price.id_currency = db.app_currency.Where(x => x.name == currency).FirstOrDefault().id_currency;
                    }
                    else
                    {
                        item_price.id_currency = db.app_currency.FirstOrDefault().id_currency;
                    }

                    string price_list = row["price_list"].ToString();
                    if (price_list != "")
                    {
                        item_price.id_price_list = db.item_price_list.Where(x => x.name == price_list).FirstOrDefault().id_price_list;
                    }
                    else
                    {
                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progitemPrice.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => itemPriceValue.Text = value.ToString()));
                        continue;
                    }

                    string product = row["product"].ToString();
                    if (db.items.Where(x => x.name == product).FirstOrDefault() != null)
                    {
                        int id = db.items.Where(x => x.name == product).FirstOrDefault().id_item;

                        item_price.id_item = id;
                    }
                    else
                    {
                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progitemPrice.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => itemPriceValue.Text = value.ToString()));
                        continue;
                    }
                }
                if (row["sell_value"] is DBNull)
                {
                    item_price.value = 0;
                }
                else
                {
                    item_price.value = Convert.ToInt32(row["sell_value"]);
                }

                if (item_price.Error == null)
                {
                    using (db db = new db())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.item_price.Add(item_price);
                        IEnumerable<DbEntityValidationResult> validationresult = db.GetValidationErrors();
                        if (validationresult.Count() == 0)
                        {
                            try
                            {
                                db.SaveChanges();
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                    value += 1;
                    Dispatcher.BeginInvoke((Action)(() => progitemPrice.Value = value));
                    Dispatcher.BeginInvoke((Action)(() => itemPriceValue.Text = value.ToString()));
                }
            }

            dtitem.Clear();
            cmd.Dispose();
            conn.Close();
        }

        public void contacts()
        {
            MySqlConnection conn = new MySqlConnection(_connString);
            MySqlCommand cmd = new MySqlCommand();

            string sql_project = " SELECT * "
                                + " FROM contact";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_project;

            cmd.CommandType = CommandType.Text;

            MySqlDataReader readercontact = cmd.ExecuteReader();
            DataTable dtcontact = exeDTMysql(sql_project);
            int count = dtcontact.Rows.Count;
            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => contactMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => contactValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progcontact.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progcontact.Value = value));

            while (readercontact.Read())
            {
                contact contact = new contact();
                contact.id_contact = readercontact.GetInt32("id");
                contact.id_contact_role = 1;

                if (!(readercontact["name_full"] is DBNull))
                {
                    contact.name = readercontact.GetString("name_full");
                }
                else
                {
                    contact.name = "name";
                }

                if (!(readercontact["contact_code"] is DBNull))
                {
                    contact.code = readercontact.GetString("contact_code");
                }
                else
                {
                    contact.code = "name";
                }

                if (!(readercontact["gov_tax_code"] is DBNull))
                {
                    contact.gov_code = readercontact.GetString("gov_tax_code");
                }
                else
                {
                    contact.gov_code = "name";
                }

                if (!(readercontact["phone1"] is DBNull))
                { contact.telephone = readercontact.GetString("phone1"); }
                else { contact.telephone = "telephone"; }
                if (!(readercontact["email"] is DBNull))
                { contact.telephone = readercontact.GetString("email"); }
                else { contact.telephone = "email"; }
                if (!(readercontact["address"] is DBNull))
                { contact.telephone = readercontact.GetString("address"); }
                else { contact.telephone = "address"; }
                contact.credit_limit = 0;
                contact.credit_availability = 0;

                if (!(readercontact["is_client"] is DBNull) && (bool)readercontact["is_client"] == true)
                {
                    contact.is_customer = true;
                }
                if (!(readercontact["is_supplier"] is DBNull) && (bool)readercontact["is_supplier"] == true)
                {
                    contact.is_supplier = true;
                }
                contact.is_employee = false;
                contact.is_sales_rep = false;

                if (contact.Error == null)
                {
                    using (db db = new db())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.contacts.Add(contact);
                        db.SaveChanges();
                    }
                    value += 1;
                    Dispatcher.BeginInvoke((Action)(() => progcontact.Value = value));
                    Dispatcher.BeginInvoke((Action)(() => contactValue.Text = value.ToString()));
                }
            }
            readercontact.Close();
            cmd.Dispose();
            conn.Close();
        }

        public void employee()
        {
            MySqlConnection conn = new MySqlConnection(_connString);
            MySqlCommand cmd = new MySqlCommand();

            string sql_project = " SELECT * "
                                + " FROM hr_employee";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_project;

            cmd.CommandType = CommandType.Text;

            MySqlDataReader readeremp = cmd.ExecuteReader();
            DataTable dtemp = exeDTMysql(sql_project);
            int count = dtemp.Rows.Count;
            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => empMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => empValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progemp.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progemp.Value = value));

            while (readeremp.Read())
            {
                using (db db = new db())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    contact contact = new contact();
                    contact.id_contact = readeremp.GetInt32("id");
                    contact.id_contact_role = 1;

                    if (!(readeremp["name_full"] is DBNull))
                    {
                        contact.name = readeremp.GetString("name_full");
                    }
                    else
                    {
                        contact.name = "placeholder_name";
                    }

                    if (!(readeremp["code_employee"] is DBNull))
                    {
                        contact.code = readeremp.GetString("code_employee");
                    }
                    else
                    {
                        contact.code = "placeholder_code";
                    }

                    if (!(readeremp["gov_code"] is DBNull))
                    {
                        contact.gov_code = readeremp.GetString("gov_code");
                    }
                    else
                    {
                        contact.gov_code = "placeholder_gov_code";
                    }

                    if (!(readeremp["address"] is DBNull))
                    {
                        contact.address = readeremp.GetString("address");
                    }
                    else
                    {
                        contact.address = "placeholder_address";
                    }

                    if (!(readeremp["date_birth"] is DBNull))
                    {
                        contact.date_birth = readeremp.GetDateTime("date_birth");
                    }
                    if (!(readeremp["id_sex"] is DBNull))
                    {
                        if (readeremp.GetInt32("id_sex") == 1)
                        {
                            contact.gender = contact.Genders.Male;
                        }
                        else
                        {
                            contact.gender = contact.Genders.Female;
                        }
                    }

                    ////Nationality
                    //if (!(readeremp["date_birth"] is DBNull))
                    //{
                    //    contact.date_birth = readeremp.GetDateTime("date_birth");
                    //}

                    ////Position
                    ////ToFix

                    contact.credit_limit = 0;
                    contact.credit_availability = 0;
                    contact.is_customer = false;
                    contact.is_supplier = false;
                    contact.is_employee = true;
                    contact.is_sales_rep = false;

                    if (contact.Error == null)
                    {
                        db.contacts.Add(contact);
                        db.SaveChanges();
                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progemp.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => empValue.Text = value.ToString()));
                    }
                }
            }

            readeremp.Close();
            cmd.Dispose();
            conn.Close();
        }

        public void Talent()
        {
            MySqlConnection conn = new MySqlConnection(_connString);
            MySqlCommand cmd = new MySqlCommand();

            string sql_project = " SELECT * "
                                + " FROM hr_talent";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_project;

            cmd.CommandType = CommandType.Text;

            MySqlDataReader readeremp = cmd.ExecuteReader();
            DataTable dtemp = exeDTMysql(sql_project);
            int count = dtemp.Rows.Count;
            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => TalentMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => TalentValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progTalent.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progTalent.Value = value));

            while (readeremp.Read())
            {
                using (db db = new db())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    hr_talent hr_talent = new hr_talent();

                    if (!(readeremp["talent"] is DBNull))
                    {
                        hr_talent.name = readeremp.GetString("talent");
                    }
                    else
                    {
                        hr_talent.name = "placeholder_name";
                    }

                    MySqlConnection conntalent = new MySqlConnection(_connString);
                    MySqlCommand cmdtalent = new MySqlCommand();

                    string sql_talent = " SELECT *, (select name_full from hr_employee where id=id_employee) as name"
                                        + " FROM hr_emp_talent_rel where id_talent=" + readeremp.GetInt32("id");
                    conntalent.Open();
                    cmdtalent.Connection = conntalent;
                    cmdtalent.CommandText = sql_talent;

                    cmdtalent.CommandType = CommandType.Text;

                    MySqlDataReader readeremptalent = cmdtalent.ExecuteReader();
                    DataTable dtemptalent = exeDTMysql(sql_talent);

                    foreach (DataRow item in dtemptalent.Rows)
                    {
                        string name = item["name"].ToString();
                        hr_talent_detail hr_talent_detail = new hr_talent_detail();
                        hr_talent_detail.id_contact = db.contacts.Where(x => x.name == name).FirstOrDefault().id_contact;
                        if (!(item["efficiency"] is DBNull))
                        {
                            hr_talent_detail.experience = Convert.ToDecimal(item["efficiency"]);
                        }
                        else
                        {
                            hr_talent_detail.experience = 0;
                        }

                        hr_talent.hr_talent_detail.Add(hr_talent_detail);
                    }

                    db.hr_talent.Add(hr_talent);
                    db.SaveChanges();
                    value += 1;
                    Dispatcher.BeginInvoke((Action)(() => progTalent.Value = value));
                    Dispatcher.BeginInvoke((Action)(() => TalentValue.Text = value.ToString()));
                }
            }

            readeremp.Close();
            cmd.Dispose();
            conn.Close();
        }

        public void production()
        {
            entity.dbContext _enitity = new dbContext();
            MySqlConnection connproj = new MySqlConnection(_connString);
            //Counts Total number of Rows we have to process
            MySqlCommand cmdproj = new MySqlCommand();
            string sql_proj = " SELECT * FROM project";
            connproj.Open();
            cmdproj.Connection = connproj;
            cmdproj.CommandText = sql_proj;

            cmdproj.CommandType = CommandType.Text;

            MySqlDataReader readerproject = cmdproj.ExecuteReader();
            DataTable dtproject = exeDTMysql(sql_proj);

            if (_enitity.db.production_line.Count() == 0)
            {
                production_line production_line = new production_line();
                if (_enitity.db.app_location.FirstOrDefault() != null)
                {
                    production_line.id_location = _enitity.db.app_location.FirstOrDefault().id_location;
                }
                else
                { MessageBox.Show("Please Create Location"); }

                production_line.name = "Linea A";
                _enitity.db.production_line.Add(production_line);
                _enitity.db.SaveChanges();
            }

            while (readerproject.Read())
            {
                using (db db = new db())
                {
                    MySqlConnection conn = new MySqlConnection(_connString);
                    MySqlCommand cmd = new MySqlCommand();
                    int project = readerproject.GetInt32("id");
                    string sql_statement = " call sp_view_project_task_to_plan(" + project.ToString() + ")";
                    conn.Open();
                    cmd.Connection = conn;
                    cmd.CommandText = sql_statement;

                    cmd.CommandType = CommandType.Text;

                    MySqlDataReader readerproduction = cmd.ExecuteReader();
                    DataTable dtproduction = exeDTMysql(sql_statement);
                    int count = dtproduction.Rows.Count;
                    int value = 0;
                    Dispatcher.BeginInvoke((Action)(() => productionMaximum.Text = count.ToString()));
                    Dispatcher.BeginInvoke((Action)(() => productionValue.Text = value.ToString()));
                    Dispatcher.BeginInvoke((Action)(() => progProduction.Maximum = count));
                    Dispatcher.BeginInvoke((Action)(() => progProduction.Value = value));

                    production_order production_order = new production_order();
                    if (dtproduction.Rows.Count > 0)
                    {
                        production_order.id_production_order = Convert.ToInt32(dtproduction.Rows[0]["id"]);
                        production_order.id_project = project;
                        if (_enitity.db.production_line.Count() > 0)
                        {
                            production_order.id_production_line = _enitity.db.production_line.FirstOrDefault().id_production_line;
                        }

                        MySqlConnection connname = new MySqlConnection(_connString);
                        connname.Open();
                        MySqlCommand cmdname = new MySqlCommand();
                        cmdname.Connection = connname;
                        cmdname.CommandText = "select (select name_plan from production_plan where id=id_plan) as name_plan from production_logistics where id_task=" + Convert.ToInt32(dtproduction.Rows[0]["id"]);

                        cmdname.CommandType = CommandType.Text;

                        if (cmdname.ExecuteScalar() != null)
                        {
                            if (cmdname.ExecuteScalar().ToString() != "")
                            {
                                production_order.name = cmdname.ExecuteScalar().ToString();
                            }
                            else { production_order.name = "name"; }
                        }
                        else
                        {
                            production_order.name = "name";
                        }

                        MySqlConnection connstatus = new MySqlConnection(_connString);
                        connstatus.Open();
                        MySqlCommand cmdstatus = new MySqlCommand();
                        cmdstatus.Connection = connstatus;
                        cmdstatus.CommandText = "select (select (select status from app_status where id=id_status) from production_plan where id=id_plan) as id_status from production_logistics where id_task=" + Convert.ToInt32(dtproduction.Rows[0]["id"]);

                        cmdstatus.CommandType = CommandType.Text;

                        if (cmdstatus.ExecuteScalar() != null)
                        {
                            if (cmdstatus.ExecuteScalar().ToString() != "")
                            {
                                if (cmdstatus.ExecuteScalar().ToString() == "Aprobado")
                                {
                                    production_order.status = Status.Production.Approved;
                                }
                                else if (cmdstatus.ExecuteScalar().ToString() == "Pendiente")
                                {
                                    production_order.status = Status.Production.Pending;
                                }
                                else { production_order.status = Status.Production.Pending; }
                            }
                        }
                        else
                        {
                            production_order.status = Status.Production.Pending;
                        }

                        if (!(dtproduction.Rows[0]["id_project"] is DBNull))
                        {
                            string projectname = dtproduction.Rows[0]["project"].ToString();
                            if (db.projects.Where(x => x.name == projectname).FirstOrDefault() != null)
                            {
                                production_order.id_project = db.projects.Where(x => x.name == projectname).FirstOrDefault().id_project;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            production_order.id_project = 0;
                            continue;
                        }

                        if (!(dtproduction.Rows[0]["begin_date"] is DBNull))
                        {
                            production_order.start_date_est = Convert.ToDateTime(dtproduction.Rows[0]["begin_date"]);
                        }

                        foreach (DataRow _readerproduction in dtproduction.Rows)
                        {
                            db.Configuration.AutoDetectChangesEnabled = false;

                            production_order_detail production_order_detail = new production_order_detail();
                            int id_task_rel = Convert.ToInt32(_readerproduction["id_task_rel"]);
                            if (db.production_order_detail.Where(x => x.id_production_order == id_task_rel).FirstOrDefault() != null)
                            {
                                production_order_detail.parent = db.production_order_detail.Where(x => x.id_production_order == id_task_rel).FirstOrDefault();
                            }
                            int id_item = Convert.ToInt32(_readerproduction["id_sku"]);
                            string name = Convert.ToString(_readerproduction["product"]);
                            if (db.items.Where(x => x.name == name).FirstOrDefault() != null)
                            {
                                item _item = db.items.Where(x => x.name == name).FirstOrDefault();
                                production_order_detail.id_item = _item.id_item;
                                production_order_detail.name = _item.name;
                            }
                            else
                            {
                                continue;
                            }

                            string project_name = Convert.ToString(_readerproduction["project"]);
                            string Task_name = Convert.ToString(dtproduction.Rows[0]["Task_denomination"]);
                            if (db.project_task.Where(x => x.item_description == Task_name && x.project.name == project_name).FirstOrDefault() != null)
                            {
                                project_task project_task = db.project_task.Where(x => x.item_description == Task_name && x.project.name == project_name).FirstOrDefault();
                                project_task.status = Status.Project.InProcess;
                                production_order_detail.id_project_task = db.project_task.Where(x => x.item_description == Task_name && x.project.name == project_name).FirstOrDefault().id_project_task;
                            }

                            if (_readerproduction["qty"] is DBNull)
                            {
                                production_order_detail.quantity = 0;
                            }
                            else
                            {
                                production_order_detail.quantity = Convert.ToInt32(_readerproduction["qty"]);
                            }

                            production_order.production_order_detail.Add(production_order_detail);
                            value += 1;
                            Dispatcher.BeginInvoke((Action)(() => progProduction.Value = value));
                            Dispatcher.BeginInvoke((Action)(() => productionValue.Text = value.ToString()));
                        }
                        if (production_order.Error == null)
                        {
                            db.production_order.Add(production_order);
                        }
                        dtproduction.Clear();
                        readerproduction.Close();
                        cmd.Dispose();
                        conn.Close();
                    }

                    db.SaveChanges();
                }
            }
            readerproject.Close();
            cmdproj.Dispose();
            connproj.Close();
        }

        public void production_Exec()
        {
            try
            {
                entity.dbContext _enitity = new dbContext();

                MySqlConnection connproduction = new MySqlConnection(_connString);
                //Counts Total number of Rows we have to process
                MySqlCommand cmdproduction = new MySqlCommand();
                string sql_statement = " SELECT production_execution.*,production_plan.name_plan FROM production_execution inner join  production_plan on production_execution.id_plan=production_plan.id";

                connproduction.Open();
                cmdproduction.Connection = connproduction;
                cmdproduction.CommandText = sql_statement;

                cmdproduction.CommandType = CommandType.Text;

                MySqlDataReader readerproduction = cmdproduction.ExecuteReader();
                DataTable dtproductionexec = exeDTMysql(sql_statement);
                int count = dtproductionexec.Rows.Count;
                int value = 0;

                Dispatcher.BeginInvoke((Action)(() => ExecMaximum.Text = count.ToString()));
                Dispatcher.BeginInvoke((Action)(() => ExecValue.Text = value.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progExec.Maximum = count));
                Dispatcher.BeginInvoke((Action)(() => progExec.Value = value));

                while (readerproduction.Read())
                {
                    using (db db = new db())
                    {
                        db.Database.CommandTimeout = 5500;
                        //  production_execution production_execution = new production_execution();
                        int id = Convert.ToInt32(readerproduction["id"]);
                        //  production_execution.id_production_execution = id;

                        //if (_enitity.db.production_line.Count() > 0)
                        //{
                        //    production_execution.id_production_line = _enitity.db.production_line.FirstOrDefault().id_production_line;
                        //}

                        //String order_name = Convert.ToString(readerproduction["name_plan"]);
                        //if (_enitity.db.production_order.Where(x => x.name == order_name).FirstOrDefault() != null)
                        //{
                        //    production_execution.id_production_order = _enitity.db.production_order.Where(x => x.name == order_name).FirstOrDefault().id_production_order;
                        //}

                        //production_execution.trans_date = Convert.ToDateTime(readerproduction["timestamp"]);

                        MySqlConnection connproduction_detail = new MySqlConnection(_connString);

                        //Counts Total number of Rows we have to process
                        MySqlCommand cmdproduction_detail = new MySqlCommand("set net_write_timeout=99999; set net_read_timeout=99999");
                        sql_statement = " select *," +
                                        " (select name_plan from production_plan where production_plan.id=id_plan) as plan_name," +
                                        " (select" +
                                        " (select project from project where project.id=id_project) from project_task where project_task.id=id_task) as project," +
                                        " (select (select product from item_sku Where item_sku.id=id_sku) from project_task where project_task.id=id_task)  as task_item" +
                                        " from (SELECT production_execution_detail.*," +
                                        " (select product from item_sku Where item_sku.id=production_execution_detail.id_sku) as product," +
                                        " (select name_full from hr_employee Where hr_employee.id=production_execution_detail.id_employee) as contact," +
                                        " (select id_task from production_logistics Where production_logistics.id=production_execution_detail.id_logistics)as id_task," +
                                        " (select id_plan from production_logistics Where production_logistics.id=production_execution_detail.id_logistics)as id_plan" +
                                        " FROM production_execution_detail) as abc where id_execution=" + id;

                        connproduction_detail.Open();
                        cmdproduction_detail.Connection = connproduction_detail;
                        cmdproduction_detail.CommandText = sql_statement;

                        cmdproduction_detail.CommandType = CommandType.Text;

                        //MySqlDataReader readerproduction_detail = cmdproduction_detail.ExecuteReader();
                        DataTable dtproductionexecdetail = exeDTMysql(sql_statement);
                        for (int i = 0; i < dtproductionexecdetail.Rows.Count - 1; i++)
                        {
                            db.Configuration.AutoDetectChangesEnabled = false;
                            production_execution_detail production_execution_detail = new production_execution_detail();
                            production_execution_detail.id_execution_detail = Convert.ToInt32(dtproductionexecdetail.Rows[i]["id"]);
                            string name = Convert.ToString(dtproductionexecdetail.Rows[i]["contact"]);
                            if (db.contacts.Where(x => x.name == name).FirstOrDefault() != null)
                            {
                                production_execution_detail.id_contact = db.contacts.Where(x => x.name == name).FirstOrDefault().id_contact;
                            }
                            name = Convert.ToString(dtproductionexecdetail.Rows[i]["product"]);
                            if (db.items.Where(x => x.name == name).FirstOrDefault() != null)
                            {
                                production_execution_detail.id_item = db.items.Where(x => x.name == name).FirstOrDefault().id_item;
                            }

                            //if (production_execution.id_production_order == 0)
                            //{
                            //    string ord_name = Convert.ToString(dtproductionexecdetail.Rows[i]["plan_name"]);

                            //    if (db.production_order.Where(x => x.name == ord_name).FirstOrDefault() != null)
                            //    {
                            //        production_order production_order = db.production_order.Where(x => x.name == ord_name).FirstOrDefault();
                            //        production_order.status = Status.Production.InProcess;
                            //        production_execution.production_order = db.production_order.Where(x => x.name == ord_name).FirstOrDefault();
                            //    }
                            //    else { continue; }
                            //}

                            production_execution_detail.quantity = Convert.ToInt32(dtproductionexecdetail.Rows[i]["qty"]);

                            int id_logi = 0;
                            id_logi = Convert.ToInt32(dtproductionexecdetail.Rows[i]["id_logistics"]);
                            if (id_logi > 0)
                            {
                                if (db.production_order_detail.Where(x => x.id_order_detail == id_logi).FirstOrDefault() != null)
                                {
                                    production_execution_detail.id_order_detail = Convert.ToInt32(dtproductionexecdetail.Rows[i]["id_logistics"]);
                                }
                                //else
                                //{ continue; }
                            }

                            String Task_name = Convert.ToString(dtproductionexecdetail.Rows[i]["task_item"]);
                            String project_name = Convert.ToString(dtproductionexecdetail.Rows[i]["project"]);
                            if (db.project_task.Where(x => x.item_description == Task_name && x.project.name == project_name).FirstOrDefault() != null)
                            {
                                production_execution_detail.id_project_task = db.project_task.Where(x => x.item_description == Task_name && x.project.name == project_name).FirstOrDefault().id_project_task;
                            }
                            db.production_execution_detail.Add(production_execution_detail);
                        }
                        //if (production_execution.Error == null && production_execution.production_execution_detail.Count() > 0)
                        //{
                        //    db.production_execution.Add(production_execution);

                        //}
                        // readerproduction_detail.Close();
                        dtproductionexec.Clear();
                        cmdproduction_detail.Dispose();
                        connproduction_detail.Close();

                        db.SaveChanges();

                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progExec.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => ExecValue.Text = value.ToString()));
                    }
                }
                readerproduction.Close();
                cmdproduction.Dispose();
                connproduction.Close();
            }
            catch
            {
                throw;
            }
        }

        public void Purchase_Invoice()
        {
            try
            {
                entity.dbContext _enitity = new dbContext();

                MySqlConnection connpurchase = new MySqlConnection(_connString);
                //Counts Total number of Rows we have to process
                MySqlCommand cmdpurchase = new MySqlCommand();
                string sql_statement = " SELECT *,(select department from app_department where app_department.id=purchase_invoice.id_department) as deptname," +
                                        "(select name_full from contact where contact.id=purchase_invoice.id_contact) as contactname," +
                                        "(select contract from app_contract where app_contract.id=purchase_invoice.id_contract) as contractname," +
                                        "(select app_condition.condition from app_condition where app_condition.id=purchase_invoice.id_condition) as conditionname " +
                                        " from purchase_invoice";

                connpurchase.Open();
                cmdpurchase.Connection = connpurchase;
                cmdpurchase.CommandText = sql_statement;

                cmdpurchase.CommandType = CommandType.Text;

                MySqlDataReader readerpurchase = cmdpurchase.ExecuteReader();
                DataTable dtpurchase = exeDTMysql(sql_statement);
                int count = dtpurchase.Rows.Count;
                int value = 0;

                Dispatcher.BeginInvoke((Action)(() => purchaseMaximum.Text = count.ToString()));
                Dispatcher.BeginInvoke((Action)(() => purchaseValue.Text = value.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progPurchase.Maximum = count));
                Dispatcher.BeginInvoke((Action)(() => progPurchase.Value = value));

                while (readerpurchase.Read())
                {
                    using (db db = new db())
                    {
                        db.Database.CommandTimeout = 5500;
                        purchase_invoice purchase_invoice = new purchase_invoice();
                        int id = Convert.ToInt32(readerpurchase["id"]);
                        purchase_invoice.id_purchase_invoice = id;

                        if (!(readerpurchase["deptname"] is DBNull))
                        {
                            string deptname = Convert.ToString(readerpurchase["deptname"]);
                            app_department app_department = db.app_department.Where(x => x.name == deptname).FirstOrDefault();
                            if (app_department != null)
                            {
                                purchase_invoice.id_department = app_department.id_department;
                            }
                            else
                            {
                                purchase_invoice.id_department = db.app_department.FirstOrDefault().id_department;
                            }
                        }
                        else
                        {
                            purchase_invoice.id_department = db.app_department.FirstOrDefault().id_department;
                        }
                        if (!(readerpurchase["contactname"] is DBNull))
                        {
                            string contactname = Convert.ToString(readerpurchase["contactname"]);
                            contact contact = db.contacts.Where(x => x.name == contactname).FirstOrDefault();
                            if (contact != null)
                            {
                                purchase_invoice.id_contact = contact.id_contact;
                            }
                        }
                        if (!(readerpurchase["contractname"] is DBNull))
                        {
                            string contractname = Convert.ToString(readerpurchase["contractname"]);
                            app_contract app_contract = db.app_contract.Where(x => x.name == contractname).FirstOrDefault();
                            if (app_contract != null)
                            {
                                purchase_invoice.id_contract = app_contract.id_contract;
                            }
                            else
                            {
                                purchase_invoice.id_contract = db.app_contract.FirstOrDefault().id_contract;
                            }
                        }
                        else
                        {
                            purchase_invoice.id_contract = db.app_contract.FirstOrDefault().id_contract;
                        }
                        if (!(readerpurchase["conditionname"] is DBNull))
                        {
                            string conditionname = Convert.ToString(readerpurchase["conditionname"]);
                            app_condition app_condition = db.app_condition.Where(x => x.name == conditionname).FirstOrDefault();
                            if (app_condition != null)
                            {
                                purchase_invoice.id_condition = app_condition.id_condition;
                            }
                            else
                            {
                                purchase_invoice.id_condition = db.app_condition.FirstOrDefault().id_condition;
                            }
                        }
                        else
                        {
                            purchase_invoice.id_condition = db.app_condition.FirstOrDefault().id_condition;
                        }

                        if (!(readerpurchase["purchase_number"] is DBNull))
                        {
                            purchase_invoice.number = Convert.ToString(readerpurchase["purchase_number"]);
                        }
                        if (!(readerpurchase["purchase_date"] is DBNull))
                        {
                            purchase_invoice.trans_date = Convert.ToDateTime(readerpurchase["purchase_date"]);
                        }
                        purchase_invoice.id_currencyfx = db.app_currencyfx.FirstOrDefault().id_currencyfx;

                        MySqlConnection connpurchasedetail = new MySqlConnection(_connString);
                        //Counts Total number of Rows we have to process
                        MySqlCommand cmdpurchasedetail = new MySqlCommand();
                        string sql_statementdetail = " SELECT  *,(select location from app_location where app_location.id=purchase_invoice_detail.id_location) as locationname," +
                                                      "(select product from item_sku where item_sku.id=purchase_invoice_detail.id_sku) as itemname," +
                                                        "(select tax from app_tax where app_tax.id=purchase_invoice_detail.id_tax) as taxname," +
                                                       "(select cost_center from app_cost_center where app_cost_center.id=purchase_invoice_detail.id_cost_center) as costcentername " +
                                                       " from purchase_invoice_detail where id_purchase_invoice=" + id;

                        connpurchasedetail.Open();
                        cmdpurchasedetail.Connection = connpurchasedetail;
                        cmdpurchasedetail.CommandText = sql_statementdetail;

                        cmdpurchasedetail.CommandType = CommandType.Text;

                        MySqlDataReader readerpurchasedetail = cmdpurchasedetail.ExecuteReader();
                        while (readerpurchasedetail.Read())
                        {
                            db.Database.CommandTimeout = 5500;
                            purchase_invoice_detail purchase_invoice_detail = new purchase_invoice_detail();
                            int id_detail = Convert.ToInt32(readerpurchasedetail["id"]);
                            purchase_invoice_detail.id_purchase_invoice_detail = id;

                            if (!(readerpurchasedetail["locationname"] is DBNull))
                            {
                                string locationname = Convert.ToString(readerpurchasedetail["locationname"]);
                                app_location app_location = db.app_location.Where(x => x.name == locationname).FirstOrDefault();
                                if (app_location != null)
                                {
                                    purchase_invoice_detail.id_location = app_location.id_location;
                                }
                                else
                                {
                                    purchase_invoice_detail.id_location = db.app_location.FirstOrDefault().id_location;
                                }
                            }
                            else
                            {
                                purchase_invoice_detail.id_location = db.app_location.FirstOrDefault().id_location;
                            }
                            if (!(readerpurchasedetail["itemname"] is DBNull))
                            {
                                string itemname = Convert.ToString(readerpurchasedetail["itemname"]);
                                item item = db.items.Where(x => x.name == itemname).FirstOrDefault();
                                if (item != null)
                                {
                                    purchase_invoice_detail.id_item = item.id_item;
                                }
                            }
                            if (!(readerpurchasedetail["item_description"] is DBNull))
                            {
                                purchase_invoice_detail.item_description = Convert.ToString(readerpurchasedetail["item_description"]);
                            }
                            if (!(readerpurchasedetail["unit_cost"] is DBNull))
                            {
                                purchase_invoice_detail.unit_cost = Convert.ToDecimal(readerpurchasedetail["unit_cost"]);
                            }
                            if (!(readerpurchasedetail["order_qty"] is DBNull))
                            {
                                purchase_invoice_detail.quantity = Convert.ToDecimal(readerpurchasedetail["order_qty"]);
                            }
                            if (!(readerpurchasedetail["costcentername"] is DBNull))
                            {
                                string costcentername = Convert.ToString(readerpurchasedetail["costcentername"]);
                                app_cost_center app_cost_center = db.app_cost_center.Where(x => x.name == costcentername).FirstOrDefault();
                                if (app_cost_center != null)
                                {
                                    purchase_invoice_detail.id_cost_center = app_cost_center.id_cost_center;
                                }
                                else
                                {
                                    purchase_invoice_detail.id_cost_center = db.app_cost_center.FirstOrDefault().id_cost_center;
                                }
                            }
                            else
                            {
                                purchase_invoice_detail.id_cost_center = db.app_cost_center.FirstOrDefault().id_cost_center;
                            }

                            if (!(readerpurchasedetail["taxname"] is DBNull))
                            {
                                string taxname = Convert.ToString(readerpurchasedetail["taxname"]);
                                app_vat_group app_vat_group = db.app_vat_group.Where(x => x.name == taxname).FirstOrDefault();
                                if (app_vat_group != null)
                                {
                                    purchase_invoice_detail.id_vat_group = app_vat_group.id_vat_group;
                                }
                                else
                                {
                                    purchase_invoice_detail.id_vat_group = db.app_vat_group.FirstOrDefault().id_vat_group;
                                }
                            }
                            else
                            {
                                purchase_invoice_detail.id_vat_group = db.app_vat_group.FirstOrDefault().id_vat_group;
                            }

                            purchase_invoice.purchase_invoice_detail.Add(purchase_invoice_detail);
                        }
                        readerpurchasedetail.Close();
                        cmdpurchasedetail.Dispose();
                        connpurchasedetail.Close();

                        if (purchase_invoice.Error == null)
                        {
                            db.purchase_invoice.Add(purchase_invoice);
                        }
                        db.SaveChanges();

                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progPurchase.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => purchaseValue.Text = value.ToString()));
                    }
                    // readerproduction_detail.Close();

                    //cmdproduction_detail.Dispose();
                    //connproduction_detail.Close();
                }

                dtpurchase.Clear();
                readerpurchase.Close();
                cmdpurchase.Dispose();
                connpurchase.Close();
            }
            catch
            {
                throw;
            }
        }

        public void Sales_Invoice()
        {
            try
            {
                entity.dbContext _enitity = new dbContext();

                MySqlConnection connsales = new MySqlConnection(_connString);
                //Counts Total number of Rows we have to process
                MySqlCommand cmdsales = new MySqlCommand();
                string sql_statement = " SELECT *,(select department from app_department where app_department.id=sales_invoice.id_department) as deptname," +
                                        "(select name_full from contact where contact.id=sales_invoice.id_contact) as contactname," +
                                        "(select contract from app_contract where app_contract.id=sales_invoice.id_contract) as contractname," +
                                        "(select app_condition.condition from app_condition where app_condition.id=sales_invoice.id_condition) as conditionname " +
                                        " from sales_invoice";

                connsales.Open();
                cmdsales.Connection = connsales;
                cmdsales.CommandText = sql_statement;

                cmdsales.CommandType = CommandType.Text;

                MySqlDataReader readersales = cmdsales.ExecuteReader();
                DataTable dtsales = exeDTMysql(sql_statement);
                int count = dtsales.Rows.Count;
                int value = 0;

                Dispatcher.BeginInvoke((Action)(() => salesMaximum.Text = count.ToString()));
                Dispatcher.BeginInvoke((Action)(() => salesValue.Text = value.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progSales.Maximum = count));
                Dispatcher.BeginInvoke((Action)(() => progSales.Value = value));

                while (readersales.Read())
                {
                    using (db db = new db())
                    {
                        db.Database.CommandTimeout = 5500;
                        sales_invoice sales_invoice = new sales_invoice();
                        int id = Convert.ToInt32(readersales["id"]);
                        sales_invoice.id_sales_invoice = id;

                        if (!(readersales["contactname"] is DBNull))
                        {
                            string contactname = Convert.ToString(readersales["contactname"]);
                            contact contact = db.contacts.Where(x => x.name == contactname).FirstOrDefault();
                            if (contact != null)
                            {
                                sales_invoice.id_contact = contact.id_contact;
                            }
                        }
                        if (!(readersales["contractname"] is DBNull))
                        {
                            string contractname = Convert.ToString(readersales["contractname"]);
                            app_contract app_contract = db.app_contract.Where(x => x.name == contractname).FirstOrDefault();
                            if (app_contract != null)
                            {
                                sales_invoice.id_contract = app_contract.id_contract;
                            }
                            else
                            {
                                sales_invoice.id_contract = db.app_contract.FirstOrDefault().id_contract;
                            }
                        }
                        else
                        {
                            sales_invoice.id_contract = db.app_contract.FirstOrDefault().id_contract;
                        }
                        if (!(readersales["conditionname"] is DBNull))
                        {
                            string conditionname = Convert.ToString(readersales["conditionname"]);
                            app_condition app_condition = db.app_condition.Where(x => x.name == conditionname).FirstOrDefault();
                            if (app_condition != null)
                            {
                                sales_invoice.id_condition = app_condition.id_condition;
                            }
                            else
                            {
                                sales_invoice.id_condition = db.app_condition.FirstOrDefault().id_condition;
                            }
                        }
                        else
                        {
                            sales_invoice.id_condition = db.app_condition.FirstOrDefault().id_condition;
                        }

                        if (!(readersales["purchase_number"] is DBNull))
                        {
                            sales_invoice.number = Convert.ToString(readersales["purchase_number"]);
                        }
                        if (!(readersales["purchase_date"] is DBNull))
                        {
                            sales_invoice.trans_date = Convert.ToDateTime(readersales["purchase_date"]);
                        }
                        sales_invoice.id_currencyfx = db.app_currencyfx.FirstOrDefault().id_currencyfx;

                        MySqlConnection connsalesdetail = new MySqlConnection(_connString);
                        //Counts Total number of Rows we have to process
                        MySqlCommand cmdsalesdetail = new MySqlCommand();
                        string sql_statementdetail = " SELECT  *,(select location from app_location where app_location.id=sales_invoice_detail.id_location) as locationname," +
                                                        "(select tax from app_tax where app_tax.id=(select id_item_tax from sales_invoice_tax where sales_invoice_tax.id_sales_detail=sales_invoice_detail.id)) as taxname," +
                                                      "(select product from item_sku where item_sku.id=sales_invoice_detail.id_sku) as itemname " +
                                                       " from sales_invoice_detail where id_sales_invoice=" + id;

                        connsalesdetail.Open();
                        cmdsalesdetail.Connection = connsalesdetail;
                        cmdsalesdetail.CommandText = sql_statementdetail;

                        cmdsalesdetail.CommandType = CommandType.Text;

                        MySqlDataReader readersalesdetail = cmdsalesdetail.ExecuteReader();
                        while (readersalesdetail.Read())
                        {
                            db.Database.CommandTimeout = 5500;
                            sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();
                            int id_detail = Convert.ToInt32(readersalesdetail["id"]);
                            sales_invoice_detail.id_sales_invoice_detail = id;

                            if (!(readersalesdetail["locationname"] is DBNull))
                            {
                                string locationname = Convert.ToString(readersalesdetail["locationname"]);
                                app_location app_location = db.app_location.Where(x => x.name == locationname).FirstOrDefault();
                                if (app_location != null)
                                {
                                    sales_invoice_detail.id_location = app_location.id_location;
                                }
                                else
                                {
                                    sales_invoice_detail.id_location = db.app_location.FirstOrDefault().id_location;
                                }
                            }
                            else
                            {
                                sales_invoice_detail.id_location = db.app_location.FirstOrDefault().id_location;
                            }
                            if (!(readersalesdetail["itemname"] is DBNull))
                            {
                                string itemname = Convert.ToString(readersalesdetail["itemname"]);
                                item item = db.items.Where(x => x.name == itemname).FirstOrDefault();
                                if (item != null)
                                {
                                    sales_invoice_detail.id_item = item.id_item;
                                }
                            }
                            if (!(readersalesdetail["item_description"] is DBNull))
                            {
                                sales_invoice_detail.item_description = Convert.ToString(readersalesdetail["item_description"]);
                            }
                            if (!(readersalesdetail["unit_cost"] is DBNull))
                            {
                                sales_invoice_detail.unit_cost = Convert.ToDecimal(readersalesdetail["unit_cost"]);
                            }
                            if (!(readersalesdetail["invoice_qty"] is DBNull))
                            {
                                sales_invoice_detail.quantity = Convert.ToDecimal(readersalesdetail["invoice_qty"]);
                            }
                            if (!(readersalesdetail["taxname"] is DBNull))
                            {
                                string taxname = Convert.ToString(readersalesdetail["taxname"]);
                                app_vat_group app_vat_group = db.app_vat_group.Where(x => x.name == taxname).FirstOrDefault();
                                if (app_vat_group != null)
                                {
                                    sales_invoice_detail.id_vat_group = app_vat_group.id_vat_group;
                                }
                                else
                                {
                                    sales_invoice_detail.id_vat_group = db.app_vat_group.FirstOrDefault().id_vat_group;
                                }
                            }
                            else
                            {
                                sales_invoice_detail.id_vat_group = db.app_vat_group.FirstOrDefault().id_vat_group;
                            }

                            sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
                        }
                        readersalesdetail.Close();
                        cmdsalesdetail.Dispose();
                        connsalesdetail.Close();

                        if (sales_invoice.Error == null)
                        {
                            db.sales_invoice.Add(sales_invoice);
                        }
                        db.SaveChanges();

                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progSales.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => salesValue.Text = value.ToString()));
                    }
                    // readerproduction_detail.Close();

                    //cmdproduction_detail.Dispose();
                    //connproduction_detail.Close();
                }

                dtsales.Clear();
                readersales.Close();
                cmdsales.Dispose();
                connsales.Close();
            }
            catch
            {
                throw;
            }
        }
    }
}