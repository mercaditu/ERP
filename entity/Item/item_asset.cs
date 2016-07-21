namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class item_asset : Audit
    {
        public item_asset()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            id_branch = CurrentSession.Id_Branch;

            is_head = true;
            item_asset_maintainance = new List<item_asset_maintainance>();
        }

        public enum DeActiveTypes
        {
            EXPIRATION_CADUCIDAD = 1,
            LOSS_EXTRAVIOOHURTO = 2,
            ESTRANGEMENT_ENAJENACION_O_VENTA = 3,
            INPERFECTION_DESPERFECTO = 4
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_asset { get; set; }
        public int id_item { get; set; }
        public int? id_branch { get; set; }
        public int? id_item_asset_group { get; set; }
        public DateTime? manufacture_date { get; set; }
        public DateTime? purchase_date { get; set; }
        public decimal? purchase_value { get; set; }
        public decimal? current_value { get; set; }

        public int? id_department { get; set; }

        public int? id_contact { get; set; }

        public decimal? speed { get; set; }
        public DeActiveTypes deactivetype { get; set; }
        //Remove all items
        public decimal? dieset_price { get; set; }
        public decimal? min_length { get; set; }
        public decimal? max_length { get; set; }
        public decimal? min_width { get; set; }
        public decimal? max_width { get; set; }

        //Nav Properties
        public virtual item item { get; set; }
        public virtual item_asset_group item_asset_group { get; set; }
        public virtual app_branch app_branch { get; set; }
        public virtual contact contact { get; set; }
        public virtual ICollection<item_asset_maintainance> item_asset_maintainance { get; set; }
    }
}
