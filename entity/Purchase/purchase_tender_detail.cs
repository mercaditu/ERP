
namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class purchase_tender_detail : Audit
    {
        public purchase_tender_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_tender_detail { get; set; }
        public int id_purchase_tender_contact { get; set; }
        public int id_purchase_tender_item { get; set; }
        public string item_description { get; set; }
        public decimal quantity { get; set; }
        public decimal unit_cost { get; set; }

        [NotMapped]
        public decimal UnitCost_Vat { get; set; }
        [NotMapped]
        public decimal SubTotal { get; set; }
        [NotMapped]
        public decimal SubTotal_Vat { get; set; }
        
        public virtual purchase_tender_contact purchase_tender_contact { get; set; }
        public virtual purchase_tender_item purchase_tender_item { get; set; }
        public virtual IEnumerable<purchase_order_detail> purchase_order_detail { get; set; }
    }
}
