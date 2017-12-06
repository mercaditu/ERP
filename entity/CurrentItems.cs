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

        private static Stock stock = new Stock();
        private static List<StockList> List { get; set; }
        //private static List<StockList> List
        //{
        //    get { return _List; }
        //    set { _List = value; }
        //}
        //static List<StockList> _List = new List<StockList>();

        public static List<StockList> GetList(int BranchID, bool GroupBy)
        {
            List = new List<StockList>();

            if (List.Count() > 0)
            {
                UpdateStock(BranchID, GroupBy);
                //if (List.Where(x => (x.BranchID == BranchID || x.BranchID == null)).Count() == 0)
                //{
                //    //update only stock items
                //    UpdateStock(BranchID, GroupBy);
                //}
            }
            else
            {
                //call data
                GetItems();
                UpdateStock(BranchID, GroupBy);
            }

            return List.Where(x => x.BranchID == BranchID || x.BranchID == null).ToList();
        }

        public static List<StockList> Refresh(int BranchID, bool GroupBy)
        {
            List = new List<StockList>();
            //call data
            GetItems();
            UpdateStock(BranchID, GroupBy);

            return List.Where(x => x.BranchID == BranchID || x.BranchID == null).ToList();
        }

        private static void UpdateStock(int BranchID, bool GroupBy)
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
                                , max(im.id_movement) as MovementID
                                , ip.id_item as ItemID
                                , ip.id_item_product as ProductID
                                , ip.can_expire
                                , (im.credit - sum(IFNULL(child.debit,0))) as Quantity
                                , sum(IFNULL(imvr.total_value, 0)) as Cost
                                , im.code as BatchCode
                                , im.expire_date as ExpiryDate
                                from item_movement as im
                                left join item_movement as child on im.id_movement = child.parent_id_movement
                                inner join item_product as ip on im.id_item_product = ip.id_item_product
                                inner join app_location as l on im.id_location = l.id_location
                                left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
                                where im.id_company = {0} and l.id_branch = {1}
                                group by im.id_movement
                                order by im.expire_date) as movement 
                                where Quantity > 0";

                strstock = String.Format(strstock, CurrentSession.Id_Company, BranchID);

                DataTable dtstock = stock.exeDT(strstock);

                foreach (DataRow itemRow in dtstock.Rows)
                {
                    int ItemID = Convert.ToInt32(itemRow["ItemID"]);
                    int ProductID = Convert.ToInt32(itemRow["ProductID"]);
                    int LocationID = Convert.ToInt32(itemRow["LocationID"]);
                    int MovementID = Convert.ToInt32(itemRow["MovementID"]);
                    decimal Quantity = Convert.ToDecimal(itemRow["Quantity"]);
                    decimal Cost = Convert.ToDecimal(itemRow["Cost"]);
                    string LocationName = Convert.ToString(itemRow["Location"]);
                    string BatchCode = Convert.ToString(itemRow["BatchCode"]);
                    bool CanExpire = Convert.ToBoolean(itemRow["can_expire"]);

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
                        Row.BranchID = BranchID;
                        Row.Quantity = Quantity;
                        Row.Cost = Cost;
                        Row.BatchCode = BatchCode;
                        Row.ExpiryDate = ExpiryDate;
                        Row.TimeStamp = DateTime.Now;
                        Row.can_expire = CanExpire;
                        //Since movement exists, there is no need to update other data, just get out and continue for.
                        continue;
                    }

                    if (List.Where(x => x.ItemID == ItemID && (x.Quantity == null || GroupBy)).Count() > 0)
                    {
                        StockList Row = List.Where(x => x.ItemID == ItemID && x.Quantity == null).FirstOrDefault();

                        if (Row == null)
                        {
                            Row = List.Where(x => x.ItemID == ItemID).FirstOrDefault();
                            Row.Quantity = Row.Quantity == null ? 0 : Row.Quantity;
                            Row.Quantity += Quantity;
                        }
                        else
                        {
                            Row.Quantity = Quantity;
                        }

                        Row.ProductID = ProductID;
                        Row.LocationID = LocationID;
                        Row.Location = LocationName;
                        Row.MovementID = MovementID;
                        Row.BranchID = BranchID;
                        Row.Cost = Cost;
                        Row.BatchCode = BatchCode;
                        Row.ExpiryDate = ExpiryDate;
                        Row.TimeStamp = DateTime.Now;
                        Row.can_expire = CanExpire;
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
                        Row.Cost = Cost;
                        Row.BatchCode = BatchCode;
                        Row.ExpiryDate = ExpiryDate;
                        Row.MovementID = MovementID;
                        Row.TimeStamp = DateTime.Now;
                        Row.can_expire = CanExpire;
                        List.Add(Row);
                    }
                }
            }
        }

        private static void GetItems()
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
                List.Add(data);
            }
        }
    }
}