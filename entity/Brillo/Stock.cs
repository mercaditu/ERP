using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace entity.Brillo
{
    public class Stock
    {
        public List<StockList> List(int BranchID, int? LocationID, int ProductID)
        {
            string query = @"select
                                loc.id_location as LocationID,
                                loc.name as Location,
                                parent.id_movement as MovementID,
                                parent.trans_date as TransDate, parent.expire_date,parent.code,
                                parent.credit - if( sum(child.debit) > 0, sum(child.debit), 0 ) as QtyBalance,
                                (select sum(unit_value) from item_movement_value as parent_val where id_movement = parent.id_movement) as Cost

                                from item_movement as parent
                                inner join app_location as loc on parent.id_location = loc.id_location
                                left join item_movement as child on child.parent_id_movement = parent.id_movement

                                where {0} and parent.id_item_product = {1} and parent.status = 2 and parent.debit = 0
                                group by parent.id_movement
                                order by parent.trans_date";
            string WhereQuery = "";

            //This determins if we should bring cost of entire block of
            if (LocationID > 0 || LocationID != null)
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

        public List<StockList> DebitList(int BranchID, int LocationID, int ProductID)
        {
            string query = @"select
                                parent.id_movement as MovementID,
                                parent.trans_date as TransDate,  parent.expire_date,parent.code,
                                parent.debit - if( sum(child.credit) > 0, sum(child.debit), 0 ) as QtyBalance,
                                (select sum(unit_value) from item_movement_value as parent_val where id_movement = parent.id_movement) as Cost

                                from item_movement as parent
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

        public List<StockList> ScalarMovement(item_movement item_movement)
        {
            string query = @"select
                                parent.id_movement as MovementID,
                                parent.trans_date as TransDate,  parent.expire_date,parent.code,
                                parent.credit  as QtyBalance,
                                (select sum(unit_value) from item_movement_value as parent_val where id_movement = parent.id_movement) as Cost

                                from item_movement as parent

                                where parent.id_movement={0}
                                group by parent.id_movement
                                order by parent.trans_date";
            query = String.Format(query, item_movement.id_movement);
            DataTable dt = exeDT(query);
            return GenerateList(dt);
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
                    Stock.TranDate = item_movement.trans_date;
                    Stock.ExpirationDate = item_movement.expire_date;
                    Stock.code = item_movement.code;
                    Stock.QtyBalance = item_movement.debit;
                    Stock.Cost = item_movement.item_movement_value.Sum(x => x.unit_value);

                    StockList.Add(Stock);
                }
            }
            return StockList;
        }

        private DataTable exeDT(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnection sqlConn = new MySqlConnection(CurrentSession.ConnectionString);
                sqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, sqlConn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                sqlConn.Close();
            }
            catch
            {
                //MessageBox.Show("Unable to Connect to Database. Please Check your credentials.");
            }
            return dt;
        }

        private List<StockList> GenerateList(DataTable dt)
        {
            List<StockList> StockList = new List<StockList>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow DataRow in dt.Select("QtyBalance > 0"))
                {
                    StockList Stock = new StockList();
                    Stock.MovementID = Convert.ToInt32(DataRow["MovementID"]);
                    Stock.TranDate = Convert.ToDateTime(DataRow["TransDate"]);
                    if (!(DataRow["expire_date"] is DBNull))
                    {
                        Stock.ExpirationDate = Convert.ToDateTime(DataRow["expire_date"]);
                    }
                    Stock.Location = Convert.ToString(DataRow["Location"]);
                    Stock.LocationID = Convert.ToInt32(DataRow["LocationID"]);
                    Stock.code = Convert.ToString(DataRow["code"]);
                    Stock.QtyBalance = Convert.ToDecimal(DataRow["QtyBalance"]);
                    Stock.Cost = Convert.ToDecimal(DataRow["Cost"]);

                    StockList.Add(Stock);
                }
            }

            return StockList;
        }
    }

    public class StockList
    {
        public int LocationID { get; set; }
        public string Location { get; set; }
        public int MovementID { get; set; }
        public DateTime TranDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string code { get; set; }
        public decimal QtyBalance { get; set; }
        public decimal Cost { get; set; }
    }
}