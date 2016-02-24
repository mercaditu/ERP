namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class bi_report_detail
    {
        public bi_report_detail()
        { }

        public enum WhereLogic
        {
            Equal,
            Between,
            Like,
            UnLike,
            Greater,
            GreaterOrEqual,
            Lesser,
            LesserOrEqual,
        }

        public enum AggregateFunction
        {
            Sum,
            Avg,
            Min,
            Max,
            Count
        }

        public enum OrderBy
        {
            Asc,
            Dsc
        }

        public enum ColumnTypes
        {
            Int,
            Bool,
            String,
            DateTime
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_bi_report_detail { get; set; }
        public int id_bi_report { get; set; }

        public string name_column { get; set; }
        public string display_column { get; set; }
        public ColumnTypes format_column { get; set; }

        public bool is_output { get; set; }
        public int? group { get; set; }
        public OrderBy? order_by { get; set; }
        public WhereLogic? filter_by { get; set; }

        public AggregateFunction? aggregate_by { get; set; }
        public bool is_runningtotal { get; set; }
        public bool is_header { get; set; }
        public bool is_footer { get; set; }
        public bool is_drilldown { get; set; }
        public string chart_axis { get; set; }

        public virtual bi_report bi_report { get; set; }
    }
}
