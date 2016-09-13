namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_vat : Audit, IDataErrorInfo
    {
        public app_vat()
        {
            on_product = false;
            on_branch = false;
            on_destination = false;
            is_active = true;
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_vat { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public decimal coefficient { get; set; }
        public bool on_product { get; set; }
        public bool on_branch { get; set; }
        public bool on_destination { get; set; }
        
        public bool is_active { get; set; }

        public virtual IEnumerable<app_branch> app_branch { get; set; }

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
                if (columnName == "coefficient")
                {
                    if (coefficient == 0)
                        return "Coefficient needs to be filled";
                }
                return "";
            }
        }
    }
}
