namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public partial class sales_invoice : CommercialHead, IDataErrorInfo
    {
        public sales_invoice()
        {
            is_head = true;
            is_issued = false;
            status = Status.Documents_General.Pending;

            sales_invoice_detail = new List<sales_invoice_detail>();
            payment_schedual = new List<payment_schedual>();
            sales_return = new List<sales_return>();
            payment_withholding_detail = new List<payment_withholding_detail>();

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
                    RaisePropertyChanged("State");
                    base.State = value;

                    foreach (sales_invoice_detail detail in sales_invoice_detail)
                    {
                        detail.State = value;
                    }
                }
            }
        }

        private System.Data.Entity.EntityState _State;

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

        public bool is_accounted { get; set; }

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
                if (_id_currencyfx != value)
                {
                    _id_currencyfx = value;
                    RaisePropertyChanged("id_currencyfx");
                    RaisePropertyChanged("app_currencyfx");

                    if (State != System.Data.Entity.EntityState.Unchanged && State > 0)
                    {
                        foreach (sales_invoice_detail _sales_invoice_detail in sales_invoice_detail)
                        {
                            _sales_invoice_detail.State = System.Data.Entity.EntityState.Modified;
                            _sales_invoice_detail.CurrencyFX_ID = _id_currencyfx;
                        }
                    }
                    RaisePropertyChanged("GrandTotal");
                }
            }
        }

        private int _id_currencyfx;

        /// <summary>
        /// Grand Total of the Sale including VAT.
        /// </summary>
        [NotMapped]
        public new decimal GrandTotal
        {
            get
            {
                _GrandTotal = sales_invoice_detail.Sum(x => x.SubTotal_Vat);

                return _GrandTotal;
            }
            set
            {
                _GrandTotal = value;
                RaisePropertyChanged("GrandTotal");
            }
        }

        private decimal _GrandTotal;

        /// <summary>
        /// Grand Total of the Sale including VAT.
        /// </summary>
        [NotMapped]
        public decimal GrandTotalCost
        {
            get
            {
                _GrandTotalCost = sales_invoice_detail.Sum(x => x.SubTotalUnitCost_Vat);
                return _GrandTotalCost;
            }
            set
            {
                _GrandTotalCost = value;
                RaisePropertyChanged("GrandTotalCost");
            }
        }
        private decimal _GrandTotalCost;

        /// <summary>
        /// Gets total value of VAT for each detail.
        /// </summary>
        [NotMapped]
        public decimal TotalVATCost
        {
            get
            {
                _TotalVatCost = sales_invoice_detail.Sum(x => x.SubTotalUnitCost_Vat - x.SubTotalUnitCost);
                return Math.Round(_TotalVatCost, 2);
            }
            set
            {
                _TotalVatCost = value;
                RaisePropertyChanged("TotalVATCost");
            }
        }

        private decimal _TotalVatCost;

        /// <summary>
        /// Gets total value of VAT for each detail.
        /// </summary>
        [NotMapped]
        public decimal TotalVat
        {
            get
            {
                _TotalVat = sales_invoice_detail.Sum(x => x.SubTotal_Vat - x.SubTotal);
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

                if (State > 0)
                {
                    decimal Discounted_GrandTotalValue = GrandTotal * DiscountPercentage;
                    decimal Fixed_GrandTotal = GrandTotal;

                    if (Discounted_GrandTotalValue >= 0 && Fixed_GrandTotal > 0)
                    {
                        foreach (sales_invoice_detail detail in this.sales_invoice_detail.Where(x => x.quantity > 0))
                        {
                            decimal WeightedAvg = detail.SubTotal_Vat / Fixed_GrandTotal;
                            detail.DiscountVat = (WeightedAvg * Discounted_GrandTotalValue) / detail.quantity;
                            detail.RaisePropertyChanged("DiscountVat");
                        }
                        RaisePropertyChanged("GrandTotal");
                    }
                    else
                    {
                        foreach (sales_invoice_detail detail in this.sales_invoice_detail.Where(x => x.quantity > 0))
                        {

                            detail.DiscountVat = 0;
                            detail.RaisePropertyChanged("DiscountVat");
                        }
                        RaisePropertyChanged("GrandTotal");

                    }
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

                if (State > 0)
                {
                    decimal DiscountValue = value;

                    if (DiscountValue >= 0)
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
        }

        private decimal _DiscountWithoutPercentage;

        [NotMapped]
        public decimal vatwithholdingpercentage { get; set; }

        [NotMapped]
        public app_location Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
                if (_Location!=null)
                {
                    foreach (sales_invoice_detail item in sales_invoice_detail.Where(x=>x.id_sales_invoice_detail>0).ToList())
                    {
                        
                            item.id_location = _Location.id_location;
                            item.RaisePropertyChanged("id_location");
                        
                     
                    }
                }
            }
        }
        app_location _Location;

        [NotMapped]
        public decimal TotalChanged { get; set; }

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
        [NotMapped]
        public decimal sub_Total_Factoredvat
        {
            get
            {
                decimal TotalValue = 0;
                foreach (var item in sales_invoice_detail)
                {
                    TotalValue += Math.Round(item.Quantity_Factored * item.UnitPrice_Vat, 2);
                }
                return TotalValue;
            }
            set { _sub_Total_Factoredvat = value; RaisePropertyChanged("sub_Total_Factoredvat"); }
        }
        decimal _sub_Total_Factoredvat;


        #region "Foreign Key"

        public virtual crm_opportunity crm_opportunity { get; set; }
        public virtual sales_order sales_order { get; set; }
        public virtual ICollection<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual ICollection<sales_return> sales_return { get; set; }
        public virtual ICollection<payment_schedual> payment_schedual { get; set; }
        public virtual IEnumerable<payment_withholding_detail> payment_withholding_detail { get; set; }

        #endregion "Foreign Key"

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
                        return Brillo.Localize.PleaseSelect;
                }
                if (columnName == "id_branch" && app_branch == null)
                {
                    if (id_branch == 0)
                        return Brillo.Localize.PleaseSelect;
                }
                if (columnName == "id_condition" && app_condition == null)
                {
                    if (id_condition == 0)
                        return Brillo.Localize.PleaseSelect;
                }
                if (columnName == "id_contract" && app_contract == null)
                {
                    if (id_contract == 0)
                        return Brillo.Localize.PleaseSelect;
                }
                if (columnName == "id_currencyfx")
                {
                    if (id_currencyfx == 0)
                        return Brillo.Localize.PleaseSelect;
                }
                if (columnName == "DiscountPercentage")
                {
                    if (DiscountPercentage > 1)
                        return "Discount percentage can't exceed 100%";
                }
                return "";
            }
        }

        #endregion "Validations"
    }
}