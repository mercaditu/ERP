namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_account_detail : Audit, IDataErrorInfo
    {
        public enum tran_types
        {
            Open = 1,
            Transaction = 2,
            Close = 3
        }

        public app_account_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            trans_date = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_account_detail { get; set; }
        [Required]
        public int id_account { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_currencyfx { get; set; }
        public int? id_payment_detail { get; set; }
        public int id_payment_type { get; set; }
        public Status.Documents_General status { get; set; }
        public decimal debit { get; set; }
        public decimal credit { get; set; }
        public string comment { get; set; }
        [Required]
        public DateTime trans_date { get; set; }
        public int? id_session { get; set; }
        public tran_types? tran_type { get; set; }

        public virtual app_account app_account { get; set; }
        public virtual app_currencyfx app_currencyfx { get; set; }
        public virtual payment_type payment_type { get; set; }
        public virtual payment_detail payment_detail { get; set; }
        public virtual app_account_session app_account_session { get; set; }
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
                if (columnName == "id_currencyfx")
                {
                    if (id_currencyfx == 0)
                        return "Currencyfx needs to be filled";
                }
                if (columnName == "trans_date")
                {
                    if (trans_date == null)
                        return "Transaction date needs to be filled";
                }
                return "";
            }
        }
    }
}
