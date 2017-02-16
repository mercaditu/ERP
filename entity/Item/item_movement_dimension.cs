namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class item_movement_dimension : Audit
    {
        public item_movement_dimension()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            timestamp = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_movement_dimension { get; set; }

        public long id_movement { get; set; }
        public int id_dimension { get; set; }
        public decimal value { get; set; }
        public int? id_measurement { get; set; }

        public virtual item_movement item_movement { get; set; }
        public virtual app_dimension app_dimension { get; set; }
        public virtual app_measurement app_measurement { get; set; }
    }
}