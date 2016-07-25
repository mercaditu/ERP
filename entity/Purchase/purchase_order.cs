namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public partial class purchase_order : CommercialHead, IDataErrorInfo
    {
        public purchase_order()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            Properties.Settings _settings = new Properties.Settings();
            purchase_order_detail = new List<purchase_order_detail>();
            purchase_invoice = new List<purchase_invoice>();
            trans_date = DateTime.Now;

            if (_settings.branch_ID > 0) { id_branch = _settings.branch_ID; }
            if (_settings.terminal_ID > 0) { id_terminal = _settings.terminal_ID; }
            status = Status.Documents_General.Pending;
         
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_order { get; set; }
        public int? id_purchase_tender { get; set; }
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
                    foreach (purchase_order_detail _purchase_order_detail in purchase_order_detail)
                    {
                        _purchase_order_detail.State = System.Data.Entity.EntityState.Modified;
                        _purchase_order_detail.CurrencyFX_ID = _id_currencyfx;
                    }
                    RaisePropertyChanged("GrandTotal");
                }
            }
        }
        private int _id_currencyfx;

        public DateTime? recieve_date_est { get; set; }

        [NotMapped]
        public new decimal GrandTotal
        {
            get
            {
                _GrandTotal = 0;
                foreach (purchase_order_detail _purchase_invoice_detail in purchase_order_detail)
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
                    decimal DifferenceValue = OriginalValue / purchase_order_detail.Count;
                    foreach (var item in purchase_order_detail)
                    {

                        item.UnitCost_Vat = item.UnitCost_Vat + DifferenceValue / item.quantity;
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

                decimal Discounted_GrandTotalValue = GrandTotal * DiscountPercentage;

                if (Discounted_GrandTotalValue != 0 && GrandTotal > 0)
                {
                    foreach (purchase_order_detail detail in this.purchase_order_detail.Where(x => x.quantity > 0))
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
                    decimal PerRawDiscount = DiscountValue / purchase_order_detail.Where(x => x.quantity > 0).Count();
                    foreach (var item in purchase_order_detail.Where(x => x.quantity > 0))
                    {

                        item.DiscountVat = PerRawDiscount / item.quantity;
                        item.RaisePropertyChanged("DiscountVat");
                        RaisePropertyChanged("GrandTotal");
                    }
                }
                else
                {
                    foreach (var item in purchase_order_detail.Where(x => x.quantity > 0))
                    {

                        item.DiscountVat = 0;
                        item.RaisePropertyChanged("DiscountVat");
                        RaisePropertyChanged("GrandTotal");
                    }
                }

            }
        }
        private decimal _DiscountWithoutPercentage;


        //TimeCapsule
        public ICollection<purchase_order> older { get; set; }
        public purchase_order newer { get; set; }

        public virtual ICollection<purchase_order_detail> purchase_order_detail { get; set; }
        public virtual ICollection<purchase_invoice> purchase_invoice { get; set; }
        public virtual IEnumerable<payment_schedual> payment_schedual { get; set; }
        public virtual app_department app_department { get; set; }
        public virtual purchase_tender purchase_tender { get; set; }

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
    }
}
