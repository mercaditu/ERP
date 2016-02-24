namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class impex_incoterm_detail : Audit
    {
        public impex_incoterm_detail()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
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
