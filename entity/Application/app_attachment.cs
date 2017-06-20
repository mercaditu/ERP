namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class app_attachment
    {
        public app_attachment()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_attachment { get; set; }

        public byte[] file { get; set; }
        public string mime { get; set; }
        public int reference_id {
            get
            {
                return _reference_id;
            }
            set
            {
                _reference_id =value; FileName = id_attachment + mime;
            }
        }
        int _reference_id;
        public bool is_default { get; set; }
        public entity.App.Names application { get; set; }

        [NotMapped]
        public string FileName { get; set; }
    

        public virtual ICollection<item_attachment> item_attachment { get; set; }
    }
}