namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_vat_group_details : Audit, IDataErrorInfo
    {
        public app_vat_group_details()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_vat_group_detail { get; set; }

        public int id_vat_group { get; set; }

        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_vat { get; set; }

        public decimal percentage
        {
            get
            {
                if (_percentage == 0)
                {
                    _percentage = 1;
                }
                return _percentage;
            }
            set { _percentage = value; }
        }

        private decimal _percentage;

        public virtual app_vat_group app_vat_group { get; set; }
        public virtual app_vat app_vat { get; set; }

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
                if (columnName == "id_vat")
                {
                    if (id_vat == 0)
                        return "VAT needs to be Selected";
                }
                return "";
            }
        }
    }
}