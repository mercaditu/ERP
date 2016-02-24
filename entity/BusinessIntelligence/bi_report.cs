namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class bi_report
    {
        public bi_report()
        { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_bi_report { get; set; }
        public string name { get; set; }
        public string short_description { get; set; }
        public string long_description { get; set; }

        public string query { get; set; }

        public virtual IEnumerable<bi_tag_role> bi_tag_role { get; set; }
        public virtual IEnumerable<bi_report_detail> bi_report_detail { get; set; }
        public virtual IEnumerable<bi_chart_report> bi_chart_report { get; set; }
    }
}
