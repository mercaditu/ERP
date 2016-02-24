
namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class security_role_privilage
    {
        public security_role_privilage()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_role_privilage { get; set; }
        public int id_role { get; set; }
        public int id_privilage { get; set; }
        public bool has_privilage { get; set; }
        public decimal? value_max { get; set; }
    
        public virtual security_privilage security_privilage { get; set; }
        public virtual security_role security_role { get; set; }
    }
}
