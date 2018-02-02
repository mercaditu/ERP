namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class item_movement_value : Audit
    {
        public item_movement_value()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            is_estimate = false;
            is_read = false;
            timestamp = DateTime.Now;
          
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_movement_value { get; set; }

        public long id_movement { get; set; }
        public int id_currencyfx { get; set; }
        public decimal unit_value { get; set; }
        public string comment { get; set; }

        public bool is_estimate { get; set; }

        public virtual item_movement item_movement { get; set; }
        public virtual item_mov_archive item_mov_archive { get; set; }

        public virtual app_currencyfx app_currencyfx { get; set; }
    }
}