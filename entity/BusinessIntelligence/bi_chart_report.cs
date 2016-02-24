namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class bi_chart_report
    {
        public bi_chart_report()
        { }

        public enum ChartTypes
        {
            Line,
            Column,
            Pie,
            StackedColumn,
            Plotter,
            HeatMap,
            Map
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_bi_chart_report { get; set; }
        public int id_bi_report { get; set; }
        public ChartTypes chart_type { get; set; }

        public bool is_default { get; set; }

        public virtual bi_report bi_report { get; set; }
    }
}
