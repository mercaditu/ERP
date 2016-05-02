namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class item_inventory_detail : Audit
    {
        public item_inventory_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            item_inventory_dimension = new List<item_inventory_dimension>();
            item_movement_value = new List<item_movement_value>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_inventory_detail { get; set; }
        public int id_inventory { get; set; }
        public int id_location { get; set; }
        public int id_item_product { get; set; }
        public Status.Documents status { get; set; }
        public decimal value_system { get; set; }
        public decimal value_counted { get; set; }
        public string comment { get; set; }

        public virtual item_inventory item_inventory { get; set; }
        public virtual app_location app_location { get; set; }
        public virtual item_product item_product { get; set; }
        public virtual ICollection<item_inventory_dimension> item_inventory_dimension { get; set; }
        public virtual ICollection<item_movement_value> item_movement_value { get; set; }
    }
}
