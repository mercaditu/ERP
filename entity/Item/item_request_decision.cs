
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
            Purchase = 2
        }

        public item_request_decision()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_request_decision { get; set; }
        public int id_item_request_detail { get; set; }
        public int? id_location { get; set; }
        public Decisions decision { get; set; }
        public decimal quantity { get; set; }

        public virtual item_request_detail item_request_detail { get; set; }
    }
}
