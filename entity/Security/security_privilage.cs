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
        public Privilage.Privilages name { get; set; }

        public virtual ICollection<security_role_privilage> security_role_privilage { get; set; }
    }

    public partial class Privilage
    {
        public enum Privilages
        {
            CanUserDiscountByPercent = 1,
            CanUserDiscountByValue = 2,
            CanUserNotUpdatePrice = 3,
            CanDisplayProduct = 4,
            CanDisplayRawMaterial = 5,
            CanDisplayService = 6,
            CanDisplayFixedAssets = 7,
            CanDisplayTask = 8,
            CanDisplaySupplies = 9,
            CanDisplayServiceContract = 10,
        }
    }
}