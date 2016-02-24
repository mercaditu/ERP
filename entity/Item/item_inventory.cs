namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class item_inventory : Audit
    {
        public item_inventory()
        {
            id_branch = Properties.Settings.Default.branch_ID;
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
            trans_date = DateTime.Now;
            status = Status.Documents.Pending;
            item_inventory_detail = new List<item_inventory_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_inventory { get; set; }
        public int id_branch { get; set; }
        public int code { get; set; }
        public string comment { get; set; }
        public DateTime trans_date { get; set; }
        public Status.Documents status { get; set; }

        public virtual app_branch app_branch { get; set; }
        public virtual ICollection<item_inventory_detail> item_inventory_detail { get; set; }
    }
}
