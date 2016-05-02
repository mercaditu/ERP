namespace entity
{
    using Brillo;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    public partial class CommercialSalesDetail : Audit
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
        public int? id_project_task { get; set; }

        /// <summary>
        /// Item ID related to this Detail.
        /// </summary>
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_item
        {
            get { return _id_item; }
            set
            {
                if (value > 0 && value != _id_item)
                {
                    _id_item = value;
                    RaisePropertyChanged("id_item");

                    //if (item == null)
                    //{
                    using (db db = new db())
                    {
                         item _item = db.items.Where(x => x.id_item == _id_item).FirstOrDefault();

                        id_vat_group = Vat.getItemVat(_item);
                        RaisePropertyChanged("id_vat_group");
                        item_description = _item.name;
                        RaisePropertyChanged("item_description");
                    }

                    update_UnitPrice();
                    update_UnitCost();
                }
            }
        }
        private int _id_item;

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

                    update_SubTotal();
                }
            }
        }
        private decimal _quantity;

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

                        //update_SubTotal();
                    }
                }
            }
        }
        private int _id_vat_group;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public int PriceList_ID { get; set; }

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
                        unit_price = Currency.convert_Values(unit_price,_CurrencyFX_ID, value, App.Modules.Sales);
                        RaisePropertyChanged("unit_price");
                    }
                    _CurrencyFX_ID = value;
                }

            }
        }
        private int _CurrencyFX_ID;


        [NotMapped]
        public contact Contact { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public decimal unit_price
        {
            get { return _unit_price; }
            set
            {
                if (_unit_price != value)
                {
                    _unit_price = value;
                    RaisePropertyChanged("unit_price");

                    update_UnitPriceVAT();
                    update_SubTotal();


                }
            }
        }
        private decimal _unit_price;

        /// <summary>
        /// 
        /// </summary>
        public decimal unit_cost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal UnitPrice_Vat
        {
            get { return _UnitPrice_Vat; }
            set
            {
                if (_UnitPrice_Vat != value)
                {
                    if (value == 0)
                    {
                        _UnitPrice_Vat = value;
                        RaisePropertyChanged("UnitPrice_Vat");
                    }
                    else
                    {
                        _UnitPrice_Vat = value;
                        RaisePropertyChanged("UnitPrice_Vat");
                        update_UnitPrice_WithoutVAT();
                        //update_SubTotal();
                    }
                }
            }
        }
        private decimal _UnitPrice_Vat;


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


        #region "Foreign Key"

        public virtual app_vat_group app_vat_group { get; set; }
        public virtual app_location app_location { get; set; }
        public virtual project_task project_task { get; set; }
        public virtual item item { get; set; }

        #endregion


        #region Methods

        public void Item_Select()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void update_UnitCost()
        {
            if (State != System.Data.Entity.EntityState.Unchanged)
            {
                if (item != null && item.unit_cost != null)
                {
                    unit_cost = (decimal)item.unit_cost;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void update_UnitPrice()
        {
            if (item != null)
            {
                unit_price = get_SalesPrice();
                RaisePropertyChanged("unit_price");
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void update_UnitPrice_WithoutVAT()
        {
            unit_price =Vat.return_ValueWithoutVAT((int)id_vat_group, UnitPrice_Vat);
            RaisePropertyChanged("unit_price");
        }

        /// <summary>
        /// 
        /// </summary>
        private void update_UnitPriceVAT()
        {
            UnitPrice_Vat = Vat.return_ValueWithVAT((int)id_vat_group, unit_price);
            RaisePropertyChanged("UnitPrice_Vat");
        }

        /// <summary>
        /// 
        /// </summary>
        private void update_SubTotal()
        {
            SubTotal = _unit_price * _quantity;
        }

        /// <summary>
        /// 
        /// </summary>
        private void update_SubTotalVAT()
        {
            SubTotal_Vat = _UnitPrice_Vat * _quantity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public decimal get_SalesPrice()
        {

            if (id_item > 0)
            {
                if (Contact != null)
                {
                    if (Contact.id_price_list != null)
                    {
                        PriceList_ID = (int)Contact.id_price_list;
                    }
                    else
                    {
                        PriceList_ID = 0;
                    }

                }

                //Step 1. If 'PriceList_ID' is 0, Get Default PriceList.
                if (PriceList_ID == 0 && PriceList_ID != null)
                {
                    using (db db = new db())
                    {
                        if (db.item_price_list.Where(x => x.is_active == true && x.id_company == Properties.Settings.Default.company_ID) != null)
                        {
                            PriceList_ID = db.item_price_list.Where(x => x.is_active == true && x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_price_list;
                        }
                    }
                }

                //Step 1 1/2. Check if Quantity gets us a better Price List.


                //Step 2. Get Price in Currency.
                using (db db = new db())
                {
                    app_currencyfx app_currencyfx = null;
                    if (db.app_currencyfx.Where(x => x.id_currencyfx == CurrencyFX_ID).FirstOrDefault() != null)
                    {
                        app_currencyfx = db.app_currencyfx.Where(x => x.id_currencyfx == CurrencyFX_ID).FirstOrDefault();

                        //Check if we have available Price for this Product, Currency, and List.
                        item_price item_price = db.item_price.Where(x => x.id_item == id_item
                                                                 && x.id_currency == app_currencyfx.id_currency
                                                                 && x.id_price_list == PriceList_ID)
                                                                 .FirstOrDefault();

                        if (item_price != null)
                        {   //Return Perfect Value
                            return item_price.value;
                        }
                        else
                        {
                            //If Perfect Value not found, get one pased on Product and List. (Ignore Currency and Convert Later basd on Current Rate.)
                            if (db.item_price.Where(x => x.id_item == id_item && x.id_price_list == PriceList_ID).FirstOrDefault() != null)
                            {
                                item_price = db.item_price.Where(x => x.id_item == id_item && x.id_price_list == PriceList_ID).FirstOrDefault();
                                app_currencyfx = db.app_currencyfx.Where(x => x.id_currency == item_price.id_currency && x.is_active == true).FirstOrDefault();
                                return Currency.convert_Values(item_price.value, app_currencyfx.id_currencyfx,CurrencyFX_ID, App.Modules.Sales);
                            }
                            else if (db.item_price.Where(x => x.id_item == id_item && x.id_currency == app_currencyfx.id_currency).FirstOrDefault() != null)
                            {
                                item_price = db.item_price.Where(x => x.id_item == id_item && x.id_currency == app_currencyfx.id_currency).FirstOrDefault();
                                app_currencyfx = db.app_currencyfx.Where(x => x.id_currency == item_price.id_currency && x.is_active == true).FirstOrDefault();
                                return Currency.convert_Values(item_price.value, app_currencyfx.id_currencyfx, CurrencyFX_ID, App.Modules.Sales);
                            }
                            else
                            {
                                item_price = db.item_price.Where(x => x.id_item == id_item).FirstOrDefault();
                                app_currencyfx = db.app_currencyfx.Where(x => x.id_currency == item_price.id_currency && x.is_active == true).FirstOrDefault();
                                return Currency.convert_Values(item_price.value, app_currencyfx.id_currencyfx, CurrencyFX_ID, App.Modules.Sales);
                            }


                        }
                    }

                }
            }

            return 0;
        }

        #endregion
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
                        ApplyDiscount_UnitPrice(_discount, value, unit_price);

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
                    // Calculate_UnitVatDiscount(value);

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
            this.unit_price = Discount.Calculate_Discount(oldDiscount, value, unit_cost);
            RaisePropertyChanged("unit_price");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discountvat"></param>
        public void Calculate_UnitDiscount(decimal discountvat)
        {
            decimal calc_discount = Vat.return_ValueWithoutVAT((int)id_vat_group, discountvat); ;

            ApplyDiscount_UnitPrice(_discount, calc_discount, unit_price);
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
    }
}
