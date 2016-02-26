
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;

    public partial class sales_invoice : CommercialHead, IDataErrorInfo
    {
        public sales_invoice()
        {
            is_head = true;
            is_issued = false;
            //is_accounted = false;
            status = Status.Documents_General.Pending;
            
            sales_invoice_detail = new List<sales_invoice_detail>();
            sales_return = new List<sales_return>();

            Properties.Settings _settings = new Properties.Settings();
            id_company = _settings.company_ID;
            id_user = _settings.user_ID;
            if (_settings.branch_ID > 0) { id_branch = _settings.branch_ID; }
            if (_settings.terminal_ID > 0) { id_terminal = _settings.terminal_ID; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_invoice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? id_sales_order { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int id_opportunity { get; set; }

        public int? id_journal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_currencyfx
        {
            get {
                return _id_currencyfx; }
            set
            {
                _id_currencyfx = value;
                RaisePropertyChanged("id_currencyfx");

                if (State != System.Data.Entity.EntityState.Unchanged && State > 0)
                {
                    foreach (sales_invoice_detail _sales_invoice_detail in sales_invoice_detail)
                    {
                        _sales_invoice_detail.State = System.Data.Entity.EntityState.Modified;
                        _sales_invoice_detail.CurrencyFX_ID = _id_currencyfx;
                    }
                }
                calc_credit(GrandTotal);
            }
        }
        private int _id_currencyfx;


        [NotMapped]
        public decimal GrandTotal
        {
            get
            {
                _GrandTotal = 0;
                foreach (sales_invoice_detail _sales_invoice_detail in sales_invoice_detail)
                {
                    _GrandTotal += _sales_invoice_detail.SubTotal_Vat;
                }

                calc_credit(_GrandTotal);
                return Math.Round(_GrandTotal, 2);
            }
            set
            {
                _GrandTotal = value;
                RaisePropertyChanged("GrandTotal");
            }
        }
        private decimal _GrandTotal;


        //TimeCapsule
        public ICollection<sales_invoice> older { get; set; }
        public sales_invoice newer { get; set; }
        
        #region "Foreign Key"
        public virtual crm_opportunity crm_opportunity { get; set; }
        public virtual sales_order sales_order { get; set; }
        public virtual accounting_journal accounting_journal { get; set; }

        public virtual ICollection<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual ICollection<sales_return> sales_return { get; set; }
        public virtual ICollection<payment_schedual> payment_schedual { get; set; }
      
        public virtual IEnumerable<payment_withholding_detail> payment_withholding_detail { get; set; }
        #endregion

        #region "Validations"
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
                if (columnName == "id_contact")
                {
                    if (id_contact == 0 )
                        return "Contact needs to be selected";
                }
                if (columnName == "id_branch" && app_branch==null)
                {
                    if (id_branch == 0)
                        return "Branch needs to be selected";
                }
                if (columnName == "id_condition" && app_condition==null)
                {
                    if (id_condition == 0)
                        return "Condition needs to be selected";
                }
                if (columnName == "id_contract" && app_contract==null)
                {
                    if (id_contract == 0)
                        return "Contract needs to be selected";
                }
                if (columnName == "id_currencyfx" )
                {
                    if (id_currencyfx == 0)
                        return "Currency needs to be selected";
                }
                return "";
            }
        }
        #endregion

    }
}
