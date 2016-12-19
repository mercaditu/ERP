namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class app_company_interest
    {
        [Key, ForeignKey("app_company")]
        public int id_interest { get; set; }
        public int grace_period { get; set; }
        public decimal interest { get; set; }
        public bool is_forced { get; set; }
        
        public virtual app_company app_company { get; set; }
    }
}
