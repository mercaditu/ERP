using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.BrilloQuery
{
    class GetContacts
    {
        ICollection<Contact> List { get; set; }

        public GetContacts()
        {
            List = new List<Contact>();
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

        ICollection<Tag> Tags { get; set; }
        public Contact()
        {
            Tags = new List<Tag>();
        }
    }

    public class Tag
    {
        public string Name { get; set; }
        ICollection<Contact> Item { get; set; }
    }
}
