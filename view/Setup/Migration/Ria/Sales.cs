using entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Cognitivo.Setup.Migration
{
    public partial class MigrationAssistant
    {
        public List<sales_invoice> SalesInvoice_ErrorList = new List<sales_invoice>();

        public void sales()
        {
            string sql = " SELECT "
                    + " VENTAS.CODVENTA, VENTAS.NUMVENTA, VENTAS.FECHAVENTA, VENTAS.PORCENTAJEDESCUENTO,"
                    + " VENTAS.TOTALEXENTA, VENTAS.TOTALGRAVADA, VENTAS.TOTALIVA, VENTAS.TOTALDESCUENTO,"
                    + " VENTAS.MODALIDADPAGO, VENTAS.FECGRA, VENTAS.ESTADO, VENTAS.MOTIVOANULADO,"
                    + " VENTAS.FECHAANULADO, VENTAS.TIPOVENTA, VENTAS.TIPOPRECIO, VENTAS.NUMVENTATIMBRADO,"
                    + " VENTAS.TOTAL5, VENTAS.TOTAL10, VENTAS.CODPRESUPUESTO, VENTAS.METODO, VENTAS.ENVIADO,"
                    + " VENTAS.TOTALGRAVADO5, VENTAS.TOTALGRAVADO10, VENTAS.ASENTADO,"
                    + " VENTAS.TOTALVENTA, VENDEDOR.DESVENDEDOR, CLIENTES.NOMBRE, CLIENTES.RUC,"
                    + " SUCURSAL.DESSUCURSAL, VENTAS.COTIZACION1, FACTURACOBRAR_1.FECHAVCTO,"
                    + " FACTURACOBRAR.FECHAVCTO AS Expr1, FACTURACOBRAR.SALDOCUOTA, FACTURACOBRAR.IMPORTECUOTA, "
                    + " FACTURACOBRAR.COTIZACION, VENTASFORMACOBRO.IMPORTE, VENTASFORMACOBRO.DESTIPOCOBRO,"
                    + " VENTASFORMACOBRO.NUMDEVOLUCION, VENTASFORMACOBRO.TIPOCOBRO"
                    + " FROM  SUCURSAL RIGHT OUTER JOIN"
                    + " FACTURACOBRAR RIGHT OUTER JOIN"
                    + " VENTAS ON FACTURACOBRAR.CODVENTA = VENTAS.CODVENTA LEFT OUTER JOIN"
                    + " VENTASFORMACOBRO ON VENTAS.CODVENTA = VENTASFORMACOBRO.CODVENTA ON SUCURSAL.CODSUCURSAL = VENTAS.CODSUCURSAL"
                    + " LEFT OUTER JOIN VENDEDOR ON VENTAS.CODVENDEDOR = VENDEDOR.CODVENDEDOR LEFT OUTER JOIN"
                    + " CLIENTES ON VENTAS.CODCLIENTE = CLIENTES.CODCLIENTE LEFT OUTER JOIN"
                    + " FACTURACOBRAR AS FACTURACOBRAR_1 ON VENTAS.CODVENTA = FACTURACOBRAR_1.CODVENTA";

            SqlConnection conn = new SqlConnection(_connString);

            //Counts Total number of Rows we have to process
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.CommandType = CommandType.Text;
            DataTable dt_sales = exeDT(sql);
            int count = (int)dt_sales.Rows.Count;
            conn.Close();

            int value = 0;

            Dispatcher.BeginInvoke((Action)(() => salesMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => salesValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progSales.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progSales.Value = value));

            //Sales Invoice Detail
            string sqlDetail = "SELECT"
            + " PRODUCTOS.DESPRODUCTO," //0
            + " VENTASDETALLE.CANTIDADVENTA," //1
            + " VENTASDETALLE.PRECIOVENTANETO, " //2
            + " VENTASDETALLE.PRECIOVENTALISTA, " //3
            + " VENTASDETALLE.COSTOPROMEDIO, " //4
            + " VENTASDETALLE.COSTOULTIMO, " //5
            + " VENTASDETALLE.IVA, " //6
            + " VENTAS.COTIZACION1, " //7
            + " MONEDA.DESMONEDA, " //8
            + " VENTASDETALLE.CODVENTA"
            + " FROM VENTAS LEFT OUTER JOIN"
            + " MONEDA ON VENTAS.CODMONEDA = MONEDA.CODMONEDA LEFT OUTER JOIN"
            + " VENTASDETALLE ON VENTAS.CODVENTA = VENTASDETALLE.CODVENTA LEFT OUTER JOIN"
            + " PRODUCTOS ON VENTASDETALLE.CODPRODUCTO = PRODUCTOS.CODPRODUCTO";

            DataTable dt_detail = exeDT(sqlDetail);

            int RoofValue = 1000;
            int FloorValue = 0;

            //Run a Foreach Lap
            for (int j = RoofValue; j < count; j = j + 1000)
            {
                for (int i = FloorValue; i < j; i++)
                {
                    using (SalesInvoiceDB db = new SalesInvoiceDB())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        List<entity.app_vat_group> VATGroupList = db.app_vat_group.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
                        List<entity.contact> ContactList = db.contacts.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
                        List<entity.sales_rep> sales_repList = db.sales_rep.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
                        List<entity.app_branch> BranchList = db.app_branch.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
                        List<entity.app_location> LocationList = db.app_location.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
                        List<entity.app_terminal> TerminalList = db.app_terminal.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
                        List<entity.item> ItemList = db.items.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
                        List<entity.app_currencyfx> app_currencyfxList = db.app_currencyfx.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

                        app_condition app_conditionCrédito = db.app_condition.Where(x => x.name == "Crédito" && x.id_company == id_company).FirstOrDefault();
                        app_condition app_conditionContado = db.app_condition.Where(x => x.name == "Contado" && x.id_company == id_company).FirstOrDefault();
                        app_currencyfx app_currencyfx = null;
                        if (app_currencyfxList.Where(x => x.is_active).FirstOrDefault() != null)
                        {
                            app_currencyfx = app_currencyfxList.Where(x => x.is_active).FirstOrDefault();
                        }

                        app_vat_group app_vat_group10 = VATGroupList.Where(x => x.name.Contains("10")).FirstOrDefault();
                        app_vat_group app_vat_group5 = VATGroupList.Where(x => x.name.Contains("5")).FirstOrDefault();
                        app_vat_group app_vat_group0 = VATGroupList.Where(x => x.name.Contains("0")).FirstOrDefault();

                        foreach (DataRow InnerRow in dt_sales.Select("CODVENTA > " + i + " AND CODVENTA < " + j + ""))
                        {
                            sales_invoice sales_invoice = new entity.sales_invoice();
                            sales_invoice.State = EntityState.Added;
                            sales_invoice.status = Status.Documents_General.Pending;
                            sales_invoice.IsSelected = true;
                            sales_invoice.trans_type = Status.TransactionTypes.Normal;
                            sales_invoice.trans_date = DateTime.Now.AddDays(0);
                            sales_invoice.timestamp = DateTime.Now;
                            sales_invoice.id_company = id_company;
                            sales_invoice.number = (InnerRow["NUMVENTA"] is DBNull) ? null : InnerRow["NUMVENTA"].ToString();

                            sales_invoice.trans_date = (InnerRow["FECHAVENTA"] is DBNull) ? DateTime.Now : Convert.ToDateTime(InnerRow["FECHAVENTA"]);

                            //Customer
                            if (!(InnerRow["NOMBRE"] is DBNull))
                            {
                                string _customer = InnerRow["NOMBRE"].ToString();
                                contact contact = ContactList.Where(x => x.name == _customer && x.id_company == id_company).FirstOrDefault();

                                if (contact != null)
                                {
                                    sales_invoice.id_contact = contact.id_contact;
                                    sales_invoice.contact = contact;
                                }
                            }

                            //Condition (Cash or Credit)
                            if (!(InnerRow["TIPOVENTA"] is DBNull) && Convert.ToByte(InnerRow["TIPOVENTA"]) == 0)
                            {
                                sales_invoice.id_condition = app_conditionContado.id_condition;
                                //Contract...

                                app_contract_detail app_contract_detail =
                                    db.app_contract_detail.Where(x => x.id_company == id_company &&
                                    x.app_contract.id_condition == app_conditionContado.id_condition)
                                        .FirstOrDefault();

                                if (app_contract_detail != null)
                                {
                                    sales_invoice.app_contract = app_contract_detail.app_contract;
                                    sales_invoice.id_contract = app_contract_detail.id_contract;
                                }
                                else
                                {
                                    app_contract app_contract = GenerateDefaultContrat(app_conditionContado, 0);
                                    db.app_contract.Add(app_contract);
                                    sales_invoice.app_contract = app_contract;
                                    sales_invoice.id_contract = app_contract.id_contract;
                                }
                            }
                            else if (!(InnerRow["TIPOVENTA"] is DBNull) && Convert.ToByte(InnerRow["TIPOVENTA"]) == 1)
                            {
                                sales_invoice.id_condition = app_conditionCrédito.id_condition;

                                //Contract...
                                if (!(InnerRow["FECHAVCTO"] is DBNull))
                                {
                                    DateTime _due_date = Convert.ToDateTime(InnerRow["FECHAVCTO"]);
                                    int interval = (_due_date - sales_invoice.trans_date).Days;

                                    app_contract_detail app_contract_detail =
                                        db.app_contract_detail.Where(x =>
                                            x.app_contract.id_condition == sales_invoice.id_condition &&
                                            x.app_contract.id_company == id_company &&
                                            x.interval == interval).FirstOrDefault();

                                    if (app_contract_detail != null)
                                    {
                                        sales_invoice.app_contract = app_contract_detail.app_contract;
                                        sales_invoice.id_contract = app_contract_detail.id_contract;
                                    }
                                    else
                                    {
                                        app_contract app_contract = GenerateDefaultContrat(app_conditionCrédito, interval);
                                        db.app_contract.Add(app_contract);
                                        sales_invoice.app_contract = app_contract;
                                        sales_invoice.id_contract = app_contract.id_contract;
                                    }
                                }
                                else
                                {
                                    if (db.app_contract.Where(x => x.name == "0 Días").Count() == 0)
                                    {
                                        app_contract app_contract = GenerateDefaultContrat(app_conditionCrédito, 0);
                                        db.app_contract.Add(app_contract);
                                        sales_invoice.app_contract = app_contract;
                                        sales_invoice.id_contract = app_contract.id_contract;
                                    }
                                    else
                                    {
                                        app_contract app_contract = db.app_contract.Where(x => x.name == "0 Días").FirstOrDefault();
                                        sales_invoice.app_contract = app_contract;
                                        sales_invoice.id_contract = app_contract.id_contract;
                                    }
                                }
                            }
                            else
                            {
                                sales_invoice.id_condition = app_conditionContado.id_condition;

                                if (db.app_contract.Where(x => x.name == "0 Días").Count() == 0)
                                {
                                    app_contract app_contract = GenerateDefaultContrat(app_conditionContado, 0);
                                    db.app_contract.Add(app_contract);
                                    sales_invoice.app_contract = app_contract;
                                    sales_invoice.id_contract = app_contract.id_contract;
                                }
                                else
                                {
                                    app_contract app_contract = db.app_contract.Where(x => x.name == "0 Días").FirstOrDefault();
                                    sales_invoice.app_contract = app_contract;
                                    sales_invoice.id_contract = app_contract.id_contract;
                                }
                            }

                            //Sales Rep
                            if (!(InnerRow["DESVENDEDOR"] is DBNull))
                            {
                                string _sales_rep = InnerRow["DESVENDEDOR"].ToString();
                                sales_rep sales_rep = sales_repList.Where(x => x.name == _sales_rep && x.id_company == id_company).FirstOrDefault();
                                sales_invoice.id_sales_rep = sales_rep.id_sales_rep;
                            }

                            int id_location = 0;
                            app_location app_location = null;

                            //Branch
                            if (!(InnerRow["DESSUCURSAL"] is DBNull))
                            {
                                //Branch
                                string _branch = InnerRow["DESSUCURSAL"].ToString();
                                app_branch app_branch = BranchList.Where(x => x.name == _branch && x.id_company == id_company).FirstOrDefault();
                                sales_invoice.id_branch = app_branch.id_branch;

                                //Location
                                if (LocationList.Where(x => x.id_branch == app_branch.id_branch && x.is_default).FirstOrDefault() != null)
                                {
                                    id_location = LocationList.Where(x => x.id_branch == app_branch.id_branch && x.is_default).FirstOrDefault().id_location;
                                    app_location = LocationList.Where(x => x.id_branch == app_branch.id_branch && x.is_default).FirstOrDefault();
                                }

                                //Terminal
                                sales_invoice.id_terminal = TerminalList.Where(x => x.app_branch.id_branch == app_branch.id_branch).FirstOrDefault().id_terminal;
                            }

                            if (app_currencyfx != null)
                            {
                                sales_invoice.id_currencyfx = app_currencyfx.id_currencyfx;
                                sales_invoice.app_currencyfx = app_currencyfx;
                            }

                            DataTable dt_CurrentDetail = new DataTable();
                            if (dt_detail.Select("CODVENTA =" + InnerRow[0].ToString()).Count() > 0)
                            {
                                dt_CurrentDetail = dt_detail.Select("CODVENTA =" + InnerRow[0].ToString()).CopyToDataTable();
                            }

                            foreach (DataRow row in dt_CurrentDetail.Rows)
                            {
                                //db Related Insertion.
                                sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();

                                string _prod_Name = row["DESPRODUCTO"].ToString();
                                item item = ItemList.Where(x => x.name == _prod_Name && x.id_company == id_company).FirstOrDefault();
                                sales_invoice_detail.id_item = item.id_item;
                                sales_invoice_detail.quantity = Convert.ToDecimal(row["CANTIDADVENTA"]);

                                if (id_location > 0)
                                {
                                    sales_invoice_detail.id_location = id_location;
                                    sales_invoice_detail.app_location = app_location;
                                }

                                string _iva = row["IVA"].ToString();
                                if (_iva == "10.00")
                                {
                                    if (app_vat_group10 != null)
                                    {
                                        sales_invoice_detail.id_vat_group = app_vat_group10.id_vat_group;
                                    }
                                }
                                else if (_iva == "5.00")
                                {
                                    if (app_vat_group5 != null)
                                    {
                                        sales_invoice_detail.id_vat_group = app_vat_group5.id_vat_group;
                                    }
                                }
                                else
                                {
                                    if (app_vat_group0 != null)
                                    {
                                        sales_invoice_detail.id_vat_group = app_vat_group0.id_vat_group;
                                    }
                                }

                                decimal cotiz1 = Convert.ToDecimal((row["COTIZACION1"] is DBNull) ? 1 : Convert.ToDecimal(row["COTIZACION1"]));
                                if (cotiz1 == 0)
                                {
                                    cotiz1 = 1;
                                }
                                decimal quantity = 1;
                                if (sales_invoice_detail.quantity > 0)
                                {
                                    quantity = sales_invoice_detail.quantity;
                                }
                                sales_invoice_detail.unit_price = (Convert.ToDecimal((row["PRECIOVENTANETO"] is DBNull) ? 1 : Convert.ToDecimal(row["PRECIOVENTANETO"])) / quantity) / cotiz1;
                                sales_invoice_detail.unit_cost = Convert.ToDecimal(row["COSTOPROMEDIO"]);

                                //Commit Sales Invoice Detail
                                sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
                            }

                            if (sales_invoice.Error == null)
                            {
                                sales_invoice.State = System.Data.Entity.EntityState.Added;
                                sales_invoice.IsSelected = true;
                                db.sales_invoice.Add(sales_invoice);

                                if (!(InnerRow["ESTADO"] is DBNull))
                                {
                                    int status = Convert.ToInt32(InnerRow["ESTADO"]);

                                    if (status == 0)
                                    {
                                        sales_invoice.status = Status.Documents_General.Pending;
                                    }
                                    else if (status == 1)
                                    {
                                        if (sales_invoice.app_branch.can_stock)
                                        {
                                            db.Approve(true);
                                        }
                                        else
                                        {
                                            db.Approve(false);
                                        }

                                        sales_invoice.State = System.Data.Entity.EntityState.Modified;
                                        sales_invoice.status = Status.Documents_General.Approved;
                                        sales_invoice.IsSelected = true;

                                        add_paymnet_detail(db, sales_invoice, InnerRow["SALDOCUOTA"], InnerRow["IMPORTE"]);
                                    }
                                    else if (status == 2)
                                    {
                                        sales_invoice.status = Status.Documents_General.Annulled;

                                        if (!(InnerRow["MOTIVOANULADO"] is DBNull))
                                        {
                                            sales_invoice.comment = InnerRow["MOTIVOANULADO"].ToString();
                                        }
                                    }

                                    db.SaveChanges();
                                    sales_invoice.IsSelected = false;
                                }
                            }
                            else
                            {
                                //Add code to include error contacts into
                                SalesInvoice_ErrorList.Add(sales_invoice);
                            }
                            // }
                            value += 1;
                            Dispatcher.BeginInvoke((Action)(() => progSales.Value = value));
                            Dispatcher.BeginInvoke((Action)(() => salesValue.Text = value.ToString()));
                        }
                        FloorValue = RoofValue;
                        //    RoofValue += 1000;
                        //}
                    }
                }
            }
        }

        public void add_paymnet_detail(db db, sales_invoice sales_invoice, object SALDOCUOTA, object IMPORTE)
        {
            if (!(SALDOCUOTA is DBNull))
            {
                decimal SALDOCUOTAValue = Convert.ToDecimal(SALDOCUOTA);
                payment_type payment_type;
                if (SALDOCUOTAValue < sales_invoice.GrandTotal)
                {
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

                    if (IMPORTE is DBNull)
                    {
                        if (sales_invoice.payment_schedual.FirstOrDefault() != null)
                        {
                            payment_schedual payment_schedual = sales_invoice.payment_schedual.FirstOrDefault();
                            decimal invoice_total = sales_invoice.GrandTotal;

                            if (invoice_total > 0)
                            {
                                payment_detail payment_detail = new payment_detail();
                                payment_detail.value = invoice_total;

                                if (db.app_account.Where(x => x.id_account_type == app_account.app_account_type.Terminal).FirstOrDefault() != null)
                                {
                                    app_account app_account = db.app_account.Where(x => x.id_account_type == app_account.app_account_type.Terminal).FirstOrDefault();
                                    payment_detail.id_account = app_account.id_account;
                                }
                                else
                                {
                                    app_account app_account = GenerateDefaultApp_Account();
                                    db.app_account.Add(app_account);
                                    payment_detail.app_account = app_account;
                                    payment_detail.id_account = app_account.id_account;
                                }

                                payment payment = new payment();

                                if (payment_schedual != null)
                                {
                                    payment.id_contact = payment_schedual.id_contact;
                                    payment.contact = payment_schedual.contact;
                                    payment_detail.id_currencyfx = payment_schedual.id_currencyfx;

                                    payment_detail.id_payment_type = payment_type.id_payment_type;
                                    payment_detail.payment_type = payment_type;
                                }

                                payment_detail.App_Name = global::entity.App.Names.SalesInvoice;

                                payment_schedual _payment_schedual = new payment_schedual();
                                _payment_schedual.credit = invoice_total;
                                _payment_schedual.parent = payment_schedual;
                                _payment_schedual.expire_date = payment_schedual.expire_date;
                                _payment_schedual.status = payment_schedual.status;
                                _payment_schedual.id_contact = payment_schedual.id_contact;
                                _payment_schedual.id_currencyfx = payment_schedual.id_currencyfx;
                                _payment_schedual.id_sales_invoice = payment_schedual.id_sales_invoice;
                                _payment_schedual.trans_date = payment_schedual.trans_date;
                                _payment_schedual.AccountReceivableBalance = invoice_total;

                                payment_detail.payment_schedual.Add(_payment_schedual);
                                payment.payment_detail.Add(payment_detail);

                                //Add Account Logic. With IF FUnction if payment type is Basic Behaviour. If not ignore.
                                app_account_detail app_account_detail = new app_account_detail();

                                if (db.app_account_session.Where(x => x.id_account == payment_detail.id_account && x.is_active).FirstOrDefault() != null)
                                {
                                    app_account_detail.id_session = db.app_account_session.Where(x => x.id_account == payment_detail.id_account && x.is_active).FirstOrDefault().id_session;
                                }

                                app_account_detail.id_account = (int)payment_detail.id_account;
                                app_account_detail.id_currencyfx = payment_schedual.id_currencyfx;
                                app_account_detail.id_payment_type = payment_detail.id_payment_type;
                                app_account_detail.payment_type = payment_type;
                                app_account_detail.debit = 0;
                                app_account_detail.credit = Convert.ToDecimal(payment_detail.value);
                                db.app_account_detail.Add(app_account_detail);

                                db.payments.Add(payment);
                            }
                        }
                    }
                }
            }
        }
    }
}