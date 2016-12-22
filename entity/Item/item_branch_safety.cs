namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class item_branch_safety : Audit
    {
        public item_branch_safety()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_branch_safety { get; set; }
        public int id_branch { get; set; }
        public int id_item_product { get; set; }
        public decimal quantity { get; set; }
        
        public virtual app_branch app_branch { get; set; }
        public virtual item_product item_product { get; set; }
    }
}
