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
            List = new ICollection<Item>;
        }
    }

    public class Item
    {
        
    }

    public class ItemProduct
    {

    }

    public class Tag
    {

    }
}
