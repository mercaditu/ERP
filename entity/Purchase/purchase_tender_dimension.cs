namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class purchase_tender_dimension : Audit
    {
        public purchase_tender_dimension()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_tender_property { get; set; }
        public int id_purchase_tender_item { get; set; }
        public int id_dimension { get; set; }
        public int id_measurement { get; set; }
        public decimal value { get; set; }

        public virtual purchase_tender_item purchase_tender_item { get; set; }
        public virtual app_dimension app_dimension { get; set; }
        public virtual app_measurement app_measurement { get; set; }
    }
}
