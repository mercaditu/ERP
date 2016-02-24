namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class bi_tag_role
    {
        public bi_tag_role() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_bi_tag_role { get; set; }
        public int id_bi_tag { get; set; }
        public int id_role { get; set; }

        public virtual bi_tag bi_tag { get; set; }
        public virtual security_role security_role { get; set; }
    }
}
