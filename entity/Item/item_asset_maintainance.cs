namespace entity
{
    using System;
    using System.Collections.Generic;
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
        public enum Status
        {
            Pending,
            Working,
            Done,
            CannotBeFixed,
            Rejected
        }

        public item_asset_maintainance()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            item_asset_maintainance_detail = new List<item_asset_maintainance_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_maintainance { get; set; }
        public int id_item_asset { get; set; }

        public Status status { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string comment { get; set; }
        public MaintainanceTypes maintainance_type { get; set; }
        
        //Nav Properties
        public virtual item_asset item_asset { get; set; }
        public virtual ICollection<item_asset_maintainance_detail> item_asset_maintainance_detail { get; set; }
    }
}
