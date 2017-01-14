namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class crm_schedual : Audit
    {
        public enum Types
        {
            Call,
            Event,
            Reminder,
            SalesCall,

        }

        public crm_schedual()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_schedual { get; set; }
        public Types type { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

        public string comment { get; set; }

    }
}
