using entity;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Cognitivo.Setup.Migration
{
    public partial class MigrationAssistant
    {
        public void supplier()
        {
            string sql = " SELECT dbo.PROVEEDOR.NUMPROVEEDOR, dbo.PROVEEDOR.NOMBRE, dbo.PROVEEDOR.RUC_CIN, dbo.PROVEEDOR.DIRECCION, dbo.PROVEEDOR.TELEFONO, dbo.PROVEEDOR.EMAIL, dbo.PROVEEDOR.observacion, dbo.PROVEEDOR.CLIENTE_ID, dbo.PROVEEDOR.CONTACTO1, dbo.PROVEEDOR.CONTACTO2, dbo.PROVEEDOR.EMAILCONT1,"
                        + " dbo.PROVEEDOR.EMAILCONT2, dbo.PROVEEDOR.TELCONT1, dbo.PROVEEDOR.TELCONT2, dbo.PROVEEDOR.CELCONT1, dbo.PROVEEDOR.CELCONT2, dbo.PROVEEDOR.DIRECCIONCONT1, dbo.PROVEEDOR.DIRECCIONCONT2, dbo.PROVEEDOR.FORMAPAGO,"
                        + " dbo.PROVEEDOR.ESTADO, dbo.PROVEEDOR.DIASVENCIMIENTO, "
                        + " dbo.CATEGORIAPROVEEDOR.DESCATEGORIAPROVEEDOR, dbo.CENTROCOSTO.DESCENTRO, dbo.MONEDA.DESMONEDA,"
                        + " (select DESZONA from ZONA where ZONA.CODZONA=PROVEEDOR.CODZONA) as DESZONA,(select DESCIUDAD from CIUDAD where CIUDAD.CODCIUDAD=PROVEEDOR.CODCIUDAD) as DESCIUDAD,(select DESPAIS from PAIS where PAIS.CODPAIS=PROVEEDOR.CODPAIS) as DESPAIS"
                        + " FROM  dbo.PROVEEDOR LEFT OUTER JOIN"
                        + " dbo.CENTROCOSTO ON dbo.PROVEEDOR.CODCENTRO = dbo.CENTROCOSTO.CODCENTRO LEFT OUTER JOIN"
                        + " dbo.MONEDA ON dbo.PROVEEDOR.CODMONEDA = dbo.MONEDA.CODMONEDA LEFT OUTER JOIN"
                        + " dbo.CATEGORIAPROVEEDOR ON dbo.PROVEEDOR.CODCATEGORIAPROVEEDOR = dbo.CATEGORIAPROVEEDOR.CODCATEGORIAPROVEEDOR";

            SqlConnection conn = new SqlConnection(_connString);
            //Counts Total number of Rows we have to process
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.CommandText = "SELECT COUNT(*) FROM PROVEEDOR";
            cmd.CommandType = CommandType.Text;
            int count = (int)cmd.ExecuteScalar();
            conn.Close();

            int value = 0;

            Dispatcher.BeginInvoke((Action)(() => progSupplier.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progSupplier.Value = value));
            Dispatcher.BeginInvoke((Action)(() => supplierMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => supplierValue.Text = value.ToString()));

            cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.CommandType = CommandType.Text;
            //  SqlDataReader reader = cmd.ExecuteReader();
            DataTable dt_supplier = exeDT(sql);
            foreach (DataRow reader in dt_supplier.Rows)
            {
                using (db db = new db())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    contact contacts = new contact();
                    contacts.id_company = id_company;

                    if (!(reader[1] is DBNull))
                    {
                        contacts.name = reader[1].ToString();
                    }
                    else
                    {
                        continue;
                    }

                    if (!(reader[0] is DBNull))
                    {
                        string str = reader[0].ToString();
                        contacts.code = str;
                    }
                    else
                    {
                        contacts.code = "0";
                    }

                    contacts.gov_code = ((reader[2] is DBNull) || reader[2].ToString() == string.Empty) ? "xxx" : reader[2].ToString();
                    contacts.is_active = true;
                    contacts.is_customer = false;
                    contacts.is_supplier = true;
                    contacts.is_employee = false;
                    contacts.address = (reader[3] is DBNull) ? null : reader[3].ToString();
                    contacts.telephone = (reader[4] is DBNull) ? null : reader[4].ToString();
                    contacts.email = (reader[5] is DBNull) ? null : reader[5].ToString();

                    int _dias = Convert.ToInt32((reader["DIASVENCIMIENTO"] is DBNull) ? 0 : reader["DIASVENCIMIENTO"]);
                    string contrat_name = _dias + " " + "Días";
                    if (db.app_contract.Where(x => x.name == contrat_name).FirstOrDefault() != null)
                    {
                        app_contract app_contract = db.app_contract.Where(x => x.name == contrat_name).FirstOrDefault();
                        contacts.id_contract = app_contract.id_contract;
                        contacts.app_contract = app_contract;
                    }
                    //db Related Insertion.
                    if (!(reader[22] is DBNull))
                    {
                        string CdC = reader[22].ToString();
                        app_cost_center app_cost_center = db.app_cost_center.Where(x => x.name == CdC && x.id_company == id_company).FirstOrDefault();
                        contacts.id_cost_center = app_cost_center.id_cost_center;
                    }

                    contacts.id_contact_role = db.contact_role.Where(x => x.is_principal == true && x.id_company == id_company).FirstOrDefault().id_contact_role;

                    if (!(reader[23] is DBNull))
                    {
                        string fx = reader[23].ToString();
                        app_currency app_currency = db.app_currency.Where(x => x.name == fx && x.id_company == id_company).FirstOrDefault();
                        contacts.id_currency = (int)app_currency.id_currency;
                    }

                    if (!(reader["DESZONA"] is DBNull))
                    {
                        string name = (string)reader["DESZONA"];
                        app_geography _app_geography = db.app_geography.Where(x => x.name == name).FirstOrDefault();
                        if (_app_geography != null)
                        {
                            contacts.app_geography = _app_geography;
                            contacts.id_geography = _app_geography.id_geography;
                        }
                    }
                    else if (!(reader["DESCIUDAD"] is DBNull))
                    {
                        string name = (string)reader["DESCIUDAD"];
                        app_geography _app_geography = db.app_geography.Where(x => x.name == name).FirstOrDefault();
                        if (_app_geography != null)
                        {
                            contacts.app_geography = _app_geography;
                            contacts.id_geography = _app_geography.id_geography;
                        }
                    }
                    else if (!(reader["DESPAIS"] is DBNull))
                    {
                        string name = (string)reader["DESPAIS"];
                        app_geography _app_geography = db.app_geography.Where(x => x.name == name).FirstOrDefault();
                        if (_app_geography != null)
                        {
                            contacts.app_geography = _app_geography;
                            contacts.id_geography = _app_geography.id_geography;
                        }
                    }

                    if (contacts.Error == null)
                    {
                        try
                        {
                            db.contacts.Add(contacts);
                            db.SaveChanges();
                            value += 1;
                            Dispatcher.BeginInvoke((Action)(() => progSupplier.Value = value));
                            Dispatcher.BeginInvoke((Action)(() => supplierValue.Text = value.ToString()));
                        }
                        catch
                        {
                            throw;
                        }
                    }
                    //db.Dispose();
                }
            }

            //cmd.Dispose();
            conn.Close();

            _supplier_Current = _supplier_Max;
        }
    }
}