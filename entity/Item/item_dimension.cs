namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class item_dimension : Audit, IDataErrorInfo
    {
        public item_dimension()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_dimension { get; set; }
        public int id_item { get; set; }
        public int id_app_dimension { get; set; }
        public int id_measurement { get; set; }
        [Required]
        public decimal value { get; set; }

        public virtual item item { get; set; }
        public virtual app_dimension app_dimension { get; set; }
        public virtual app_measurement app_measurement { get; set; }
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
                //if (columnName == "value")
                //{
                //    //if (value == 0)
                //    //    return "Value needs to be filled";
                //}
                return "";
            }
        }
    }

}
