
namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class item_transfer_detail : Audit
    {
        public item_transfer_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_transfer_detail { get; set; }
        public int id_transfer { get; set; }
        public int? id_project_task { get; set; }
        public int id_item_product { get; set; }
        public decimal quantity_origin { get; set; }
        public decimal quantity_destination { get; set; }
    
        public virtual item_transfer item_transfer { get; set; }
        public virtual item_product item_product { get; set; }
        public virtual project_task project_task { get; set; }
    }
}
