using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace entity
{
    public partial class app_department : Audit, IDataErrorInfo
    {
        public app_department()
        {
            is_active = true;
            is_head = true;
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_department { get; set; }
        [Required]
        public string name { get; set; }
        public bool is_active { get; set; }

        public virtual ICollection<hr_position> hr_position { get; set; }
        public virtual ICollection<item_request> item_request { get; set; }
        public virtual IEnumerable<accounting_chart> accounting_chart { get; set; }
        public virtual IEnumerable<security_role> security_role { get; set; }
        public virtual IEnumerable<purchase_invoice> purchase_invoice { get; set; }

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
