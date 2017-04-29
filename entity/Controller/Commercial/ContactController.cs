using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace entity.Controller.Commercial
{
    public class ContactController
    {
        public int NumberOfRecords;
        public db db { get; set; }

        public void Initialize()
        {
            //Start Context
            db = new db();
        }

        public async void Load(Window Win)
        {
            if (Win.Title == entity.Brillo.Localize.StringText("Customer"))
            {
                await db.contacts.Where(a => (a.is_supplier == false || a.is_customer == true) && (a.id_company == CurrentSession.Id_Company || a.id_company == null) && a.is_employee == false).OrderBy(a => a.name).LoadAsync();
            }
            else if (Win.Title == entity.Brillo.Localize.StringText("Supplier"))
            {
                await db.contacts.Where(a => (a.is_supplier == true || a.is_customer == false) && (a.id_company == CurrentSession.Id_Company || a.id_company == null) && a.is_employee == false).OrderBy(a => a.name).LoadAsync();
            }
            else
            {
                await db.contacts.Where(a => (a.id_company == CurrentSession.Id_Company || a.id_company == null) && a.is_employee == false).OrderBy(a => a.name).LoadAsync();
            }

        }

        public contact Create()
        {
            contact contact = new contact()
            {
                IsSelected = true,
                State = EntityState.Added,
                contact_field_value = new List<contact_field_value>(),
                contact_subscription = new List<contact_subscription>(),
                hr_education = new List<hr_education>(),
                hr_contract = new List<hr_contract>(),
                hr_family = new List<hr_family>(),
                hr_talent_detail = new List<hr_talent_detail>(),
                id_company = CurrentSession.Id_Company,
                id_user = CurrentSession.Id_User,
                is_head = true,
                is_active = true,
                lead_time = 0,
                is_employee = false
            };
            if (db.contact_role.Where(c => c.is_principal == true && c.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
            {
                int _id_contact_role = Convert.ToInt32(db.contact_role.Where(c => c.is_principal == true && c.id_company == CurrentSession.Id_Company).FirstOrDefault().id_contact_role);
                if (_id_contact_role != 0)
                {
                    contact.id_contact_role = _id_contact_role;
                }
            }



            return contact;
        }

        public bool Edit(contact contact)
        {
            contact.IsSelected = true;
            contact.State = EntityState.Modified;
            db.Entry(contact).State = EntityState.Modified;
            return true;
        }

        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;
            foreach (contact contact in db.contacts.Local)
            {
                if (contact.IsSelected)
                {
                    if (contact.State == EntityState.Added)
                    {
                        contact.timestamp = DateTime.Now;
                        contact.State = EntityState.Unchanged;
                        db.Entry(contact).State = EntityState.Added;
                    }
                    else if (contact.State == EntityState.Modified)
                    {
                        contact.timestamp = DateTime.Now;
                        contact.State = EntityState.Unchanged;
                        db.Entry(contact).State = EntityState.Modified;
                    }
                    else if (contact.State == EntityState.Deleted)
                    {
                        contact.timestamp = DateTime.Now;
                        contact.State = EntityState.Unchanged;
                        db.contacts.Remove(contact);
                    }
                    NumberOfRecords += 1;
                }
                else if (contact.State > 0)
                {
                    if (contact.State != EntityState.Unchanged)
                    {
                        db.Entry(contact).State = EntityState.Unchanged;
                    }
                }
            }
            return true;
        }

        public bool CancelChanges()
        {

            return true;
        }

      
    }
}
