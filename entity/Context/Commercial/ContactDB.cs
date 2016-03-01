using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace entity
{
    public partial class ContactDB : BaseDB
    {
        public void New(contact contact)
        {
            contact.contact_field_value = new List<contact_field_value>();
            contact.contact_subscription = new List<contact_subscription>();
            contact.hr_education = new List<hr_education>();
            contact.hr_contract = new List<hr_contract>();
            contact.hr_family = new List<hr_family>();
            contact.hr_talent_detail = new List<hr_talent_detail>();

            contact.id_company = CurrentSession.Id_Company;
            contact.id_user =  CurrentSession.Id_User;
            contact.is_head = true;
            contact.is_active = true;
            contact.lead_time = 0;

            using (db db = new db())
            {
                if (db.contact_role.Where(c => c.is_principal == true && c.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
                {
                    int _id_contact_role = Convert.ToInt32(db.contact_role.Where(c => c.is_principal == true && c.id_company == CurrentSession.Id_Company).FirstOrDefault().id_contact_role);
                    if (_id_contact_role != 0)
                    {
                        contact.id_contact_role = _id_contact_role;
                    }
                }
            }

        }

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
