using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace entity
{
    public partial class project_task_dimension : Audit, IDataErrorInfo
    {
        public project_task_dimension()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_task_dimension { get; set; }
        public int id_project_task { get; set; }
        public int id_dimension { get; set; }
        public int id_measurement { get; set; }
        [Required]
        public decimal value { get; set; }

        public virtual project_task project_task { get; set; }
        public virtual app_dimension app_dimension { get; set; }
        public virtual app_measurement app_measurement { get; set; }

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
                //if (columnName == "value")
                //{
                //    if (value <= 0)
                //        return "Value cannot be zero";
               
                //}
                return "";
            }
        }
    }
}
