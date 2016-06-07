namespace entity
{
    using entity.Class;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class security_demand
    {
        enum modes
        {
            Pending,
            Approved
        }
        public enum Privilages
        {
            //[LocalizedDescription("CanUserDiscountByPercent")]
            CanUserDiscountByPercent = 1,
            //[LocalizedDescription("CanUserDiscountByValue")]
            CanUserDiscountByValue = 2,
           
           

        }
        public security_demand()
        {
          
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_privilage { get; set; }
        public App.Names id_application { get; set; }
        public Privilages Privilage { get; set; }
        public modes mode { get; set; }
    
      
    }
   

    
}