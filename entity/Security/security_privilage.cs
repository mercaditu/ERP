namespace entity
{
    using entity.Class;
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
        public Privilage.Privilages name { get; set; }
    
        public virtual ICollection<security_role_privilage> security_role_privilage { get; set; }
    }

    public partial class Privilage
    {
        public enum Privilages
        {
            [LocalizedDescription("CanUserDiscountByPercent")]
            CanUserDiscountByPercent=1,
            [LocalizedDescription("CanUserDiscountByValue")]
            CanUserDiscountByValue=2,
            [LocalizedDescription("CanUserUpdatePrice")]
            CanUserUpdatePrice=3,
        }
    }
}