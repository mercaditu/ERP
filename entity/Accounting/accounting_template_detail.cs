namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class accounting_template_detail : Audit
    {
        public accounting_template_detail()
        {
        
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
         
          
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_template_detail { get; set; }
        public int id_template { get; set; }
        public int id_chart { get; set; }
        public decimal coefficeint { get; set; }
        public bool is_debit { get; set; }
        
        public virtual accounting_template accounting_template { get; set; }
        public virtual accounting_chart accounting_chart { get; set; }
    }
}
