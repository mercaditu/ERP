using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class UserDB : BaseDB
    {

        public override int SaveChanges()
        {
            validate_User();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_User();
            return base.SaveChangesAsync();
        }

        private void validate_User()
        {
            foreach (security_user security_user in base.security_user.Local)
            {
                if (security_user.IsSelected && security_user.Error == null)
                {
                    if (security_user.State == EntityState.Added)
                    {
                        //security_user.timestamp = DateTime.Now;
                        security_user.State = EntityState.Unchanged;
                        Entry(security_user).State = EntityState.Added;
                    }
                    else if (security_user.State == EntityState.Modified)
                    {
                        //security_user.timestamp = DateTime.Now;
                        security_user.State = EntityState.Unchanged;
                        Entry(security_user).State = EntityState.Modified;
                    }
                    else if (security_user.State == EntityState.Deleted)
                    {
                        //security_user.timestamp = DateTime.Now;
                        security_user.State = EntityState.Unchanged;
                        base.security_user.Remove(security_user);
                    }
                }
                else
                {
                    Entry(security_user).State = EntityState.Unchanged;
                }
            }
        }
    }
}