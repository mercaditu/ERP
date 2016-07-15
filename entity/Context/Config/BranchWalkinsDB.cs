using System; 
using System.Data.Entity;
using System.Linq; 
using System.Threading.Tasks;

namespace entity 
{
    public partial class BranchWalkinsDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_BranchWalkins();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_BranchWalkins();
            return base.SaveChangesAsync();
        }

        private void validate_BranchWalkins()
        {
            foreach (app_branch_walkins app_branch_walkins in base.app_branch_walkins.Local)
            {
                if (app_branch_walkins.IsSelected )
                {
                    if (app_branch_walkins.State == EntityState.Added)
                    {
                        app_branch_walkins.timestamp = DateTime.Now;
                        app_branch_walkins.State = EntityState.Unchanged;
                        Entry(app_branch_walkins).State = EntityState.Added;
                    }
                    else if (app_branch_walkins.State == EntityState.Modified)
                    {
                        app_branch_walkins.timestamp = DateTime.Now;
                        app_branch_walkins.State = EntityState.Unchanged;
                        Entry(app_branch_walkins).State = EntityState.Modified;
                    }
                    else if (app_branch_walkins.State == EntityState.Deleted)
                    {
                        app_branch_walkins.timestamp = DateTime.Now;
                        app_branch_walkins.State = EntityState.Unchanged;
                        base.app_branch_walkins.Remove(app_branch_walkins);
                    }
                }
                else if (app_branch_walkins.State > 0)
                {
                    if (app_branch_walkins.State != EntityState.Unchanged)
                    {
                        Entry(app_branch_walkins).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
