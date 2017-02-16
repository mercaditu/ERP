using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace entity
{
    public partial class LineDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Line();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Line();
            return base.SaveChangesAsync();
        }

        private void validate_Line()
        {
            NumberOfRecords = 0;

            foreach (production_line production_line in base.production_line.Local)
            {
                if (production_line.IsSelected && production_line.Error == null)
                {
                    if (production_line.State == EntityState.Added)
                    {
                        production_line.timestamp = DateTime.Now;
                        production_line.State = EntityState.Unchanged;
                        Entry(production_line).State = EntityState.Added;
                    }
                    else if (production_line.State == EntityState.Modified)
                    {
                        production_line.timestamp = DateTime.Now;
                        production_line.State = EntityState.Unchanged;
                        Entry(production_line).State = EntityState.Modified;
                    }
                    else if (production_line.State == EntityState.Deleted)
                    {
                        production_line.timestamp = DateTime.Now;
                        production_line.State = EntityState.Unchanged;
                        base.production_line.Remove(production_line);
                    }
                    NumberOfRecords += 1;
                }
                else if (production_line.State > 0)
                {
                    if (production_line.State != EntityState.Unchanged)
                    {
                        Entry(production_line).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}