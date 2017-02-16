namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class security_question
    {
        public security_question()
        {
            // security_user = new List<security_user>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_question { get; set; }

        public string question { get; set; }

        public virtual ICollection<security_user> security_user { get; set; }
    }
}