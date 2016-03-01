namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class item_request_dimension : Audit
    {
        public item_request_dimension()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_request_property { get; set; }
        public long id_item_request_detail { get; set; }
        public int id_dimension { get; set; }
        public int id_measurement { get; set; }
        public decimal value { get; set; }

        public virtual item_request_detail item_request_detail { get; set; }
        public virtual app_dimension app_dimension { get; set; }
        public virtual app_measurement app_measurement { get; set; }
    }
}
