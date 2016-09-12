namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_currency_denomination : AuditGeneric, IDataErrorInfo
    {
        public app_currency_denomination()
        {
            is_active = true;
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_denomination { get; set; }
        [Required]
        [CustomValidation(typeof(entity.Class.EntityValidation), "CheckId")]
        public int id_currency { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public bool is_bill { get; set; }
        public bool is_active { get; set; }
        public virtual app_currency app_currency { get; set; }

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
                if (columnName == "id_currency")
                {
                    if (id_currency == 0)
                        return "Currency needs to bo selected";
                }
                return "";
            }
        }
    }
}
