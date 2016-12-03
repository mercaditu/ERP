namespace entity
{

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

            status = Status.Documents_General.Pending;

            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            if (CurrentSession.Id_Branch > 0) { id_branch = CurrentSession.Id_Branch; }
            if (CurrentSession.Id_Terminal > 0) { id_terminal = CurrentSession.Id_Terminal; }

            purchase_invoice_detail = new List<purchase_invoice_detail>();
            purchase_return = new List<purchase_return>();

            payment_withholding_detail = new List<payment_withholding_detail>();
            impex_expense = new List<impex_expense>();
          
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
                //if (_id_currencyfx != value)
                //{
                _id_currencyfx = value;
                RaisePropertyChanged("id_currencyfx");

                if (State != System.Data.Entity.EntityState.Unchanged && State > 0)
                {
                    foreach (purchase_invoice_detail _purchase_invoice_detail in purchase_invoice_detail)
                    {
                        _purchase_invoice_detail.State = System.Data.Entity.EntityState.Modified;
                        _purchase_invoice_detail.CurrencyFX_ID = _id_currencyfx;
                    }
                    RaisePropertyChanged("GrandTotal");
                }
                //}
            }
        }
        private int _id_currencyfx;
        public bool is_accounted { get; set; }

        //TimeCapsule
        public ICollection<purchase_invoice> older { get; set; }
        public purchase_invoice newer { get; set; }

        [NotMapped]
        public new decimal GrandTotal
        {
            get
            {
                _GrandTotal = purchase_invoice_detail.Sum(x => x.SubTotal_Vat);
                return _GrandTotal;
            }
            set
            {
                //decimal OriginalValue = value - _GrandTotal;
                //if (OriginalValue != 0)
                //{
                //    decimal DifferenceValue = OriginalValue / purchase_invoice_detail.Count;
                //    foreach (var item in purchase_invoice_detail)
                //    {

                //        item.UnitCost_Vat = item.UnitCost_Vat + DifferenceValue / item.quantity;
                //        item.RaisePropertyChanged("UnitCost_Vat");
                //    }


                //}

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
                foreach (var item in purchase_invoice_detail)
                {
                    _TotalVat += item.SubTotal_Vat - item.SubTotal;
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
                    foreach (purchase_invoice_detail detail in this.purchase_invoice_detail.Where(x => x.quantity > 0))
                    {
                        decimal WeightedAvg = detail.SubTotal_Vat / GrandTotal;
                        detail.DiscountVat = (WeightedAvg * Discounted_GrandTotalValue) / detail.quantity;
                        detail.RaisePropertyChanged("DiscountVat");
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

                decimal DiscountValue = value;
                if (DiscountValue != 0)
                {
                    decimal PerRawDiscount = DiscountValue / purchase_invoice_detail.Where(x => x.quantity > 0).Count();
                    foreach (var item in purchase_invoice_detail.Where(x => x.quantity > 0))
                    {

                        item.DiscountVat = PerRawDiscount / item.quantity;
                        item.RaisePropertyChanged("DiscountVat");
                        RaisePropertyChanged("GrandTotal");
                    }
                }
                else
                {
                    foreach (var item in purchase_invoice_detail.Where(x => x.quantity > 0))
                    {

                        item.DiscountVat = 0;
                        item.RaisePropertyChanged("DiscountVat");
                        RaisePropertyChanged("GrandTotal");
                    }
                }

            }
        }
        private decimal _DiscountWithoutPercentage;

        [NotMapped]
        public bool displayexpire
        {
            get
            {
                foreach (purchase_invoice_detail _purchase_invoice_detail in purchase_invoice_detail)
                {
                    if (_purchase_invoice_detail.item != null)
                    {
                        if (_purchase_invoice_detail.item.item_product.FirstOrDefault() != null)
                        {
                            if (_purchase_invoice_detail.item.item_product.FirstOrDefault().can_expire)
                            {
                                _displayexpire = true;
                                return _displayexpire;
                            }
                        }
                    }
                }
                _displayexpire = false;
                return _displayexpire;
            }
            set { _displayexpire=value; }
        }
        Boolean _displayexpire=false;

        #region "Navigation Properties"
        public virtual purchase_order purchase_order { get; set; }
        public virtual app_department app_department { get; set; }


        public virtual ICollection<purchase_invoice_detail> purchase_invoice_detail
        {
            get
            {
                return _purchase_invoice_detail;
            }
            set
            {
                _purchase_invoice_detail = value;
            }
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
                    if (id_contract == 0 && app_contract == null)
                        return "Contract needs to be selected";
                }
                if (columnName == "id_currencyfx")
                {
                    if (id_currencyfx == 0 && app_currencyfx == null)
                        return "Currency needs to be selected";
                }
                return "";
            }
        }
        #endregion
    }
}
