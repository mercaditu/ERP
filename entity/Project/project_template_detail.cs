namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class project_template_detail : Audit
    {
        public project_template_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            child = new List<project_template_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_template_detail { get; set; }

        public int id_project_template { get; set; }
        public int? sequence { get; set; }

        public int? id_item
        {
            get { return _id_item; }
            set
            {
                if (_id_item != value)
                {
                    _id_item = value;
                    RaisePropertyChanged("id_item");
                }
            }
        }

        private int? _id_item;

        public Status.Project? status { get; set; }

        public int? id_tag { get; set; }

        public string item_description
        {
            get { return _item_description; }
            set { _item_description = value; RaisePropertyChanged("item_description"); }
        }

        private string _item_description;

        public string code { get { return _code; } set { _code = value; RaisePropertyChanged("code"); } }
        private string _code;
        public string logic { get; set; }

        //Heirarchy Nav Property
        public virtual project_template_detail parent { get; set; }

        public virtual ICollection<project_template_detail> child { get; set; }

        public virtual project_template project_type { get; set; }

        public virtual item item
        {
            get { return _items; }
            set
            {
                if (value != null)
                {
                    _items = value;
                    RaisePropertyChanged("item");
                    _item_description = item.name;
                    RaisePropertyChanged("item_description");
                }
            }
        }

        private item _items;
        public virtual item_tag item_tag { get; set; }
    }
}