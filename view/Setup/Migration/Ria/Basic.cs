using entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Cognitivo.Setup.Migration
{
    public partial class MigrationAssistant
    {
        //string company_Name { get; set; }

        public void start()
        {
            //basic();
            //Task basic_Task = Task.Factory.StartNew(() => basic());
            //basic_Task.Wait();

            //Task customer_Task = Task.Factory.StartNew(() => customer());
            //Task supplier_Task = Task.Factory.StartNew(() => supplier());
            //Task product_Task = Task.Factory.StartNew(() => product());

            ////////Wait for Related Tables to finish so we can be 
            ////////assured that they are available for us when 
            ////////we start with sales and purchase.
            //customer_Task.Wait();
            //supplier_Task.Wait();
            //product_Task.Wait();

            //Task accounting_Task = Task.Factory.StartNew(() => accounting());
            //accounting_Task.Wait();
            //////  Start Sales and Purchase
            //Task purchase_Task = Task.Factory.StartNew(() => purchase());

            //purchase_Task.Wait();
            //sales();
            Task sales_Task = Task.Factory.StartNew(() => sales());
            sales_Task.Wait();
            Task salesReturn_Task = Task.Factory.StartNew(() => salesReturn());
            salesReturn_Task.Wait();
            
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

            sync_MeasureList();

            sync_country();
            sync_state();
            sync_city();
            sync_zone();

            using (db db = new db())
            {
                payment_type payment_type;
                if (db.payment_type.Where(x => x.is_default).FirstOrDefault() == null)
                {
                    payment_type = GenerateDefaultPaymentType();
                    db.payment_type.Add(payment_type);
                    db.SaveChanges();
                }
                else
                {
                    payment_type = db.payment_type.Where(x => x.is_default).FirstOrDefault();
                }   
            }

            Dispatcher.BeginInvoke((Action)(() => progBasic.IsIndeterminate = false));
        }

        public app_contract GenerateDefaultContrat(app_condition app_condition, int interval)
        {
            app_contract app_contract = new app_contract();
            app_contract.app_condition = app_condition;
            app_contract.name = interval.ToString() + " Días";
            app_contract_detail _app_contract_detail = new app_contract_detail();
            _app_contract_detail.coefficient = 1;
            _app_contract_detail.app_contract = app_contract;
            _app_contract_detail.interval = (short)interval;
            _app_contract_detail.is_order =false;
            app_contract.app_contract_detail.Add(_app_contract_detail);
            return app_contract;
        }

        public payment_type GenerateDefaultPaymentType()
        {
            payment_type payment_type = new payment_type();
            payment_type.name = "Default";
            payment_type.payment_behavior = entity.payment_type.payment_behaviours.Normal;
            payment_type.is_default = true;
           return payment_type;
        }

        public app_account GenerateDefaultApp_Account()
        {

            app_account app_account = new app_account();
            app_account.name = "Default";
            app_account.id_account_type = entity.app_account.app_account_type.Terminal;
            app_account.code = "Default";
            app_account.is_active = true;
            return app_account;
        }

        private void sync_Company()
        {
            DataTable dt = exeDT("SELECT * FROM EMPRESA");
            DataTable dt_Branch = exeDT("SELECT * FROM SUCURSAL");
            DataTable dt_Terminal = exeDT("SELECT * FROM PC");

            foreach (DataRow row in dt.Rows)
            {
                app_company _app_company = new app_company();
                _app_company.name = row["NOMCONTRIBUYENTE"].ToString();
                _app_company.alias = row["NOMFANTASIA"].ToString();

                if (_app_company.name != null && _app_company.alias != null)
                {
                    _app_company.name = _app_company.alias;
                }
                else
                {
                    continue;
                }

                _app_company.address = (row["DIRECCION"].ToString() == "") ? "Address Placeholder" : row["DIRECCION"].ToString();
                _app_company.gov_code = (row["RUCCONTRIBUYENTE"].ToString() == "") ? "GovID Placeholder" : row["RUCCONTRIBUYENTE"].ToString();

                dbContext.app_company.Add(_app_company);
                dbContext.SaveChanges();

                id_company = _app_company.id_company;
                CurrentSession.Id_Company = id_company;

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    entity.Properties.Settings.Default.company_ID = id_company;
                    entity.Properties.Settings.Default.Save();
                }
                ));

                sync_Users();

                id_user = dbContext.security_user.Where(i => i.id_company == id_company).FirstOrDefault().id_user;
                CurrentSession.Id_User = id_company;

                foreach (DataRow row_Branch in dt_Branch.Rows)
                {
                    app_branch _app_branch = new app_branch();
                    _app_branch.id_company = id_company;
                    _app_branch.name = row_Branch["DESSUCURSAL"].ToString();
                    _app_branch.code = row_Branch["SUCURSALTIMBRADO"].ToString();
                    _app_branch.can_invoice = (row_Branch["TIPOSUCURSAL"].ToString().Contains("Factura")) ? true : false;
                    _app_branch.can_stock = (row_Branch["TIPOSUCURSAL"].ToString().Contains("Stock")) ? true : false;

                    if (_app_branch.can_stock)
                    {
                        app_location app_location = new app_location();
                        app_location.is_active = true;
                        app_location.is_default = true;
                        app_location.name = "Deposito";
                        _app_branch.app_location.Add(app_location);
                    }

                    string id_branchString = row_Branch["CODSUCURSAL"].ToString();
                    
                    foreach (DataRow row_Terminal in dt_Terminal.Select("CODSUCURSAL = " + id_branchString))
                    {
                        app_terminal app_terminal = new app_terminal();
                        app_terminal.is_active = true;
                        app_terminal.code = row_Terminal["NUMMAQUINA"].ToString();
                        app_terminal.name = row_Terminal["NOMBRE"].ToString();
                        _app_branch.app_terminal.Add(app_terminal);
                    }

                    if (_app_branch.Error == null)
                    {
                        dbContext.app_branch.Add(_app_branch);
                        dbContext.SaveChanges();
                    }
                }
                id_branch = dbContext.app_branch.Where(i => i.id_company == id_company).FirstOrDefault().id_branch;
                id_terminal = dbContext.app_terminal.Where(i => i.id_company == id_company).FirstOrDefault().id_terminal;
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    entity.Properties.Settings.Default.branch_ID = id_branch;
                    entity.Properties.Settings.Default.terminal_ID = id_terminal;
                    entity.Properties.Settings.Default.Save();
                }
              ));
            }

            dt.Clear();
            dt_Branch.Clear();
            dt_Terminal.Clear();
        }
   
        private void sync_ContactRole()
        {
            using (db db = new db())
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

            if (_connString.Contains("Angelius"))
            {
                SqlConnection conn = new SqlConnection(_connString);
                string sql = "SELECT CUSTOMFIELD FROM dbo.CLIENTES GROUP BY CUSTOMFIELD ORDER BY CUSTOMFIELD DESC";
                DataTable dt_customer = exeDT(sql);
                foreach (DataRow item in dt_customer.Rows)
                {
                    if (Regex.IsMatch(item[0].ToString(), @"^[a-zA-Z]+$"))
                    {
                        using (db db = new db())
                        {
                            contact_role contact_role = new contact_role();
                            contact_role.id_company = id_company;
                            contact_role.is_active = true;
                            contact_role.is_principal = false;
                            contact_role.can_transact = false;
                            contact_role.name = item[0].ToString();
                            db.contact_role.Add(contact_role);
                            db.SaveChanges();
                        }
                    }
                }

            }
        }

        private void sync_CostCenter()
        {
            DataTable dt = exeDT("SELECT * FROM CENTROCOSTO");
            foreach (DataRow row in dt.Rows)
            {
                app_cost_center app_cost_center = new app_cost_center();
                app_cost_center.name = (string)row["DESCENTRO"];
                app_cost_center.is_product = Convert.ToBoolean(row["ENLAZADO"]);
                app_cost_center.is_administrative = Convert.ToBoolean(row["COSTOFIJO"]);
                app_cost_center.id_company = id_company;
                app_cost_center.is_active = true;
                dbContext.app_cost_center.Add(app_cost_center);
            }
            dt.Clear();
            dbContext.SaveChanges();
        }

        private void sync_Currency()
        {
            DataTable dt = exeDT("SELECT * FROM MONEDA");
            foreach (DataRow row in dt.Rows)
            {
                app_currency app_currency = new app_currency();
                if (!(row["DESMONEDA"] is DBNull))
                {


                    app_currency.name = (string)row["DESMONEDA"];
                    app_currency.id_company = id_company;
                    app_currency.is_priority = Convert.ToBoolean(row["PRIORIDAD"]);
                    DataTable dtfx = exeDT("SELECT * FROM COTIZACION where CODMONEDA=" + Convert.ToInt32(row["CODMONEDA"]));
                    if (dtfx.Rows.Count > 0)
                    {

                        app_currencyfx app_currencyfx = new app_currencyfx();
                        app_currencyfx.buy_value = Convert.ToInt32(dtfx.Rows[0]["FACTORVENTA"]);
                        app_currencyfx.sell_value = Convert.ToInt32(dtfx.Rows[0]["FACTORCOBRO"]);
                        app_currencyfx.is_active = true;
                        app_currency.app_currencyfx.Add(app_currencyfx);
                    }
                    else
                    {
                        app_currencyfx app_currencyfx = new app_currencyfx();
                        app_currencyfx.buy_value = 0;
                        app_currencyfx.sell_value = 0;
                        app_currencyfx.is_active = true;
                        app_currency.app_currencyfx.Add(app_currencyfx);
                    }

                    app_currency.is_active = true;
                    //app_currency.id_country = id_country;
                    if (app_currency.Error == null)
                    {
                        dbContext.app_currency.Add(app_currency);
                    }
                }
            }
            dt.Clear();
            dbContext.SaveChanges();
        }

        private void sync_Accounts()
        {
            DataTable dt = exeDT("SELECT * FROM CAJA;");
            foreach (DataRow row in dt.Rows)
            {
                app_account app_account = new app_account();
                app_account.name = (string)row["NUMEROCAJA"];
                app_account.id_company = id_company;
                app_account.is_active = true;
                dbContext.app_account.Add(app_account);
            }
            dt.Clear();
            dbContext.SaveChanges();
        }

        private void sync_Condition()
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

        private void sync_Contracts()
        {
            string sql = " SELECT DIASVENCIMIENTO"
                       + " FROM  dbo.CLIENTES"
                       + " GROUP BY DIASVENCIMIENTO";
            DataTable dt = exeDT(sql);
            foreach (DataRow row in dt.Rows)
            {
                int _dias = Convert.ToInt32((row["DIASVENCIMIENTO"] is DBNull) ? 0 : row["DIASVENCIMIENTO"]);
                app_contract app_contract = new app_contract();
                app_contract.name = _dias + " " + "Días";
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

        private void sync_PriceList()
        {
            DataTable dt = exeDT("SELECT * FROM TIPOCLIENTE");
            foreach (DataRow row in dt.Rows)
            {
                item_price_list price = new item_price_list();
                price.name = (string)row["DESTIPOCLIENTE"];
                price.id_company = id_company;
                price.is_active = true;
                dbContext.item_price_list.Add(price);
            }
            dt.Clear();
            dbContext.SaveChanges();
        }

        private void sync_VatList()
        {
            DataTable dt = exeDT("SELECT * FROM IVA");
            foreach (DataRow row in dt.Rows)
            {
                app_vat_group_details app_vat_group_details = new app_vat_group_details();

                app_vat vat = new app_vat();
                vat.name = (string)row["DESIVA"];
                vat.id_company = id_company;
                vat.coefficient = (decimal)row["COHEFICIENTE"] - 1;
                dbContext.app_vat.Add(vat);
                dbContext.SaveChanges();

                app_vat_group vat_group = new app_vat_group();
                vat_group.name = (string)row["DESIVA"];
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

        private void sync_MeasureList()
        {
            //DataTable dt = exeDT("SELECT * FROM UNIDADMEDIDA");

            //app_measurement_type measure_type = new app_measurement_type();
            //measure_type.id_company = id_company;
            //measure_type.id_user = id_user;
            //measure_type.name = "Unit";

            //foreach (DataRow row in dt.Rows)
            //{
            //    app_measurement app_measurement = new app_measurement();
            //    app_measurement.name = (string)row["DESMEDIDA"];
            //    app_measurement.id_company = id_company;
            //    app_measurement.id_user = id_user;
            //    app_measurement.code_iso = (row["SIMBOLO"] == null) ? " " : row["SIMBOLO"].ToString();
            //    //app_measurement.app_measurement_type = measure_type;

            //    measure_type.app_measurement.Add(app_measurement);
            //}

            //dbContext.app_measurement_type.Add(measure_type);

            //dt.Clear();
            //dbContext.SaveChanges();
        }

        private void sync_TagsList()
        {
            DataTable dt = new DataTable();

            dt = exeDT("SELECT * FROM FAMILIA");
            foreach (DataRow row in dt.Rows)
            {
                using (db db = new db())
                {
                    string _tag = row["DESFAMILIA"].ToString();
                    if (db.item_tag.Any(x => x.name == _tag && x.id_company == id_company))
                    {
                        continue;
                    }
                    item_tag tag = new item_tag();
                    tag.id_company = id_company;
                    tag.name = (string)row["DESFAMILIA"];
                    db.item_tag.Add(tag);
                    db.SaveChanges();
                }
            }
            dt.Clear();

            dt = exeDT("SELECT * FROM LINEA");
            foreach (DataRow row in dt.Rows)
            {
                using (db db = new db())
                {
                    string _tag = row["DESLINEA"].ToString();
                    if (db.item_tag.Any(x => x.name == _tag && x.id_company == id_company))
                    {
                        continue;
                    }
                    item_tag tag = new item_tag();
                    tag.id_company = id_company;
                    tag.name = (string)row["DESLINEA"];
                    db.item_tag.Add(tag);
                    db.SaveChanges();
                }
            }
            dt.Clear();

            dt = exeDT("SELECT * FROM RUBRO");
            foreach (DataRow row in dt.Rows)
            {
                using (db db = new db())
                {
                    string _tag = row["DESRUBRO"].ToString();
                    if (db.item_tag.Any(x => x.name == _tag && x.id_company == id_company))
                    {
                        continue;
                    }
                    item_tag tag = new item_tag();
                    tag.id_company = id_company;
                    tag.name = (string)row["DESRUBRO"];
                    db.item_tag.Add(tag);
                    db.SaveChanges();
                }
            }
            dt.Clear();
        }

        private void sync_SalesRep()
        {
            DataTable dt = exeDT("SELECT * FROM VENDEDOR");
            foreach (DataRow row in dt.Rows)
            {
                sales_rep sales_rep = new sales_rep();
                sales_rep.name = (string)row["DESVENDEDOR"];
                sales_rep.id_company = id_company;
                sales_rep.is_active = true;
                dbContext.sales_rep.Add(sales_rep);
            }
            dt.Clear();
            dbContext.SaveChanges();
        }

        private void sync_country()
        {
            DataTable dt = exeDT("SELECT * FROM PAIS");
            foreach (DataRow row in dt.Rows)
            {
                app_geography app_geography = new app_geography();
                app_geography.code = (string)row["DESPAIS"];
                app_geography.name = (string)row["DESPAIS"];
                app_geography.type = Status.geo_types.Country;
                dbContext.app_geography.Add(app_geography);
            }
            dt.Clear();
            dbContext.SaveChanges();
        }


        private void sync_state()
        {
            DataTable dt = exeDT("SELECT *,(select DESPAIS from PAIS where PAIS.CODPAIS=DEPARTAMENTO.CODPAIS) as DESPAIS FROM DEPARTAMENTO");
            foreach (DataRow row in dt.Rows)
            {
                app_geography app_geography = new app_geography();
                app_geography.code = (string)row["DESDEPARTAMENTO"];
                app_geography.name = (string)row["DESDEPARTAMENTO"];
                app_geography.type = Status.geo_types.State;
                string name = (string)row["DESPAIS"];
                app_geography _app_geography = dbContext.app_geography.Where(x => x.name == name).FirstOrDefault();
                if (_app_geography!=null)
                {
                    app_geography.parent = _app_geography;
                }
                dbContext.app_geography.Add(app_geography);
            }
            dt.Clear();
            dbContext.SaveChanges();
        }
        private void sync_city()
        {
            DataTable dt = exeDT("SELECT *,(select DESDEPARTAMENTO from DEPARTAMENTO where DEPARTAMENTO.CODDEPARTAMENTO=CIUDAD.CODDEPARTAMENTO) as DESDEPARTAMENTO,(select DESPAIS from PAIS where PAIS.CODPAIS=CIUDAD.CODPAIS) as DESPAIS FROM CIUDAD");
            foreach (DataRow row in dt.Rows)
            {
                app_geography app_geography = new app_geography();
                app_geography.code = (string)row["DESCIUDAD"];
                app_geography.name = (string)row["DESCIUDAD"];
                app_geography.type = Status.geo_types.City;
                if (row["DESDEPARTAMENTO"] is DBNull)
                {
                    if (!(row["DESPAIS"] is DBNull))
                    {
                        string name = (string)row["DESPAIS"];
                        app_geography _app_geography = dbContext.app_geography.Where(x => x.name == name).FirstOrDefault();
                        if (_app_geography != null)
                        {
                            app_geography.parent = _app_geography;
                        }
                    }
                 
                }
                else
                {
                    string name = (string)row["DESDEPARTAMENTO"];
                    app_geography _app_geography = dbContext.app_geography.Where(x => x.name == name).FirstOrDefault();
                    if (_app_geography != null)
                    {
                        app_geography.parent = _app_geography;
                    }
                }
               
                dbContext.app_geography.Add(app_geography);
            }
            dt.Clear();
            dbContext.SaveChanges();
        }
        private void sync_zone()
        {
            DataTable dt = exeDT("SELECT *,(select DESCIUDAD from CIUDAD where CIUDAD.CODCIUDAD=ZONA.CODCIUDAD) as DESCIUDAD FROM ZONA");
            foreach (DataRow row in dt.Rows)
            {
                app_geography app_geography = new app_geography();
                app_geography.code = (string)row["DESZONA"];
                app_geography.name = (string)row["DESZONA"];
                app_geography.type = Status.geo_types.Zone;
                if (!(row["DESCIUDAD"] is DBNull))
                {
                    string name = (string)row["DESCIUDAD"];
                    app_geography _app_geography = dbContext.app_geography.Where(x => x.name == name).FirstOrDefault();
                    if (_app_geography != null)
                    {
                        app_geography.parent = _app_geography;
                    }

                }
                
               
                dbContext.app_geography.Add(app_geography);
            }
            dt.Clear();
            dbContext.SaveChanges();
        }

        private void sync_Users()
        {
            try
            {
            security_role security_role = new security_role();
            security_role.is_active = true;
            security_role.name = "Administrador";
            security_role.id_company = id_company;
            dbContext.security_role.Add(security_role);
            dbContext.SaveChanges();
            DataTable dt = exeDT("SELECT * FROM USUARIO");
            foreach (DataRow row in dt.Rows)
            {
                security_user security_user = new security_user();
                if (row["DESUSUARIO"].ToString() != "")
                {
                    security_user.name = row["DESUSUARIO"].ToString();
                }
                else
                {
                    security_user.name = "name";
                }
                if (row["PASSUSUARIO"].ToString() != "")
                {
                    security_user.password = row["PASSUSUARIO"].ToString();
                }
                else
                {
                    security_user.password = "123";
                }
                if (row["NOMBRE"].ToString() != "")
                {
                    security_user.name_full = row["NOMBRE"].ToString();
                }
                else
                {
                    security_user.name_full = "name";
                }
                security_user.id_company = id_company;
                security_user.is_active = true;
                security_user.id_role = security_role.id_role;
                security_user.security_role = security_role;

                dbContext.security_user.Add(security_user);
            }
            dt.Clear();
          
                IEnumerable<DbEntityValidationResult> validationresult = dbContext.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    dbContext.SaveChanges();
                }
            }
            catch 
            {
                throw;
            }

        }
    }
}
