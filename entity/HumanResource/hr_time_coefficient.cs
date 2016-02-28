namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class hr_time_coefficient : Audit
    {
        public hr_time_coefficient()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_time_coefficient { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public decimal coefficient { get; set; }
        [Required]
        public DateTime start_time { get; set; }
        [Required]
        public DateTime end_time { get; set; }
        
        public bool weekend_only { get; set; }

    }
}
