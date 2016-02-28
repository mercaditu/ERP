using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace entity
{
    public class project_event_template_fixed : Audit, IDataErrorInfo
    {
        public project_event_template_fixed()
        {
            is_active = true;
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_project_event_template_fixed { get; set; }
        public int id_project_event_template { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_tag { get; set; }
        public bool is_active { get; set; }

        public virtual item_tag item_tag { get; set; }
        public virtual project_event_template project_event_template { get; set; }

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