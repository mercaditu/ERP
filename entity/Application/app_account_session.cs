namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_account_session : Audit, IDataErrorInfo
    {
        public app_account_session()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;

            op_date = DateTime.Now;
            is_active = true;
            app_account_detail = new List<app_account_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_session { get; set; }

        public DateTime op_date { get; set; }

        public DateTime? cl_date { get; set; }

        public int id_account { get; set; }
        public bool is_active { get; set; }
        public virtual ICollection<app_account_detail> app_account_detail { get; set; }
        public virtual ICollection<app_account_detail_archive> app_account_detail_archive { get; set; }
        public virtual app_account app_account { get; set; }

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

                return "";
            }
        }
    }
}