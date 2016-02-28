namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class purchase_invoice_dimension : Audit
    {
        public purchase_invoice_dimension()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_invoice_property { get; set; }
        public long id_purchase_invoice_detail { get; set; }
        public int id_dimension { get; set; }
        public decimal value { get; set; }

        public virtual purchase_invoice_detail purchase_invoice_detail { get; set; }
        public virtual app_dimension app_dimension { get; set; }
    }
}
