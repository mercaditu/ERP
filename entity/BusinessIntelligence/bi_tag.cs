namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class bi_tag
    {
        public bi_tag()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_bi_tag { get; set; }
        public string name { get; set; }

        public int? id_module { get; set; }
        public bool behavior { get; set; }

        public virtual IEnumerable<bi_tag_role> bi_tag_role { get; set; }
    }
}
