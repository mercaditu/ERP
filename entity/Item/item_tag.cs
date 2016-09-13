namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    
    public partial class item_tag : Audit, IDataErrorInfo
    {
        public item_tag()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            item_tag_detail = new List<item_tag_detail>();
            is_active = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_tag { get; set; }

        [Required]
        public string name { get; set; }
        public bool is_active { get; set; }

        public virtual ICollection<item_tag_detail> item_tag_detail { get; set; }
        public virtual IEnumerable<project_template_detail> project_template_detail { get; set; }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();
                
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    String propertyError = this[prop.Name];
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
                // apply property level validation rules
                if (columnName == "name")
                {
                    if (string.IsNullOrEmpty(name))
                        return "Name needs to be filled";
                }
                return "";
            }
        }
    }
}
