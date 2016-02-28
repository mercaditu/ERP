namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class hr_timesheet : Audit
    {
        public hr_timesheet()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_timesheet { get; set; }
        [Required]
        public int id_contact { get; set; }
        [Required]
        public int id_branch { get; set; }
        [Required]
        public bool is_entry { get; set; }
        [Required]
        public DateTime time { get; set; }

        public string comment { get; set; }

        public virtual contact contact { get; set; }
        public virtual app_branch app_branch { get; set; }
    }
}
