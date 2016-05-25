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
        public List<sales_return> sales_return_ErrorList = new List<sales_return>();

        public void salesReturn()
        {
            string sql = " SELECT "
                       + " DEVOLUCION.CODDEVOLUCION,"
                       + " CLIENTES.NOMBRE,"
                       + " VENTAS.NUMVENTA,"
                       + " DEVOLUCION.CODCOMPROBANTE,"
                       + " DEVOLUCION.CODVENTA,"
                       + " DEVOLUCION.NUMDEVOLUCION,"
                       + " DEVOLUCION.FECHADEVOLUCION, "
                       + " DEVOLUCION.TOTALEXENTA,"
                       + " DEVOLUCION.TOTALGRAVADA,"
                       + " DEVOLUCION.TOTALIVA,"
                       + " DEVOLUCION.TOTALDEVOLUCION,"
                       + " DEVOLUCION.COTIZACION1,"
                       + " DEVOLUCION.FECGRA,"
                       + " DEVOLUCION.CODVENDEDOR, "
                       + " DEVOLUCION.CODCOMPROBANTERECP,"
                       + " DEVOLUCION.COBRADO,"
                       + " DEVOLUCION.CODDEPOSITO,"
                       + " DEVOLUCION.ESTADO,"
                       + " DEVOLUCION.MOTIVOANULADO,"
                       + " DEVOLUCION.MOTIVODESCARTE, "
                       + " DEVOLUCION.TOTALIVA5,"
                       + " DEVOLUCION.TOTALIVA10,"
                       + " DEVOLUCION.TIPODEVOLUCION,"
                       + " DEVOLUCION.DESCONTARMONTO,"
                       + " MONEDA.DESMONEDA,"
                       + " SUCURSAL.DESSUCURSAL,"
                       + " DEVOLUCION.SALDO, "
                       + " VENDEDOR.DESVENDEDOR"
                       + " FROM DEVOLUCION INNER JOIN"
                       + " CLIENTES ON DEVOLUCION.CODCLIENTE = CLIENTES.CODCLIENTE INNER JOIN"
                       + " MONEDA ON DEVOLUCION.CODMONEDA = MONEDA.CODMONEDA INNER JOIN"
                       + " SUCURSAL ON DEVOLUCION.CODSUCURSAL = SUCURSAL.CODSUCURSAL LEFT OUTER JOIN"
                       + " VENDEDOR ON DEVOLUCION.CODVENDEDOR = VENDEDOR.CODVENDEDOR LEFT OUTER JOIN"
                       + " VENTAS ON CLIENTES.CODCLIENTE = VENTAS.CODCLIENTE AND DEVOLUCION.CODVENTA = VENTAS.CODVENTA ";


            SqlConnection conn = new SqlConnection(_connString);

            //Counts Total number of Rows we have to process
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.CommandText = "SELECT COUNT(*) FROM DEVOLUCION ";
            cmd.CommandType = CommandType.Text;
            int count = (int)cmd.ExecuteScalar();
            conn.Close();

            int value = 0;

            Dispatcher.BeginInvoke((Action)(() => salesReturnMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => salesReturnValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progSalesReturn.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progSalesReturn.Value = value));

            cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.CommandType = CommandType.Text;
            //SqlDataReader reader = cmd.ExecuteReader();
            DataTable dt_salesReturn = exeDT(sql);
            foreach (DataRow reader in dt_salesReturn.Rows)
            {
                using (SalesReturnDB db = new SalesReturnDB())
                {

               
                    db.Configuration.AutoDetectChangesEnabled = false;

                    sales_return sales_return = db.New();
                    sales_return.id_company = id_company;

                    //if ((reader[6] is DBNull))
                    //{
                    //    sales_invoice.is_accounted = false;
                    //}
                    //else
                    //{
                    //    sales_invoice.is_accounted = (Convert.ToByte(reader[23]) == 0) ? false : true;
                    //}


                    //sales_invoice.version = 1;

                    sales_return.number = (reader["NUMDEVOLUCION"] is DBNull) ? null : reader["NUMDEVOLUCION"].ToString();
                    sales_return.trans_date = Convert.ToDateTime(reader["FECHADEVOLUCION"]);

                    //Customer
                    if (!(reader["NOMBRE"] is DBNull))
                    {
                        string _customer = reader["NOMBRE"].ToString();
                        contact contact = db.contacts.Where(x => x.name == _customer && x.id_company == id_company).FirstOrDefault();
                        if (contact != null)
                        {

                            sales_return.id_contact = contact.id_contact;
                            sales_return.contact = contact;
                        }


                    }

                    //Condition (Cash or Credit)

                    app_condition app_condition = db.app_condition.Where(x => x.name == "Contado").FirstOrDefault();
                    sales_return.id_condition = app_condition.id_condition;
                    if (db.app_contract.Where(x => x.name == "0 Días").Count() == 0)
                    {
                        app_contract app_contract = GenerateDefaultContrat(app_condition, 0);
                        db.app_contract.Add(app_contract);
                        sales_return.app_contract = app_contract;
                        sales_return.id_contract = app_contract.id_contract;
                    }
                    else
                    {
                        app_contract app_contract = db.app_contract.Where(x => x.name == "0 Días").FirstOrDefault();
                        sales_return.app_contract = app_contract;
                        sales_return.id_contract = app_contract.id_contract;
                    }




                    int id_location = 0;
                    app_location app_location = null;

                    //Branch
                    if (!(reader["DESSUCURSAL"] is DBNull))
                    {
                        //Branch
                        string _branch = reader["DESSUCURSAL"].ToString();
                        app_branch app_branch = db.app_branch.Where(x => x.name == _branch && x.id_company == id_company).FirstOrDefault();
                        sales_return.id_branch = app_branch.id_branch;

                        //Location
                        id_location = db.app_location.Where(x => x.id_branch == app_branch.id_branch && x.is_default).FirstOrDefault().id_location;
                        app_location = db.app_location.Where(x => x.id_branch == app_branch.id_branch && x.is_default).FirstOrDefault();
                        //Terminal
                        sales_return.id_terminal = db.app_terminal.Where(x => x.app_branch.id_branch == app_branch.id_branch).FirstOrDefault().id_terminal;
                    }

                    if (!(reader["NUMVENTA"] is DBNull))
                    {
                        string _salesNumber = reader["NUMVENTA"].ToString();
                        sales_invoice sales_invoice = db.sales_invoice.Where(x => x.number == _salesNumber ).FirstOrDefault();
                        sales_return.id_sales_invoice = sales_invoice.id_sales_invoice;
                     //   sales_return.sales_invoice = sales_invoice;
                    }

                    string _desMoneda = string.Empty;

                    //Sales Invoice Detail
                    string sqlDetail = "SELECT"
                    + " dbo.PRODUCTOS.DESPRODUCTO," //0
                    + " dbo.DEVOLUCIONDETALLE.CANTIDADDEVUELTA," //1
                    + " dbo.DEVOLUCIONDETALLE.PRECIONETO, " //2
                    + " dbo.DEVOLUCIONDETALLE.COSTOPROMEDIO, " //4
                    + " dbo.DEVOLUCIONDETALLE.COSTOULTIMO, " //5
                    + " dbo.DEVOLUCIONDETALLE.IVA, " //6
                    + " dbo.DEVOLUCION.COTIZACION1 " //6
                    + " FROM dbo.DEVOLUCION LEFT OUTER JOIN"
                    + " dbo.DEVOLUCIONDETALLE ON dbo.DEVOLUCION.CODDEVOLUCION = dbo.DEVOLUCIONDETALLE.CODDEVOLUCION LEFT OUTER JOIN"
                    + " dbo.PRODUCTOS ON dbo.DEVOLUCIONDETALLE.CODPRODUCTO = dbo.PRODUCTOS.CODPRODUCTO"
                    + " WHERE (dbo.DEVOLUCIONDETALLE.CODDEVOLUCION = " + reader["CODDEVOLUCION"].ToString() + ")";

                    DataTable dt = exeDT(sqlDetail);
                    foreach (DataRow row in dt.Rows)
                    {
                        //db Related Insertion.
                        sales_return.id_currencyfx = db.app_currencyfx.Where(x => x.is_active).FirstOrDefault().id_currencyfx;
                        sales_return.app_currencyfx = db.app_currencyfx.Where(x => x.is_active).FirstOrDefault();

                        sales_return_detail sales_return_detail = new sales_return_detail();

                        string _prod_Name = row["DESPRODUCTO"].ToString();
                        item item = db.items.Where(x => x.name == _prod_Name && x.id_company == id_company).FirstOrDefault();
                        if (item!=null)
                        {
                                 sales_return_detail.id_item = item.id_item;
                            
                               

                        }
                        else
                        {
                            value += 1;
                            Dispatcher.BeginInvoke((Action)(() => progSalesReturn.Value = value));
                            Dispatcher.BeginInvoke((Action)(() => salesReturnValue.Text = value.ToString()));
                            continue;
                        }
                        sales_return_detail.id_item = item.id_item;
                        sales_return_detail.quantity = Convert.ToDecimal(row["CANTIDADDEVUELTA"]);

                        sales_return_detail.id_location = id_location;
                        sales_return_detail.app_location = app_location;

                        string _iva = row["IVA"].ToString();
                        if (_iva == "10.00")
                        {
                            sales_return_detail.id_vat_group = db.app_vat_group.Where(x => x.name == "10%").FirstOrDefault().id_vat_group;
                        }
                        else if (_iva == "5.00")
                        {
                            sales_return_detail.id_vat_group = db.app_vat_group.Where(x => x.name == "5%").FirstOrDefault().id_vat_group;
                        }
                        else
                        {
                            if (db.app_vat_group.Where(x => x.name == "Excento").FirstOrDefault() != null)
                            {
                                sales_return_detail.id_vat_group = db.app_vat_group.Where(x => x.name == "Excento").FirstOrDefault().id_vat_group;
                            }

                        }

                        decimal cotiz1 = Convert.ToDecimal((row["COTIZACION1"] is DBNull) ? 1 : Convert.ToDecimal(row["COTIZACION1"]));
                        sales_return_detail.unit_price = (Convert.ToDecimal(row["PRECIONETO"]) / sales_return_detail.quantity) / cotiz1;
                        if (!(row["COSTOPROMEDIO"] is DBNull))
                        { sales_return_detail.unit_cost = Convert.ToDecimal(row["COSTOPROMEDIO"]); }
                      

                        //Commit Sales Invoice Detail
                        sales_return.sales_return_detail.Add(sales_return_detail);

                      
                    }
                    sales_return.return_type = Status.ReturnTypes.Bonus;

                    if (sales_return.Error == null)
                    {

                        sales_return.State = System.Data.Entity.EntityState.Added;
                        sales_return.IsSelected = true;
                        db.sales_return.Add(sales_return);
                        try
                        {
                            db.SaveChanges();
                        }
                     catch  (Exception ex)
                        {
                            throw ex;
                        }
                        if (!(reader["ESTADO"] is DBNull))
                        {
                            int status = Convert.ToInt32(reader["ESTADO"]);
                            if (status == 0)
                            {
                                sales_return.status = Status.Documents_General.Pending;
                                if (!(reader[11] is DBNull))
                                {
                                    sales_return.comment = reader[11].ToString();
                                }

                            }
                            else if (status == 1)
                            {
                                sales_return.status = Status.Documents_General.Approved;
                                if (!(reader[11] is DBNull))
                                {
                                    sales_return.comment = reader[11].ToString();
                                }
                                db.Approve();
                            }
                            else if (status == 2)
                            {
                                sales_return.status = Status.Documents_General.Annulled;
                                if (!(reader[11] is DBNull))
                                {
                                    sales_return.comment = reader[11].ToString();
                                }
                                db.Approve();
                                db.Anull();
                            }

                        }


                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progSalesReturn.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => salesReturnValue.Text = value.ToString()));
                    }
                    else
                    {
                        //Add code to include error contacts into
                        sales_return_ErrorList.Add(sales_return);
                    }
                }
            }
            // reader.Close();
            //cmd.Dispose();
            conn.Close();

            //_customer_Current = _customer_Max;
        }

    }
}
