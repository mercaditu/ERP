using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace entity.EventManagement
{
    public class TemplateDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Template();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Template();
            return base.SaveChangesAsync();
        }

        private void validate_Template()
        {
            foreach (project_event_template project_event_template in base.project_event_template.Local)
            {
                if (project_event_template.IsSelected && project_event_template.Error == null)
                {
                    if (project_event_template.State == EntityState.Added)
                    {
                        project_event_template.timestamp = DateTime.Now;
                        project_event_template.State = EntityState.Unchanged;
                        Entry(project_event_template).State = EntityState.Added;
                    }
                    else if (project_event_template.State == EntityState.Modified)
                    {
                        project_event_template.timestamp = DateTime.Now;
                        project_event_template.State = EntityState.Unchanged;
                        Entry(project_event_template).State = EntityState.Modified;
                    }
                    else if (project_event_template.State == EntityState.Deleted)
                    {
                        project_event_template.timestamp = DateTime.Now;
                        project_event_template.is_head = false;
                    }
                }
                else if (project_event_template.State > 0)
                {
                    if (project_event_template.State != EntityState.Unchanged)
                    {
                        Entry(project_event_template).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}