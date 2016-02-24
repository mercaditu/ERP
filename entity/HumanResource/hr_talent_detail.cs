namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class hr_talent_detail : Audit
    {

        public hr_talent_detail()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_talent_detail { get; set; }
        public int id_talent { get; set; }
        public int id_contact { get; set; }

        [Required]
        public decimal? experience { get; set; }

        public virtual contact contact { get; set; }
        public virtual hr_talent hr_talent { get; set; }
    }
}