
namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class item_request_decision : Audit
    {
        public enum Decisions
        {
            Movement = 0,
            Transfer = 1,
            Purchase = 2,
            Production = 3,
            Internal = 4
        }

        public item_request_decision()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_request_decision { get; set; }
        public int id_item_request_detail { get; set; }
        public int? id_location { get; set; }
        public Decisions decision { get; set; }
        public decimal quantity { get; set; }
        public int? movement_id { get; set; }

        public virtual item_request_detail item_request_detail { get; set; }
        public virtual app_location app_location { get; set; }
    }
}
