namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_bank : Audit, IDataErrorInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_bank { get; set; }
        public int? id_geography { get; set; }
        [Required]
        public string name { get; set; }
        public bool is_active
        {
            get { return _is_active; }
            set
            {
                if (_is_active != value)
                {
                    _is_active = value;
                    RaisePropertyChanged("is_active");
                }
            }
        }
        private bool _is_active;

        public bool for_supplier { get; set; }
        public string branch_name { get; set; }
        public string branch_address { get; set; }
        public string country { get; set; }
        public string swift_code { get; set; }
        public string intermediary_bank { get; set; }

        
        public app_bank()
        { 
            app_account = new List<app_account>();
            payment_detail = new List<payment_detail>();
            is_active = true;
            id_company = CurrentSession.Id_Company;
            is_head = true;
            id_user =  CurrentSession.Id_User;
        }

        public virtual ICollection<app_account> app_account { get; set; }
        public virtual ICollection<payment_detail> payment_detail { get; set; }
        public virtual app_geography app_geography { get; set; }

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
                if(columnName == "name")
                {
                    if (String.IsNullOrEmpty(name))
                        return "Name needs to be filled";
                }
                return "";
            }
        }
    }
}
