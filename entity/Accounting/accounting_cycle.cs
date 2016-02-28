namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class accounting_cycle : Audit
    {

        public accounting_cycle()
        {
            accounting_budget = new List<accounting_budget>();
            is_active = true;
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
            DateTime Now = DateTime.Now;
            start_date = new DateTime(Now.Year, 1, 1);
            end_date = new DateTime(Now.Year, 12, 31);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_cycle { get; set; }

        [Required]
        public string name { get; set; }
        [Required]
        public DateTime start_date { get; set; }
        [Required]
        public DateTime end_date { get; set; }
        [Required]
        public bool is_active { get; set; }

        public virtual ICollection<accounting_budget> accounting_budget { get; set; }
        public virtual IEnumerable<accounting_journal_detail> accounting_journal_detail { get; set; }
    }
}
