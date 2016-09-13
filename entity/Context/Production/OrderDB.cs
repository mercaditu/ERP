using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class OrderDB : BaseDB
    {
        public production_order New(string name, production_order.ProductionOrderTypes Type, int Line)
        {
            production_order production_order = new production_order();
            production_order.id_production_line = Line;
            production_order.type = Type;
            production_order.trans_date = DateTime.Now;
            production_order.status = Status.Production.Pending;
            production_order.name = name;
            production_order.IsSelected = true;
            return production_order;
        }


        public override int SaveChanges()
        {
            validate_order();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_order();
            return base.SaveChangesAsync();
        }

        private void validate_order()
        {
            NumberOfRecords = 0;
            foreach (production_order production_order in base.production_order.Local)
            {
                if (production_order.IsSelected && production_order.Error == null)
                {
                    if (production_order.State == EntityState.Added)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        Entry(production_order).State = EntityState.Added;
                    }
                    else if (production_order.State == EntityState.Modified)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        Entry(production_order).State = EntityState.Modified;
                    }
                    else if (production_order.State == EntityState.Deleted)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        base.production_order.Remove(production_order);
                    }
                    NumberOfRecords += 1;
                }
                else if (production_order.State > 0)
                {
                    if (production_order.State != EntityState.Unchanged)
                    {
                        Entry(production_order).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
