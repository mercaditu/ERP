namespace entity
{
    using entity.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class security_request:Audit
    {
        public enum States
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
        public int id_request { get; set; }
        public App.Names id_application { get; set; }
        public Privilage.Privilages id_privilage { get; set; }
        public States state { get; set; }
        public decimal? value { get; set; }
        public DateTime request_date { get; set; }
        public DateTime approve_date { get; set; }

        public virtual security_user request_user { get; set; }
        public virtual security_user approve_user { get; set; }
    }
   

    
}