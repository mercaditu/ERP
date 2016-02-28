namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class accounting_budget : Audit
    {
       public accounting_budget()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
            
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_budget { get; set; }
        public int id_chart { get; set; }
        public int id_cycle { get; set; }
        [Required]
        public decimal value { get; set; }
        
        //Navigation Properties
        public virtual accounting_chart accounting_chart { get; set; }
        public virtual accounting_cycle accounting_period { get; set; }
    }
}

