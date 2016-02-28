namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class hr_education : Audit
    {
        public enum Level
        {
            [Display(Name = "PrimarySchool")]
            [Description("desc_PrimarySchool")]
            Primary,
            [Display(Name = "SecondarySchool")]
            [Description("desc_SecondarySchool")]
            Secondary,
            [Display(Name = "College")]
            [Description("desc_College")]
            College,
            [Display(Name = "Masters")]
            [Description("desc_Masters")]
            Masters,
            [Display(Name = "Doctorate")]
            [Description("desc_Doctorate")]
            Doctorate
        }

        public hr_education()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;

            end_date = null;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_education { get; set; }
        public int id_contact { get; set; }
        [Required]
        public string institution { get; set; }
        public Level? education_level { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public decimal? average { get; set; }
        public string comment { get; set; }

        public virtual contact contact { get; set; }
    }
}
