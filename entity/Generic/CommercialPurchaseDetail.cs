namespace entity
{
    using Brillo;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CommercialPurchaseDetail : Audit
    {
        /// <summary>
        /// Location ID. Nullable, because
        /// 1) We can sell non stockable items.
        /// 2) Location is irrelevant until we approve.
        /// </summary>
        public int? id_location { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int id_cost_center
        {
            get { return _id_cost_center; }
            set
            {
                _id_cost_center = value;
                RaisePropertyChanged("id_cost_center");
            }
        }
        private int _id_cost_center;

        /// <summary>
        /// Item ID related to this Detail.
        /// </summary>
        public int? id_item
        {
            get { return _id_item; }
            set
            {
                if (value > 0 && value != _id_item)
                {
                    _id_item = value;
                    RaisePropertyChanged("id_item");

                    if (State != System.Data.Entity.EntityState.Unchanged && State > 0)
                    {
                        //If Item Exist, then load up Data.
                        if (item != null)
                        {
                            id_vat_group = Vat.getItemVat(item);
                            RaisePropertyChanged("id_vat_group");
                            item_description = item.name;
                            RaisePropertyChanged("item_description");
                        }

                        //Update Cost based
                        update_UnitCost(_CurrencyFX_ID);
                    }
                }
            }
        }
        private int? _id_item;

        /// <summary>
        /// 
        /// </summary>
        public string item_description
        {
            get
            {
                return _item_description;
            }
            set
            {
                if (value != _item_description)
                {
                    _item_description = value;
                    RaisePropertyChanged("item_description");
                }
            }
        }
        private string _item_description;

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public decimal quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                   _quantity = value;
                   RaisePropertyChanged("quantity");
                   //update quantity
                   update_SubTotal();
                   //_Quantity_Factored = Brillo.ConversionFactor.Factor_Quantity(item, quantity,);
                   //RaisePropertyChanged("_Quantity_Factored");
                }
            }
        }
        private decimal _quantity;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal Quantity_Factored
        {
            get { return _Quantity_Factored; }
            set
            {
              if (_Quantity_Factored != value)
                {
                    _Quantity_Factored = value;
                    RaisePropertyChanged("Quantity_Factored");
                    //quantity = Brillo.ConversionFactor.Factor_Quantity_Back(item, Quantity_Factored);
                    //RaisePropertyChanged("quantity");
                }
            }
        }
        private decimal _Quantity_Factored;

        /// <summary>
        /// 
        /// </summary>
        public string lot_number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? expiration_date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int? id_vat_group
        {
            get { return _id_vat_group; }
            set
            {
                if (value != null)
                {
                    if (_id_vat_group != value)
                    {
                        _id_vat_group = (int)value;
                        RaisePropertyChanged("id_vat_group");

                        update_UnitPriceVAT();
                    }
                }

            }
        }
        private int _id_vat_group;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public int CurrencyFX_ID
        {
            get
            {
                return _CurrencyFX_ID;
            }
            set
            {
                if (_CurrencyFX_ID != value)
                {
                    if (State != System.Data.Entity.EntityState.Unchanged && State > 0)
                    {
                        unit_cost = Currency.convert_Values(unit_cost, _CurrencyFX_ID, value, App.Modules.Purchase);
                        RaisePropertyChanged("unit_cost");
                    }
                    _CurrencyFX_ID = value;
                }
            }
        }
        private int _CurrencyFX_ID;

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public decimal unit_cost
        {
            get { return _unit_cost; }
            set
            {
                if (_unit_cost != value)
                {
                    _unit_cost = value;
                    RaisePropertyChanged("unit_cost");

                    update_UnitPriceVAT();
                    update_SubTotal();
                }
            }
        }
        private decimal _unit_cost;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal UnitCost_Vat
        {
            get { return _UnitCost_Vat; }
            set
            {
                if (_UnitCost_Vat != value)
                {
                    _UnitCost_Vat = value;
                    RaisePropertyChanged("UnitCost_Vat");
                    update_UnitPrice_WithoutVAT();
                }
                update_SubTotalVAT();
            }
        }
        private decimal _UnitCost_Vat;



        /// <summary>
        /// 
        /// </summary>
        public string comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal SubTotal
        {
            get { return _SubTotal; }
            set
            {
                _SubTotal = value;
                RaisePropertyChanged("SubTotal");
                update_SubTotalVAT();
            }
        }
        private decimal _SubTotal;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal SubTotal_Vat
        {
            get { return _SubTotal_Vat; }
            set
            {
                _SubTotal_Vat = value;
                RaisePropertyChanged("SubTotal_Vat");

                RaisePropertyChanged("GrandTotal");
            }
        }
        private decimal _SubTotal_Vat;


        #region Discount Calculations

        /// <summary>
        /// Basic Discount column that is stored in database. Realvalue.
        /// </summary>
        public decimal discount
        {
            get { return _discount; }
            set
            {
                if (_discount != value)
                {
                    if (State > 0)
                    {
                        ApplyDiscount_UnitPrice(_discount, value, unit_cost);

                        _discount = value;
                        RaisePropertyChanged("discount");

                        Calculate_UnitVatDiscount(_discount);
                        Calculate_SubTotalDiscount(_discount);

                    }
                    else
                    {
                        _discount = value;
                        RaisePropertyChanged("discount");

                        Calculate_UnitVatDiscount(_discount);
                        Calculate_SubTotalDiscount(_discount);
                    }
                }
            }
        }
        private decimal _discount;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal DiscountVat
        {
            get { return _DiscountVat; }
            set
            {
                if (_DiscountVat != value)
                {
                    Calculate_UnitDiscount(value);

                    _DiscountVat = value;
                    RaisePropertyChanged("DiscountVat");

                    Calculate_SubTotalVatDiscount(_DiscountVat);
                }
            }
        }
        private decimal _DiscountVat;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal Discount_SubTotal
        {
            get { return _Discount_SubTotal; }
            set
            {
                if (_Discount_SubTotal != value)
                {
                    //  Calculate_UnitDiscount(_Discount_SubTotal);

                    _Discount_SubTotal = value;
                    RaisePropertyChanged("Discount_SubTotal");
                }
            }
        }
        private decimal _Discount_SubTotal;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal Discount_SubTotal_Vat
        {
            get { return _Discount_SubTotal_Vat; }
            set
            {
                if (_Discount_SubTotal_Vat != value)
                {
                    _Discount_SubTotal_Vat = value;
                    RaisePropertyChanged("Discount_SubTotal_Vat");
                }
            }
        }
        private decimal _Discount_SubTotal_Vat;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldDiscount"></param>
        /// <param name="value"></param>
        /// <param name="unit_cost"></param>
        public void ApplyDiscount_UnitPrice(decimal oldDiscount, decimal value, decimal unit_cost)
        {
            this.unit_cost = Discount.Calculate_Discount(oldDiscount, value, unit_cost);
            RaisePropertyChanged("unit_cost");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discountvat"></param>
        public void Calculate_UnitDiscount(decimal discountvat)
        {
            decimal calc_discount = Vat.return_ValueWithoutVAT((int)id_vat_group, discountvat); ;

            ApplyDiscount_UnitPrice(_discount, calc_discount, unit_cost);
            _discount = calc_discount;
            Calculate_SubTotalDiscount(_discount);
            RaisePropertyChanged("discount");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discount"></param>
        public void Calculate_UnitVatDiscount(decimal discount)
        {
            _DiscountVat = Vat.return_ValueWithVAT((int)id_vat_group, discount);
            Calculate_SubTotalVatDiscount(_DiscountVat);
            RaisePropertyChanged("DiscountVat");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discount"></param>
        public void Calculate_SubTotalDiscount(decimal discount)
        {
            _Discount_SubTotal = _discount * _quantity;
            RaisePropertyChanged("Discount_SubTotal");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DiscountVat"></param>
        public void Calculate_SubTotalVatDiscount(decimal DiscountVat)
        {
            _Discount_SubTotal_Vat = _DiscountVat * _quantity;
            RaisePropertyChanged("Discount_SubTotal_Vat");
        }

        #endregion

        #region "Foreign Key"
        public virtual app_vat_group app_vat_group { get; set; }
        public virtual app_cost_center app_cost_center { get; set; }
        public virtual app_location app_location { get; set; }
        public virtual item item { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void update_UnitCost(int id_currecyfx)
        {
            if (State != System.Data.Entity.EntityState.Unchanged)
            {
                if (item != null && item.unit_cost != null)
                {
                    _unit_cost = Currency.convert_Values((decimal)item.unit_cost, CurrentSession.Get_Currency_Default_Rate().id_currencyfx, id_currecyfx, App.Modules.Purchase);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void update_UnitPrice_WithoutVAT()
        {
            _unit_cost = Vat.return_ValueWithoutVAT((int)id_vat_group, UnitCost_Vat);
            RaisePropertyChanged("unit_cost");
        }

        /// <summary>
        /// 
        /// </summary>
        private void update_UnitPriceVAT()
        {
            _UnitCost_Vat = Vat.return_ValueWithVAT((int)id_vat_group, _unit_cost);
            RaisePropertyChanged("UnitCost_Vat");
        }

        /// <summary>
        /// 
        /// </summary>
        public void update_SubTotal()
        {
            SubTotal = _unit_cost * _quantity;
        }

        /// <summary>
        /// 
        /// </summary>
        private void update_SubTotalVAT()
        {
            SubTotal_Vat = _UnitCost_Vat * _quantity;
        }

        #endregion
    }
}
