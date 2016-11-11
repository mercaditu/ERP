using System;
using System.Collections.Generic;
using System.Data;
using Cognitivo.Reporting.Data;

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

        public DataTable Inventory_OnDate(DateTime TransDate)
        {
            string query = @" select branch.name as BranchName,
                                item.code as ItemCode, 
                                item.name as ItemName,
                                inv.credit as Credit, 
                                inv.DebitChild, 
                                inv.credit - inv.DebitChild as Balance,
                                UnitCost, 
                                (UnitCost * (inv.credit - if(inv.DebitChild is null, 0, inv.DebitChild))) as TotalCost,
                                inv.trans_date as TransDate

                                from (
                                select item_movement.*, sum(val.unit_value) as UnitCost,
                                (select if(sum(debit) is null, 0, sum(debit)) 
                                    from item_movement as mov 
                                    where mov.parent_id_movement = item_movement.id_movement
                                    and mov.trans_date <= '{0}'
                                    ) as DebitChild

                                from item_movement 
                                left outer join item_movement_value as val on item_movement.id_movement = val.id_movement
                                where item_movement.id_company = {1} and item_movement.trans_date <= '{0}'
                                group by item_movement.id_movement
                                ) as inv

                                inner join item_product as prod on inv.id_item_product = prod.id_item_product
                                inner join items as item on prod.id_item = item.id_item
                                inner join app_location as loc on inv.id_location = loc.id_location
                                inner join app_branch as branch on loc.id_branch = branch.id_branch
                                where inv.credit > 0
                                group by inv.id_movement";
            query = string.Format(query, TransDate.ToString("yyyy-MM-dd 23:59:59"), entity.CurrentSession.Id_Company);
            return Generate.DataTable(query);
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

            query = String.Format(query, WhereQuery, StartDate.ToString("yyyy-MM-dd 00:00:00"), EndDate.ToString("yyyy-MM-dd 23:59:59"));
            
            return Generate.DataTable(query);
        }

        public DataTable CostBreakDown(DateTime StartDate, DateTime EndDate)
        {
            string query = @" select i.code as Code, i.name as Name, im.trans_date as TransDate, im.comment as Comment, imv.id_movement as MovID, imv.unit_value as UnitValue, 
                                imv.comment as Concept, im.credit as Quantity,
                                (select sum(unit_value) from item_movement_value where item_movement_value.id_movement = imv.id_movement) as SubTotal
                                from item_movement_value as imv
                                inner join item_movement as im on imv.id_movement = im.id_movement
                                inner join item_product as p on im.id_item_product = p.id_item_product
                                inner join items as i on p.id_item = i.id_item
                                where (im.id_purchase_invoice_detail is not null or im.id_execution_detail is not null) 
                                and {0} im.trans_date >= '{1}' and im.trans_date <= '{2}' 
                                order by im.trans_date";

            string WhereQuery = string.Format("imv.id_company = {0} and ", entity.CurrentSession.Id_Company);
            query = string.Format(query, WhereQuery, StartDate.ToString("yyyy-MM-dd 00:00:00"), EndDate.ToString("yyyy-MM-dd 23:59:59"));
            return Generate.DataTable(query);
        }

        public DataTable MerchandiseExit(DateTime StartDate, DateTime EndDate)
        {
            string query = @"
                select branch.name as BranchName,
                inv.comment as TransComment,
                item.code as ItemCode,
                item.name as ItemName,
                inv.debit as Credit,
                UnitCost,
                (UnitCost* inv.debit) as TotalCost,
                inv.trans_date as TransDate

              from(
              select item_movement.*, sum(val.unit_value) as UnitCost
              from item_movement
              left outer join item_movement_value as val on item_movement.id_movement = val.id_movement
              where item_movement.id_company = {0} and item_movement.trans_date between '{1}' and '{2}' 
              and (
                    item_movement.id_sales_invoice_detail > 0 or 
                    item_movement.id_execution_detail > 0 or 
                    item_movement.id_inventory_detail > 0 or
                    item_movement.id_transfer_detail > 0)

              group by item_movement.id_movement
                ) as inv

              inner join item_product as prod on inv.id_item_product = prod.id_item_product
              inner join items as item on prod.id_item = item.id_item
              inner join app_location as loc on inv.id_location = loc.id_location
              inner join app_branch as branch on loc.id_branch = branch.id_branch
              
              group by inv.id_movement
              order by inv.trans_date";

            query = string.Format(query, entity.CurrentSession.Id_Company, StartDate.ToString("yyyy-MM-dd 00:00:00"), EndDate.ToString("yyyy-MM-dd 23:59:59"));
            return Generate.DataTable(query);
        }
        public DataTable MerchandiseEntry(DateTime StartDate, DateTime EndDate)
        {
            string query = @"
                select branch.name as BranchName,
                inv.comment as TransComment,
                item.code as ItemCode,
                item.name as ItemName,
                inv.credit as Credit,
                UnitCost,
                (UnitCost* inv.credit) as TotalCost,
                inv.trans_date as TransDate

              from(
              select item_movement.*, sum(val.unit_value) as UnitCost
              from item_movement
              left outer join item_movement_value as val on item_movement.id_movement = val.id_movement
              where item_movement.id_company = {0} and item_movement.trans_date between '{1}' and '{2}' 
              and (
                    item_movement.id_purchase_invoice_detail > 0 or 
                    item_movement.id_execution_detail > 0 or 
                    item_movement.id_inventory_detail > 0 or
                    item_movement.id_transfer_detail > 0)

              group by item_movement.id_movement
                ) as inv

              inner join item_product as prod on inv.id_item_product = prod.id_item_product
              inner join items as item on prod.id_item = item.id_item
              inner join app_location as loc on inv.id_location = loc.id_location
              inner join app_branch as branch on loc.id_branch = branch.id_branch
              
              group by inv.id_movement
              order by inv.trans_date";

            query = string.Format(query, entity.CurrentSession.Id_Company, StartDate.ToString("yyyy-MM-dd 00:00:00"), EndDate.ToString("yyyy-MM-dd 23:59:59"));
            return Generate.DataTable(query);
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
