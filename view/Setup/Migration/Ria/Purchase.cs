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
            " dbo.SUCURSAL.DESSUCURSAL, " +//28
            " COMPRAS.COTIZACION1, " +
            " dbo.FACTURAPAGAR.FECHAVCTO" +
            " FROM  COMPRAS RIGHT OUTER JOIN " +
            " PROVEEDOR ON COMPRAS.CODPROVEEDOR = PROVEEDOR.CODPROVEEDOR" +
            " LEFT OUTER JOIN FACTURAPAGAR ON COMPRAS.CODCOMPRA = FACTURAPAGAR.CODCOMPRA" +
             " RIGHT OUTER JOIN SUCURSAL ON COMPRAS.CODSUCURSAL = SUCURSAL.CODSUCURSAL";

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
            DataTable dt_purchase = exeDT(sql);
            //SqlDataReader reader = cmd.ExecuteReader();

            foreach (DataRow purchaserow in dt_purchase.Rows)
            {
                using (PurchaseInvoiceDB db = new PurchaseInvoiceDB())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    purchase_invoice purchase_invoice = db.New(0);

                    purchase_invoice.number = purchaserow["NUMCOMPRA"] is DBNull ? null : purchaserow["NUMCOMPRA"].ToString();
                    if (!(purchaserow["FECHACOMPRA"] is DBNull))
                    {
                        purchase_invoice.trans_date = Convert.ToDateTime(purchaserow["FECHACOMPRA"]);
                    }
                    else
                    {
                        continue;
                    }

                    //Supplier
                    if (!(purchaserow["NOMBRE"] is DBNull))
                    {
                        string _customer = purchaserow["NOMBRE"].ToString();
                        contact contact = db.contacts.Where(x => x.name == _customer && x.id_company == id_company).FirstOrDefault();
                        purchase_invoice.contact = contact;
                        purchase_invoice.id_contact = contact.id_contact;
                    }

                    //Condition (Cash or Credit)
                    if (!(purchaserow["MODALIDADPAGO"] is DBNull) && Convert.ToInt32(purchaserow["MODALIDADPAGO"]) == 0)
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
                            app_contract app_contract = GenerateDefaultContrat(app_condition, 0);
                            db.app_contract.Add(app_contract);

                            purchase_invoice.app_contract = app_contract;
                            purchase_invoice.id_contract = app_contract.id_contract;
                        }
                    }
                    else if (!(purchaserow["MODALIDADPAGO"] is DBNull) && Convert.ToInt32(purchaserow["MODALIDADPAGO"]) == 1)
                    {
                        app_condition app_condition = db.app_condition.Where(x => x.name == "Crédito" && x.id_company == id_company).FirstOrDefault();
                        purchase_invoice.id_condition = app_condition.id_condition;
                        //Contract...
                        if (!(purchaserow["FECHAVCTO"] is DBNull))
                        {
                            DateTime _due_date = Convert.ToDateTime(purchaserow["FECHAVCTO"]);
                            int interval = (_due_date - purchase_invoice.trans_date).Days;
                            app_contract_detail app_contract_detail = db.app_contract_detail.Where(x => x.app_contract.id_condition == purchase_invoice.id_condition && x.app_contract.id_company == id_company && x.interval == interval).FirstOrDefault();
                            if (app_contract_detail != null)
                            {
                                purchase_invoice.id_contract = app_contract_detail.id_contract;
                            }
                            else
                            {
                                app_contract app_contract = GenerateDefaultContrat(app_condition, interval);
                                db.app_contract.Add(app_contract);

                                purchase_invoice.app_contract = app_contract;
                                purchase_invoice.id_contract = app_contract.id_contract;
                            }
                        }
                    }

                    int id_location = 0;
                    //Branch
                    if (!(purchaserow["DESSUCURSAL"] is DBNull))
                    {
                        //Branch
                        string _branch = purchaserow["DESSUCURSAL"].ToString();
                        app_branch app_branch = db.app_branch.Where(x => x.name == _branch && x.id_company == id_company).FirstOrDefault();
                        purchase_invoice.id_branch = app_branch.id_branch;

                        //Location
                        id_location = db.app_location.Where(x => x.id_branch == app_branch.id_branch && x.is_default).FirstOrDefault().id_location;

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
                    + " dbo.MONEDA.DESMONEDA " //6
                    + " FROM dbo.COMPRAS LEFT OUTER JOIN"
                    + " dbo.MONEDA ON dbo.COMPRAS.CODMONEDA = dbo.MONEDA.CODMONEDA LEFT OUTER JOIN"
                    + " dbo.COMPRASDETALLE ON dbo.COMPRAS.CODCOMPRA = dbo.COMPRASDETALLE.CODCOMPRA LEFT OUTER JOIN"
                    + " dbo.PRODUCTOS ON dbo.COMPRASDETALLE.CODPRODUCTO = dbo.PRODUCTOS.CODPRODUCTO"
                    + " WHERE (dbo.COMPRASDETALLE.CODCOMPRA = " + purchaserow["CODCOMPRA"].ToString() + ")";

                    DataTable dt = exeDT(sqlDetail);
                    foreach (DataRow row in dt.Rows)
                    {
                        //db Related Insertion.
                        purchase_invoice.id_currencyfx = 1;

                        purchase_invoice_detail purchase_invoice_detail = new purchase_invoice_detail();

                        string _prod_Name = row["ITEM_DESPRODUCTO"].ToString();
                        if (db.items.Where(x => x.name == _prod_Name && x.id_company == id_company).FirstOrDefault() != null)
                        {
                            //Only if Item Exists
                            item item = db.items.Where(x => x.name == _prod_Name && x.id_company == id_company).FirstOrDefault();
                            purchase_invoice_detail.id_item = item.id_item;
                        }

                        if (row["DESPRODUCTO"] is DBNull)
                        {
                            //If not Item Description, then just continue out of this loop.
                            continue;
                        }

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

                        if (row["COSTOUNITARIO"] is DBNull)
                        {
                            purchase_invoice_detail.unit_cost = 0;
                        }
                        else
                        {
                            purchase_invoice_detail.unit_cost = Convert.ToDecimal(row["COSTOUNITARIO"]);
                        }
                        //Commit Sales Invoice Detail
                        purchase_invoice.purchase_invoice_detail.Add(purchase_invoice_detail);
                    }

                    if (purchase_invoice.Error == null)
                    {
                        purchase_invoice.State = System.Data.Entity.EntityState.Added;
                        purchase_invoice.IsSelected = true;

                        db.purchase_invoice.Add(purchase_invoice);
                        IEnumerable<DbEntityValidationResult> validationresult = db.GetValidationErrors();
                        if (validationresult.Count() == 0)
                        {
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                System.Windows.Forms.MessageBox.Show(ex.ToString());
                            }
                        }

                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progPurchase.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => purchaseValue.Text = value.ToString()));
                    }
                    else
                    {
                        //Add code to include error contacts into
                        purchase_invoice_ErrorList.Add(purchase_invoice);
                    }
                }
            }
            // reader.Close();
            cmd.Dispose();
            conn.Close();

            _customer_Current = _customer_Max;
        }
    }
}