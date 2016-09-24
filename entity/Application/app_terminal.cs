namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    
    public partial class app_terminal : Audit, IDataErrorInfo
    {
        public app_terminal()
        {
            app_account = new List<app_account>();
            sales_invoice = new List<sales_invoice>();
            purchase_invoice = new List<purchase_invoice>();
            is_active = true;
            id_company = Properties.Settings.Default.company_ID;
            id_user = CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_terminal { get; set; }
        public int id_branch { get; set; }
        [Required]
        public string name { get; set; }
        public string code { get; set; }


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
        public virtual app_branch app_branch { get; set; }

        public virtual ICollection<app_account> app_account { get; set; }
        //Sales
        public virtual IEnumerable<sales_order> sales_order { get; set; }
        public virtual IEnumerable<sales_invoice> sales_invoice { get; set; }
        public virtual IEnumerable<sales_return> sales_return { get; set; }
        public virtual IEnumerable<sales_budget> sales_budget { get; set; }
        //Purchase
        public virtual IEnumerable<purchase_order> purchase_order { get; set; }
        public virtual IEnumerable<purchase_invoice> purchase_invoice { get; set; }
        public virtual IEnumerable<purchase_return> purchase_return { get; set; }
        public virtual IEnumerable<purchase_tender> purchase_tender { get; set; }
        public virtual IEnumerable<payment_promissory_note> payment_promissory_note { get; set; }

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
