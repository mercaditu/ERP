
namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
  
    public partial class project_template : Audit
    {
        public project_template()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            project = new List<project>();
            project_template_detail = new List<project_template_detail>();
            is_active = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_project_template { get; set; }
        public int id_item_output { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public bool is_active { get; set; }

        public virtual IEnumerable<project> project { get; set; }
        public virtual ICollection<project_template_detail> project_template_detail { get; set; }
        public virtual IEnumerable<project_event> project_costing { get; set; }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (propertyError != string.Empty)
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                return error.Length == 0 ? null : error.ToString();
            }
        }
        public string this[string columnName]
        {
            get
            {
                //apply property level validation rules
                if (columnName == "value")
                {
                    if (name=="")
                        return "Name Cannot br Blank";

                }
                return "";
            }
        }
    }
}
