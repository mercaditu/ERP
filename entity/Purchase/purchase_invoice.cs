namespace entity
{
    using Brillo;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public partial class purchase_invoice : CommercialHead, IDataErrorInfo
    {
        public purchase_invoice()
        {
            is_head = true;
            is_issued = false;
            //is_accounted = false;

            status = Status.Documents_General.Pending;
              
            Properties.Settings _settings = new Properties.Settings();
            id_company = _settings.company_ID;
            id_user = _settings.user_ID;
            if (_settings.branch_ID > 0) { id_branch = _settings.branch_ID; }
            if (_settings.terminal_ID > 0) { id_terminal = _settings.terminal_ID; }

            purchase_invoice_detail = new List<purchase_invoice_detail>();
            purchase_return = new List<purchase_return>();
         
         
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int id_purchase_invoice { get; set; }
        public int? id_purchase_order { get; set; }
        public int? id_department { get; set; }
        
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_currencyfx
        {
            get
            {
                return _id_currencyfx;
            }
            set
            {
                _id_currencyfx = value;
                RaisePropertyChanged("id_currencyfx");

                if (State != System.Data.Entity.EntityState.Unchanged && State > 0)
                {
                    foreach (purchase_invoice_detail _purchase_invoice_detail in purchase_invoice_detail)
                    {
                        _purchase_invoice_detail.State = System.Data.Entity.EntityState.Modified;
                        _purchase_invoice_detail.CurrencyFX_ID = _id_currencyfx;
                    }
                }
            }
        }
        private int _id_currencyfx;

        public int? id_journal { get; set; }

        //TimeCapsule
        public ICollection<purchase_invoice> older { get; set; }
        public purchase_invoice newer { get; set; }

        [NotMapped]
        public new decimal GrandTotal
        {
            get
            {
                _GrandTotal = 0;
                foreach (purchase_invoice_detail _purchase_invoice_detail in purchase_invoice_detail)
                {
                    _GrandTotal += _purchase_invoice_detail.SubTotal_Vat;
                }
                return Math.Round(_GrandTotal, 2);
            }
            set
            {
                decimal OriginalValue = value - _GrandTotal;
                if (OriginalValue != 0)
                {
                    decimal DifferenceValue = OriginalValue / purchase_invoice_detail.Sum(x => x.quantity);
                    foreach (var item in purchase_invoice_detail)
                    {
                        item.UnitCost_Vat = item.UnitCost_Vat + DifferenceValue;
                        item.RaisePropertyChanged("UnitCost_Vat");
                    }

                    _GrandTotal = value;
                    RaisePropertyChanged("GrandTotal");
                }
            }
        }
        private decimal _GrandTotal;


        /// <summary>
        /// Discounts based on percentage value inserted by user. Converts into value, and returns it to Discount Property.
        /// </summary>
        [NotMapped]
        public decimal DiscountPercentage
        {
            get { return _DiscountPercentage; }
            set
            {
                _DiscountPercentage = value;
                RaisePropertyChanged("DiscountPercentage");

               

                decimal OriginalValue =GrandTotal * DiscountPercentage ;
                if (OriginalValue != 0)
                {
                    decimal DifferenceValue = OriginalValue / purchase_invoice_detail.Count;
                    foreach (var item in purchase_invoice_detail)
                    {
                        DifferenceValue = DifferenceValue / item.quantity;
                        item.discount = DifferenceValue;
                        item.RaisePropertyChanged("discount");
                    }

                    
                }
            }
        }
        private decimal _DiscountPercentage;

        #region "Navigation Properties"
        public virtual purchase_order purchase_order { get; set; }
        public virtual app_department app_department { get; set; }

        public virtual accounting_journal accounting_journal { get; set; }

        public virtual ICollection<purchase_invoice_detail> purchase_invoice_detail 
        { 
            get { 
               return  _purchase_invoice_detail;
            } 
            set { 
                _purchase_invoice_detail=value;} 
        }
        ICollection<purchase_invoice_detail> _purchase_invoice_detail; 
        public virtual IEnumerable<purchase_packing_relation> purchase_packing_relation { get; set; }

        public virtual IEnumerable<purchase_return> purchase_return { get; set; }
        public virtual IEnumerable<impex_expense> impex_expense { get; set; }
        public virtual IEnumerable<payment_withholding_detail> payment_withholding_detail { get; set; }
        public virtual ICollection<payment_schedual> payment_schedual { get; set; }
        #endregion

        #region Validation
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
                if (columnName == "id_contact")
                {
                    if (id_contact == 0)
                        return "Contact needs to be selected";
                }
                if (columnName == "id_branch")
                {
                    if (id_branch == 0)
                        return "Branch needs to be selected";
                }
                if (columnName == "id_condition")
                {
                    if (id_condition == 0)
                        return "Condition needs to be selected";
                }
                if (columnName == "id_contract")
                {
                    if (id_contract == 0)
                        return "Contract needs to be selected";
                }
                if (columnName == "id_currencyfx")
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
