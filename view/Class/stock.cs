﻿using System;
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
                                select ItemName,ItemCode,ProductID,LocationID,Location,sum(Quantity) as Quantity,Measurement,Cost,Brand,BatchCode,ExpiryDate,MovementID from(
                                select  l.id_location as LocationID,l.name as Location,i.code as ItemCode, i.name as ItemName,
                                ip.id_item_product as ProductID,im.credit,sum(IFNULL(child.debit, 0)),   (im.credit - sum(IFNULL(child.debit,0))) as Quantity,
                                 measure.name as Measurement, sum(IFNULL(imvr.total_value, 0)) as Cost,
                                brand.name as Brand,  im.code as BatchCode, im.expire_date as ExpiryDate,
                                max(im.id_movement) as MovementID
                                 from item_movement as im
                                left join item_movement as child on im.id_movement = child.parent_id_movement
                                inner join item_product as ip on im.id_item_product = ip.id_item_product
                                inner join items as i on ip.id_item = i.id_item
                                inner join app_location as l on im.id_location = l.id_location
                                inner join app_branch as b on l.id_branch = b.id_branch
                                left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
                                left join item_brand as brand on brand.id_brand = i.id_brand
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                 where im.id_company = {0} and b.id_branch = {1} and im.trans_date <= '{2}' 
                                -- and im.credit > 0
                                group by im.id_movement
                                order by im.expire_date) as movement where Quantity > 0 group by ProductID,LocationID";

            query = String.Format(query, entity.CurrentSession.Id_Company, BranchID, TransDate.ToString("yyyy-MM-dd 23:59:59"));
            return GenerateList(Generate.DataTable(query));
        }

        public List<StockList> ByLot(int BranchID, DateTime TransDate)
        {
            string query = @"
                                set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                select ItemName, ItemCode, ProductID, LocationID, Location, Quantity, Measurement, Cost, Brand, BatchCode, ExpiryDate, MovementID from (
                                select  l.id_location as LocationID,l.name as Location,i.code as ItemCode, i.name as ItemName,
                                ip.id_item_product as ProductID,im.credit,sum(IFNULL(child.debit, 0)), (im.credit - sum(IFNULL(child.debit,0))) as Quantity,
                                 measure.name as Measurement, sum(IFNULL(imvr.total_value, 0)) as Cost,
                                brand.name as Brand,  im.code as BatchCode, im.expire_date as ExpiryDate,
                                max(im.id_movement) as MovementID
                                 from item_movement as im
                                left join item_movement as child on im.id_movement = child.parent_id_movement
                                inner join item_product as ip on im.id_item_product = ip.id_item_product
                                inner join items as i on ip.id_item = i.id_item
                                inner join app_location as l on im.id_location = l.id_location
                                inner join app_branch as b on l.id_branch = b.id_branch
                                left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
                                left join item_brand as brand on brand.id_brand = i.id_brand
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                 where im.id_company = {0} and b.id_branch = {1} and im.trans_date <= '{2}' 
                                -- and im.credit > 0
                                group by im.id_movement
                                order by im.expire_date) as movement where Quantity > 0";

            //group by ProductID,LocationID
            query = String.Format(query, entity.CurrentSession.Id_Company, BranchID, TransDate.ToString("yyyy-MM-dd 23:59:59"));
            return GenerateList(Generate.DataTable(query));
        }

        public List<StockList> ByLocation_BatchCode(int LocationID, DateTime TransDate)
        {
            string query = @"
                                set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                select ItemName,ItemCode,ProductID,LocationID,Location,sum(Quantity) as Quantity,Measurement,Cost,Brand,BatchCode,ExpiryDate,MovementID from(
                                select  l.id_location as LocationID,l.name as Location,i.code as ItemCode, i.name as ItemName,
                                ip.id_item_product as ProductID,im.credit,sum(IFNULL(child.debit, 0)),   (im.credit - sum(IFNULL(child.debit,0))) as Quantity,
                                 measure.name as Measurement, sum(IFNULL(imvr.total_value, 0)) as Cost,
                                brand.name as Brand,  im.code as BatchCode, im.expire_date as ExpiryDate,
                                max(im.id_movement) as MovementID
                                 from item_movement as im
                                left join item_movement as child on im.id_movement = child.parent_id_movement
                                inner join item_product as ip on im.id_item_product = ip.id_item_product
                                inner join items as i on ip.id_item = i.id_item
                                inner join app_location as l on im.id_location = l.id_location
                                inner join app_branch as b on l.id_branch = b.id_branch
                                left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
                                left join item_brand as brand on brand.id_brand = i.id_brand
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                 where im.id_company = {0} and b.id_branch = {1} and im.trans_date <= '{2}' and ip.can_expire
                                -- and im.credit > 0
                                group by im.id_movement
                                order by i.name) as movement where Quantity > 0 group by ProductID,LocationID";

            query = String.Format(query, entity.CurrentSession.Id_Company, LocationID, TransDate.ToString("yyyy-MM-dd 23:59:59"));
            return GenerateList(Generate.DataTable(query));
        }

        public List<StockList> ByBranchLocation(int LocationID, DateTime TransDate)
        {
            string query = @" 
                                 set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                select ItemName, ItemCode, ProductID, LocationID, Location, Quantity, Measurement, Cost, Brand, BatchCode, ExpiryDate, MovementID from (
                                select  l.id_location as LocationID,l.name as Location,i.code as ItemCode, i.name as ItemName,
                                ip.id_item_product as ProductID,im.credit,sum(IFNULL(child.debit, 0)), (IFNULL(im.credit,0) - sum(IFNULL(child.debit,0))) as Quantity,
                                 measure.name as Measurement, sum(IFNULL(imvr.total_value, 0)) as Cost,
                                brand.name as Brand,  im.code as BatchCode, im.expire_date as ExpiryDate,
                                max(im.id_movement) as MovementID
                                 from item_movement as im
                                left join item_movement as child on im.id_movement = child.parent_id_movement
                                inner join item_product as ip on im.id_item_product = ip.id_item_product
                                inner join items as i on ip.id_item = i.id_item
                                inner join app_location as l on im.id_location = l.id_location
                                inner join app_branch as b on l.id_branch = b.id_branch
                                left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
                                left join item_brand as brand on brand.id_brand = i.id_brand
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                 where im.id_company = {0} and l.id_location = {1} and im.trans_date <= '{2}'

                                 group by im.id_movement
                                order by im.expire_date) as movement where Quantity > 0";
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