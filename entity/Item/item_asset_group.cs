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
            id_user = CurrentSession.Id_User;
            is_head = true;

            item_asset = new List<item_asset>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_asset_group { get; set; }

        public string name { get; set; }
        public decimal? depreciation_rate { get; set; }
        public DateTime? depreciation_run { get; set; }
        public int ref_id { get; set; }
        //Nav Properties
        public virtual ICollection<item_asset> item_asset { get; set; }
    }
}