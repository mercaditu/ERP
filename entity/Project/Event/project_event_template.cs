using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace entity
{
    public class project_event_template : Audit, IDataErrorInfo
    {
        public project_event_template()
        {
            is_active = true;
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;

            project_event_template_fixed = new List<project_event_template_fixed>();
            project_event_template_variable = new List<project_event_template_variable>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_project_event_template { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_tag { get; set; }
        [Required]
        public string name { get; set; }
        public bool is_active { get; set; }

        public virtual item_tag item_tag { get; set; }
        public virtual ICollection<project_event_template_fixed> project_event_template_fixed { get; set; }
        public virtual ICollection<project_event_template_variable> project_event_template_variable { get; set; }
        public virtual ICollection<project_event> project_event { get; set; }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
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
                // apply property level validation rules
                if (columnName == "name")
                {
                    if (string.IsNullOrEmpty(name))
                        return "Name needs to be filled";
                }
                if (columnName == "id_tag")
                {
                    if (id_tag == 0)
                        return "Tag needs to be selected";
                }
                return "";
            }
        }
    }
}