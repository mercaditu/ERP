
namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class impex_expense : Audit
    {
        public impex_expense()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_impex_expense { get; set; }
        public int id_impex { get; set; }
        public int? id_purchase_invoice { get; set; }
        public int id_incoterm_condition { get; set; }
        public decimal value { get; set; }
        public int id_currencyfx { get; set; }
    
        public virtual impex impex { get; set; }
        public virtual impex_incoterm_condition impex_incoterm_condition { get; set; }
        public virtual purchase_invoice purchase_invoice { get; set; }
    }
}
