using System;
using System.Collections.Generic;
using System.Data;
using Cognitivo.Reporting.Data;
using MySql.Data.MySqlClient;

namespace Cognitivo.Class
{
    public class StockList
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string Location { get; set; }
        public string Brand { get; set; }
        public int ProductID { get; set; }
        public int LocationID { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public string Measurement { get; set; }
    }

    public class StockCalculations
    {
        public List<StockList> ByBranch(int BranchID, DateTime TransDate)
        {
            string query = @"select loc.id_location as LocationID, loc.name as Location, item.code as ItemCode, 
                             item.name as ItemName, prod.id_item_product as ProductID, 
                             (sum(mov.credit) - sum(mov.debit)) as Quantity, 
                             measure.name as Measurement,
                             (SELECT sum(val.unit_value) FROM item_movement_value as val WHERE val.id_movement = MAX(mov.id_movement)) AS Cost,
                             brand.name as Brand        
                             from item_movement as mov
                             inner join app_location as loc on mov.id_location = loc.id_location
                             inner join app_branch as branch on loc.id_branch = branch.id_branch
                             inner join item_product as prod on mov.id_item_product = prod.id_item_product 
                             inner join items as item on prod.id_item = item.id_item
                             left join item_brand as brand on brand.id_brand = item.id_brand
                             left join app_measurement as measure on item.id_measurement = measure.id_measurement 
                             where mov.id_company = {0} and branch.id_branch = {1} and mov.trans_date <= '{2}'
                             group by loc.id_location, prod.id_item_product
                             order by item.name";
            query = String.Format(query, entity.CurrentSession.Id_Company, BranchID, TransDate.ToString("yyyy-MM-dd 23:59:59"));
            DataTable dt = exeDT(query);
            return GenerateList(dt);
        }

        public List<StockList> ByBranchLocation(int LocationID, DateTime TransDate)
        {
            string query = @" select loc.id_location as LocationID, loc.name as Location, item.code as ItemCode, item.name as ItemName, 
                                 prod.id_item_product as ProductID, (sum(mov.credit) - sum(mov.debit)) as Quantity, measure.name as Measurement, 
                                 (SELECT sum(val.unit_value) FROM item_movement_value as val WHERE val.id_movement = MAX(mov.id_movement)) AS Cost
                                 from item_movement as mov
                                 inner join app_location as loc on mov.id_location = loc.id_location
                                 inner join app_branch as branch on loc.id_branch = branch.id_branch
                                 inner join item_product as prod on mov.id_item_product = prod.id_item_product 
                                 inner join items as item on prod.id_item = item.id_item
                                 left join app_measurement as measure on item.id_measurement = measure.id_measurement 
                                 where mov.id_company = {0} and mov.id_location = {1} and mov.trans_date <= '{2}'

                                 group by loc.id_location, prod.id_item_product 
                                 order by item.name";
            query = String.Format(query, entity.CurrentSession.Id_Company, LocationID, TransDate.ToString("yyyy-MM-dd 23:59:59"));
            DataTable dt = exeDT(query);
            return GenerateList(dt);
        }

        public DataTable Inventory_OnDate(DateTime TransDate)
        {
            string query = @"   select branch.name as BranchName,
                                item.code as ItemCode, item.name as ItemName,
                                inv.credit as Credit, inv.DebitChild, inv.credit - inv.DebitChild as Balance,
                                UnitCost, (UnitCost * (inv.credit - if(inv.DebitChild is null, 0, inv.DebitChild))) as TotalCost,inv.trans_date as TransDate

                                from (
                                select item_movement.*, sum(val.unit_value) as UnitCost,
                                (select if(sum(debit) is null, 0, sum(debit)) from item_movement as mov where mov.parent_id_movement = item_movement.id_movement) as DebitChild

                                from item_movement 
                                left outer join item_movement_value as val on item_movement.id_movement = val.id_movement
                                where item_movement.id_company = {0} and item_movement.trans_date <= '{1}'
                                group by item_movement.id_movement
                                ) as inv

                                inner join item_product as prod on inv.id_item_product = prod.id_item_product
                                inner join items as item on prod.id_item = item.id_item
                                inner join app_location as loc on inv.id_location = loc.id_location
                                inner join app_branch as branch on loc.id_branch = branch.id_branch
                                where inv.credit  > 0
                                group by inv.id_movement";

            query = String.Format(query, entity.CurrentSession.Id_Company, TransDate.ToString("yyyy-MM-dd 23:59:59"));
            return exeDT(query);
        }

        public DataTable TransferSummary(DateTime StartDate, DateTime EndDate)
        {
            string query = @" select 
                              it.trans_date Date, 
                              CONCAT(Origin.name, ' => ', Destination.name) as Movement, it.number as Transfer, it.comment as Comment, u.name as UserName, r.name as RequestedName,
                              i.name as ItemName, i.code as ItemCode, 
                              itd.quantity_destination as Quantity_D, itd.quantity_origin as Quantity_O
                              from item_transfer as it
                              inner join item_transfer_detail as itd on it.id_transfer = itd.id_transfer
                              inner join item_product as ip on itd.id_item_product = ip.id_item_product
                              inner join items as i on ip.id_item = i.id_item
                              inner join app_location as Origin on it.app_location_origin_id_location = Origin.id_location
                              inner join app_location as Destination on it.app_location_destination_id_location = Destination.id_location
                              left join projects as p on it.id_project = p.id_project
                              inner join security_user as u on it.id_user = u.id_user
                              left join security_user as r on it.user_requested_id_user = r.id_user
                              where {0} it.trans_date >= '{1}' and it.trans_date <= '{2}'
                              order by it.trans_date";
            
            string WhereQuery = String.Format("it.id_company = {0} and ", entity.CurrentSession.Id_Company);

            //if (TransferNumber < 0)
            //{
            //    WhereQuery = WhereQuery + " it.number like '%" + TransferNumber + "%' and ";
            //}

            //if (Project != null)
            //{
            //    WhereQuery = WhereQuery + " it.id_project =" + Project.id_project + " and ";
            //}

            //if (ItemID > 0)
            //{
            //    WhereQuery = WhereQuery + " ip.id_item =" + ItemID + " and ";
            //}

            query = String.Format(query, WhereQuery, StartDate.ToString("yyyy-MM-dd 00:00:00"), EndDate.ToString("yyyy-MM-dd 23:59:59"));
            
            return exeDT(query);
        }

        public decimal? Count_ByBranch(int BranchID, int ItemID, DateTime TransDate)
        {
            ProductDS ProductDS = new ProductDS();
            ProductDS.BeginInit();

            Cognitivo.Reporting.Data.ProductDSTableAdapters.QueriesTableAdapter QueriesTableAdapter = new Cognitivo.Reporting.Data.ProductDSTableAdapters.QueriesTableAdapter();

            //fill data
            decimal? i = QueriesTableAdapter.GetStock_ByBranch(entity.CurrentSession.Id_Company, BranchID, TransDate, ItemID);

            ProductDS.EndInit();
            
            return i;
        }

        public decimal? Count_ByLocation(int LocationID, int ProductID, DateTime TransDate)
        {
            ProductDS ProductDS = new ProductDS();
            ProductDS.BeginInit();

            Cognitivo.Reporting.Data.ProductDSTableAdapters.QueriesTableAdapter QueriesTableAdapter = new Cognitivo.Reporting.Data.ProductDSTableAdapters.QueriesTableAdapter();

            //fill data
            decimal? i = QueriesTableAdapter.GetStock_ByLocation(entity.CurrentSession.Id_Company, LocationID, TransDate, ProductID);

            ProductDS.EndInit();

            return i;
        }

        private DataTable exeDT(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnection sqlConn = new MySqlConnection(Properties.Settings.Default.MySQLconnString);
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
            List<StockList> StockList = new List<Class.StockList>();
            foreach (DataRow DataRow in dt.Rows)
            {
                StockList Stock = new Class.StockList();
                Stock.ItemCode = DataRow["ItemCode"].ToString();
                Stock.ItemName = DataRow["ItemName"].ToString();
                Stock.Location = DataRow["Location"].ToString();
                Stock.LocationID = Convert.ToInt16(DataRow["LocationID"]);
                Stock.Measurement = DataRow["Measurement"].ToString();
                Stock.ProductID = Convert.ToInt16(DataRow["ProductID"]);
                
                if (!DataRow.IsNull("Quantity"))
                {
                    Stock.Quantity = Convert.ToDecimal(DataRow["Quantity"]);
                }
                else
                {
                    Stock.Quantity = 0;
                }
                if (!DataRow.IsNull("Cost"))
                {
                    Stock.Cost = Convert.ToDecimal(DataRow["Cost"]);
                }
                else
                {
                    Stock.Cost = 0;
                }

                StockList.Add(Stock);
            }
            return StockList;
        }
    }
}
