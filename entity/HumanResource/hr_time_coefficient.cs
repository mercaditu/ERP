namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class hr_time_coefficient : Audit
    {
        public hr_time_coefficient()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            item_asset_maintainance_detail = new List<item_asset_maintainance_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_time_coefficient { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public decimal coefficient { get; set; }
        [Required]
        public DateTime start_time { get; set; }
        [Required]
        public DateTime end_time { get; set; }
        
        public bool weekend_only { get; set; }

        public virtual ICollection<item_asset_maintainance_detail> item_asset_maintainance_detail { get; set; }

    }
}
