namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_notification : Audit, IDataErrorInfo
    {
        public app_notification()
        {

            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            timestamp = DateTime.Now;
        
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_notification { get; set; }

        [Required]
        public App.Names id_application { get; set; }

        [Required]
        public int ref_id { get; set; }

        [Required]
        public string comment { get; set; }

        public virtual app_department notified_department { get; set; }
        public virtual security_user notified_user { get; set; }

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
                if (columnName == "comment")
                {
                    if (string.IsNullOrEmpty(comment))
                        return "Comment needs to be filled";
                }
                if (columnName == "id_application")
                {
                    if (id_application == 0)
                        return "Application needs to be Selected";
                }
             
                return "";
            }
        }
    }
}