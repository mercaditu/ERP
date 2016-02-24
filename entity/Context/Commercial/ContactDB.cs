using System;
using System.Threading.Tasks;
using System.Data.Entity;

namespace entity
{
    public partial class ContactDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Contact();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Contact();
            return base.SaveChangesAsync();
        }

        private void validate_Contact()
        {
            foreach (contact contact in base.contacts.Local)
            {
                if (contact.IsSelected )
                {
                    if (contact.State == EntityState.Added)
                    {
                        contact.timestamp = DateTime.Now;
                        contact.State = EntityState.Unchanged;
                        Entry(contact).State = EntityState.Added;
                    }
                    else if (contact.State == EntityState.Modified)
                    {
                        contact.timestamp = DateTime.Now;
                        contact.State = EntityState.Unchanged;
                        Entry(contact).State = EntityState.Modified;
                    }
                    else if (contact.State == EntityState.Deleted)
                    {
                        contact.timestamp = DateTime.Now;
                        contact.State = EntityState.Unchanged;
                        contacts.Remove(contact);
                    }
                }
                else if (contact.State > 0)
                {
                    if (contact.State != EntityState.Unchanged)
                    {
                        Entry(contact).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
