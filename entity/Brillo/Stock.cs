using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace entity.Brillo
{
    public class Stock
    {
        public List<StockList> getItems_ByBranch(int? BranchID, DateTime TransDate)
        {
            string query = @"
                               set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                               select ItemName,ItemCode,ProductID,LocationID,Location,Quantity as Quantity,Measurement,Cost,Brand,BatchCode,ExpiryDate,MovementID,can_expire from(
                                (select l.id_location as LocationID,l.name as Location,i.code as ItemCode, i.name as ItemName,
                                ip.id_item_product as ProductID, (im.credit - sum(IFNULL(child.debit,0))) as Quantity,
                                 measure.name as Measurement, sum(IFNULL(imvr.total_value, 0)) as Cost,i.is_active as IsActive,i.company_id as CompanyID,
                                brand.name as Brand,  im.code as BatchCode, im.expire_date as ExpiryDate,im.trans_date as TranDate,i.id_item_type as Type,
                                max(im.id_movement) as MovementID,i.id_item_type,ip.can_expire,imvr.id_movement_value_rel as MovementRelID
                                  from items  as i
                                 left join item_product as ip on i.id_item = ip.id_item
                                   left join item_movement as im on im.id_item_product = ip.id_item_product
                                  left join item_movement as child on im.id_movement = child.parent_id_movement

                                left join app_location as l on im.id_location = l.id_location
                                left join app_branch as b on l.id_branch = b.id_branch
                                left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
                                left join item_brand as brand on brand.id_brand = i.id_brand
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                 where i.id_company = {0} and {1} and im.trans_date <= '{2}' 
                                
                                group by im.id_movement
                                order by im.expire_date)
                               
                                
                                ) as movement where Quantity >0
                                 union(select i.name as ItemName, i.code as ItemCode,0 as ProductID,0 as LocationID,'' as Location,
                              0 as Quantity,false as can_expire
                                 measure.name as Measurement, 0 as Cost,i.is_active as IsActive,i.company_id as CompanyID,i.id_item_type as Type,
                                brand.name as Brand,  '' as BatchCode, null as ExpiryDate,null as TranDate,
                               0 as MovementID,0 as MovementRelID,,i.is_active as IsActive,i.company_id as CompanyID,
                                  from items  as i



                                left join item_brand as brand on brand.id_brand = i.id_brand
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                 where i.id_company = {0} and id_item_type = 3
                                 group by i.id_item)
                               ";
            string WhereQuery = "";
            if (BranchID > 0 || BranchID != null)
            {
                WhereQuery = string.Format(" and b.id_branch = {1}", BranchID);
            }


            query = String.Format(query, entity.CurrentSession.Id_Company, WhereQuery, TransDate.ToString("yyyy-MM-dd 23:59:59"));

            return GenerateList(exeDT(query));
        }
        public List<StockList> getInStock_ByBranch(int? BranchID, DateTime TransDate)
        {
            string query = @"
                               set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                select ItemName,ItemCode,ProductID,LocationID,Location,sum(Quantity) as Quantity,Measurement,Cost,Brand,BatchCode,ExpiryDate,MovementID,can_expire from(
                                select  l.id_location as LocationID,l.name as Location,i.code as ItemCode, i.name as ItemName,
                                ip.id_item_product as ProductID,im.credit,sum(IFNULL(child.debit, 0)),   (im.credit - sum(IFNULL(child.debit,0))) as Quantity,
                                 measure.name as Measurement, sum(IFNULL(imvr.total_value, 0)) as Cost,
                                brand.name as Brand,  im.code as BatchCode, im.expire_date as ExpiryDate,im.trans_date as TranDate,i.id_item_type as Type,
                                max(im.id_movement) as MovementID,ip.can_expire,imvr.id_movement_value_rel as MovementRelID,i.is_active as IsActive,i.company_id as CompanyID
                                 from item_movement as im
                                left join item_movement as child on im.id_movement = child.parent_id_movement
                                inner join item_product as ip on im.id_item_product = ip.id_item_product
                                inner join items as i on ip.id_item = i.id_item
                                inner join app_location as l on im.id_location = l.id_location
                                inner join app_branch as b on l.id_branch = b.id_branch
                                left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
                                left join item_brand as brand on brand.id_brand = i.id_brand
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                 where im.id_company = {0} and {1} and im.trans_date <= '{2}' 
                                -- and im.credit > 0
                                group by im.id_movement
                                order by im.expire_date) as movement where Quantity > 0 group by ProductID,LocationID";
            string WhereQuery = "";
            if (BranchID > 0 || BranchID != null)
            {
                WhereQuery = string.Format(" and b.id_branch = {1}", BranchID);
            }
            

            query = String.Format(query, entity.CurrentSession.Id_Company, WhereQuery, TransDate.ToString("yyyy-MM-dd 23:59:59"));
            return GenerateList(exeDT(query));
        }
        public List<StockList> getItems_All()
        {
            string query = @"
                               set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                               select ItemName,ItemCode,ProductID,LocationID,Location,Quantity as Quantity,Measurement,Cost,Brand,BatchCode,ExpiryDate,MovementID,can_expire from(
                                (select l.id_location as LocationID,l.name as Location,i.code as ItemCode, i.name as ItemName,
                                ip.id_item_product as ProductID, (im.credit - sum(IFNULL(child.debit,0))) as Quantity,
                                 measure.name as Measurement, sum(IFNULL(imvr.total_value, 0)) as Cost,i.is_active as IsActive,i.company_id as CompanyID,i.id_item_type as Type,
                                brand.name as Brand,  im.code as BatchCode, im.expire_date as ExpiryDate,im.trans_date as TranDate,
                                max(im.id_movement) as MovementID,i.id_item_type,ip.can_expire,imvr.id_movement_value_rel as MovementRelID
                                  from items  as i
                                 left join item_product as ip on i.id_item = ip.id_item
                                   left join item_movement as im on im.id_item_product = ip.id_item_product
                                  left join item_movement as child on im.id_movement = child.parent_id_movement

                                left join app_location as l on im.id_location = l.id_location
                                left join app_branch as b on l.id_branch = b.id_branch
                                left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
                                left join item_brand as brand on brand.id_brand = i.id_brand
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                 where i.id_company = {0} 
                                
                                group by im.id_movement
                                order by im.expire_date)
                               
                                
                                ) as movement
                                 union(select i.name as ItemName, i.code as ItemCode,0 as ProductID,0 as LocationID,'' as Location,
                              0 as Quantity,false as can_expire
                                 measure.name as Measurement, 0 as Cost,
                                brand.name as Brand,  '' as BatchCode, null as ExpiryDate,null as TranDate,i.id_item_type as Type,
                               0 as MovementID,0 as MovementRelID,,i.is_active as IsActive,i.company_id as CompanyID
                                  from items  as i



                                left join item_brand as brand on brand.id_brand = i.id_brand
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                 where i.id_company = {0} and id_item_type = 3
                                 group by i.id_item)
                               ";

            query = String.Format(query, entity.CurrentSession.Id_Company);

            return GenerateList(exeDT(query));
        }





        //public List<StockList> List(int BranchID, int? LocationID, int ProductID)
        //{
        //    string query = @" set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
        //                        set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
        //                        select
        //                        loc.id_location as LocationID,
        //                        loc.name as Location,
        //                        item.name as Item, 
        //                        parent.id_movement as MovementID,
        //                        parent.id_movement_value_rel as MovementRelID,
        //                        parent.trans_date as TransDate, parent.expire_date,parent.code,parent.comment,
        //                        parent.credit - if( sum(child.debit) > 0, sum(child.debit), 0 ) as QtyBalance,
        //                        (select sum(unit_value) as cost from item_movement_value_detail as imvd
        //                        join item_movement_value_rel as imvr on imvd.id_movement_value_rel = imvr.id_movement_value_rel
        //                        where imvr.id_movement_value_rel = parent.id_movement_value_rel) as Cost

        //                        from item_movement as parent
        //                        inner join app_location as loc on parent.id_location = loc.id_location
        //                        left join item_movement as child on child.parent_id_movement = parent.id_movement
        //                        left join item_product as ip on ip.id_item_product = parent.id_item_product
        //                        left join items as item on item.id_item = ip.id_item

        //                        where {0} and parent.id_item_product = {1} and parent.status = 2 and parent.debit = 0
        //                        group by parent.id_movement
        //                        order by parent.trans_date";
        //    string WhereQuery = "";

        //    //This determins if we should bring cost of entire block of
        //    if (LocationID > 0 || LocationID != null)
        //    {
        //        WhereQuery = string.Format("parent.id_location = {0}", LocationID);
        //    }
        //    else
        //    {
        //        WhereQuery = string.Format("loc.id_branch = {0}", BranchID);
        //    }

        //    query = string.Format(query, WhereQuery, ProductID);
        //    DataTable dt = exeDT(query);
        //    return GenerateList(dt);
        //}

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
                    Stock.MovementRelID = (int)item_movement.id_movement_value_rel;
                    Stock.TranDate = item_movement.trans_date;
                    Stock.ExpiryDate = item_movement.expire_date;
                    Stock.BatchCode = item_movement.code;
                    Stock.Quantity = item_movement.debit;
                    Stock.Cost = item_movement.item_movement_value.Sum(x => x.unit_value);

                    StockList.Add(Stock);
                }
            }
            return StockList;
        }

        private DataTable exeDT(string sql)
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
                    Cost = !DataRow.IsNull("Cost") ? Convert.ToDecimal(DataRow["Cost"]) : 0,
                    can_expire= Convert.ToBoolean(DataRow["can_expire"]),
                    IsActive = Convert.ToBoolean(DataRow["IsActive"]),
                    CompanyID = Convert.ToInt16(DataRow["CompanyID"]),
                    Type = Convert.ToInt16(DataRow["Type"])
                };

                if (!DataRow.IsNull("ExpiryDate"))
                {
                    Stock.TranDate = Convert.ToDateTime(DataRow["ExpiryDate"]);
                }
                if (!DataRow.IsNull("TransDate"))
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
            

                StockList.Add(Stock);
            }
            return StockList;
        }

        //private List<StockList> GenerateList(DataTable dt)
        //{
        //    List<StockList> StockList = new List<StockList>();
        //    if (dt.Rows.Count > 0)
        //    {
        //        foreach (DataRow DataRow in dt.Select("QtyBalance > 0"))
        //        {
        //            StockList Stock = new StockList();
        //            Stock.MovementID = Convert.ToInt32(DataRow["MovementID"]);

        //            if (!(DataRow["MovementRelID"] is DBNull))
        //            {
        //                Stock.MovementRelID = Convert.ToInt32(DataRow["MovementRelID"]);
        //            }
        //            else
        //            {
        //                Stock.MovementRelID = 0;
        //            }
        //            Stock.TranDate = Convert.ToDateTime(DataRow["TransDate"]);

        //            if (!(DataRow["expire_date"] is DBNull))
        //            {
        //                Stock.ExpirationDate = Convert.ToDateTime(DataRow["expire_date"]);
        //            }

        //            if (dt.Columns.Contains("Location"))
        //            {
        //                Stock.Location = Convert.ToString(DataRow["Location"]);
        //                Stock.LocationID = Convert.ToInt32(DataRow["LocationID"]);
        //            }
        //            if (dt.Columns.Contains("Item"))
        //            {
        //                Stock.Item = Convert.ToString(DataRow["Item"]);

        //            }

        //            Stock.code = Convert.ToString(DataRow["code"]);
        //            if (dt.Columns.Contains("comment"))
        //            {
        //                Stock.comment = Convert.ToString(DataRow["comment"]);

        //            }

        //            Stock.QtyBalance = Convert.ToDecimal(DataRow["QtyBalance"]);
        //            if (!(DataRow["Cost"] is DBNull))
        //            {
        //                Stock.Cost = Convert.ToDecimal(DataRow["Cost"]);
        //            }


        //            StockList.Add(Stock);
        //        }
        //    }

        //    return StockList;
        //}
    }

  

    public class StockList
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string Location { get; set; }
        public string Brand { get; set; }
        public int ProductID { get; set; }
        public int LocationID { get; set; }
        public int MovementID { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public string Measurement { get; set; }
        public string BatchCode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool can_expire { get; set; }
        public DateTime TranDate { get; set; }
        public int MovementRelID { get; set; }
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int Type { get; set; }
    }
}