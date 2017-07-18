using System;
using System.Collections.Generic;
using System.Data;

namespace entity.BrilloQuery
{
    public class GetItems : IDisposable
    {
        public ICollection<Item> Items { get; set; }

        public GetItems()
        {
            Items = new List<Item>();
            string query = @"SET sql_mode = '';
							select
								 item.id_item as ID,
								 item.code as Code,
								 item.name as Name,
								 brand.name as Brand,
								 item.id_company as CompanyID,
								 item.is_active as IsActive,
								 item.id_item_type,
								 loc.name as Location,
                                 branch.name as Branch,
								 (sum(mov.credit) - sum(mov.debit)) as Quantity

								 from items as item

								 left outer join item_product as prod on prod.id_item = item.id_item
								 left outer join item_brand as brand on brand.id_brand = item.id_brand
								 left outer join item_movement as mov on mov.id_item_product = prod.id_item_product
								 left outer join app_location as loc on mov.id_location = loc.id_location
								 left outer join app_branch as branch on loc.id_branch = branch.id_branch

								 where (item.id_company = {0} or item.id_company is null)
									and (loc.id_branch = {1} or mov.id_location is null or prod.id_item_product is null)
									and item.is_active = 1

								 group by item.id_item
								 order by item.name";

            query = string.Format(query, CurrentSession.Id_Company, CurrentSession.Id_Branch);

            using (DataTable dt = QueryExecutor.DT(query))
            {
                foreach (DataRow DataRow in dt.Rows)
                {
                    bool Is_Product = false;

                    //Item Type will determine if it can stock (Is Product) or not.
                    int type = Convert.ToInt16(DataRow["id_item_type"]);
                    if (type == 1 || type == 2 || type == 6)
                    {
                        Is_Product = true;
                    }

                    Item Item = new Item()
                    {
                        ID = Convert.ToInt32(DataRow["ID"]),
                        Type = (item.item_type)type,
                        IsProduct = Is_Product,
                        IsActive = Convert.ToBoolean(DataRow["IsActive"]),
                        Name = Convert.ToString(DataRow["Name"]),
                        Code = Convert.ToString(DataRow["Code"]),
                        Brand = Convert.ToString(DataRow["Brand"]),
                        InStock = Convert.ToDecimal(DataRow["Quantity"] is DBNull ? 0 : DataRow["Quantity"])
                    };

                    if (!(DataRow["CompanyID"] is DBNull))
                    {
                        Item.ComapnyID = Convert.ToInt16(DataRow["CompanyID"]);
                    }

                    Items.Add(Item);
                }
            }
        }
        
        public GetItems(int ItemID, int LocationID)
        {
            Items = new List<Item>();
            string query = @"SET sql_mode = '';
							select
								 item.id_item as ID,
								 item.code as Code,
								 item.name as Name,
								 brand.name as Brand,
								 item.id_company as CompanyID,
								 item.is_active as IsActive,
								 item.id_item_type,
								 loc.name as Location,
                                 loc.id_location as LocationID,
								 branch.name as Branch,
								 (sum(mov.credit) - sum(mov.debit)) as Quantity

								 from items as item

								 join item_product as prod on prod.id_item = item.id_item
								 left outer join item_brand as brand on brand.id_brand = item.id_brand
								 join item_movement as mov on mov.id_item_product = prod.id_item_product
							     join app_location as loc on mov.id_location = loc.id_location
								 join app_branch as branch on loc.id_branch = branch.id_branch

								 where item.id_company = {0}
									and mov.id_location = {1}
									and item.is_active = 1 and item.id_item={2} 
								 group by item.id_item";

            query = string.Format(query, CurrentSession.Id_Company, LocationID, ItemID);

            using (DataTable dt = QueryExecutor.DT(query))
            {
                foreach (DataRow DataRow in dt.Rows)
                {
                    bool Is_Product = false;

                    //Item Type will determine if it can stock (Is Product) or not.
                    int type = Convert.ToInt16(DataRow["id_item_type"]);
                    if (type == 1 || type == 2 || type == 6)
                    {
                        Is_Product = true;
                    }

                    Item Item = new Item();
                    Item.ID = Convert.ToInt16(DataRow["ID"]);
                    Item.Type = (item.item_type)type;
                    Item.IsProduct = Is_Product;
                    Item.IsActive = Convert.ToBoolean(DataRow["IsActive"]);
                    if (!(DataRow["CompanyID"] is DBNull))
                    {
                        Item.ComapnyID = Convert.ToInt16(DataRow["CompanyID"]);
                    }

                    Item.Name = Convert.ToString(DataRow["Name"]);
                    Item.Code = Convert.ToString(DataRow["Code"]);
                    Item.Brand = Convert.ToString(DataRow["Brand"]);
                    Item.InStock = Convert.ToDecimal(DataRow["Quantity"] is DBNull ? 0 : DataRow["Quantity"]);
                    Item.LocationID = Convert.ToInt32(DataRow["LocationID"]); ;

                    Items.Add(Item);
                }
            }

        }

        public GetItems(int LocationID)
        {
            Items = new List<Item>();
            string query = @"SET sql_mode = '';
							select
								 item.id_item as ID,
								 item.code as Code,
								 item.name as Name,
								 brand.name as Brand,
								 item.id_company as CompanyID,
								 item.is_active as IsActive,
								 item.id_item_type,
								 loc.name as Location,
                                 loc.id_location as LocationID,
								 branch.name as Branch,
								 (sum(mov.credit) - sum(mov.debit)) as Quantity

								 from items as item

                                 left outer join item_product as prod on prod.id_item = item.id_item
								 left outer join item_brand as brand on brand.id_brand = item.id_brand
								 left outer join item_movement as mov on mov.id_item_product = prod.id_item_product
								 left outer join app_location as loc on mov.id_location = loc.id_location
								 left outer join app_branch as branch on loc.id_branch = branch.id_branch

								 where item.id_company = {0}
									and (mov.id_location = {1} or mov.id_location is null)
									and item.is_active = 1 
								 group by item.id_item";

            query = string.Format(query, CurrentSession.Id_Company, LocationID);

            using (DataTable dt = QueryExecutor.DT(query))
            {
                foreach (DataRow DataRow in dt.Rows)
                {
                    bool Is_Product = false;

                    //Item Type will determine if it can stock (Is Product) or not.
                    int type = Convert.ToInt16(DataRow["id_item_type"]);
                    if (type == 1 || type == 2 || type == 6)
                    {
                        Is_Product = true;
                    }

                    Item Item = new Item();
                    Item.ID = Convert.ToInt16(DataRow["ID"]);
                    Item.Type = (item.item_type)type;
                    Item.IsProduct = Is_Product;
                    Item.IsActive = Convert.ToBoolean(DataRow["IsActive"]);
                    if (!(DataRow["CompanyID"] is DBNull))
                    {
                        Item.ComapnyID = Convert.ToInt16(DataRow["CompanyID"]);
                    }

                    Item.Name = Convert.ToString(DataRow["Name"]);
                    Item.Code = Convert.ToString(DataRow["Code"]);
                    Item.Brand = Convert.ToString(DataRow["Brand"]);
                    Item.InStock = Convert.ToDecimal(DataRow["Quantity"] is DBNull ? 0 : DataRow["Quantity"]);
                    Item.LocationID = Convert.ToInt32(DataRow["LocationID"] is DBNull ? 0 : DataRow["LocationID"]);

                    Items.Add(Item);
                }
            }
        }
        
        public List<Item> GetItemsByLocation()
        {
            List<Item> ItemsLocation = new List<Item>();
            string query = @"SET sql_mode = '';
							select
								 item.id_item as ID,
								 item.code as Code,
								 item.name as Name,
								 brand.name as Brand,
								 item.id_company as CompanyID,
								 item.is_active as IsActive,
								 item.id_item_type,
								 loc.name as Location,
                                 loc.id_location as LocationID,
								 branch.name as Branch,
								 (sum(mov.credit) - sum(mov.debit)) as Quantity

								 from items as item

								 left outer join item_product as prod on prod.id_item = item.id_item
								 left outer join item_brand as brand on brand.id_brand = item.id_brand
								 left outer join item_movement as mov on mov.id_item_product = prod.id_item_product
								 left outer join app_location as loc on mov.id_location = loc.id_location
								 left outer join app_branch as branch on loc.id_branch = branch.id_branch

								 where (item.id_company = {0} or item.id_company is null)
									and (loc.id_branch = {1} or loc.id_branch is null)
									and item.is_active = 1

								 group by item.id_item,loc.id_location
								 order by item.name";

            query = string.Format(query, CurrentSession.Id_Company, CurrentSession.Id_Branch);

            using (DataTable dt = QueryExecutor.DT(query))
            {
                foreach (DataRow DataRow in dt.Rows)
                {
                    bool Is_Product = false;

                    //Item Type will determine if it can stock (Is Product) or not.
                    int type = Convert.ToInt16(DataRow["id_item_type"]);
                    if (type == 1 || type == 2 || type == 6)
                    {
                        Is_Product = true;
                    }

                    Item Item = new Item();
                    Item.ID = Convert.ToInt16(DataRow["ID"]);
                    Item.Type = (item.item_type)type;
                    Item.IsProduct = Is_Product;
                    Item.IsActive = Convert.ToBoolean(DataRow["IsActive"]);
                    if (!(DataRow["CompanyID"] is DBNull))
                    {
                        Item.ComapnyID = Convert.ToInt16(DataRow["CompanyID"]);
                    }

                    Item.Name = Convert.ToString(DataRow["Name"]);
                    Item.Code = Convert.ToString(DataRow["Code"]);
                    Item.Brand = Convert.ToString(DataRow["Brand"]);
                    Item.InStock = Convert.ToDecimal(DataRow["Quantity"] is DBNull ? 0 : DataRow["Quantity"]);
                    Item.LocationID = Convert.ToInt32(DataRow["LocationID"]); ;

                    ItemsLocation.Add(Item);
                }
            }
            return ItemsLocation;
        }

        public void Dispose()
        {
            // Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class GetProducts
    {
        private ICollection<Item> Items { get; set; }

        public GetProducts()
        {
            Items = new List<Item>();
        }
    }

    public class Item
    {
        public int? ComapnyID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public item.item_type Type { get; set; }
        public string Brand { get; set; }
        public bool IsActive { get; set; }
        public bool IsProduct { get; set; }

        public decimal Location { get; set; }
        public int? LocationID { get; set; }
        public decimal InStock { get; set; }
        public decimal Cost { get; set; }

        private ICollection<Tag> Tags { get; set; }

        public Item()
        {
            Tags = new List<Tag>();
        }
    }

    public class Tag
    {
        public string Name { get; set; }
        private ICollection<Item> Item { get; set; }
    }
}