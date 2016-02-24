using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class hr_TalentDB: BaseDB
    {
        public override int SaveChanges()
        {
            validate_Talent();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Talent();
            return base.SaveChangesAsync();
        }

        private void validate_Talent()
        {
            foreach (hr_talent hr_talent in base.hr_talent.Local)
            {
                if (hr_talent.IsSelected)
                {
                    if (hr_talent.State == EntityState.Added)
                    {
                        hr_talent.timestamp = DateTime.Now;
                        hr_talent.State = EntityState.Unchanged;
                        Entry(hr_talent).State = EntityState.Added;
                    }
                    else if (hr_talent.State == EntityState.Modified)
                    {
                        hr_talent.timestamp = DateTime.Now;
                        hr_talent.State = EntityState.Unchanged;
                        Entry(hr_talent).State = EntityState.Modified;
                    }
                    else if (hr_talent.State == EntityState.Deleted)
                    {
                        hr_talent.timestamp = DateTime.Now;
                        hr_talent.State = EntityState.Unchanged;
                        base.hr_talent.Remove(hr_talent);
                    }
                }
                else if (hr_talent.State > 0)
                {
                    if (hr_talent.State != EntityState.Unchanged)
                    {
                        Entry(hr_talent).State = EntityState.Unchanged;
                    }
                }
            }
        }
   
    }
}
