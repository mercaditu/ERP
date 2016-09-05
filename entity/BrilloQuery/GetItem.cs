using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.BrilloQuery
{
    public class GetItems
    {
        public ICollection<GetItem> List { get; set; }
        
        public GetItems()
        {
            List = new List<GetItem>();
            string query = @"select 
                             item.id_item as ID, 
                             item.code as ItemCode, 
                             item.name as ItemName,
                             brand.name as Brand,
                             ComapnyID=item.id_company,
                             is_active=item.is_active,
                             id_item_type=item.id_item_type,
                             loc.id_location as LocationID, loc.name as Location,
                             (sum(mov.credit) - sum(mov.debit)) as Quantity
                             from items as item
                             
                             left join item_product as prod on prod.id_item = item.id_item
                               left join item_brand as brand on brand.id_brand = item.id_brand
                             left join item_movement as mov on mov.id_item_product = prod.id_item_product  
							 left join app_location as loc on mov.id_location = loc.id_location
							 left join app_branch as branch on loc.id_branch = branch.id_branch
                         
                        
                             where mov.id_company = {0} and branch.id_branch = {1} and mov.trans_date <= '{2}'
                             group by loc.id_location, prod.id_item_product
                             order by mov.trans_date, mov.id_movement
                                ";

            query = String.Format(query, entity.CurrentSession.Id_Company,CurrentSession.Id_Branch,DateTime.Now);

            using (DataTable dt = QueryExecutor.DT(query))
            {
                foreach (DataRow DataRow in dt.Rows)
                {
                    GetItem GetItem = new GetItem();
                    GetItem.id_item_type = Convert.ToBoolean(DataRow["id_item_type"]);
                    GetItem.is_active = Convert.ToBoolean(DataRow["is_active"]);
                    GetItem.ComapnyID = Convert.ToInt16(DataRow["ComapnyID"]);
                    GetItem.Name = Convert.ToString(DataRow["ItemName"]);
                    GetItem.Code = Convert.ToString(DataRow["ItemCode"]);
                    GetItem.BrandName = Convert.ToString(DataRow["Brand"]);
                    GetItem.InStock = Convert.ToDecimal(DataRow["Quantity"]);
                

                    List.Add(GetItem);
                }
            }
        }
    }

    public class GetProducts
    {
        ICollection<GetItem> List { get; set; }

        public GetProducts()
        {
            List = new List<GetItem>();
        }
    }

    public class GetItem
    {
        public int ComapnyID { get; set; }
        public int ID { get; set; }
        public entity.item.item_type id_item_type { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public item.item_type Type { get; set; }
        public string BrandName { get; set; }
        public bool is_active { get; set; }

        public decimal Location { get; set; }
        public decimal InStock { get; set; }
        public decimal Cost { get; set; }

        ICollection<Tag> Tags { get; set; }
        public GetItem()
        {
            Tags = new List<Tag>();
        }
    }

    public class Tag
    {
        public string Name { get; set; }

        ICollection<GetItem> Item { get; set; }
    }
}
