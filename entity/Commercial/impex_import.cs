
namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class impex_import : Audit
    {
        public impex_import()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short id_import { get; set; }
        public int id_impex { get; set; }
        public int id_purchase_invoice { get; set; }
        
        public virtual impex impex { get; set; }
        public virtual purchase_invoice purchase_invoice { get; set; }
       
    }
}
