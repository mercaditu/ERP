using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.BrilloQuery
{
    public class GetItems : IDisposable
    {
        public ICollection<Item> Items { get; set; }
        
        public GetItems()
        {
            Items = new List<Item>();
            string query = @"
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

	                             where item.id_company = {0}
	                          
	                             group by item.id_item
	                             order by item.name";

            query = String.Format(query, entity.CurrentSession.Id_Company, CurrentSession.Id_Branch);

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
                    Item.ComapnyID = Convert.ToInt16(DataRow["CompanyID"]);
                    Item.Name = Convert.ToString(DataRow["Name"]);
                    Item.Code = Convert.ToString(DataRow["Code"]);
                    Item.Brand = Convert.ToString(DataRow["Brand"]);
                    Item.InStock = Convert.ToDecimal(DataRow["Quantity"] is DBNull ?0:DataRow["Quantity"]);

                    Items.Add(Item);
                }
            }
        }

        public void Dispose()
        {
            // Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this != null)
            {
                if (disposing)
                {
                    this.Dispose();
                }
                //release unmanaged resources.
            }
        }
    }

    public class GetProducts
    {
        ICollection<Item> Items { get; set; }

        public GetProducts()
        {
            Items = new List<Item>();
        }
    }

    public class Item
    {
        public int ComapnyID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public item.item_type Type { get; set; }
        public string Brand { get; set; }
        public bool IsActive { get; set; }
        public bool IsProduct { get; set; }

        public decimal Location { get; set; }
        public decimal InStock { get; set; }
        public decimal Cost { get; set; }

        ICollection<Tag> Tags { get; set; }
        public Item()
        {
            Tags = new List<Tag>();
        }
    }

    public class Tag
    {
        public string Name { get; set; }
        ICollection<Item> Item { get; set; }
    }
}
