namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class crm_schedual : Audit
    {
        public enum Types
        {
            Call,
            Meeting,
            Reminder,
            Delivery,
            Payment
        }

        public crm_schedual()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            timestamp = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_schedual { get; set; }

        public int? id_sales_rep { get; set; }
        public int? id_opportunity { get; set; }
        public int? id_contact { get; set; }

        public Types type { get; set; }
        public int ref_id { get; set; }

        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

        public string comment { get; set; }
        public virtual crm_opportunity crm_opportunity { get; set; }
        public virtual contact contact { get; set; }
        public virtual sales_rep sales_rep { get; set; }
    }
}