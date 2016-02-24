
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
  

    
    public partial class app_attachment : Audit
    {
        public app_attachment()
        {
            is_active = true;
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_attachment { get; set; }
        public byte[] file { get; set; }
        public string mime { get; set; }
        public bool is_active { get; set; }
        public virtual ICollection<item_attachment> item_attachment { get; set; }
    }
}
