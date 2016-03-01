namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class hr_talent : Audit
    {
        public hr_talent()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            is_active = true;
            timestamp = DateTime.Now;
            hr_talent_detail = new List<hr_talent_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_talent { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public bool is_active { get; set; }

        public virtual ICollection<hr_talent_detail> hr_talent_detail { get; set; }

    }
}
