namespace entity
{
    using entity.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class security_request:Audit
    {
        enum status
        {
            Pending,
            Approved,
            Rejected
        }

        public security_request()
        {
          
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_privilage { get; set; }
        //public int id_user_requested { get; set; }
        //public int id_user_approval { get; set; }
        public App.Names id_application { get; set; }
        public Privilage.Privilages privilage { get; set; }
        public status status { get; set; }
        public decimal? value { get; set; }
        public DateTime requested_date { get; set; }
        public DateTime approval_date { get; set; }

        public virtual security_user requested_user { get; set; }
        public virtual security_user approval_user { get; set; }
    }
   

    
}