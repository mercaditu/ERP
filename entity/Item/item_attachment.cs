namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class item_attachment : Audit
    {
        public item_attachment()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_attachment { get; set; }
        public int id_item { get; set; }
        public int id_attachment { get; set; }
    
        public virtual item item { get; set; }
        public virtual app_attachment app_attachment { get; set; }
    }
}
