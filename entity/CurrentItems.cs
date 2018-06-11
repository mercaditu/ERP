using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using entity.Brillo;
using System.Data;

namespace entity
{
    public static class CurrentItems
    {

        #region PropertyChanged

        // INotifyPropertyChanged implementation
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static void NotifyStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        #endregion PropertyChanged

        public static Stock stock = new Stock();
        //private static List<StockList> List { get; set; }
        public static List<StockList> List
        {
            get { return _List; }
            set { _List = value.ToList(); }
        }
        static List<StockList> _List = new List<StockList>();


        public static List<StockList> getItems_ByBranch(int? BranchID, bool forceData)
        {
            return CurrentItems.GetList((int)BranchID, forceData);
        }

        public static List<StockList> getProducts_InStock(int? BranchID, DateTime? TransDate, bool forceData)
        {
            if (TransDate == null)
            {
                return CurrentItems.GetList((int)BranchID, forceData).Where(x => x.Quantity > 0).ToList();
            }
            else
            {
                //Get Specific Data based on date. Fill up NEW DT and send back.
                return CurrentItems.GetList((int)BranchID, forceData).Where(x => x.Quantity > 0).ToList();
            }
        }

        public static IEnumerable<StockList> getProducts_InStock_GroupBy(int? BranchID, DateTime? TransDate, bool forceData)
        {
            var list = getItems_ByBranch((int)BranchID, forceData)
                .GroupBy(x => x.ItemID)
                .Select(x => new
                {
                    Code = x.Max(y => y.Code),
                    Name = x.Max(y => y.Name),
                    Location = x.Max(y => y.Location),
                    Measurement = x.Max(y => y.Measurement),
                    Quantity = x.Sum(y => y.Quantity),
                    ConversionQuantity = x.Sum(y => y.ConversionQuantity),
                    Cost = x.Max(y => y.Cost),
                    MovementID = x.Max(y => y.MovementID),
                    MovementRelID = x.Max(y => y.MovementRelID),
                    ItemID = x.Max(y => y.ItemID),
                    ProductID = x.Max(y => y.ProductID),
                    LocationID = x.Max(y => y.LocationID),
                    CompanyID = x.Max(y => y.CompanyID),
                    Type = x.Max(y => y.Type),
                    BatchCode = x.Max(y => y.BatchCode),
                    ExpiryDate = x.Max(y => y.ExpiryDate),
                    TranDate = x.Max(y => y.TranDate),
                    BarCode = x.Max(y => y.BarCode)
                }).Where(x => x.Quantity > 0).ToList();

            List<StockList> StockList = new List<StockList>();

            foreach (dynamic item in list)
            {
                StockList Stock = new StockList();

                Stock.IsSelected = false;
                Stock.Code = item.Code;
                Stock.Name = item.Name;
                Stock.Location = item.Location;
                Stock.Measurement = item.Measurement;
                Stock.Quantity = item.Quantity;
                Stock.ConversionQuantity = item.ConversionQuantity;
                Stock.Cost = item.Cost;
                Stock.MovementID = item.MovementID;
                Stock.MovementRelID = item.MovementRelID;
                Stock.ItemID = item.ItemID;
                Stock.ProductID = item.ProductID;
                Stock.LocationID = item.LocationID;
                Stock.CompanyID = item.CompanyID;
                Stock.Type = item.Type;
                Stock.BatchCode = item.BatchCode;
                Stock.ExpiryDate = item.ExpiryDate;
                Stock.TranDate = item.TranDate;
                Stock.BarCode = item.BarCode;
                StockList.Add(Stock);
            }

            return StockList;
        }
        public static IEnumerable<StockList> getProducts_InStock_GroupByLocationBatch(int? BranchID, DateTime? TransDate, bool forceData)
        {
            var list = getItems_ByBranch((int)BranchID, forceData)
               //  .GroupBy(x => new { x.LocationID, x.ItemID, x.m })
               .GroupBy(x => new { x.MovementID })
                .Select(x => new
                {
                    Code = x.Max(y => y.Code),
                    Name = x.Max(y => y.Name),
                    Location = x.Max(y => y.Location),
                    Measurement = x.Max(y => y.Measurement),
                    Quantity = x.Sum(y => y.Quantity),
                    ConversionQuantity = x.Sum(y => y.ConversionQuantity),
                    Cost = x.Max(y => y.Cost),
                    MovementID = x.Max(y => y.MovementID),
                    MovementRelID = x.Max(y => y.MovementRelID),
                    ItemID = x.Max(y => y.ItemID),
                    ProductID = x.Max(y => y.ProductID),
                    LocationID = x.Max(y => y.LocationID),
                    CompanyID = x.Max(y => y.CompanyID),
                    Type = x.Max(y => y.Type),
                    BatchCode = x.Max(y => y.BatchCode),
                    ExpiryDate = x.Max(y => y.ExpiryDate),
                    TranDate = x.Max(y => y.TranDate),
                    BarCode = x.Max(y => y.BarCode)
                }).Where(x => x.Quantity > 0).ToList();

            List<StockList> StockList = new List<StockList>();

            foreach (dynamic item in list)
            {
                StockList Stock = new StockList();
                Stock.IsSelected = false;
                Stock.Code = item.Code;
                Stock.Name = item.Name;
                Stock.Location = item.Location;
                Stock.Measurement = item.Measurement;
                Stock.Quantity = item.Quantity;
                Stock.ConversionQuantity = item.ConversionQuantity;
                Stock.Cost = item.Cost;
                Stock.MovementID = item.MovementID;
                Stock.MovementRelID = item.MovementRelID;
                Stock.ItemID = item.ItemID;
                Stock.ProductID = item.ProductID;
                Stock.LocationID = item.LocationID;
                Stock.CompanyID = item.CompanyID;
                Stock.Type = item.Type;
                Stock.BatchCode = item.BatchCode;
                Stock.ExpiryDate = item.ExpiryDate;
                Stock.TranDate = item.TranDate;
                Stock.BarCode = item.BarCode;
                StockList.Add(Stock);
            }

            return StockList;
        }
        public static IEnumerable<StockList> getProducts_InStock_GroupByLocation(int? BranchID, DateTime? TransDate, bool forceData)
        {
            var list = getItems_ByBranch((int)BranchID, forceData)
                .GroupBy(x => new { x.LocationID, x.ItemID })
                .Select(x => new
                {
                    Code = x.Max(y => y.Code),
                    Name = x.Max(y => y.Name),
                    Location = x.Max(y => y.Location),
                    Measurement = x.Max(y => y.Measurement),
                    Quantity = x.Sum(y => y.Quantity),
                    ConversionQuantity = x.Sum(y => y.ConversionQuantity),
                    Cost = x.Max(y => y.Cost),
                    MovementID = x.Max(y => y.MovementID),
                    MovementRelID = x.Max(y => y.MovementRelID),
                    CanExpire = x.Max(y => y.can_expire),
                    ItemID = x.Max(y => y.ItemID),
                    ProductID = x.Max(y => y.ProductID),
                    LocationID = x.Max(y => y.LocationID),
                    CompanyID = x.Max(y => y.CompanyID),
                    Type = x.Max(y => y.Type),
                    BatchCode = x.Max(y => y.BatchCode),
                    ExpiryDate = x.Max(y => y.ExpiryDate)
                }).Where(x => x.Quantity > 0).ToList();

            List<StockList> StockList = new List<StockList>();

            foreach (dynamic item in list)
            {
                StockList Stock = new StockList();

                Stock.Code = item.Code;
                Stock.Name = item.Name;
                Stock.Location = item.Location;
                Stock.Measurement = item.Measurement;
                Stock.Quantity = item.Quantity;
                Stock.ConversionQuantity = item.ConversionQuantity;
                Stock.Cost = item.Cost;
                Stock.MovementID = item.MovementID;
                Stock.MovementRelID = item.MovementRelID;
                Stock.ItemID = item.ItemID;
                Stock.ProductID = item.ProductID;
                Stock.LocationID = item.LocationID;
                Stock.CompanyID = item.CompanyID;
                Stock.Type = item.Type;
                Stock.BatchCode = item.BatchCode;
                Stock.ExpiryDate = item.ExpiryDate;
                Stock.can_expire = item.CanExpire;
                StockList.Add(Stock);
            }

            return StockList;
        }

        public static IEnumerable<StockList> getItems_GroupBy(int? BranchID, DateTime? TransDate, bool forceData, bool InStock_Only)
        {
            var list = getItems_ByBranch((int)BranchID, forceData)
                .GroupBy(x => x.ItemID)
                .Select(x => new
                {
                    Code = x.Max(y => y.Code),
                    Name = x.Max(y => y.Name),
                    Location = x.Max(y => y.Location),
                    Measurement = x.Max(y => y.Measurement),
                    Quantity = x.Sum(y => y.Quantity),
                    ConversionQuantity = x.Sum(y => y.ConversionQuantity),
                    Cost = x.Max(y => y.Cost),
                    MovementID = x.Max(y => y.MovementID),
                    MovementRelID = x.Max(y => y.MovementRelID),
                    ItemID = x.Max(y => y.ItemID),
                    ProductID = x.Max(y => y.ProductID),
                    LocationID = x.Max(y => y.LocationID),
                    CompanyID = x.Max(y => y.CompanyID),
                    Type = x.Max(y => y.Type),
                    CanExpire = x.Max(y => y.can_expire),
                    BatchCode = x.Max(y => y.BatchCode),
                    ExpiryDate = x.Max(y => y.ExpiryDate),
                    TranDate = x.Max(y => y.TranDate),
                    BarCode = x.Max(y => y.BarCode)

                }).ToList();

            if (InStock_Only)
            {
                list = list.Where(x => x.Quantity > 0 || (x.Quantity == 0 && (x.Type == 3 || x.Type == 7 || x.Type == 5))).ToList();
            }

            List<StockList> StockList = new List<StockList>();

            foreach (dynamic item in list)
            {
                StockList Stock = new StockList();
                Stock.IsSelected = false;
                Stock.Code = item.Code;
                Stock.Name = item.Name;
                Stock.Location = item.Location;
                Stock.Measurement = item.Measurement;
                Stock.Quantity = item.Quantity;
                Stock.ConversionQuantity = item.ConversionQuantity;
                Stock.Cost = item.Cost;
                Stock.MovementID = item.MovementID;
                Stock.MovementRelID = item.MovementRelID;
                Stock.ItemID = item.ItemID;
                Stock.ProductID = item.ProductID;
                Stock.LocationID = item.LocationID;
                Stock.CompanyID = item.CompanyID;
                Stock.Type = item.Type;
                Stock.can_expire = item.CanExpire;
                Stock.BatchCode = item.BatchCode;
                Stock.ExpiryDate = item.ExpiryDate;
                Stock.TranDate = item.TranDate;
                Stock.BarCode = item.BarCode;
                StockList.Add(Stock);
            }
            return StockList;
        }

        public static List<StockList> GetList(int BranchID, bool forceData)
        {
            if (List.Count() > 0 && forceData == false)
            {
                //UpdateStock(BranchID, GroupBy);
                if (List.Where(x => (x.BranchID == BranchID || x.BranchID == null)).Count() == 0)
                {
                    //update only stock items
                    UpdateStock(BranchID);
                }
            }
            else //List.Count() == 0 || ForceData == True
            {
                //If IsForced is True or Count is 0 then make a new list. Clean.
                List = new List<StockList>();

                //call data
                GetItems();
                UpdateStock(BranchID);
            }

            return List.Where(x => x.BranchID == BranchID || x.BranchID == null).ToList();
        }


        public static List<StockList> GetListwithoutstock(int BranchID)
        {

            //If IsForced is True or Count is 0 then make a new list. Clean.
            List = new List<StockList>();

            //call data
            GetItems();
            //UpdateStockwithoutstock(BranchID);


            return List.Where(x => x.BranchID == BranchID || x.BranchID == null).ToList();
        }

        public static bool updateStockList(item_movement mov)
        {
            app_location loc = CurrentSession.Locations.Where(x => x.id_location == mov.id_location).FirstOrDefault(); //db.app_location.Find(mov.id_location);

            if (loc != null)
            {
                if (mov.parent != null)
                {
                    StockList updatedMovement = getProducts_InStock(loc.id_branch, DateTime.Now, false).Where(x => x.MovementID == mov.parent.id_movement).FirstOrDefault();

                    if (updatedMovement != null)
                    {
                        updatedMovement.Quantity += mov.credit - mov.debit;
                        return true;
                    }
                }
            }

            return false;
        }

        private static void UpdateStock(int BranchID)
        {
          
            //run code to bring stock.
            if (BranchID > 0)
            {
                                  
                    string strstock = @"
                                 select *                                
                                from (
                                select  
                                l.id_location as LocationID
                                , l.name as Location
                                , l.id_branch as BranchID
                                , im.id_movement as MovementID
                                , ip.id_item as ItemID
                                , ip.id_item_product as ProductID
                                , ip.can_expire
                                , im.id_movement_value_rel as MovementRelID
                                , (select sum(unit_value) from item_movement_value_detail where id_movement_value_rel = im.id_movement_value_rel) as Cost
                                , (im.credit - sum(IFNULL(child.debit,0))) as Quantity
                                , (im.credit - sum(IFNULL(child.debit,0))) * max(icf.value) * (select ROUND(EXP(SUM(LOG(`value`))),4) as value from item_movement_dimension where id_movement = im.id_movement) as ConversionQuantity
                                , im.code as BatchCode
                                , im.expire_date as ExpiryDate
                                ,im.trans_date as TransDate
                                ,im.barcode as BarCode

                                from item_movement as im
                                left join item_movement as child on im.id_movement = child.parent_id_movement
                                inner join item_product as ip on im.id_item_product = ip.id_item_product
                                left join item_conversion_factor as icf on ip.id_item_product = icf.id_item_product
                                inner join app_location as l on im.id_location = l.id_location
                                left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
                                where im.id_company = {0} and l.id_branch = {1}
                                group by im.id_movement
                                order by im.expire_date) as movement 
                                where Quantity > 0";
                                
                //, sum(IFNULL(imvr.total_value, 0)) as Cost

                strstock = String.Format(strstock, CurrentSession.Id_Company, BranchID);

                DataTable dtstock = stock.exeDT(strstock);
              //  dtstock = dtstock.Select("Quantity>0").CopyToDataTable();
                foreach (DataRow itemRow in dtstock.Rows)
                {
                    int ItemID = Convert.ToInt32(itemRow["ItemID"]);
                    int ProductID = Convert.ToInt32(itemRow["ProductID"]);
                    int LocationID = Convert.ToInt32(itemRow["LocationID"]);
                    int MovementID = Convert.ToInt32(itemRow["MovementID"]);
                    int? MovementRelID = null;
                    if (!(itemRow["MovementRelID"] is DBNull))
                    {
                        MovementRelID = Convert.ToInt32(itemRow["MovementRelID"]);
                    }
                    decimal Quantity = Convert.ToDecimal(itemRow["Quantity"]);
                    decimal? ConversionQuantity = null;
                    if (!(itemRow["ConversionQuantity"] is DBNull))
                    {
                        ConversionQuantity = Convert.ToDecimal(itemRow["ConversionQuantity"]);
                    }
                    decimal Cost = 0;
                    if (!(itemRow["Cost"] is DBNull))
                    {
                        Cost = Convert.ToDecimal(itemRow["Cost"]);
                    }

                    string LocationName = Convert.ToString(itemRow["Location"]);
                    string BatchCode = Convert.ToString(itemRow["BatchCode"]);
                    bool CanExpire = Convert.ToBoolean(itemRow["can_expire"]);
                    DateTime TranDate = Convert.ToDateTime(itemRow["TransDate"]);
                    string BarCode = Convert.ToString(itemRow["BarCode"]);

                    DateTime? ExpiryDate = null;

                    if (!itemRow.IsNull("ExpiryDate"))
                    {
                        ExpiryDate = Convert.ToDateTime(itemRow["ExpiryDate"]);
                    }

                    if (List.Where(x => x.MovementID == MovementID).Count() > 0)
                    {
                        //Has MovementID
                        StockList Row = List.Where(x => x.MovementID == MovementID).FirstOrDefault();
                        Row.ProductID = ProductID;
                        Row.LocationID = LocationID;
                        Row.Location = LocationName;
                        Row.MovementID = MovementID;
                        Row.MovementRelID = MovementRelID;
                        Row.BranchID = BranchID;
                        Row.ConversionQuantity = ConversionQuantity;
                        Row.Quantity = Quantity;
                        Row.Cost = Cost;
                        Row.BatchCode = BatchCode;
                        Row.ExpiryDate = ExpiryDate;
                        Row.TimeStamp = DateTime.Now;
                        Row.can_expire = CanExpire;
                        Row.TranDate = TranDate;
                        Row.BarCode = BarCode;
                        //Since movement exists, there is no need to update other data, just get out and continue for.
                        continue;
                    }

                    if (List.Where(x => x.ItemID == ItemID && (x.Quantity == null)).Count() > 0)
                    {
                        StockList Row = List.Where(x => x.ItemID == ItemID && x.Quantity == null).FirstOrDefault();

                        Row.Quantity = Quantity;
                        Row.ConversionQuantity = ConversionQuantity;
                        Row.ProductID = ProductID;
                        Row.LocationID = LocationID;
                        Row.Location = LocationName;
                        Row.MovementID = MovementID;
                        Row.MovementRelID = MovementRelID;
                        Row.BranchID = BranchID;
                        Row.Cost = Cost;
                        Row.BatchCode = BatchCode;
                        Row.ExpiryDate = ExpiryDate;
                        Row.TimeStamp = DateTime.Now;
                        Row.can_expire = CanExpire;
                        Row.TranDate = TranDate;
                        Row.BarCode = BarCode;
                    }
                    else if (List.Where(x => x.ItemID == ItemID).Count() > 0)
                    {
                        StockList Original = List.Where(x => x.ItemID == ItemID).FirstOrDefault();
                        StockList Row = new StockList();
                        Row.ItemID = Original.ItemID;
                        Row.Code = Original.Code;
                        Row.Name = Original.Name;
                        Row.Location = LocationName;
                        Row.LocationID = LocationID;
                        Row.BranchID = BranchID;
                        Row.Measurement = Original.Measurement;
                        Row.ProductID = ProductID;
                        Row.IsActive = Original.IsActive;
                        Row.CompanyID = Original.CompanyID;
                        Row.Type = Original.Type;
                        Row.Quantity = Quantity;
                        Row.ConversionQuantity = ConversionQuantity;
                        Row.Cost = Cost;
                        Row.BatchCode = BatchCode;
                        Row.ExpiryDate = ExpiryDate;
                        Row.MovementID = MovementID;
                        Row.MovementRelID = MovementRelID;
                        Row.TimeStamp = DateTime.Now;
                        Row.can_expire = CanExpire;
                        Row.TranDate = TranDate;
                        Row.BarCode = BarCode;
                        List.Add(Row);
                    }
                }
            }
        }

        //private static void UpdateStockwithoutstock(int BranchID)
        //{
        //    //run code to bring stock.
        //    if (BranchID > 0)
        //    {
        //        string strstock = @"
        //                        select *                                
        //                        from (
        //                        select  
        //                        l.id_location as LocationID
        //                        , l.name as Location
        //                        , l.id_branch as BranchID
        //                        , im.id_movement as MovementID
        //                        , ip.id_item as ItemID
        //                        , ip.id_item_product as ProductID
        //                        , ip.can_expire
        //                        , im.id_movement_value_rel as MovementRelID
        //                        , (select sum(unit_value) from item_movement_value_detail where id_movement_value_rel = im.id_movement_value_rel) as Cost
        //                        , (im.credit - sum(IFNULL(child.debit,0))) as Quantity
        //                        , (im.credit - sum(IFNULL(child.debit,0))) * max(icf.value) * (select ROUND(EXP(SUM(LOG(`value`))),4) as value from item_movement_dimension where id_movement = im.id_movement) as ConversionQuantity
        //                        , im.code as BatchCode
        //                        , im.expire_date as ExpiryDate
        //                        ,im.trans_date as TransDate
        //                        ,im.barcode as BarCode

        //                        from item_movement as im
        //                        left join item_movement as child on im.id_movement = child.parent_id_movement
        //                        inner join item_product as ip on im.id_item_product = ip.id_item_product
        //                        left join item_conversion_factor as icf on ip.id_item_product = icf.id_item_product
        //                        inner join app_location as l on im.id_location = l.id_location
        //                        left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
        //                        where im.id_company = {0} and l.id_branch = {1} 
        //                        group by im.id_movement
        //                        order by im.expire_date) as movement 
        //                        where Quantity = 0";
        //        //, sum(IFNULL(imvr.total_value, 0)) as Cost

        //        strstock = String.Format(strstock, CurrentSession.Id_Company, BranchID);

        //        DataTable dtstock = stock.exeDT(strstock);
        //        if (dtstock.Rows.Count > 0)
        //        {


        //            dtstock = dtstock.Select("MovementID>0").CopyToDataTable();
        //            foreach (DataRow itemRow in dtstock.Rows)
        //            {
        //                int ItemID = Convert.ToInt32(itemRow["ItemID"]);
        //                int ProductID = Convert.ToInt32(itemRow["ProductID"]);
        //                int LocationID = Convert.ToInt32(itemRow["LocationID"]);
        //                int MovementID = Convert.ToInt32(itemRow["MovementID"]);
        //                int? MovementRelID = null;
        //                if (!(itemRow["MovementRelID"] is DBNull))
        //                {
        //                    MovementRelID = Convert.ToInt32(itemRow["MovementRelID"]);
        //                }
        //                decimal Quantity = Convert.ToDecimal(itemRow["Quantity"]);
        //                decimal? ConversionQuantity = null;
        //                if (!(itemRow["ConversionQuantity"] is DBNull))
        //                {
        //                    ConversionQuantity = Convert.ToDecimal(itemRow["ConversionQuantity"]);
        //                }
        //                decimal Cost = 0;
        //                if (!(itemRow["Cost"] is DBNull))
        //                {
        //                    Cost = Convert.ToDecimal(itemRow["Cost"]);
        //                }
        //                string LocationName = Convert.ToString(itemRow["Location"]);
        //                string BatchCode = Convert.ToString(itemRow["BatchCode"]);
        //                bool CanExpire = Convert.ToBoolean(itemRow["can_expire"]);
        //                DateTime TranDate = Convert.ToDateTime(itemRow["TransDate"]);
        //                string BarCode = Convert.ToString(itemRow["BarCode"]);

        //                DateTime? ExpiryDate = null;

        //                if (!itemRow.IsNull("ExpiryDate"))
        //                {
        //                    ExpiryDate = Convert.ToDateTime(itemRow["ExpiryDate"]);
        //                }

        //                if (List.Where(x => x.MovementID == MovementID).Count() > 0)
        //                {
        //                    //Has MovementID
        //                    StockList Row = List.Where(x => x.MovementID == MovementID).FirstOrDefault();
        //                    Row.ProductID = ProductID;
        //                    Row.LocationID = LocationID;
        //                    Row.Location = LocationName;
        //                    Row.MovementID = MovementID;
        //                    Row.MovementRelID = MovementRelID;
        //                    Row.BranchID = BranchID;
        //                    Row.ConversionQuantity = ConversionQuantity;
        //                    Row.Quantity = Quantity;
        //                    Row.Cost = Cost;
        //                    Row.BatchCode = BatchCode;
        //                    Row.ExpiryDate = ExpiryDate;
        //                    Row.TimeStamp = DateTime.Now;
        //                    Row.can_expire = CanExpire;
        //                    Row.TranDate = TranDate;
        //                    Row.BarCode = BarCode;
        //                    //Since movement exists, there is no need to update other data, just get out and continue for.
        //                    continue;
        //                }

        //                if (List.Where(x => x.ItemID == ItemID && (x.Quantity == null)).Count() > 0)
        //                {
        //                    StockList Row = List.Where(x => x.ItemID == ItemID && x.Quantity == null).FirstOrDefault();

        //                    Row.Quantity = Quantity;
        //                    Row.ConversionQuantity = ConversionQuantity;
        //                    Row.ProductID = ProductID;
        //                    Row.LocationID = LocationID;
        //                    Row.Location = LocationName;
        //                    Row.MovementID = MovementID;
        //                    Row.MovementRelID = MovementRelID;
        //                    Row.BranchID = BranchID;
        //                    Row.Cost = Cost;
        //                    Row.BatchCode = BatchCode;
        //                    Row.ExpiryDate = ExpiryDate;
        //                    Row.TimeStamp = DateTime.Now;
        //                    Row.can_expire = CanExpire;
        //                    Row.TranDate = TranDate;
        //                    Row.BarCode = BarCode;
        //                }
        //                else if (List.Where(x => x.ItemID == ItemID).Count() > 0)
        //                {
        //                    StockList Original = List.Where(x => x.ItemID == ItemID).FirstOrDefault();
        //                    StockList Row = new StockList();
        //                    Row.ItemID = Original.ItemID;
        //                    Row.Code = Original.Code;
        //                    Row.Name = Original.Name;
        //                    Row.Location = LocationName;
        //                    Row.LocationID = LocationID;
        //                    Row.BranchID = BranchID;
        //                    Row.Measurement = Original.Measurement;
        //                    Row.ProductID = ProductID;
        //                    Row.IsActive = Original.IsActive;
        //                    Row.CompanyID = Original.CompanyID;
        //                    Row.Type = Original.Type;
        //                    Row.Quantity = Quantity;
        //                    Row.ConversionQuantity = ConversionQuantity;
        //                    Row.Cost = Cost;
        //                    Row.BatchCode = BatchCode;
        //                    Row.ExpiryDate = ExpiryDate;
        //                    Row.MovementID = MovementID;
        //                    Row.MovementRelID = MovementRelID;
        //                    Row.TimeStamp = DateTime.Now;
        //                    Row.can_expire = CanExpire;
        //                    Row.TranDate = TranDate;
        //                    Row.BarCode = BarCode;
        //                    List.Add(Row);
        //                }
        //            }
        //        }
        //    }
        //}

        public static void GetItems()
        {
            //run code to bring active items.
            string stritem = @"
                                select 
                                i.name as ItemName
                                , i.code as ItemCode
                                , measure.name as Measurement
                                , brand.name as Brand
                                , i.is_active as IsActive
                                , i.id_item_type as Type, i.id_company as CompanyID
                                , i.id_item as ItemID 
                                from items as i
                                left join item_brand as brand on brand.id_brand = i.id_brand
                                left join app_measurement as measure on i.id_measurement = measure.id_measurement
                                where i.is_active = 1 and i.id_company = {0}";

            stritem = String.Format(stritem, CurrentSession.Id_Company);

            DataTable dtitem = stock.exeDT(stritem);

            foreach (DataRow itemRow in dtitem.Rows)
            {
                StockList data = new StockList();
                data.ItemID = Convert.ToInt32(itemRow["ItemID"]);
                data.CompanyID = Convert.ToInt32(itemRow["CompanyID"]);
                data.Name = Convert.ToString(itemRow["ItemName"]);
                data.Code = Convert.ToString(itemRow["ItemCode"]);
                data.Measurement = Convert.ToString(itemRow["Measurement"]);
                data.Brand = Convert.ToString(itemRow["Brand"]);
                data.Type = Convert.ToInt32(itemRow["Type"]);
                data.can_expire = false;
                List.Add(data);
            }
        }
    }
}