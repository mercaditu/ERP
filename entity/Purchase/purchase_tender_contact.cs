
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class purchase_tender_contact : Audit
    {
        public purchase_tender_contact()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
            if (contact!=null)
            {
                recieve_date_est = DateTime.Now.AddDays((double)contact.lead_time);
            }
            purchase_tender_detail = new List<purchase_tender_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_tender_contact { get; set; }
        public int id_purchase_tender { get; set; }
        public int id_contact { get; set; }
        public int id_contract { get; set; }
        public int id_condition { get; set; }
        public int id_currencyfx { get; set; }
        public string comment { get; set; }
        public DateTime? recieve_date_est { get; set; }

        public virtual purchase_tender purchase_tender { get; set; }
        public virtual app_currencyfx app_currencyfx { get; set; }
        public virtual contact contact { get; set; }
        public virtual app_condition app_condition { get; set; }
        public virtual app_contract app_contract { get; set; }
        public virtual ICollection<purchase_tender_detail> purchase_tender_detail { get; set; }
    }
}
