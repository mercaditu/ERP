using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace entity.Brillo
{
    public class Stock
    {
        public List<StockList> DebitList(int BranchID, int LocationID, int ProductID)
        {
            string query = @"
                                 set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                 set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                select
                                loc.id_location as LocationID,
                                loc.name as Location,
                                parent.id_movement as MovementID,
                                parent.id_movement_value_rel as MovementRelID, 
                                parent.trans_date as TransDate,  parent.expire_date,parent.code,
                                parent.debit - if( sum(child.credit) > 0, sum(child.debit), 0 ) as QtyBalance,
                                imvd.unit_value as Cost

                                from item_movement as parent
             left join item_movement_value_rel as imvr on parent.id_movement_value_rel=imvr.id_movement_value_rel
             left join item_movement_value_detail as imvd on imvr.id_movement_value_rel=imvd.id_movement_value_rel
                                inner join app_location as loc on parent.id_location = loc.id_location
                                left join item_movement as child on child.parent_id_movement = parent.id_movement

                                where {0} and parent.id_item_product = {1} and parent.status = 2 and parent.credit = 0
                                group by parent.id_movement
                                order by parent.trans_date";
            string WhereQuery = "";

            //This determins if we should bring cost of entire block of
            if (LocationID > 0)
            {
                WhereQuery = string.Format("parent.id_location = {0}", LocationID);
            }
            else
            {
                WhereQuery = string.Format("loc.id_branch = {0}", BranchID);
            }

            query = string.Format(query, WhereQuery, ProductID);
            DataTable dt = exeDT(query);
            return GenerateList(dt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MovementID"> Send Movement ID of Item Movement</param>
        /// <returns></returns>
        public List<StockList> ScalarMovement(long MovementID)
        {
            string query = @"
                                set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                
                                select

                                parent.id_movement as MovementID,parent.id_location as LocationID,Loc.name as Location,
                                parent.trans_date as TransDate,  parent.expire_date,parent.code,
                                parent.credit  as QtyBalance,
                                parent.id_movement_value_rel as MovementRelID, 
                                imvd.unit_value as Cost

                                from item_movement as parent
 left join item_movement_value_rel as imvr on parent.id_movement_value_rel=imvr.id_movement_value_rel
             left join item_movement_value_detail as imvd on imvr.id_movement_value_rel=imvd.id_movement_value_rel

                                inner join  app_location Loc on parent.id_location=Loc.id_location

                                where parent.id_movement={0}
                                group by parent.id_movement
                                order by parent.trans_date";
            query = String.Format(query, MovementID);
            DataTable dt = exeDT(query);
            return GenerateListScalar(dt);
        }

        /// <summary>
        /// Why is this here???
        /// </summary>
        /// <param name="id_transfer_detail"></param>
        /// <param name="id_item_product"></param>
        /// <returns></returns>
        public List<StockList> MovementForTransfer(int id_transfer_detail, int id_item_product)
        {
            List<StockList> StockList = new List<StockList>();
            using (db db = new db())
            {
                List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_transfer_detail == id_transfer_detail && x.id_item_product == id_item_product && x.debit > 0).ToList();
                foreach (item_movement item_movement in Items_InStockLIST)
                {
                    StockList Stock = new StockList();
                    Stock.MovementID = (int)item_movement.id_movement;
                    Stock.MovementRelID = (int)item_movement.id_movement_value_rel;
                    Stock.TranDate = item_movement.trans_date;
                    Stock.ExpiryDate = item_movement.expire_date;
                    Stock.BatchCode = item_movement.code;
                    Stock.Quantity = item_movement.debit;
                    Stock.Cost = item_movement.item_movement_value_rel!=null? item_movement.item_movement_value_rel.total_value:0;

                    StockList.Add(Stock);
                }
            }
            return StockList;
        }

        public DataTable exeDT(string sql)
        {
            DataTable dt = new DataTable();
            //try
            //{
            MySqlConnection sqlConn = new MySqlConnection(CurrentSession.ConnectionString);
            sqlConn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, sqlConn);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            sqlConn.Close();
            //}
            //catch
            //{
            //    //MessageBox.Show("Unable to Connect to Database. Please Check your credentials.");
            //}
            return dt;
        }

        public List<StockList> GenerateList(DataTable dt)
        {
            List<StockList> StockList = new List<StockList>();
            foreach (DataRow DataRow in dt.Rows)
            {
                StockList Stock = new StockList();

                if (!DataRow.IsNull("ItemCode"))
                {
                    Stock.Code = DataRow["ItemCode"].ToString();
                }
                else
                {
                    Stock.Code = "";
                }
                if (!DataRow.IsNull("ItemName"))
                {
                    Stock.Name = DataRow["ItemName"].ToString();
                }
                else
                {
                    Stock.Name = "";
                }
                if (!DataRow.IsNull("Location"))
                {
                    Stock.Location = DataRow["Location"].ToString();
                }
                else
                {
                    Stock.Location = "";
                }
                if (!DataRow.IsNull("LocationID"))
                {
                    Stock.LocationID = Convert.ToInt16(DataRow["LocationID"]);
                }
                if (!DataRow.IsNull("Measurement"))
                {
                    Stock.Measurement = DataRow["Measurement"].ToString();
                }
                else
                {
                    Stock.Measurement = "";
                }
                if (!DataRow.IsNull("ProductID"))
                {
                    Stock.ProductID = Convert.ToInt16(DataRow["ProductID"]);
                }
                if (!DataRow.IsNull("Quantity"))
                {
                    Stock.Quantity = Convert.ToDecimal(DataRow["Quantity"]);
                }
                if (!DataRow.IsNull("Cost"))
                {
                    Stock.Cost = Convert.ToDecimal(DataRow["Cost"]);
                }

                if (!DataRow.IsNull("BatchCode"))
                {
                    Stock.BatchCode = DataRow["BatchCode"].ToString();
                }
                else
                {
                    Stock.BatchCode = "";
                }
                if (!DataRow.IsNull("can_expire"))
                {
                    Stock.can_expire = Convert.ToBoolean(DataRow["can_expire"]);
                }
                if (!DataRow.IsNull("IsActive"))
                {
                    Stock.IsActive = Convert.ToBoolean(DataRow["IsActive"]);
                }
                if (!DataRow.IsNull("CompanyID"))
                {

                    Stock.CompanyID = Convert.ToInt16(DataRow["CompanyID"]);
                }
                if (!DataRow.IsNull("Type"))
                {
                    Stock.Type = Convert.ToInt16(DataRow["Type"]);
                }

                if (!DataRow.IsNull("ExpiryDate"))
                {
                    Stock.ExpiryDate = Convert.ToDateTime(DataRow["ExpiryDate"]);
                }
                if (!DataRow.IsNull("TranDate") && DataRow["TranDate"].ToString() != "")
                {
                    Stock.TranDate = Convert.ToDateTime(DataRow["TranDate"]);
                }

                if (!DataRow.IsNull("MovementID"))
                {
                    Stock.MovementID = Convert.ToInt32(DataRow["MovementID"]);
                }
                if (!DataRow.IsNull("MovementRelID"))
                {
                    Stock.MovementRelID = Convert.ToInt32(DataRow["MovementRelID"]);
                }
                if (!DataRow.IsNull("ID"))
                {
                    Stock.ItemID = Convert.ToInt32(DataRow["ID"]);
                }


                StockList.Add(Stock);
            }
            return StockList;
        }
        public List<StockList> GenerateListScalar(DataTable dt)
        {
            List<StockList> StockList = new List<StockList>();
            foreach (DataRow DataRow in dt.Rows)
            {
                StockList Stock = new StockList();

                if (!DataRow.IsNull("Location"))
                {
                    Stock.Location = DataRow["Location"].ToString();
                }
                else
                {
                    Stock.Location = "";
                }
                if (!DataRow.IsNull("LocationID"))
                {
                    Stock.LocationID = Convert.ToInt16(DataRow["LocationID"]);
                }

                if (!DataRow.IsNull("QtyBalance"))
                {
                    Stock.Quantity = Convert.ToDecimal(DataRow["QtyBalance"]);
                }
                if (!DataRow.IsNull("Cost"))
                {
                    Stock.Cost = Convert.ToDecimal(DataRow["Cost"]);
                }

                if (!DataRow.IsNull("expire_date"))
                {
                    Stock.ExpiryDate = Convert.ToDateTime(DataRow["expire_date"]);
                }
                if (!DataRow.IsNull("TransDate") && DataRow["TransDate"].ToString() != "")
                {
                    Stock.TranDate = Convert.ToDateTime(DataRow["TransDate"]);
                }

                if (!DataRow.IsNull("MovementID"))
                {
                    Stock.MovementID = Convert.ToInt32(DataRow["MovementID"]);
                }
                if (!DataRow.IsNull("MovementRelID"))
                {
                    Stock.MovementRelID = Convert.ToInt32(DataRow["MovementRelID"]);
                }
                if (!DataRow.IsNull("code"))
                {
                    Stock.BatchCode = DataRow["code"].ToString();
                }
                else
                {
                    Stock.BatchCode = "";
                }



                StockList.Add(Stock);
            }
            return StockList;
        }
    }

    public class StockList
    {
        public bool IsSelected { get; set; }

        public int ItemID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? BranchID { get; set; }
        public string Branch { get; set; }
        public int? LocationID { get; set; }
        public string Location { get; set; }
        public string Brand { get; set; }
        public int? ProductID { get; set; }
        public int? ParentID { get; set; }
        public int? MovementID { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ConversionQuantity { get; set; }
        public decimal? Cost { get; set; }
        public string Measurement { get; set; }
        public string BatchCode { get; set; }
        public string BarCode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool can_expire { get; set; }
        public DateTime TranDate { get; set; }
        public int? MovementRelID { get; set; }
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int Type { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}