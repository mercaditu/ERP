using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace entity.Controller.Commercial
{
    public class ContactController : Base
    {

        public async void LoadCustomers()
        {
            await db.contacts.Where(a => (a.is_supplier == false || a.is_customer == true) && (a.id_company == CurrentSession.Id_Company || a.id_company == null) && a.is_employee == false).OrderBy(a => a.name).LoadAsync();
            LoadSecondary();
        }

        public async void LoadSuppliers()
        {
            await db.contacts.Where(a => (a.is_supplier == true || a.is_customer == false) && (a.id_company == CurrentSession.Id_Company || a.id_company == null) && a.is_employee == false).OrderBy(a => a.name).LoadAsync();
            LoadSecondary();
        }

        public async void Load()
        {
            await db.contacts.Where(a => (a.id_company == CurrentSession.Id_Company || a.id_company == null) && a.is_employee == false).OrderBy(a => a.name).LoadAsync();
            LoadSecondary();
        }

        private async void LoadSecondary()
        {
            await db.contact_role.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().LoadAsync();
            await db.app_field.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            await db.app_bank.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().LoadAsync();
            await db.app_cost_center.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().LoadAsync();
            await db.contact_tag.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active == true).OrderBy(x => x.name).LoadAsync();
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
        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;

            
            foreach (var error in db.GetValidationErrors())
            {
                db.Entry(error.Entry.Entity).State = EntityState.Detached;
            }

            db.SaveChanges();
            foreach (contact contact in db.contacts.Local)
            {
                contact.State = EntityState.Unchanged;
            }
            return true;
        }

        public bool Edit(contact contact)
        {
            contact.IsSelected = true;
            contact.State = EntityState.Modified;
            db.Entry(contact).State = EntityState.Modified;
            return true;
        }

       
    }
}
