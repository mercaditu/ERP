using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.BrilloQuery
{
    public class GetItems
    {
        ICollection<Item> List { get; set; }
        
        public GetItems()
        {
            List = new List<Item>();
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
