namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class payment_type : Audit, IDataErrorInfo
    {
        public payment_type()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            payment_type_detail = new List<payment_type_detail>();
            is_active = true;
        }

        public enum payment_behaviours
        {
            [Display(Name = "Normal")]
            [Description("desc_PaymentType_Normal")]
            Normal,

            [Display(Name = "Credit Note")]
            [Description("desc_PaymentType_CreditNote")]
            CreditNote,

            [Display(Name = "WithHolding VAT")]
            [Description("desc_PaymentType_WithHoldingVAT")]
            WithHoldingVAT, //Removing VAT (full or partial) from the invoice total to pay less.
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_payment_type { get; set; }

        public int? id_document { get; set; }

        [Required]
        public string name { get; set; }

        public payment_behaviours payment_behavior { get; set; }
        public bool is_direct { get; set; }
        public bool is_default { get; set; }

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
        public bool has_bank { get; set; }

        [NotMapped]
        public bool PrintDocument
        {
            get { return id_document != null ? true : false; }
        }

        public virtual app_document app_document { get; set; }
        public virtual ICollection<payment_type_detail> payment_type_detail { get; set; }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();
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