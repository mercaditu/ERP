namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class impex_incoterm_detail : Audit
    {
        public impex_incoterm_detail()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_incoterm_detail { get; set; }
        public int id_incoterm { get; set; }
        public int id_incoterm_condition { get; set; }
        public bool buyer { get; set; }
        public bool seller { get; set; }
   
        public virtual impex_incoterm impex_incoterm { get; set; }
        public virtual impex_incoterm_condition impex_incoterm_condition { get; set; }
    }
}
