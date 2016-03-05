namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel;
    using System.Text;

    public partial class sales_invoice_detail : CommercialSalesDetail, IDataErrorInfo
    {
        public sales_invoice_detail()
        {
            sales_packing_relation = new List<sales_packing_relation>();


            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
           // quantity = 1;

            timestamp = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_sales_invoice_detail { get; set; }
        public int id_sales_invoice { get; set; }
        public int? id_sales_order_detail { get; set; }

        #region Discount Calculations
        /// <summary>
        /// 
        /// </summary>
        public decimal discount
        {
            get { return _discount; }
            set
            {
                if (sales_invoice!=null)
                {
                    if (_discount != value && sales_invoice.State > 0)
                    {
                        unit_price = Calculate_UnitCostDiscount(_discount, value, unit_price);
                        RaisePropertyChanged("unit_price");
                    }
                }
              
                _discount = value;
                RaisePropertyChanged("discount");
            }
        }
        private decimal _discount;
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
            }
        }
        private decimal _DiscountPercentage;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal Discount_SubTotal
        {
            get { return _Discount_SubTotal; }
            set
            {
                if (_Discount_SubTotal != value && quantity > 0) // && value <= unit_cost
                {
                    //Take discount sub total, minus value to create total discount value.
                    //decimal new_discount = _Discount_SubTotal - value;

                    //Update with new value.
                    _Discount_SubTotal = value;
                    RaisePropertyChanged("Discount_SubTotal");

                    //Sends unit_discount value to discount.
                    discount = (_Discount_SubTotal / quantity);
                    RaisePropertyChanged("discount");
                }
            }
        }
        private decimal _Discount_SubTotal;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal Discount_SubTotalPercentage
        {
            get { return _Discount_SubTotalPercentage; }
            set
            {
                _Discount_SubTotalPercentage = value;
                RaisePropertyChanged("Discount_SubTotalPercentage");
            }
        }
        private decimal _Discount_SubTotalPercentage;

        #endregion

        #region "Nav Properties"
        public virtual sales_invoice sales_invoice
        {
            get { return _sales_invoice; }
            set
            {
                if (value != null)
                {


                    if (_sales_invoice != value)
                    {
                        _sales_invoice = value;
                        CurrencyFX_ID = value.id_currencyfx;
                    }
                }
                else
                {
                    _sales_invoice = null;
                    RaisePropertyChanged("sales_invoice");
                }
            }
        }
        private sales_invoice _sales_invoice;
       // public virtual sales_invoice sales_invoice { get; set; }
        public virtual sales_order_detail sales_order_detail { get; set; }
        public virtual ICollection<sales_packing_relation> sales_packing_relation { get; set; }
        public virtual IEnumerable<sales_return_detail> sales_return_detail { get; set; }
        #endregion

        #region "Validation"
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
                if (columnName == "id_item")
                {
                    if (id_item == 0)
                        return "Item needs to be selected";
                }
                if (columnName == "quantity")
                {
                    if (quantity == 0)
                        return "Quantity can not be zero";
                }
                if (columnName == "unit_price")
                {
                    if (unit_price < 0)
                        return "Cannot be less than Zero";
                }
                return "";
            }
        }
        #endregion
    }
}
