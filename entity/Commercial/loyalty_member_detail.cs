namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class loyalty_member_detail : Audit
    {
        public loyalty_member_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_member_detail { get; set; }

        public int id_member { get; set; }
        public DateTime trans_date { get; set; }
        public DateTime expire_date { get; set; }
        public decimal debit { get; set; }
        public decimal credit { get; set; }

        public string comment { get; set; }
        public virtual loyalty_member loyalty_member { get; set; }
    }
}