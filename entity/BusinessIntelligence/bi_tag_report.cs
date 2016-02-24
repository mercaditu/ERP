namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class bi_tag_report
    {
        public bi_tag_report() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_bi_tag_report { get; set; }
        public int id_bi_tag { get; set; }
        public int id_bi_report { get; set; }

        public virtual bi_tag bi_tag { get; set; }
        public virtual bi_report bi_report { get; set; }
    }
}
