using System;
using System.Collections.Generic;
using System.Data;

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
            return GenerateList(Generate.DataTable(query));
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
            return GenerateList(Generate.DataTable(query));
        }

        private List<StockList> GenerateList(DataTable dt)
        {
            List<StockList> StockList = new List<StockList>();
            foreach (DataRow DataRow in dt.Rows)
            {
                StockList Stock = new StockList();
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