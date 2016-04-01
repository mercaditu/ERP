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
        public List<purchase_invoice> purchase_invoice_ErrorList = new List<purchase_invoice>();
        public void purchase()
        {
            try
            {
                string sql = " SELECT " +
                " COMPRAS.CODCOMPRA, " +        //0
                " COMPRAS.NUMCOMPRA, " +        //1
                " COMPRAS.FECHACOMPRA, " +      //2
                " COMPRAS.TOTALDESCUENTO, " +   //3
                " COMPRAS.TOTALEXENTA, " +      //4
                " COMPRAS.TOTALGRAVADA, " +     //5
                " COMPRAS.TOTALIVA, " +         //6
                " COMPRAS.MODALIDADPAGO, " +    //7
                " COMPRAS.FECGRA, " +           //8
                " COMPRAS.ESTADO, " +           //9
                " COMPRAS.MOTIVOANULADO, " +    //10
                " COMPRAS.FECHACOMPRA, " +      //11
                " COMPRAS.TIMBRADOPROV, " +     //12
                " COMPRAS.TOTALIVA5, " +        //13
                " COMPRAS.TOTALIVA10, " +       //14
                " COMPRAS.METODO, " +           //15
                " COMPRAS.TOTALGRAVADO5, " +    //16
                " COMPRAS.TOTALGRAVADO10, " +   //17
                " COMPRAS.ASENTADO, " +         //18
                " COMPRAS.TOTALCOMPRA, " +      //19
                " PROVEEDOR.NOMBRE, " +         //20
                " PROVEEDOR.RUC_CIN, " +        //21
                " COMPRAS.COTIZACION1 " +       //22
                " FROM  COMPRAS RIGHT OUTER JOIN " +
                " PROVEEDOR ON COMPRAS.CODPROVEEDOR = PROVEEDOR.CODPROVEEDOR";

                SqlConnection conn = new SqlConnection(_connString);

                //Counts Total number of Rows we have to process
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.CommandText = "SELECT COUNT(*) FROM COMPRAS";
                cmd.CommandType = CommandType.Text;
                int count = (int)cmd.ExecuteScalar();
                conn.Close();

                int value = 0;

                Dispatcher.BeginInvoke((Action)(() => purchaseMaximum.Text = count.ToString()));
                Dispatcher.BeginInvoke((Action)(() => purchaseValue.Text = value.ToString()));
                Dispatcher.BeginInvoke((Action)(() => progPurchase.Maximum = count));
                Dispatcher.BeginInvoke((Action)(() => progPurchase.Value = value));

                cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.CommandType = CommandType.Text;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    using (PurchaseInvoiceDB db = new PurchaseInvoiceDB())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        purchase_invoice purchase_invoice = db.New();

                        purchase_invoice.number = (reader.IsDBNull(1)) ? null : reader.GetString(1);
                        purchase_invoice.trans_date = reader.GetDateTime(2);

                        //Customer
                        if (!reader.IsDBNull(26))
                        {
                            string _customer = reader.GetString(26);
                            contact contact = db.contacts.Where(x => x.name == _customer && x.id_company == id_company).FirstOrDefault();
                            purchase_invoice.id_contact = contact.id_contact;
                        }

                        //Condition (Cash or Credit)
                        if (!reader.IsDBNull(13) && reader.GetByte(13) == 0)
                        {
                            app_condition app_condition = db.app_condition.Where(x => x.name == "Contado").FirstOrDefault();
                            purchase_invoice.id_condition = app_condition.id_condition;
                            //Contract...

                            app_contract_detail app_contract_detail = db.app_contract_detail.Where(x => x.app_contract.id_condition == purchase_invoice.id_condition && x.app_contract.id_company == id_company).FirstOrDefault();
                            if (app_contract_detail != null)
                            {
                                purchase_invoice.id_contract = app_contract_detail.id_contract;
                            }
                            else
                            {
                                app_contract app_contract = new app_contract();
                                app_contract.app_condition = app_condition;
                                app_contract.name = "0 Días";
                                app_contract_detail _app_contract_detail = new app_contract_detail();
                                _app_contract_detail.coefficient = 1;
                                _app_contract_detail.app_contract = app_contract;
                                _app_contract_detail.interval = 0;
                                db.app_contract_detail.Add(_app_contract_detail);
                                purchase_invoice.app_contract = app_contract;
                                purchase_invoice.id_contract = app_contract.id_contract;
                            }
                        }
                        else if (!reader.IsDBNull(13) && reader.GetByte(13) == 1)
                        {
                            app_condition app_condition = db.app_condition.Where(x => x.name == "Crédito" && x.id_company == id_company).FirstOrDefault();
                            purchase_invoice.id_condition = app_condition.id_condition;
                            //Contract...
                            if (!reader.IsDBNull(30))
                            {
                                DateTime _due_date = reader.GetDateTime(30);
                                int interval = (_due_date - purchase_invoice.trans_date).Days;
                                app_contract_detail app_contract_detail = db.app_contract_detail.Where(x => x.app_contract.id_condition == purchase_invoice.id_condition && x.app_contract.id_company == id_company && x.interval == interval).FirstOrDefault();
                                if (app_contract_detail != null)
                                {
                                    purchase_invoice.id_contract = app_contract_detail.id_contract;
                                }
                                else
                                {
                                    app_contract app_contract = new app_contract();
                                    app_contract.app_condition = app_condition;
                                    app_contract.name = interval.ToString() + " Días";
                                    app_contract_detail _app_contract_detail = new app_contract_detail();
                                    _app_contract_detail.coefficient = 1;
                                    _app_contract_detail.app_contract = app_contract;
                                    _app_contract_detail.interval = (short)interval;
                                    db.app_contract_detail.Add(_app_contract_detail);
                                    purchase_invoice.app_contract = app_contract;
                                    purchase_invoice.id_contract = app_contract.id_contract;
                                }
                            }
                        }

                        int id_location = 0;
                        //Branch
                        if (!reader.IsDBNull(28))
                        {
                            //Branch
                            string _branch = reader.GetString(28);
                            app_branch app_branch = db.app_branch.Where(x => x.name == _branch && x.id_company == id_company).FirstOrDefault();
                            purchase_invoice.id_branch = app_branch.id_branch;

                            //Location
                            id_location = db.app_location.Where(x => x.id_branch == app_branch.id_branch).FirstOrDefault().id_location;

                            //Terminal
                            purchase_invoice.id_terminal = db.app_terminal.Where(x => x.app_branch.id_branch == app_branch.id_branch).FirstOrDefault().id_terminal;
                        }


                        string _desMoneda = string.Empty;

                        //Sales Invoice Detail
                        string sqlDetail = "SELECT"
                        + " dbo.PRODUCTOS.DESPRODUCTO as ITEM_DESPRODUCTO," //0
                        + " dbo.COMPRASDETALLE.DESPRODUCTO," //1
                        + " dbo.COMPRASDETALLE.CANTIDADCOMPRA, " //2
                        + " dbo.COMPRASDETALLE.COSTOUNITARIO, " //3
                        + " dbo.COMPRASDETALLE.IVA, " //4
                        + " dbo.COMPRAS.COTIZACION1, " //5
                        + " dbo.MONEDA.DESMONEDA, " //6
                        + " FROM dbo.COMPRAS LEFT OUTER JOIN"
                        + " dbo.MONEDA ON dbo.COMPRAS.CODMONEDA = dbo.MONEDA.CODMONEDA LEFT OUTER JOIN"
                        + " dbo.COMPRASDETALLE ON dbo.COMPRAS.CODVENTA = dbo.COMPRASDETALLE.CODVENTA LEFT OUTER JOIN"
                        + " dbo.PRODUCTOS ON dbo.COMPRASDETALLE.CODPRODUCTO = dbo.PRODUCTOS.CODPRODUCTO"
                        + " WHERE (dbo.COMPRASDETALLE.CODCOMPRA = " + reader.GetInt32(0).ToString() + ")";

                        DataTable dt = exeDT(sqlDetail);
                        foreach (DataRow row in dt.Rows)
                        {
                            //db Related Insertion.
                            purchase_invoice.id_currencyfx = 1;

                            purchase_invoice_detail purchase_invoice_detail = new purchase_invoice_detail();

                            string _prod_Name = row["ITEM_DESPRODUCTO"].ToString();
                            item item = db.items.Where(x => x.name == _prod_Name && x.id_company == id_company).FirstOrDefault();
                            purchase_invoice_detail.id_item = item.id_item;
                            purchase_invoice_detail.item_description = row["DESPRODUCTO"].ToString();
                            purchase_invoice_detail.quantity = Convert.ToDecimal(row["CANTIDADCOMPRA"]);

                            purchase_invoice_detail.id_location = id_location;

                            string _iva = row["IVA"].ToString();
                            if (_iva == "10.00")
                            {
                                purchase_invoice_detail.id_vat_group = db.app_vat_group.Where(x => x.name == "10%").FirstOrDefault().id_vat_group;
                            }
                            else if (_iva == "5.00")
                            {
                                purchase_invoice_detail.id_vat_group = db.app_vat_group.Where(x => x.name == "5%").FirstOrDefault().id_vat_group;
                            }
                            else
                            {
                                if (db.app_vat_group.Where(x => x.name == "Excento").FirstOrDefault() != null)
                                {
                                    purchase_invoice_detail.id_vat_group = db.app_vat_group.Where(x => x.name == "Excento").FirstOrDefault().id_vat_group;
                                }
                            }

                            decimal cotiz1 = Convert.ToDecimal((row["COTIZACION1"] is DBNull) ? 1 : Convert.ToDecimal(row["COTIZACION1"]));
                            // purchase_invoice_detail.unit_price = (Convert.ToDecimal(row["PRECIOVENTANETO"]) / purchase_invoice_detail.quantity) / cotiz1;
                            purchase_invoice_detail.unit_cost = Convert.ToDecimal(row["COSTOUNITARIO"]);

                            //Commit Sales Invoice Detail
                            purchase_invoice.purchase_invoice_detail.Add(purchase_invoice_detail);
                        }

                        if (purchase_invoice.Error == null)
                        {
                            try
                            {
                                purchase_invoice.State = System.Data.Entity.EntityState.Added;
                                purchase_invoice.IsSelected = true;

                                // db.purchase_invoice.Add(purchase_invoice);
                                IEnumerable<DbEntityValidationResult> validationresult = db.GetValidationErrors();
                                if (validationresult.Count() == 0)
                                {
                                    db.SaveChanges();
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                            //Sales Brillo
                            //if (reader.GetInt32(10) == 1)
                            //{
                            //    entity.Brillo.Approve.SalesInvoice salesBrillo = new entity.Brillo.Approve.SalesInvoice();
                            //    salesBrillo.Start(ref db, sales_invoice);
                            //    sales_invoice.status = 0; ?????
                            //}
                            //else if (reader.GetInt32(10))
                            //{
                            //    entity.Brillo.Approve.SalesInvoice salesBrillo = new entity.Brillo.Approve.SalesInvoice();
                            //    salesBrillo.Start(ref db, sales_invoice);
                            //    entity.Brillo.Annul.SalesInvoice salesAnullBrillo = new entity.Brillo.Annul.SalesInvoice();
                            //    salesAnullBrillo.Start(ref db, sales_invoice);
                            //    sales_invoice.status = 0; ?????
                            //}

                            value += 1;
                            Dispatcher.BeginInvoke((Action)(() => progSales.Value = value));
                            Dispatcher.BeginInvoke((Action)(() => salesValue.Text = value.ToString()));
                        }
                        else
                        {
                            //Add code to include error contacts into
                            purchase_invoice_ErrorList.Add(purchase_invoice);
                        }
                    }
                }
                reader.Close();
                cmd.Dispose();
                conn.Close();

                _customer_Current = _customer_Max;
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }
    }
}
