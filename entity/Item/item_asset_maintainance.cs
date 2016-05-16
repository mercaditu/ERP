namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class item_asset_maintainance : Audit
    {
        public enum MaintainanceTypes
        {
            Preventive,
            Corrective,
            AddValue
        }

        public item_asset_maintainance()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_maintainance { get; set; }
        public int id_item_asset { get; set; }

        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string comment { get; set; }
        
        //Nav Properties
        public virtual item_asset item_asset { get; set; }
    }
}
