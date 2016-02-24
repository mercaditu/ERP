using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace entity
{
    public partial class StockDB : BaseDB
    {
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        public decimal getStockCount (int id_location, int id_item_product)
        {
            //return base.item_movement.Where(x => x.id_location == id_location
            //                && x.id_item_product == id_item_product
            //                && x.status == Status.Stock.InStock).Sum(u => u.credit - u.debit);
            return 0;
        }
    }
}
