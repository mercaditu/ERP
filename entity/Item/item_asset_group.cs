namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class item_asset_group : Audit
    {
        public item_asset_group()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_asset_group { get; set; }
        public string name { get; set; }
        public decimal? depreciation_rate { get; set; }
        public DateTime? depreciation_run { get; set; } 
        
        //Nav Properties
        public virtual IEnumerable<item_asset> item_asset { get; set; }
        public virtual IEnumerable<accounting_chart> accounting_chart { get; set; }
    }
}
