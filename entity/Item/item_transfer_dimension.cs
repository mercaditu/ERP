namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class item_transfer_dimension : Audit
    {
        public item_transfer_dimension()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_transfer_property { get; set; }
        public long id_transfer_detail { get; set; }
        public int id_dimension { get; set; }
        public decimal value { get; set; }
      
        public virtual item_transfer_detail item_transfer_detail { get; set; }
        public virtual app_dimension app_dimension { get; set; }
    }
}
