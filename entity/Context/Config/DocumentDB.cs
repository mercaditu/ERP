using System; 
using System.Data.Entity;
using System.Linq; 
using System.Threading.Tasks;

namespace entity 
{
     public partial class DocumentDB:BaseDB
    {
        public override int SaveChanges()
        {
            validate_Document();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Document();
            return base.SaveChangesAsync();
        }

        private void validate_Document()
        {
            foreach (app_document app_document in base.app_document.Local)
            {
                if (app_document.IsSelected && app_document.Error == null)
                {
                    if (app_document.State == EntityState.Added)
                    {
                        app_document.timestamp = DateTime.Now;
                        app_document.State = EntityState.Unchanged;
                        Entry(app_document).State = EntityState.Added;
                    }
                    else if (app_document.State == EntityState.Modified)
                    {
                        app_document.timestamp = DateTime.Now;
                        app_document.State = EntityState.Unchanged;
                        Entry(app_document).State = EntityState.Modified;
                    }
                    else if (app_document.State == EntityState.Deleted)
                    {
                        app_document.timestamp = DateTime.Now;
                        app_document.State = EntityState.Unchanged;
                        base.app_document.Remove(app_document);
                    }
                }
                else if (app_document.State > 0)
                {
                    if (app_document.State != EntityState.Unchanged)
                    {
                        Entry(app_document).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
