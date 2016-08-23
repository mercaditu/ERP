using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Brillo
{
    public class Stock
    {
        public List<StockList> List(app_branch app_branch, app_location app_location, item_product item_product)
        {
            string query = @"select 
                                parent.id_movement as MovementID, 
                                parent.trans_date as TransDate, 
                                parent.credit - if( sum(child.debit) > 0, sum(child.debit), 0 ) as QtyBalance, 
                                (select sum(unit_value) from item_movement_value as parent_val where id_movement = parent.id_movement) as Cost2

                                from item_movement as parent
                                left join item_movement as child on child._parent_id_movement = parent.id_movement

                                where parent.id_location = {0} and parent.id_item_product = {1} and parent.status = 2 and parent.debit = 0
                                group by parent.id_movement
                                order by parent.trans_date";
            query = String.Format(query, app_location.id_location, item_product.id_item_product);
            DataTable dt = exeDT(query);
            return GenerateList(dt);
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
            foreach (DataRow DataRow in dt.Rows)
            {
                StockList Stock = new StockList();
                Stock.MovementID = Convert.ToInt16(DataRow["MovementID"]);
                Stock.TranDate = Convert.ToDateTime(DataRow["TranDate"]);
                Stock.QtyBalance = Convert.ToDecimal(DataRow["QtyBalance"]);
                Stock.Cost = Convert.ToDecimal(DataRow["Cost"]);

                StockList.Add(Stock);
            }
            return StockList;
        }
    }

    public class StockList
    {
        public int MovementID { get; set; }
        public DateTime TranDate { get; set; }
        public decimal QtyBalance { get; set; }
        public decimal Cost { get; set; }
    }
}
