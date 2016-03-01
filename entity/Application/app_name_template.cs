namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_name_template : Audit
    {
        public app_name_template()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        public enum AppLists
        {
            Item,
            HumanResource,
            Contact,
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short id_name_template { get; set; }
        public AppLists app_name { get; set; }
        [Required]
        public string name { get; set; }

        public virtual ICollection<app_name_template_detail> app_name_template_detail { get; set; }
    }
}
