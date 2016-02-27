using entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;

namespace Cognitivo.Setup.Migration
{
    public partial class MigrationAssistant
    {
        public void accounting()
        {
            SqlConnection conn = new SqlConnection(_connString);
            SqlCommand cmd = new SqlCommand();

            string sql = " SELECT plancuentas.NUMPLANCUENTA, plancuentas.DESPLANCUENTA, plancuentas.TIPOCUENTA, plancuentas.NIVELCUENTA, plancuentas.ASENTABLE, plancuentas.DESTIPOCUENTA, plancuentas.REGLA, plancuentas.REGLAFK, CENTROCOSTO.DESCENTRO, CENTROCOSTO.ENLAZADO, CLIENTES.NOMBRE AS CUSTOMER, "
                        + " PROVEEDOR.NOMBRE AS SUPPLIER, CAJA.NUMEROCAJA, IVA.DESIVA"
                        + " FROM IVA RIGHT OUTER JOIN"
                        + " plancuentas ON IVA.CODIVA = plancuentas.REGLAFK LEFT OUTER JOIN"
                        + " CAJA ON plancuentas.REGLAFK = CAJA.NUMCAJA LEFT OUTER JOIN"
                        + " PROVEEDOR ON plancuentas.REGLAFK = PROVEEDOR.CODPROVEEDOR LEFT OUTER JOIN"
                        + " CLIENTES ON plancuentas.REGLAFK = CLIENTES.CODCLIENTE LEFT OUTER JOIN"
                        + " CENTROCOSTO ON plancuentas.REGLAFK = CENTROCOSTO.CODCENTRO"
                        + " ORDER BY plancuentas.NUMPLANCUENTA";

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            DataTable dt = exeDT(sql);
          //  SqlDataReader chart_reader = cmd.ExecuteReader();

            Dispatcher.BeginInvoke((Action)(() => progAccounting.IsIndeterminate = true));

            foreach (DataRow chart_reader in dt.Rows)
            {
                
            
                using (db db = new db())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    accounting_chart accounting_chart = new accounting_chart();
                    accounting_chart.id_user = db.security_user.Where(x => x.id_company == id_company).FirstOrDefault().id_user;
                    accounting_chart.id_company = id_company;
                    accounting_chart.is_active = true;

                    if ((chart_reader["DESPLANCUENTA"] is DBNull))
                    { continue; }
                    else
                    { accounting_chart.name = (string)chart_reader["DESPLANCUENTA"]; }

                    if ((chart_reader["NUMPLANCUENTA"] is DBNull))
                    { continue;  }
                    else
                    { accounting_chart.code = (string)chart_reader["NUMPLANCUENTA"]; }

                    //Find Parent Account
                    string code = accounting_chart.code;
                    var pos = code.LastIndexOf('.');
                    if (pos >= 0)
                    {
                        code = accounting_chart.code.Substring(0, pos);
                        accounting_chart.parent = db.accounting_chart.Where(p => p.code == code
                                                                              && p.id_account == accounting_chart.id_account
                                                                           ).FirstOrDefault();
                    }

                    //Tipo de Cuenta
                    if (!(chart_reader[2] is DBNull))
                    {
                        if (Convert.ToInt16(chart_reader["TIPOCUENTA"]) == 1)
                        {
                            accounting_chart.chart_type = accounting_chart.ChartType.Assets;
                        }
                        else if (Convert.ToInt16(chart_reader["TIPOCUENTA"]) == 2)
                        {
                            accounting_chart.chart_type = accounting_chart.ChartType.Liability;
                        }
                        else if (Convert.ToInt16(chart_reader["TIPOCUENTA"]) == 3)
                        {
                            accounting_chart.chart_type = accounting_chart.ChartType.Equity;
                        }
                        else if (Convert.ToInt16(chart_reader["TIPOCUENTA"]) == 4)
                        {
                            accounting_chart.chart_type = accounting_chart.ChartType.Revenue;
                        }
                        else if (Convert.ToInt16(chart_reader["TIPOCUENTA"]) == 5)
                        {
                            accounting_chart.chart_type = accounting_chart.ChartType.Expenses;
                        }
                    }

                    accounting_chart.is_generic = true;

                    // Linking
                    if (chart_reader[4].ToString() == "Si")
                    {
                        if (Convert.ToInt16(chart_reader["REGLA"]) == 1)
                        {

                        }
                        else if (Convert.ToInt16(chart_reader["REGLA"]) == 2)
                        {
                            //Fix issue.
                        }
                        //ACCOUNTS RECEIVABLE
                        else if (Convert.ToInt16(chart_reader["REGLA"]) == 3)
                        {
                            accounting_chart.chartsub_type = accounting_chart.ChartSubType.AccountsReceivable;
                            string _supplier = Convert.ToString(chart_reader["SUPPLIER"]);
                            if (Convert.ToInt16(chart_reader["REGLAFK"]) != 0 && _supplier != null)
                            {
                                accounting_chart.contact = db.contacts.Where(c => c.name == _supplier).FirstOrDefault();
                                accounting_chart.is_generic = false;
                            }
                        }
                        //ACCOUNTS PAYABLE
                        else if (Convert.ToInt16(chart_reader["REGLA"]) == 4)
                        {
                            accounting_chart.chartsub_type = accounting_chart.ChartSubType.AccountsPayable;
                            string _customer = Convert.ToString(chart_reader["CUSTOMER"]);
                            if (Convert.ToInt16(chart_reader["REGLAFK"]) != 0 && _customer != null)
                            {
                                accounting_chart.contact = db.contacts.Where(c => c.name == _customer).FirstOrDefault();
                                accounting_chart.is_generic = false;
                            }
                        }
                        //VALUE ADDED TAX
                        else if (Convert.ToInt16(chart_reader["REGLA"]) == 5)
                        {
                            accounting_chart.chartsub_type = accounting_chart.ChartSubType.VAT;
                            string _iva = Convert.ToString(chart_reader["DESIVA"]);
                            if (Convert.ToInt16(chart_reader["REGLAFK"]) != 0 && _iva != null)
                            {
                                accounting_chart.app_vat = db.app_vat.Where(c => c.name == _iva).FirstOrDefault();
                                accounting_chart.is_generic = false;
                            }
                        }
                        //VALUE ADDED TAX
                        else if (Convert.ToInt16(chart_reader["REGLA"]) == 6)
                        {
                            accounting_chart.chartsub_type = accounting_chart.ChartSubType.Revenue;
                        }
                        else if (Convert.ToInt16(chart_reader["REGLA"]) == 7)
                        {

                        }
                        //CASH ACCOUNTS
                        else if (Convert.ToInt16(chart_reader["REGLA"]) == 8)
                        {
                            accounting_chart.chartsub_type = accounting_chart.ChartSubType.Cash;
                            string _account_name = Convert.ToString(chart_reader["NUMEROCAJA"]);
                            if (Convert.ToInt16(chart_reader["REGLAFK"]) != 0 && _account_name != null)
                            {
                                accounting_chart.app_account = db.app_account.Where(c => c.name == _account_name).FirstOrDefault();
                                accounting_chart.is_generic = false;
                            }
                        }
                        else if (Convert.ToInt16(chart_reader["REGLA"]) == 9)
                        {

                        }
                        else if (Convert.ToInt16(chart_reader["REGLA"]) == 10)
                        {

                        }
                        //COST CENTER
                        else if (Convert.ToInt16(chart_reader["REGLA"]) == 11)
                        {
                            if (Convert.ToString(chart_reader["ENLAZADO"]) != null)
                            {
                                //INVENTORY COST CENTER
                                accounting_chart.chartsub_type = accounting_chart.ChartSubType.Inventory;
                            }
                            else
                            {
                                //EXPENSE COST CENTER
                                accounting_chart.chartsub_type = accounting_chart.ChartSubType.AdministrationExpense;
                                //add small logic to seperate stockable from expense. 
                                string _cost_center = Convert.ToString(chart_reader["DESCENTRO"]);
                                if (Convert.ToInt16(chart_reader["REGLAFK"]) != 0 && _cost_center != null)
                                {
                                    accounting_chart.app_cost_center = db.app_cost_center.Where(c => c.name == _cost_center).FirstOrDefault();
                                    accounting_chart.is_generic = false;
                                }
                            }
                        }
                    }
                    if (accounting_chart.Error == null)
                    {
                        db.accounting_chart.Add(accounting_chart);
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
            }
            //chart_reader.Close();
            dt.Clear();
            cmd.Dispose();
            conn.Close();

            string sql_periodofiscal = " SELECT * from periodofiscal ";

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_periodofiscal;
            cmd.CommandType = CommandType.Text;
            SqlDataReader periodofiscal = cmd.ExecuteReader();
            while (periodofiscal.Read())
            {
                using (db db = new db())
                {
                    accounting_cycle accounting_cycle = new accounting_cycle();
                    accounting_cycle.id_user = db.security_user.Where(x => x.id_company == id_company).FirstOrDefault().id_user;
                    accounting_cycle.start_date = Convert.ToDateTime(periodofiscal["FECHAINICIO"]);
                    accounting_cycle.end_date = Convert.ToDateTime(periodofiscal["FECHAFIN"]);
                    accounting_cycle.name = Convert.ToString(periodofiscal["DESEJERCICIO"]);
                    accounting_cycle.id_cycle = Convert.ToInt32(periodofiscal["CODPERIODOFISCAL"]);
                    if (periodofiscal["ESTADO"] is DBNull)
                    {
                        accounting_cycle.is_active = false;
                    }
                    else
                    {
                        accounting_cycle.is_active = Convert.ToBoolean(periodofiscal["ESTADO"]);
                    }
                    db.accounting_cycle.Add(accounting_cycle);
                    try
                    {
                        IEnumerable<DbEntityValidationResult> validationresult = dbContext.GetValidationErrors();
                        if (validationresult.Count() == 0)
                        {
                            db.SaveChanges();
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }

            }
            periodofiscal.Close();
    
            //cmd.Dispose();
            conn.Close();
            Dispatcher.BeginInvoke((Action)(() => progAccounting.IsIndeterminate = false));
   
            string sql_journal = " SELECT plancuentas.DESPLANCUENTA,periodofiscal.CODPERIODOFISCAL, periodofiscal.DESEJERCICIO, ASIENTOS.CODMONEDA,ASIENTOS.ModuloID, ASIENTOS.NUMASIENTO, ASIENTOS.FECHAASIENTO, ASIENTOS.COTIZACION, ASIENTOS.DESCRIPCION, ASIENTOS.IMPORTED, ASIENTOS.IMPORTEH, ASIENTOS.NUMCOMPROBANTE, ASIENTOS.DETALLE, ASIENTOS.TIMBRADO, SUCURSAL.DESSUCURSAL"
                               + " FROM ASIENTOS INNER JOIN"
                               + " plancuentas ON ASIENTOS.CODPLANCUENTA = plancuentas.CODPLANCUENTA INNER JOIN"
                               + " periodofiscal ON ASIENTOS.CODPERIODOFISCAL = periodofiscal.CODPERIODOFISCAL INNER JOIN"
                               + " SUCURSAL ON ASIENTOS.CODSUCURSAL = SUCURSAL.CODSUCURSAL";
            SqlDataReader chart_journal;

            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = sql_journal;
                cmd.CommandType = CommandType.Text;
                chart_journal = cmd.ExecuteReader();
                DataTable dtjournal = exeDT(sql_journal);
                int count = dtjournal.Rows.Count;

               
                int value = 0;
                Dispatcher.BeginInvoke((Action)(() => accountingMaximum.Text = count.ToString()));
                Dispatcher.BeginInvoke((Action)(() => accountingValue.Text = value.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progAccounting.Maximum = count));
                Dispatcher.BeginInvoke((Action)(() => progAccounting.Value = value));

                while (chart_journal.Read())
                {
                   
                    using (db db = new db())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        accounting_journal accounting_journal;
                        int _code = 0;

                        if (!(chart_journal["NUMASIENTO"] is DBNull))
                        {
                            _code = Convert.ToInt16(chart_journal["NUMASIENTO"]);

                        }
                         int id=Convert.ToInt32(chart_journal["CODPERIODOFISCAL"]);
                         int id_cycle = 0;
                        if (db.accounting_cycle.Where(x=>x.id_cycle==id).FirstOrDefault()!=null)
                        {
                            id_cycle = Convert.ToInt32(chart_journal["CODPERIODOFISCAL"]);
                        }
                        else
                        {
                            id_cycle = db.accounting_cycle.FirstOrDefault().id_cycle;
                        }
                        try
                        {
                            if (db.accounting_journal.Where(i => i.code == _code && i.id_company == id_company && i.id_cycle == id_cycle).Count() == 0)
                            {

                                accounting_journal = new accounting_journal();
                                accounting_journal.id_cycle = id_cycle;
                                accounting_journal.code = _code;
                                accounting_journal.id_company = id_company;
                                string branch_name = Convert.ToString(chart_journal["DESSUCURSAL"]);
                                accounting_journal.app_branch = db.app_branch.Where(i => i.name == branch_name && i.id_company == id_company).FirstOrDefault();
                                accounting_journal.trans_date = Convert.ToDateTime(chart_journal["FECHAASIENTO"]);
                                accounting_journal.comment = Convert.ToString(chart_journal["DESCRIPCION"]); 
                                accounting_journal_detail accounting_journal_detail = create_Detail(ref accounting_journal, chart_journal);
                                if (accounting_journal_detail != null)
                                {
                                    accounting_journal.accounting_journal_detail.Add(accounting_journal_detail);
                                }
                                db.accounting_journal.Add(accounting_journal);
                            }
                            else
                            {
                                accounting_journal = db.accounting_journal.Where(i => i.code == _code && i.id_cycle == id_cycle).FirstOrDefault();
                                accounting_journal.trans_date = Convert.ToDateTime(chart_journal["FECHAASIENTO"]);
                                accounting_journal_detail accounting_journal_detail = create_Detail(ref accounting_journal, chart_journal);
                                if (accounting_journal_detail != null)
                                {
                                    db.accounting_journal_detail.Add(accounting_journal_detail);
                                }
                            }
                            try
                            {
                                IEnumerable<DbEntityValidationResult> validationresult = db.GetValidationErrors();
                                if (validationresult.Count() == 0)
                                {
                                    db.SaveChanges();
                                   value += 1;
                                   Dispatcher.BeginInvoke((Action)(() => progAccounting.Value = value));
                                   Dispatcher.BeginInvoke((Action)(() => accountingValue.Text = value.ToString()));
                                }
                            }
                            catch
                            {
                                throw;
                            }
                        }
                        catch
                        {
                            throw; 
                        }
                        
                    }
                }
            }
            catch
            {
                throw;
            }

            Dispatcher.BeginInvoke((Action)(() => progAccounting.IsIndeterminate = false));

        }

        private accounting_journal_detail create_Detail(ref accounting_journal accounting_journal, SqlDataReader chart_reader)
        {
            accounting_journal_detail accounting_journal_detail = new accounting_journal_detail();

            using (db db = new db())
            {
                String DESPLANCUENTA= Convert.ToString(chart_reader["DESPLANCUENTA"]);
                if (db.accounting_chart.Where(x => x.name == DESPLANCUENTA).FirstOrDefault() == null)
                {
                    return null;
                }
                accounting_journal_detail.id_chart = db.accounting_chart.Where(x => x.name == DESPLANCUENTA).FirstOrDefault().id_chart;

                int COTIZACION = Convert.ToInt32(chart_reader["COTIZACION"]);
                if (db.app_currencyfx.Where(x => x.buy_value == COTIZACION).FirstOrDefault() == null)
                {
                    return null;
                }
                accounting_journal_detail.id_currencyfx = db.app_currencyfx.Where(x => x.buy_value == COTIZACION).FirstOrDefault().id_currencyfx;
            }

            int ModuloID = Convert.ToInt16(chart_reader["ModuloID"]);
            if (ModuloID == 3)
            {
                accounting_journal_detail.id_application = entity.App.Names.PurchaseInvoice;
            }
            else if (ModuloID == 4)
            {
                accounting_journal_detail.id_application = entity.App.Names.AccountsPayable;
            }
            else if (ModuloID == 8)
            {
                accounting_journal_detail.id_application = entity.App.Names.SalesInvoice;
            }
            else if (ModuloID == 26)
            {
                accounting_journal_detail.id_application = entity.App.Names.PaymentUtility;
            }

            accounting_journal_detail.is_head = true;
            accounting_journal_detail.credit = Convert.ToDecimal(chart_reader["IMPORTEH"]);
            accounting_journal_detail.debit = Convert.ToDecimal(chart_reader["IMPORTED"]);
            accounting_journal_detail.accounting_journal = accounting_journal;
            accounting_journal_detail.trans_date = accounting_journal.trans_date;
            return accounting_journal_detail;
        }

    }
}