namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class payment_type_detail : Audit
    {
        public payment_type_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_payment_type_detail { get; set; }

        public int id_payment_type { get; set; }
        public int id_payment_detail { get; set; }
        public int? id_payment_approve_detail { get; set; }
        public short id_field { get; set; }
        public string value { get; set; }

        public virtual app_field app_field { get; set; }
        public virtual payment_detail payment_detail { get; set; }
        public virtual payment_approve_detail payment_approve_detail { get; set; }
    }
}