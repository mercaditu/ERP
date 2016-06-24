namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class purchase_order_dimension : Audit
    {
        public purchase_order_dimension()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_order_property { get; set; }
        public long id_purchase_order_detail { get; set; }
        public int id_dimension { get; set; }
        public decimal value { get; set; }
        public int id_measurement { get; set; }

        public virtual purchase_order_detail purchase_order_detail { get; set; }
        public virtual app_dimension app_dimension { get; set; }
        public virtual app_measurement app_measurement { get; set; }
    }
}
