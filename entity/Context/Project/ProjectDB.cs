using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace entity
{
   
    public class ProjectDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Project();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Project();
            return base.SaveChangesAsync();
        }

        private void validate_Project()
        {
            foreach (project project in base.projects.Local)
            {
                if (project.IsSelected && project.Error == null)
                {
                    if (project.State == EntityState.Added)
                    {
                        project.timestamp = DateTime.Now;
                        project.State = EntityState.Unchanged;
                        Entry(project).State = EntityState.Added;
                    }
                    else if (project.State == EntityState.Modified)
                    {
                        project.timestamp = DateTime.Now;
                        project.State = EntityState.Unchanged;
                        Entry(project).State = EntityState.Modified;
                    }
                    else if (project.State == EntityState.Deleted)
                    {
                        project.timestamp = DateTime.Now;
                        project.State = EntityState.Unchanged;
                        base.projects.Remove(project);
                    }
                }
                else if (project.State > 0)
                {
                    if (project.State != EntityState.Unchanged)
                    {
                        Entry(project).State = EntityState.Unchanged;
                    }
                }
            }
        }

      
    }
}
