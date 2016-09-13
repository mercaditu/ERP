using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace entity
{
    public class project_event_variable : Audit, IDataErrorInfo
    {
        public project_event_variable()
        {
            id_user =  CurrentSession.Id_User;
           
            id_company = CurrentSession.Id_Company;
         
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_project_event_variable { get; set; }
        public int id_project_event { get; set; }

        public int? id_tag { get; set; }

        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_item { get; set; }
        public decimal adult_consumption { get; set; }
        public decimal child_consumption { get; set; }
        public bool is_included { get; set; }

        public virtual project_event project_event { get; set; }
        public virtual item_tag item_tag { get; set; }
        public virtual item item { get; set; }

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
                if (columnName == "id_item")
                {
                    if (id_item == 0)
                        return "Item needs to be selected";
                }
                return "";
            }
        }
    }
}