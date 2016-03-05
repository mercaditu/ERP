
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class purchase_return_detail : CommercialPurchaseDetail, INotifyPropertyChanged, IDataErrorInfo
    {
        public purchase_return_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            quantity = 1;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_return_detail { get; set; }
        public int id_purchase_return { get; set; }
        public int? id_purchase_invoice_detail { get; set; }

        public bool has_return { get; set; }

        #region Discount Calculations
        /// <summary>
        /// 
        /// </summary>
        public decimal discount
        {
            get { return _discount; }
            set
            {
                if (_discount != value)
                {
                    unit_cost = Discount.Calculate_Discount(_discount, value, unit_cost);
                    RaisePropertyChanged("unit_cost");
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

        public virtual purchase_return purchase_return { get; set; }
        public virtual purchase_invoice_detail purchase_invoice_detail { get; set; }
        public virtual ICollection<purchase_return_dimension> purchase_return_dimension { get; set; }

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
                if (columnName == "quantity")
                {
                    if (quantity == 0)
                        return "Quantity can not be zero";
                }
                return "";
            }
        }
        #endregion
    }
}
