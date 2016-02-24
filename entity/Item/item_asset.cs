namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class item_asset : Audit
    {
        public item_asset()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_asset { get; set; }
        public int id_item { get; set; }
        public int? id_item_asset_group { get; set; }
        public DateTime? manufacture_date { get; set; }
        public DateTime? purchase_date { get; set; }
        public decimal? purchase_value { get; set; }
        public decimal? current_value { get; set; }

        public decimal? speed { get; set; }

        //Remove all items
        public decimal? dieset_price { get; set; }
        public decimal? min_length { get; set; }
        public decimal? max_length { get; set; }
        public decimal? min_width { get; set; }
        public decimal? max_width { get; set; }

        //Nav Properties
        public virtual item item { get; set; }
        public virtual item_asset_group item_asset_group { get; set; }
    }
}
