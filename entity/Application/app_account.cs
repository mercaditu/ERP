namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;

    public partial class app_account : Audit, IDataErrorInfo
    {
        public enum app_account_type
        {
            Bank = 1,
            Terminal = 2
        }

        app_account_type actype;

        public app_account()
        {
            app_account_detail = new List<app_account_detail>();
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            app_account_session = new List<app_account_session>();
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_account { get; set; }
        public app_account_type id_account_type
        {
            get
            {
                return actype;
            }
            set
            {
                actype = value;
                //if (id_account_type == app_account_type.Bank)
                //    is_active = true;
                //else
                //    is_active = false;
            }
        }
        public int? id_bank { get; set; }
        public int? id_currency { get; set; }
        public int? id_terminal { get; set; }

        [Required]
        public string name { get; set; }
        public string code { get; set; }
        public decimal? initial_amount { get; set; }
        public bool is_active { get; set; }
        public virtual ICollection<app_account_detail> app_account_detail { get; set; }
        public virtual ICollection<app_account_session> app_account_session { get; set; }
        public virtual IEnumerable<payment_detail> payment_detail { get; set; }
        public virtual app_bank app_bank { get; set; }
        public virtual app_terminal app_terminal { get; set; }
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
                    if (String.IsNullOrEmpty(name))
                        return "Name needs to be filled";
                }
                return "";
            }
        }
    }
}
