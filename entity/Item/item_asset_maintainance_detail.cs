namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class item_asset_maintainance_detail : Audit
    {
        public enum MaintainanceTypes
        {
            Preventive,
            Corrective,
            AddValue
        }

        public item_asset_maintainance_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_maintainance_detail { get; set; }
        public int id_maintainance { get; set; }

        public int? id_item { get; set; }
        public string item_description { get; set; }
        public decimal quantity { get; set; }
        public decimal unit_cost { get; set; }
        public int? id_currencyfx { get; set; }

        //Nav Properties
        public virtual item_asset_maintainance item_asset_maintainance { get; set; }
        public virtual item item { get; set; }
        public virtual app_currencyfx app_currencyfx { get; set; }
    }
}
