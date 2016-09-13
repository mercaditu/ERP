
namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class payment_withholding_detail : Audit
    {
        public payment_withholding_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            timestamp = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_withholding_detail { get; set; }
        public int id_withholding { get; set; }
        public int? id_sales_invoice { get; set; }
        public int? id_purchase_invoice { get; set; }

        public virtual payment_withholding_tax payment_withholding_tax { get; set; }
        public virtual sales_invoice sales_invoice { get; set; }
        public virtual purchase_invoice purchase_invoice { get; set; }
    }
}
