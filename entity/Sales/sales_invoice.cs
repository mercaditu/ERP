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
            status = Status.Documents_General.Pending;

            sales_invoice_detail = new List<sales_invoice_detail>();
            sales_return = new List<sales_return>();
            payment_withholding_details = new List<payment_withholding_details>();

            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            if (CurrentSession.Id_Branch > 0) { id_branch = CurrentSession.Id_Branch; }
            if (CurrentSession.Id_Terminal > 0) { id_terminal = CurrentSession.Id_Terminal; }
        }

        [NotMapped]
        public new System.Data.Entity.EntityState State
        {
            get { return _State; }
            set
            {
                if (value != _State)
                {
                    _State = value;
                    base.State = value;
                    RaisePropertyChanged("State");

                    foreach (sales_invoice_detail detail in sales_invoice_detail)
                    {
                        detail.State = value;
                    }
                }
            }
        }
        System.Data.Entity.EntityState _State;

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
                    foreach (sales_invoice_detail _sales_invoice_detail in sales_invoice_detail)
                    {
                        _sales_invoice_detail.State = System.Data.Entity.EntityState.Modified;
                        _sales_invoice_detail.CurrencyFX_ID = _id_currencyfx;
                    }
                }
                RaisePropertyChanged("GrandTotal");
                calc_credit(GrandTotal);
            }
        }
        private int _id_currencyfx;


        [NotMapped]
        public new decimal GrandTotal
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

        [NotMapped]
        public decimal TotalVat
        {
            get
            {
                _TotalVat = 0;
                foreach (sales_invoice_detail _sales_invoice_detail in sales_invoice_detail)
                {
                    _TotalVat += _sales_invoice_detail.SubTotal_Vat - _sales_invoice_detail.SubTotal;
                }
                return Math.Round(_TotalVat, 2);
            }
            set
            {
                _TotalVat = value;
                RaisePropertyChanged("TotalVat");
            }
        }
        private decimal _TotalVat;

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

                decimal Discounted_GrandTotalValue = GrandTotal * DiscountPercentage;

                if (Discounted_GrandTotalValue != 0 && GrandTotal > 0)
                {
                    foreach (sales_invoice_detail detail in this.sales_invoice_detail.Where(x => x.quantity > 0))
                    {
                        decimal WeightedAvg = detail.SubTotal_Vat / GrandTotal;
                        detail.DiscountVat = (WeightedAvg * Discounted_GrandTotalValue) / detail.quantity;
                        detail.RaisePropertyChanged("DiscountVat");
                    }
                    RaisePropertyChanged("GrandTotal");
                }
            }
        }
        private decimal _DiscountPercentage;

        [NotMapped]
        public decimal DiscountWithoutPercentage
        {
            get { return _DiscountWithoutPercentage; }
            set
            {
                _DiscountWithoutPercentage = value;
                RaisePropertyChanged("DiscountWithoutPercentage");

                decimal DiscountValue = value;
                if (DiscountValue != 0)
                {
                    decimal PerRawDiscount = DiscountValue / sales_invoice_detail.Where(x => x.quantity > 0).Count();
                    foreach (var item in sales_invoice_detail.Where(x => x.quantity > 0))
                    {
                        item.DiscountVat = PerRawDiscount / item.quantity;
                        item.RaisePropertyChanged("DiscountVat");
                    }
                    RaisePropertyChanged("GrandTotal");
                }
                else
                {
                    foreach (var item in sales_invoice_detail.Where(x => x.quantity > 0))
                    {
                        item.DiscountVat = 0;
                        item.RaisePropertyChanged("DiscountVat");
                    }
                    RaisePropertyChanged("GrandTotal");
                }

            }
        }
        private decimal _DiscountWithoutPercentage;
        [NotMapped]
        public decimal vatwithholdingpercentage { get; set; }
        //TimeCapsule
        public ICollection<sales_invoice> older { get; set; }
        public sales_invoice newer { get; set; }

        public void UpdateVAT_Totals()
        {
            ICollection<CommercialVAT> _CommercialVAT = new List<CommercialVAT>();
            foreach (sales_invoice_detail detail in sales_invoice_detail)
            {
                _CommercialVAT.Add(null);
                //Calculate here, and return sum values into. Put entire code into VAT Brillo.
            }
        }

        #region "Foreign Key"
        public virtual crm_opportunity crm_opportunity { get; set; }
        public virtual sales_order sales_order { get; set; }
        public virtual accounting_journal accounting_journal { get; set; }

        public virtual ICollection<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual ICollection<sales_return> sales_return { get; set; }
        public virtual ICollection<payment_schedual> payment_schedual { get; set; }

        public virtual IEnumerable<payment_withholding_detail> payment_withholding_detail { get; set; }
        public virtual ICollection<payment_withholding_details> payment_withholding_details { get; set; }
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
                    if (id_contact == 0)
                        return "Contact needs to be selected";
                }
                if (columnName == "id_branch" && app_branch == null)
                {
                    if (id_branch == 0)
                        return "Branch needs to be selected";
                }
                if (columnName == "id_condition" && app_condition == null)
                {
                    if (id_condition == 0)
                        return "Condition needs to be selected";
                }
                if (columnName == "id_contract" && app_contract == null)
                {
                    if (id_contract == 0)
                        return "Contract needs to be selected";
                }
                if (columnName == "id_currencyfx")
                {
                    if (id_currencyfx == 0)
                        return "Currency needs to be selected";
                }
                if (columnName == "DiscountPercentage")
                {
                    if (DiscountPercentage > 1)
                        return "Discount percentage not excedd 100%";
                }
                return "";
            }
        }
        #endregion

    }
}
