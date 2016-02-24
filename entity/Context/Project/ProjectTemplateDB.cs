using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;


namespace entity
{
    public class ProjectTemplateDB:BaseDB
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
            foreach (project_template project_template in base.project_template.Local)
            {
                if (project_template.IsSelected && project_template.Error == null)
                {
                    if (project_template.State == EntityState.Added)
                    {
                        project_template.timestamp = DateTime.Now;
                        project_template.State = EntityState.Unchanged;
                        Entry(project_template).State = EntityState.Added;
                    }
                    else if (project_template.State == EntityState.Modified)
                    {
                        project_template.timestamp = DateTime.Now;
                        project_template.State = EntityState.Unchanged;
                        Entry(project_template).State = EntityState.Modified;
                    }
                    else if (project_template.State == EntityState.Deleted)
                    {
                        project_template.timestamp = DateTime.Now;
                        project_template.State = EntityState.Unchanged;
                        base.project_template.Remove(project_template);
                    }
                }
                else if (project_template.State > 0)
                {
                    if (project_template.State != EntityState.Unchanged)
                    {
                        Entry(project_template).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public void Approve()
        {
            foreach (project_template project_template in base.project_template.Local.Where(x => x.is_active != true))
            {
                if (project_template.is_active != true &&
                    project_template.IsSelected &&
                    project_template.Error == null)
                {
                    project_template.is_active = true;
                   // project_template.status = Status.Documents_General.Approved;
                }
            }
            SaveChanges();
        }

        public void Anull()
        {
            foreach (project_template project_template in base.project_template.Local.Where(x => x.is_active == true))
            {
                if (project_template.is_active == true &&
                    project_template.IsSelected &&
                    project_template.Error == null)
                {
                    project_template.is_active = false;
                 //   project_template.status = Status.Documents_General.Annulled;
                }
            }
            SaveChanges();
        }
    }
}
