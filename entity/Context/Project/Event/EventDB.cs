using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.EventManagement
{
    
    public class EventDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Event();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Event();
            return base.SaveChangesAsync();
        }

        private void validate_Event()
        {
            foreach (project_event project_event in base.project_event.Local)
            {
                if (project_event.IsSelected && project_event.Error == null)
                {
                    if (project_event.State == EntityState.Added)
                    {
                        project_event.timestamp = DateTime.Now;
                        project_event.State = EntityState.Unchanged;
                        Entry(project_event).State = EntityState.Added;
                    }
                    else if (project_event.State == EntityState.Modified)
                    {
                        project_event.timestamp = DateTime.Now;
                        project_event.State = EntityState.Unchanged;
                        Entry(project_event).State = EntityState.Modified;
                    }
                    else if (project_event.State == EntityState.Deleted)
                    {
                        project_event.timestamp = DateTime.Now;
                        project_event.is_head = false;
                    }
                }
                else if (project_event.State > 0)
                {
                    if (project_event.State != EntityState.Unchanged)
                    {
                        Entry(project_event).State = EntityState.Unchanged;
                    }
                }
            }
        }


    }
}
