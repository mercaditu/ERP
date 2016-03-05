namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel;
    using System.Text;

    public partial class sales_budget_detail : CommercialSalesDetail, IDataErrorInfo
    {
        public sales_budget_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            quantity = 1;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_budget_detail { get; set; }
        public int id_sales_budget { get; set; }

        #region Discount Calculations
        /// <summary>
        /// 
        /// </summary>
        public decimal discount
        {
            get { return _discount; }
            set
            {
                if (sales_budget!=null)
                {
                    if (_discount != value && sales_budget.State > 0)
                    {
                        //unit_price = Calculate_UnitCostDiscount(_discount, value, unit_price);
                        RaisePropertyChanged("unit_price");
                    }
                }
              
                _discount = value;
                RaisePropertyChanged("discount");
            }
        }
        private decimal _discount;

         [NotMapped]
        public decimal discountVat
        {
            get { return _discountVat; }
            set
            {
                if (_discountVat != value)
                {
                   // UnitPrice_Vat = Calculate_UnitCostVatDiscount();
                    RaisePropertyChanged("UnitPrice_Vat");
                }
                _discountVat = value;
                RaisePropertyChanged("discountVat");
            }
        }
        private decimal _discountVat;
         //<summary>
         //Discounts based on percentage value inserted by user. Converts into value, and returns it to Discount Property.
         //</summary>
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
                //if (_Discount_SubTotal != value && quantity > 0) // && value <= unit_cost
                //{
                //    //Take discount sub total, minus value to create total discount value.
                //    //decimal new_discount = _Discount_SubTotal - value;

                //    //Update with new value.
                //    _Discount_SubTotal = value;
                //    RaisePropertyChanged("Discount_SubTotal");

                //    //Sends unit_discount value to discount.
                //    discount = (_Discount_SubTotal / quantity);
                //    RaisePropertyChanged("discount");
                //}
                if (_Discount_SubTotal != value)
                {
                   // SubTotal = Calculate_SubTotalDiscount();
                    RaisePropertyChanged("SubTotal");
                }
                _Discount_SubTotal = value;
                RaisePropertyChanged("Discount_SubTotal");
            }
        }
        private decimal _Discount_SubTotal;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal Discount_SubTotalVat
        {
            get { return _Discount_SubTotalVat; }
            set
            {
                //if (_Discount_SubTotal != value && quantity > 0) // && value <= unit_cost
                //{
                //    //Take discount sub total, minus value to create total discount value.
                //    //decimal new_discount = _Discount_SubTotal - value;

                //    //Update with new value.
                //    _Discount_SubTotal = value;
                //    RaisePropertyChanged("Discount_SubTotal");

                //    //Sends unit_discount value to discount.
                //    discount = (_Discount_SubTotal / quantity);
                //    RaisePropertyChanged("discount");
                //}
                if (_Discount_SubTotalVat != value)
                {
                   // SubTotal_Vat = Calculate_SubTotalVatDiscount();
                    RaisePropertyChanged("SubTotal_Vat");
                }
                _Discount_SubTotalVat = value;
                RaisePropertyChanged("_Discount_SubTotalVat");
            }
        }
        private decimal _Discount_SubTotalVat;

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

        #region "Foreign Key"
        public virtual sales_budget sales_budget
        {
            get { return _sales_budget; }
            set
            {
                if (value!=null)
                {
                    if (_sales_budget != value)
                    {
                        _sales_budget = value;
                        CurrencyFX_ID = value.id_currencyfx;
                    }
                }
                else
                {
                    _sales_budget = null;
                    RaisePropertyChanged("sales_budget");
                    
                }
               
            }
        }
        private sales_budget _sales_budget;
        public virtual IEnumerable<sales_order_detail> sales_order_detail { get; set; }
        #endregion

        #region "Validation"
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
