using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace entity
{
    public partial class TaskViewDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Task();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Task();
            return base.SaveChangesAsync();
        }

        private void validate_Task()
        {
            foreach (project_task project_task in base.project_task.Local)
            {
                if (project_task.IsSelected)
                {
                    if (project_task.State == EntityState.Added)
                    {
                        project_task.timestamp = DateTime.Now;
                        project_task.State = EntityState.Unchanged;
                        Entry(project_task).State = EntityState.Added;
                    }
                    else if (project_task.State == EntityState.Modified)
                    {
                        project_task.timestamp = DateTime.Now;
                        project_task.State = EntityState.Unchanged;
                        Entry(project_task).State = EntityState.Modified;
                    }
                    else if (project_task.State == EntityState.Deleted)
                    {
                        project_task.timestamp = DateTime.Now;
                        project_task.State = EntityState.Unchanged;
                        base.project_task.Remove(project_task);
                    }
                }
                else if (project_task.State > 0)
                {
                    if (project_task.State != EntityState.Unchanged)
                    {
                        Entry(project_task).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}