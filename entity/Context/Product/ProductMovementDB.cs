using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class ProductMovementDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_ProductMovement();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_ProductMovement();
            return base.SaveChangesAsync();
        }

        private void validate_ProductMovement()
        {
            foreach (item_movement item_movement in base.item_movement.Local)
            {
                if (item_movement.IsSelected)
                {
                    if (item_movement.State == EntityState.Added)
                    {
                        item_movement.timestamp = DateTime.Now;
                        item_movement.State = EntityState.Unchanged;
                        Entry(item_movement).State = EntityState.Added;
                    }
                    else if (item_movement.State == EntityState.Modified)
                    {
                        item_movement.timestamp = DateTime.Now;
                        item_movement.State = EntityState.Unchanged;
                        Entry(item_movement).State = EntityState.Modified;
                    }
                    else if (item_movement.State == EntityState.Deleted)
                    {
                        item_movement.timestamp = DateTime.Now;
                        item_movement.State = EntityState.Unchanged;
                        base.item_movement.Remove(item_movement);
                    }
                }
                else if (item_movement.State > 0)
                {
                    if (item_movement.State != EntityState.Unchanged)
                    {
                        Entry(item_movement).State = EntityState.Unchanged;
                    }
                }
            }
        }

         private void ReArrange_ProductMovement()
        {
            List<item_movement> item_movementcredit = base.item_movement.OrderBy(x => new { x.trans_date, x.credit }).Where(x=>x.credit>0).ToList();
            List<item_movement> item_movementdebit = base.item_movement.OrderBy(x => new { x.trans_date, x.credit }).Where(x => x.debit > 0).ToList();
            foreach (item_movement _item_movementdebit in item_movementdebit)
            {
                foreach (item_movement _item_movementcredit in item_movementcredit)
                {
                    //if (_item_movementdebit)
                    //{
                        
                    //}
                }
            }
        }
    }
}
