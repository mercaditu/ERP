namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class hr_position : Audit
    {
        public hr_position()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
            is_active = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_position { get; set; }
        public int id_department { get; set; }

        public string name { get; set; }
        [Required]
        public bool is_active { get; set; }

        //Heirarchy Nav Properties
        public virtual hr_position parent { get; set; }
        public virtual ICollection<hr_position> child { get; set; }

        //Nav Properties
        public virtual app_department app_department { get; set; }
    }
}
