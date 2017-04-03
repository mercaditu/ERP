namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class security_request : Audit
    {
        public enum States
        {
            Pending,
            Approved,
            Rejected
        }

        public enum Request_Types
        {
            Discount,
            Credit,
        }

        public security_request()
        {
            timestamp = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_request { get; set; }

        public App.Names id_application { get; set; }
        public States state { get; set; }

        public Request_Types type { get; set; }
        public decimal? value { get; set; }
        public DateTime request_date { get; set; }
        public DateTime approve_date { get; set; }

        public string comment { get; set; }

        public virtual security_user request_user { get; set; }
        public virtual security_user approve_user { get; set; }
    }
}