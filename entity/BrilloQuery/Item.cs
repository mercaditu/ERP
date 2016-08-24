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
        public ICollection<Item> List { get; set; }
        
        public GetItems()
        {
            List = new List<Item>();
            string query = @" select 
                             item.id_item as ID, 
                             item.code as ItemCode, 
                             item.name as ItemName,
                             loc.id_location as LocationID, loc.name as Location,
                             (sum(mov.credit) - sum(mov.debit)) as Quantity
                             from items as item
                             
                             left join item_product as prod on prod.id_item = item.id_item
                             left join item_movement as mov on mov.id_item_product = prod.id_item_product  
							 left join app_location as loc on mov.id_location = loc.id_location
							 left join app_branch as branch on loc.id_branch = branch.id_branch
                             where mov.id_company = {0} and branch.id_branch = {1} and mov.trans_date <= '{2}'
                             group by loc.id_location, prod.id_item_product
                             order by mov.trans_date, mov.id_movement
                                ";

            query = String.Format(query, entity.CurrentSession.Id_Company);

            //using (DataTable dt = QueryExecutor.DT(query))
            //{
            //    foreach (DataRow DataRow in dt.Rows)
            //    {
            //        Contact Contact = new Contact();

            //        Contact.ID = Convert.ToInt16(DataRow["ID"]);
            //        Contact.Name = Convert.ToString(DataRow["Name"]);
            //        Contact.Alias = Convert.ToString(DataRow["Alias"]);
            //        Contact.Gov_Code = Convert.ToString(DataRow["Gov_Code"]);
            //        Contact.Code = Convert.ToString(DataRow["Code"]);
            //        Contact.Telephone = Convert.ToString(DataRow["Telephone"]);
            //        Contact.Email = Convert.ToString(DataRow["Email"]);
            //        Contact.Address = Convert.ToString(DataRow["Address"]);
            //        Contact.IsCustomer = Convert.ToBoolean(DataRow["IsCustomer"]);
            //        Contact.IsSupplier = Convert.ToBoolean(DataRow["IsSupplier"]);
            //        Contact.IsEmployee = Convert.ToBoolean(DataRow["IsEmployee"]);

            //        List.Add(Contact);
            //    }
            //}
        }
    }

    public class GetProducts
    {
        ICollection<Item> List { get; set; }

        public GetProducts()
        {
            List = new List<Item>();
        }
    }

    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public item.item_type Type { get; set; }
        public string BrandName { get; set; }

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
