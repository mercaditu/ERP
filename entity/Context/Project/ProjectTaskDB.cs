using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;

namespace entity
{

    public class ProjectTaskDB : BaseDB
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
            NumberOfRecords = 0;

            List<project_task> list = base.project_task.Local.ToList();

            foreach (project_task project_task in list)
            {
                if (project_task.State == EntityState.Added)
                {
                    project_task.timestamp = DateTime.Now;
                    project_task.State = EntityState.Unchanged;
                    Entry(project_task).State = EntityState.Added;

                    NumberOfRecords += 1;
                }
                else if (project_task.State == EntityState.Modified)
                {
                    project_task.timestamp = DateTime.Now;
                    project_task.State = EntityState.Unchanged;
                    Entry(project_task).State = EntityState.Modified;

                    NumberOfRecords += 1;
                }
                else if (project_task.State == EntityState.Deleted)
                {
                    project_task.timestamp = DateTime.Now;
                    project_task.State = EntityState.Unchanged;
                    Entry(project_task).State = EntityState.Deleted;

                    NumberOfRecords += 1;
                }
            }
        }
    }
}
