
namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class impex_export : Audit
    {
        public impex_export()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short id_export { get; set; }
        public int id_impex { get; set; }
        public int id_sales_invoice { get; set; }

        public virtual impex impex { get; set; }
        public virtual sales_invoice sales_invoice { get; set; }
    }
}
