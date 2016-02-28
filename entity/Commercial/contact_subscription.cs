
namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class contact_subscription : Audit
    {
        public contact_subscription()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
            is_active = true;
            start_date = DateTime.Now;
            end_date = DateTime.Now.AddDays(365);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_subscription { get; set; }
        public int id_contact { get; set; }
        public string id_item { get; set; }
        public int id_contract { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public short bill_cycle { get; set; }
        public short bill_on { get; set; }
        public bool is_active { get; set; }
    
        public virtual contact contact { get; set; }
    }
}
