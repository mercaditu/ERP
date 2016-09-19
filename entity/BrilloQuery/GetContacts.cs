using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.BrilloQuery
{
    public class GetContacts: IDisposable
    {
        public ICollection<Contact> List { get; set; }

        /// <summary>
        /// Generates DB Query for Contacts and loads it into "List".
        /// </summary>
        public GetContacts()
        {
            List = new List<Contact>();

            string query = @" select 
                                id_contact as ID,
                                name as Name,
                                alias as Alias,
                                gov_code as Gov_Code,
                                code as Code,
                                telephone as Telephone,
                                email as Email,
                                address as Address,
                                is_customer as IsCustomer,
                                is_supplier as IsSupplier,
                                is_employee as IsEmployee,
                                is_active as IsActive
                                from contacts
                                where id_company = {0} and is_active = 1
                                order by name
                                ";

            query = String.Format(query, entity.CurrentSession.Id_Company);

            using (DataTable dt = QueryExecutor.DT(query))
            {
                foreach (DataRow DataRow in dt.Rows)
                {
                    Contact Contact = new Contact();

                    Contact.ID = Convert.ToInt16(DataRow["ID"]);
                    Contact.Name = Convert.ToString(DataRow["Name"]);
                    Contact.Alias = Convert.ToString(DataRow["Alias"]);
                    Contact.Gov_Code = Convert.ToString(DataRow["Gov_Code"]);
                    Contact.Code = Convert.ToString(DataRow["Code"]);
                    Contact.Telephone = Convert.ToString(DataRow["Telephone"]);
                    Contact.Email = Convert.ToString(DataRow["Email"]);
                    Contact.Address = Convert.ToString(DataRow["Address"]);
                    Contact.IsCustomer = Convert.ToBoolean(DataRow["IsCustomer"]);
                    Contact.IsSupplier = Convert.ToBoolean(DataRow["IsSupplier"]);
                    Contact.IsEmployee = Convert.ToBoolean(DataRow["IsEmployee"]);
                    Contact.IsActive = Convert.ToBoolean(DataRow["IsActive"]);
                  
                    List.Add(Contact);
                }   
            }
        }

        public void Dispose()
        {
           // Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this != null)
            {
                if (disposing)
                {
                    this.Dispose();
                    // Dispose other managed resources.
                }
                //release unmanaged resources.
            }
        }
    }

    public class Contact
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Gov_Code { get; set; }
        public string Code { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsActive { get; set; }
     
    }
}
