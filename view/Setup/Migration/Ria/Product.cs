using entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Cognitivo.Setup.Migration
{
    public partial class MigrationAssistant
    {
        public void product()
        {
            string sql = " SELECT dbo.FAMILIA.DESFAMILIA, dbo.LINEA.DESLINEA, dbo.RUBRO.DESRUBRO, dbo.IVA.DESIVA, dbo.CODIGOS.DESCODIGO1, dbo.CODIGOS.CODIGO, dbo.CODIGOS.PESABLE, dbo.CODIGOS.VENCIMIENTO, dbo.CODIGOS.BALANZA, dbo.PRODUCTOS.DESPRODUCTO, dbo.PRODUCTOS.STOCKMINIMO, dbo.PRODUCTOS.STOCKMAXIMO, "
                       + " dbo.PRODUCTOS.SERVICIO, dbo.PRODUCTOS.ESTADO, dbo.PRODUCTOS.ESPECIFICACIONES, dbo.PRODUCTOS.PRODUCTO, dbo.UNIDADMEDIDA.DESMEDIDA, dbo.CODIGOS.CODCODIGO"
                       + " FROM dbo.CODIGOS LEFT OUTER JOIN"
                       + " dbo.PRODUCTOS ON dbo.CODIGOS.CODPRODUCTO = dbo.PRODUCTOS.CODPRODUCTO LEFT OUTER JOIN"
                       + " dbo.UNIDADMEDIDA ON dbo.PRODUCTOS.CODMEDIDA = dbo.UNIDADMEDIDA.CODMEDIDA LEFT OUTER JOIN"
                       + " dbo.RUBRO ON dbo.PRODUCTOS.CODRUBRO = dbo.RUBRO.CODRUBRO LEFT OUTER JOIN"
                       + " dbo.FAMILIA ON dbo.PRODUCTOS.CODFAMILIA = dbo.FAMILIA.CODFAMILIA LEFT OUTER JOIN"
                       + " dbo.LINEA ON dbo.PRODUCTOS.CODLINEA = dbo.LINEA.CODLINEA LEFT OUTER JOIN"
                       + " dbo.IVA ON dbo.PRODUCTOS.CODIVA = dbo.IVA.CODIVA";

            SqlConnection conn = new SqlConnection(_connString);
            //Counts Total number of Rows we have to process
            SqlCommand cmd = new SqlCommand();
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT COUNT(*) FROM CODIGOS";
            cmd.CommandType = CommandType.Text;
            int count = (int)cmd.ExecuteScalar();
            //cmd.Dispose();
            conn.Close();

            int value = 0;

            Dispatcher.BeginInvoke((Action)(() => progItem.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progItem.Value += value));
            Dispatcher.BeginInvoke((Action)(() => itemMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => itemValue.Text = value.ToString()));

            string sql_price = " SELECT dbo.TIPOCLIENTE.DESTIPOCLIENTE, dbo.PRECIO.CANTIDAD, dbo.PRECIO.PRECIOVENTA, dbo.MONEDA.DESMONEDA, dbo.PRODUCTOS.DESPRODUCTO"
                             + " FROM  dbo.PRECIO LEFT OUTER JOIN"
                             + " dbo.MONEDA ON dbo.PRECIO.CODMONEDA = dbo.MONEDA.CODMONEDA LEFT OUTER JOIN"
                             + " dbo.PRODUCTOS ON dbo.PRECIO.CODPRODUCTO = dbo.PRODUCTOS.CODPRODUCTO LEFT OUTER JOIN"
                             + " dbo.TIPOCLIENTE ON dbo.PRECIO.CODTIPOCLIENTE = dbo.TIPOCLIENTE.CODTIPOCLIENTE";
            DataTable dt_Price = exeDT(sql_price);

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            DataTable dt_product = exeDT(sql);
            foreach (DataRow reader in dt_product.Rows)
            {


                using (db db = new db())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    item item = new item();
                    item.id_company = id_company;

                    if (!(reader[9] is DBNull))
                    { item.name = reader[9].ToString(); }
                    else
                    { continue; }

                    item.code = (reader[5] is DBNull) ? string.Empty : reader[5].ToString();
                    item.variation = (reader[9] is DBNull) ? string.Empty : reader[9].ToString();
                    item.is_active = (reader[13] is DBNull) ? false : true;
                    item.description = (reader[14] is DBNull) ? string.Empty : reader[14].ToString();

                    string name = (reader[16] is DBNull) ? string.Empty : reader[16].ToString();
                    if (db.app_measurement.Where(x => x.name == name && x.id_company == id_company).FirstOrDefault() != null)
                    {
                        item.id_measurement = db.app_measurement.Where(x => x.name == name && x.id_company == id_company).FirstOrDefault().id_measurement;
                    }

                    //string FAMILIA;
                    if (!(reader[0] is DBNull))
                    {
                        string tagname = reader[0].ToString();
                        item_tag item_tagFam = db.item_tag.Where(x => x.name == tagname && x.id_company == id_company).FirstOrDefault();
                        item_tag_detail tag_detailFam = new item_tag_detail();
                        tag_detailFam.id_tag = item_tagFam.id_tag;
                        item.item_tag_detail.Add(tag_detailFam);
                    }

                    //string LINEA;
                    if (!(reader[1] is DBNull))
                    {
                        string tagLinname = reader[1].ToString();
                        item_tag item_tagLin = db.item_tag.Where(x => x.name == tagLinname && x.id_company == id_company).FirstOrDefault();
                        item_tag_detail tag_detailLin = new item_tag_detail();
                        tag_detailLin.id_tag = item_tagLin.id_tag;
                        item.item_tag_detail.Add(tag_detailLin);
                    }

                    //string RUBRO;
                    if (!(reader[2] is DBNull))
                    {
                        string tagrubro = reader[2].ToString();
                        item_tag item_tagRub = db.item_tag.Where(x => x.name == tagrubro && x.id_company == id_company).FirstOrDefault();
                        item_tag_detail tag_detailRub = new item_tag_detail();
                        tag_detailRub.id_tag = item_tagRub.id_tag;
                        item.item_tag_detail.Add(tag_detailRub);
                    }

                    if (!(reader[15] is DBNull))
                    {
                        if (Convert.ToInt32(reader[15]) == 1)
                        {
                            //Product
                            item.id_item_type = item.item_type.Product;

                            item_product product = new item_product();
                            product.id_company = id_company;
                            product.can_expire = (reader[7] is DBNull || Convert.ToInt32(reader[7]) == 0) ? false : true;
                            product.is_weigted = (reader[6] is DBNull || Convert.ToInt32(reader[6]) == 0) ? false : true;
                            product.stock_max = (reader[11] is DBNull) ? 0M : (decimal)reader[11];
                            product.stock_min = (reader[10] is DBNull) ? 0M : (decimal)reader[10];
                            item.item_product.Add(product);
                        }
                        else
                        {
                            item.id_item_type = item.item_type.Task; //Generic Service
                            //item_service service
                        }
                    }
                    else
                    {
                        item.id_item_type = item.item_type.Task; //Generic Service
                        //item_service service
                    }
                    decimal _vat_coeficient = 0;

                    if (!(reader[3] is DBNull))
                    {
                        string vatname = reader[3].ToString();
                        app_vat_group app_vat_group = db.app_vat_group.Where(x => x.name == vatname && x.id_company == id_company).FirstOrDefault();
                        item.id_vat_group = app_vat_group.id_vat_group;
                    }

                    decimal coefficient = 0;
                    List<app_vat_group_details> app_vat_group_details = db.app_vat_group_details.Where(x => x.id_vat_group == item.id_vat_group).ToList();

                    foreach (app_vat_group_details app_vat_group in app_vat_group_details)
                    {
                        coefficient = coefficient + app_vat_group.app_vat.coefficient;
                    }

                    string _DESPRODUCTO = reader["DESPRODUCTO"].ToString();
                    _DESPRODUCTO=_DESPRODUCTO.Replace("'", "");
                    try
                    {
                        foreach (DataRow price_row in dt_Price.Select("DESPRODUCTO = '" + _DESPRODUCTO + "'"))
                        {
                            string _desTipoCliente = (price_row.IsNull("DESTIPOCLIENTE")) ? string.Empty : price_row["DESTIPOCLIENTE"].ToString();
                            string _desMoneda = (price_row.IsNull("DESMONEDA")) ? string.Empty : price_row["DESMONEDA"].ToString();

                            if (_desTipoCliente != string.Empty && _desMoneda != string.Empty)
                            {
                                item_price_list item_price_list = db.item_price_list.Where(x => x.name == _desTipoCliente && x.id_company == id_company).FirstOrDefault();
                                app_currency app_currency = db.app_currency.Where(x => x.name == _desMoneda && x.id_company == id_company).FirstOrDefault();

                                if (item_price_list != null && app_currency != null && _vat_coeficient != -1)
                                {
                                    item_price item_price = new item_price();
                                    item_price.item = item;
                                    if (price_row["PRECIOVENTA"] is DBNull)
                                    {
                                        item_price.value = 0;
                                    }
                                    else
                                    {
                                        item_price.value = ((decimal)price_row["PRECIOVENTA"] / (1 + coefficient));
                                    }
                                    item_price.min_quantity = (price_row.IsNull("CANTIDAD")) ? 0 : Convert.ToDecimal(price_row["CANTIDAD"]);
                                    item_price.id_currency = app_currency.id_currency;
                                    item_price.id_price_list = item_price_list.id_price_list;
                                    item.item_price.Add(item_price);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    try
                    {
                        if (item.Error == null)
                        {
                            db.items.Add(item);
                            db.SaveChanges();
                            value += 1;
                            Dispatcher.BeginInvoke((Action)(() => progItem.Value = value));
                            Dispatcher.BeginInvoke((Action)(() => itemValue.Text = value.ToString()));
                        }
                    }
                    catch { }
                }
            }


            //cmd.Dispose();
            conn.Close();

            _product_Current = _product_Max;
        }
    }
}