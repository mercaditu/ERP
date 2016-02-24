namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class app_weather
    {
        public app_weather()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_weather { get; set; }
        public int id_branch { get; set; }

        public decimal? temp { get; set; }
        public decimal? temp_min { get; set; }
        public decimal? temp_max { get; set; }
        public decimal? pressure { get; set; }
        public decimal? humidity { get; set; }
        public decimal? wind_speed { get; set; }
        public decimal? wind_type { get; set; }
        public decimal? wind_direction { get; set; }

        public DateTime timestamp { get; set; }

        public virtual app_branch app_branch { get; set; }

        public virtual IEnumerable<sales_budget> sales_budget { get; set; }
        public virtual IEnumerable<sales_order> sales_order { get; set; }
        public virtual IEnumerable<sales_invoice> sales_invoice { get; set; }
        public virtual IEnumerable<sales_return> sales_return { get; set; }

        public virtual IEnumerable<purchase_tender> purchase_tender { get; set; }
        public virtual IEnumerable<purchase_order> purchase_order { get; set; }
        public virtual IEnumerable<purchase_invoice> purchase_invoice { get; set; }
        public virtual IEnumerable<purchase_return> purchase_return { get; set; }

        public virtual IEnumerable<payment> payment { get; set; }

        public virtual IEnumerable<production_order> production_order { get; set; }
        public virtual IEnumerable<production_execution> production_execution { get; set; }

        public virtual IEnumerable<item_transfer> item_transfer { get; set; }
    }
}
