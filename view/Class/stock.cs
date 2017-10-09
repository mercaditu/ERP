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
        public int? MovementID { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public string Measurement { get; set; }
        public string BatchCode { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class StockCalculations
    {
        public List<StockList> ByBranch(int BranchID, DateTime TransDate)
        {
            string query = @"
                            set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                            select loc.id_location as LocationID, loc.name as Location, item.code as ItemCode,
                             item.name as ItemName, prod.id_item_product as ProductID,
                             (sum(mov.credit) - sum(mov.debit)) as Quantity,
                             measure.name as Measurement,
                                 sum(imvr.total_value) as Cost,
                             brand.name as Brand,
                                 mov.code as BatchCode,
                                 mov.expire_date as ExpiryDate,
                        mov.id_movement as MovementID
                             from item_movement as mov
                             inner join app_location as loc on mov.id_location = loc.id_location
                             inner join app_branch as branch on loc.id_branch = branch.id_branch
                             inner join item_product as prod on mov.id_item_product = prod.id_item_product
                             inner join items as item on prod.id_item = item.id_item
                             left join item_movement_value_rel as imvr on mov.id_movement_value_rel=imvr.id_movement_value_rel
                             left join item_brand as brand on brand.id_brand = item.id_brand
                             left join app_measurement as measure on item.id_measurement = measure.id_measurement
                             where mov.id_company = {0} and branch.id_branch = {1} and mov.trans_date <= '{2}'
                             group by loc.id_location, prod.id_item_product
                             order by item.name";
            query = String.Format(query, entity.CurrentSession.Id_Company, BranchID, TransDate.ToString("yyyy-MM-dd 23:59:59"));
            return GenerateList(Generate.DataTable(query));
        }

        public List<StockList> ByLot(int BranchID, DateTime TransDate)
        {
            string query = @"
                               set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                 select * from(
                                select  l.id_location as LocationID,l.name as Location,i.code as ItemCode, i.name as ItemName,
                                ip.id_item_product as ProductID,  (im.credit - sum(IFNULL(child.debit,0))) as Quantity,
                                 measure.name as Measurement,    
                                      sum(imvr.total_value) as Cost,
                                brand.name as Brand,  im.code as BatchCode, im.expire_date as ExpiryDate,
                                im.id_movement as MovementID
                                 from item_movement as im
                                left join item_movement as child on im.id_movement = child.parent_id_movement
                                inner join item_product as ip on im.id_item_product = ip.id_item_product
                                inner join items as i on ip.id_item = i.id_item
                                inner join app_location as l on im.id_location = l.id_location
                                inner join app_branch as b on l.id_branch = b.id_branch
                                left join item_movement_value_rel as imvr on mov.id_movement_value_rel = imvr.id_movement_value_rel
                         
                                left join item_brand as brand on brand.id_brand = i.id_brand
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                where im.id_company = {0} and b.id_branch = {1} and im.trans_date <= '{2}'
                                group by im.id_movement
                                order by im.expire_date) as movement where Quantity>0";
            query = String.Format(query, entity.CurrentSession.Id_Company, BranchID, TransDate.ToString("yyyy-MM-dd 23:59:59"));
            return GenerateList(Generate.DataTable(query));
        }

        public List<StockList> ByLocation_BatchCode(int LocationID, DateTime TransDate)
        {
            string query = @"
                                 set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                 set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                select 
                                ip.id_item_product as ProductID,
                                i.name as ItemName, 
                                i.code ItemCode,
                                im.id_location as LocationID,
                                l.name as Location,
                                im.trans_date as Date,
                                im.code as BatchCode,
                                im.expire_date as ExpiryDate,
                                im.credit - if(sum(imc.debit) is not null,sum(imc.debit), 0) as Quantity,
                                measure.name as Measurement,
                                im.id_movement as MovementID,
                                      sum(imvr.total_value) as Cost

                                from item_movement as im

                                inner join item_product as ip on im.id_item_product = ip.id_item_product
                                inner join items as i on ip.id_item = i.id_item
                                inner join app_location as l on im.id_location = l.id_location
                                left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
                            
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                left join item_movement as imc on im.id_movement = imc.parent_id_movement
                                where im.id_company = {0} and im.id_location = {1} and im.trans_date <= '{2}' and ip.can_expire
                                group by im.id_movement
                                order by i.name";

            query = String.Format(query, entity.CurrentSession.Id_Company, LocationID, TransDate.ToString("yyyy-MM-dd 23:59:59"));
            return GenerateList(Generate.DataTable(query));
        }

        public List<StockList> ByBranchLocation(int LocationID, DateTime TransDate)
        {
            string query = @" 
                                 set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                 set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                 select loc.id_location as LocationID, loc.name as Location, item.code as ItemCode, item.name as ItemName,
                                 prod.id_item_product as ProductID, (sum(mov.credit) - sum(mov.debit)) as Quantity, measure.name as Measurement,
                                    sum(imvr.total_value) as Cost,
                                 mov.code as BatchCode,
                                 mov.expire_date as ExpiryDate,
                                    mov.id_movement as MovementID
                                 from item_movement as mov

                                 inner join app_location as loc on mov.id_location = loc.id_location
                                 inner join app_branch as branch on loc.id_branch = branch.id_branch
                                 inner join item_product as prod on mov.id_item_product = prod.id_item_product
                                 inner join items as item on prod.id_item = item.id_item
                                 left join item_movement_value_rel as imvr on mov.id_movement_value_rel=imvr.id_movement_value_rel
                           
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
                StockList Stock = new StockList()
                {
                    ItemCode = DataRow["ItemCode"].ToString(),
                    ItemName = DataRow["ItemName"].ToString(),
                    Location = DataRow["Location"].ToString(),
                    LocationID = Convert.ToInt16(DataRow["LocationID"]),
                    Measurement = DataRow["Measurement"].ToString(),
                    ProductID = Convert.ToInt16(DataRow["ProductID"]),
                    BatchCode = DataRow["BatchCode"].ToString(),
                    Quantity = !DataRow.IsNull("Quantity") ? Convert.ToDecimal(DataRow["Quantity"]) : 0,
                    Cost = !DataRow.IsNull("Cost") ? Convert.ToDecimal(DataRow["Cost"]) : 0
                };

                if (!DataRow.IsNull("ExpiryDate"))
                {
                    Stock.ExpiryDate = Convert.ToDateTime(DataRow["ExpiryDate"]);
                }

                if (!DataRow.IsNull("MovementID"))
                {
                    Stock.MovementID = Convert.ToInt32(DataRow["MovementID"]);
                }

                StockList.Add(Stock);
            }
            return StockList;
        }
    }
}