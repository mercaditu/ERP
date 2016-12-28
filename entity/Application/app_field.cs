namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_field : Audit, IDataErrorInfo
    {
        public app_field()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        public enum field_types { Telephone = 1, Account = 2,Email=3,Address=4}

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short id_field { get; set; }
        [Required]
        public string name { get; set; }
        public field_types field_type { get; set; }
        public string mask { get; set; }

        public virtual IEnumerable<contact_field_value> contact_field_value { get; set; }
        public virtual IEnumerable<payment_type_detail> payment_type_detail { get; set; }

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
