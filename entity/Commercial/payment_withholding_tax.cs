
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class payment_withholding_tax : Audit
    {
        public payment_withholding_tax()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;

            payment_detail = new List<payment_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_withholding { get; set; }
        public Status.Documents_General status { get; set; }
        public int id_contact { get; set; }
        public int id_range { get; set; }
        public int id_currencyfx { get; set; }

        ///public bool is_accounted { get; set; }
        public int? id_journal { get; set; }

        public string withholding_number { get; set; }
        public string code { get; set; }
        public decimal value { get; set; }
        public DateTime trans_date { get; set; }
        public DateTime expire_date { get; set; }
        
        public virtual app_currencyfx app_currencyfx { get; set; }
        public virtual app_document_range app_document_range { get; set; }
        public virtual contact contact { get; set; }
        public virtual accounting_journal accounting_journal { get; set; }

        public virtual IEnumerable<payment_detail> payment_detail { get; set; }
        public virtual IEnumerable<payment_withholding_detail> payment_withholding_detail { get; set; }
    }
}
