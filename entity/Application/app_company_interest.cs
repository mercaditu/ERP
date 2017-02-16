namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class app_company_interest
    {
        [Key, ForeignKey("app_company")]
        public int id_interest { get; set; }

        public int grace_period { get; set; }

        public decimal interest
        {
            get
            {
                return _interest;
            }
            set
            {
                _interest = value;
                InterestDaily = ((_interest / 12) / 30);
            }
        }

        private decimal _interest;

        [NotMapped]
        public decimal InterestDaily { get; set; }

        public bool is_forced { get; set; }

        public virtual app_company app_company { get; set; }
    }
}