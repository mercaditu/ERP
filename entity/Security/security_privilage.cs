
namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class security_privilage
    {
        public security_privilage()
        {
            security_role_privilage = new List<security_role_privilage>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_privilage { get; set; }
        public App.Names id_application { get; set; }
        public string name { get; set; }
    
        public virtual ICollection<security_role_privilage> security_role_privilage { get; set; }
    }
}
