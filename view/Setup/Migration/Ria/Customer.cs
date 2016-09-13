using entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Cognitivo.Setup.Migration
{
    public partial class MigrationAssistant
    {
        public List<entity.contact> Contact_ErrorList = new List<contact>();

        public void customer()
        {
            string sql = " SELECT "
            + " VENDEDOR.DESVENDEDOR,"
            + " TIPOCLIENTE.DESTIPOCLIENTE, "
            + " ZONA.DESZONA, "
            + " CIUDAD.DESCIUDAD, "
            + " PAIS.DESPAIS, "
            + " CLIENTES.NUMCLIENTE, "  //5
            + " CLIENTES.NOMBRE, "
            + " CLIENTES.RUC,"
            + " CLIENTES.DIRECCION, "
            + " CLIENTES.TELEFONO,"
            + " CLIENTES.CELULAR, " //10
            + " CLIENTES.FAX, "
            + " CLIENTES.EMAIL, "
            + " CLIENTES.WEB, "
            + " CLIENTES.LIMCREDITO, "
            + " CLIENTES.PORCENTAJE, " //15
            + " CLIENTES.OBSERVACION, "
            + " CLIENTES.SEPSA, "
            + " CLIENTES.RELACION, " // 18
            + " CLIENTES.TIPOVENTA, "
            + " CLIENTES.TELEFONO1, " //20
            + " CLIENTES.EMAIL1, "
            + " CLIENTES.DIASVENCIMIENTO, "
            + " CLIENTES.CONDICIONVENTA, "
            + " CLIENTES.NUMEROTOL, "
            + " CLIENTES.PROVEEDOR_ID, " //25
            + " CLIENTES.PERSONAJURIDICA, "
            + " CLIENTES.TIMBRADORETENCION, "
            + " CLIENTES.CODCATEGORIACLIENTE, "
            + " CATEGORIACLIENTE.DESCATEGORIACLIENTE,"
            + " CLIENTES.NOMBREFANTASIA, "  //30
            + " CLIENTES.CUSTOMFIELD, "
            + " CLIENTES.CODCLIENTE, "
            + " CLIENTES.SEXO, "
            + " CLIENTES.FECHANACIMIENTO,"
            + " CLIENTES.FECHAINGRESO," //35
            + " PRODUCTOS.DESPRODUCTO,"
            + " CLIENTES.EMPPRESACLIENTE" //35
            + " FROM VENDEDOR "
            + " RIGHT OUTER JOIN  CIUDAD RIGHT OUTER JOIN ZONA ON CIUDAD.CODCIUDAD = ZONA.CODCIUDAD RIGHT OUTER JOIN"
            + " TIPOCLIENTE RIGHT OUTER JOIN CLIENTES LEFT OUTER JOIN CATEGORIACLIENTE ON CLIENTES.CODCATEGORIACLIENTE = CATEGORIACLIENTE.CODCATEGORIACLIENTE ON "
            + " TIPOCLIENTE.CODTIPOCLIENTE = CLIENTES.CODTIPOCLIENTE ON ZONA.CODZONA = CLIENTES.CODZONA ON "
            + " VENDEDOR.CODVENDEDOR = CLIENTES.CODVENDEDOR LEFT OUTER JOIN"
            + " PAIS ON CIUDAD.CODPAIS = PAIS.CODPAIS  LEFT OUTER JOIN"
            + " PRODUCTOS ON CLIENTES.PROVEEDOR_ID = PRODUCTOS.CODPRODUCTO ";

            SqlConnection conn = new SqlConnection(_connString);

            //Counts Total number of Rows we have to process
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.CommandText = "SELECT COUNT(*) FROM CLIENTES";
            cmd.CommandType = CommandType.Text;
            int count = (int)cmd.ExecuteScalar();
            conn.Close();

            int value = 0;

            Dispatcher.BeginInvoke((Action)(() => customerMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => customerValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progCustomer.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progCustomer.Value = value));

            cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.CommandType = CommandType.Text;
            // SqlDataReader reader = cmd.ExecuteReader();
            DataTable dt_customer = exeDT(sql);
            foreach (DataRow reader in dt_customer.Rows)
            {
                using (entity.db db = new entity.db())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    contact contacts = new contact();
                    contacts.id_company = id_company;

                    contacts.is_active = true;
                    contacts.is_customer = true;
                    contacts.is_supplier = false;
                    contacts.is_employee = false;

                    if (!(reader[6] is DBNull))
                    { contacts.name = reader[6].ToString(); }
                    else
                    { continue; }

                    contacts.code = (reader[5] is DBNull) ? null : reader[5].ToString();
                    contacts.gov_code = (reader[7] is DBNull) ? "xxx" : reader[7].ToString();
                    contacts.credit_limit = (reader[14] is DBNull) ? 0 : (decimal)reader[14];
                    contacts.address = (reader[8] is DBNull) ? null : reader[8].ToString();
                    contacts.telephone = (reader[9] is DBNull) ? null : reader[9].ToString();
                    contacts.email = (reader[12] is DBNull) ? null : reader[12].ToString();
                    contacts.alias = (reader[30] is DBNull) ? null : reader[30].ToString();

                    int _dias = Convert.ToInt32((reader["DIASVENCIMIENTO"] is DBNull) ? 0 : reader["DIASVENCIMIENTO"]);
                    string contrat_name = _dias + " " + "Días";
                    if (db.app_contract.Where(x => x.name == contrat_name).FirstOrDefault() != null)
                    {
                        app_contract app_contract = db.app_contract.Where(x => x.name == contrat_name).FirstOrDefault();
                        contacts.id_contract = app_contract.id_contract;
                        contacts.app_contract = app_contract;
                    }

                    contacts.id_contact_role = db.contact_role.Where(x => x.is_principal == true && x.id_company == id_company).FirstOrDefault().id_contact_role;

                    if (_connString.Contains("Angelius"))
                    {
                        if (!(reader[18] is DBNull))
                        {
                            contacts.timestamp = Convert.ToDateTime(reader[35]);
                        }

                        contacts.is_person = true;

                        string role_name = (reader[31] is DBNull) ? null : reader[31].ToString();
                        if (db.contact_role.Where(x => x.name == role_name && x.id_company == id_company).FirstOrDefault() != null)
                        {
                            contacts.id_contact_role = db.contact_role.Where(x => x.name == role_name && x.id_company == id_company).FirstOrDefault().id_contact_role;
                        }
                        else
                        {
                            if (db.app_field.Where(x => x.field_type == app_field.field_types.Account).FirstOrDefault() != null)
                            {
                                app_field app_field = db.app_field.Where(x => x.field_type == app_field.field_types.Account).FirstOrDefault();
                                contact_field_value contact_field_value = new entity.contact_field_value();
                                contact_field_value.contact = contacts;
                                contact_field_value.value = role_name;
                                contact_field_value.id_field = app_field.id_field;
                                contacts.contact_field_value.Add(contact_field_value);
                            }
                        }

                        int relationid = Convert.ToInt32((reader[18] is DBNull) ? null : reader[18].ToString());
                        int code = Convert.ToInt32((reader[32] is DBNull) ? null : reader[32].ToString());
                        if (relationid != code)
                        {
                            DataTable dt_newcustomer = exeDT("select NOMBRE from CLIENTES where CODCLIENTE=" + relationid);
                            string name = (dt_newcustomer.Rows[0][0]).ToString();
                            if (db.contacts.Where(x => x.name == name).FirstOrDefault() != null)
                            {
                                contacts.parent = db.contacts.Where(x => x.name == name).FirstOrDefault();
                            }
                        }

                        var SEXO = Convert.ToInt32((reader[33] is DBNull) ? "0" : reader[33].ToString());
                        if (SEXO > 0)
                        {
                            if (SEXO == 0)
                            {
                                contacts.gender = contact.Genders.Male;
                            }
                            else
                            {
                                contacts.gender = contact.Genders.Female;
                            }
                        }

                        DateTime FECHANACIMINETO = Convert.ToDateTime((reader[34] is DBNull) ? null : reader[34].ToString());

                        if (FECHANACIMINETO != null)
                        {
                            if (FECHANACIMINETO.Year < 2014)
                            {
                                contacts.date_birth = FECHANACIMINETO;
                            }
                        }
                        contact_subscription contact_subscription = new contact_subscription();
                        if (!(reader[33] is DBNull))
                        {
                            if (Convert.ToInt32((reader[33])) > 0)
                            {
                                contact_subscription.unit_price = Convert.ToInt32((reader[33]));
                            }
                        }

                        if (!(reader[25] is DBNull))
                        {
                            string name = reader[36].ToString();
                            if (db.items.Where(x => x.name == name).FirstOrDefault() != null)
                            {
                                contact_subscription.id_item = db.items.Where(x => x.name == name).FirstOrDefault().id_item;
                                if (db.items.Where(x => x.name == name).FirstOrDefault() != null)
                                {
                                    contact_subscription.id_vat_group = db.items.Where(x => x.name == name).FirstOrDefault().id_vat_group;
                                }
                                else
                                {
                                    if (db.app_vat_group.Where(x => x.is_default).FirstOrDefault() != null)
                                    {
                                        contact_subscription.id_vat_group = db.app_vat_group.Where(x => x.is_default).FirstOrDefault().id_vat_group;
                                    }

                                }
                            }

                        }
                        else
                        {
                            if (contacts.parent != null)
                            {
                                contact_subscription.id_item = contacts.parent.contact_subscription.FirstOrDefault().id_item;

                                contact_subscription.id_vat_group = contacts.parent.contact_subscription.FirstOrDefault().id_vat_group;
                            }
                        }
                        if (db.app_contract.Where(x => x.is_active).FirstOrDefault() != null)
                        {
                            contact_subscription.id_contract = db.app_contract.Where(x => x.is_active).FirstOrDefault().id_contract;
                        }
                        contacts.contact_subscription.Add(contact_subscription);

                        if (!(reader[37] is DBNull))
                        {
                            string name = reader[37].ToString();
                            if (db.contact_tag.Local.Where(x => x.name == name).FirstOrDefault() == null)
                            {

                                if (reader[37].ToString() != "")
                                {
                                    contact_tag contact_tag = new contact_tag();
                                    contact_tag.name = reader[37].ToString();
                                    db.contact_tag.Add(contact_tag);
                                    contact_tag_detail contact_tag_detail = new contact_tag_detail();
                                    contact_tag_detail.contact = contacts;
                                    contact_tag_detail.contact_tag = contact_tag;
                                    db.contact_tag_detail.Add(contact_tag_detail);

                                }

                            }
                            else
                            {
                                contact_tag_detail contact_tag_detail = new contact_tag_detail();
                                contact_tag_detail.contact = contacts;
                                contact_tag_detail.contact_tag = db.contact_tag.Local.Where(x => x.name == name).FirstOrDefault();
                                db.contact_tag_detail.Add(contact_tag_detail);
                            }
                        }


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

                    //db Related Insertion.
                    if (!(reader[1] is DBNull))
                    {
                        string _price = reader[1].ToString();
                        item_price_list price_list = db.item_price_list.Where(x => x.name == _price && x.id_company == id_company).FirstOrDefault();
                        contacts.id_price_list = price_list.id_price_list;
                    }

                    if (!(reader[0] is DBNull))
                    {
                        string _sales_rep = reader[0].ToString();
                        sales_rep sales_rep = db.sales_rep.Where(x => x.name == _sales_rep && x.id_company == id_company).FirstOrDefault();
                        contacts.id_sales_rep = (short)sales_rep.id_sales_rep;
                    }

                    if (contacts.Error == null)
                    {
                        db.contacts.Add(contacts);
                        db.SaveChanges();

                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progCustomer.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => customerValue.Text = value.ToString()));
                    }
                    else
                    {
                        //Add code to include error contacts into
                        Contact_ErrorList.Add(contacts);
                    }
                }
            }


            //cmd.Dispose();
            conn.Close();

            _customer_Current = _customer_Max;
        }
    }
}
