namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class item_inventory_dimension : Audit
    {
        public item_inventory_dimension()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_inventory_dimension { get; set; }
        public int id_inventory_detail { get; set; }
        public int id_dimension { get; set; }
        public decimal value { get; set; }

        public virtual item_inventory_detail item_inventory_detail { get; set; }
        public virtual app_dimension app_dimension { get; set; }
    }
}
