namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    

    public partial class app_branch_walkins : Audit
    {
        public app_branch_walkins()
        {
            id_company = CurrentSession.Id_Company;
            id_branch = CurrentSession.Id_Branch;
            id_user = CurrentSession.Id_User;
            is_head = true;
            start_date = DateTime.Now;
            end_date = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_branch_walkin { get; set; }
        public int id_branch { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public decimal quantity { get; set; }

        public virtual app_branch app_branch { get; set; }
    }
}
