using System.Data.Entity;
using System.Threading.Tasks;

namespace entity
{
    public partial class UserRoleDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_UserRole();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_UserRole();
            return base.SaveChangesAsync();
        }

        private void validate_UserRole()
        {
            foreach (security_role security_role in base.security_role.Local)
            {
                if (security_role.IsSelected && security_role.Error == null)
                {
                    if (security_role.State == EntityState.Added)
                    {
                        security_role.State = EntityState.Unchanged;
                        Entry(security_role).State = EntityState.Added;
                    }
                    else if (security_role.State == EntityState.Modified)
                    {
                        security_role.State = EntityState.Unchanged;
                        Entry(security_role).State = EntityState.Modified;
                    }
                    else if (security_role.State == EntityState.Deleted)
                    {
                        security_role.State = EntityState.Unchanged;
                        base.security_role.Remove(security_role);
                    }
                }
                else if (security_role.State > 0)
                {
                    if (security_role.State != EntityState.Unchanged)
                    {
                        Entry(security_role).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}