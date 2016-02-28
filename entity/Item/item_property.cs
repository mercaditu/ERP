namespace entity
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class item_property : Audit, IDataErrorInfo
    {
        public item_property()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_property { get; set; }
        [Required]
        public int id_item { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_property { get; set; }
        public int? id_measurement { get; set; }
        [Required]
        public decimal value { get; set; }

        public virtual item item { get; set; }
        public virtual app_property app_property { get; set; }
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
                // apply property level validation rules
                if (columnName == "value")
                {
                    if (value == 0)
                        return "Value needs to be filled";
                }
                if (columnName == "id_property")
                {
                    if (id_property == 0)
                        return "Property needs to be selected";
                }
                return "";
            }
        }
    }
}
