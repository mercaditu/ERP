namespace entity
{
    using Brillo;
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

                    if (State != System.Data.Entity.EntityState.Unchanged && State > 0)
                    {
                        using (db db = new db())
                        {
                            //  db.Configuration.AutoDetectChangesEnabled = false;
                            item _item = db.items.Find(_id_item);

                            id_vat_group = Vat.getItemVat(_item);
                            RaisePropertyChanged("id_vat_group");
                            item_description = _item.name;
                            RaisePropertyChanged("item_description");
                        }

                        update_UnitPrice();
                    }
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
					update_UnitCostSubTotal();
                    using (db db = new db())
                    {
                      
                        //  db.Configuration.AutoDetectChangesEnabled = false;
                        item _item = db.items.Find(_id_item);
                        _Quantity_Factored = Brillo.ConversionFactor.Factor_Quantity(_item, quantity, 0);
                        RaisePropertyChanged("_Quantity_Factored");
                    }
                    if (Quantity_InStock != null)
                    {
                        if (quantity > Quantity_InStock)
                        {
                            InStock = false;
                            RaisePropertyChanged("InStock");
                        }
                        else
                        { InStock = true; RaisePropertyChanged("InStock"); }
                    }
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

        [NotMapped]
        public decimal? Quantity_InStock { get; set; }

        [NotMapped]
        public decimal? Quantity_InStockLot { get; set; }

        [NotMapped]
        public bool InStock { get; set; }



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
                        unit_price = Currency.convert_Values(unit_price, _CurrencyFX_ID, value, App.Modules.Sales);
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
                        update_SubTotal();
                    }
                }
            }
        }

        private decimal _UnitPrice_Vat;

		/// <summary>
		///
		/// </summary>
		public decimal unit_cost
		{
			get { return _unit_cost; }
			set
			{
				if (_unit_cost != value)
				{
					_unit_cost = value;
					RaisePropertyChanged("unit_cost");

					update_UnitCostVAT();
					update_SubTotal();
				}
			}
		}
		private decimal _unit_cost;

		[NotMapped]
		public decimal UnitCost_Vat
		{
			get { return _UnitCost_Vat; }
			set
			{
				if (UnitCost_Vat != value)
				{
					if (value == 0)
					{
						_UnitCost_Vat = value;
						RaisePropertyChanged("UnitCost_Vat");
					}
					else
					{
						_UnitCost_Vat = value;
						RaisePropertyChanged("UnitCost_Vat");
						update_UnitCost_WithoutVAT();
						update_UnitCostSubTotal();
					}
				}
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
                RaisePropertyChanged("Total_Vat");
            }
        }
		private decimal _SubTotal_Vat;

		/// <summary>
		///
		/// </summary>
		[NotMapped]
		public decimal SubTotalUnitCost
		{
			get { return _SubTotalUnitCost; }
			set
			{
				_SubTotalUnitCost = value;
				RaisePropertyChanged("SubTotal");
				update_UnitCostSubTotalVAT();
			}
		}

		private decimal _SubTotalUnitCost;

		/// <summary>
		///
		/// </summary>
		[NotMapped]
		public decimal SubTotalUnitCost_Vat
		{
			get { return _SubTotalUnitCost_Vat; }
			set
			{
				_SubTotalUnitCost_Vat = value;
				RaisePropertyChanged("SubTotalUnitCost_Vat");
			
			}
		}

		private decimal _SubTotalUnitCost_Vat;

        [NotMapped]
        public decimal Total_Vat
        {
            get { return SubTotal_Vat - SubTotal; }
        }

        public int? id_sales_promotion { get; set; }

        [NotMapped]
        public string DecimalStringFormat { get; set; }

        #region "Foreign Key"

        public virtual app_vat_group app_vat_group { get; set; }
        public virtual app_location app_location { get; set; }
        public virtual project_task project_task { get; set; }

        public virtual item item
        {
            get { return _item; }
            set
            {
                if (_item != value)
                {
                    _item = value;

                    if (_item != null && string.IsNullOrEmpty(item_description))
                    {
                        item_description = _item.name;
                        RaisePropertyChanged("item_description");
                    }
                }
            }
        }

        private item _item;

        public virtual sales_promotion sales_promotion { get; set; }

        #endregion "Foreign Key"

        #region Methods

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
            unit_price = Vat.return_ValueWithoutVAT((int)id_vat_group, UnitPrice_Vat);
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
            RaisePropertyChanged("SubTotal");
        }

        /// <summary>
        ///
        /// </summary>
        private void update_SubTotalVAT()
        {
            SubTotal_Vat = _UnitPrice_Vat * _quantity;
            RaisePropertyChanged("SubTotal_Vat");
        }



		/// <summary>
		///
		/// </summary>
		private void update_UnitCost_WithoutVAT()
		{
			unit_cost = Vat.return_ValueWithoutVAT((int)id_vat_group, UnitCost_Vat);
			RaisePropertyChanged("unit_cost");
		}

		/// <summary>
		///
		/// </summary>
		private void update_UnitCostVAT()
		{
			UnitCost_Vat = Vat.return_ValueWithVAT((int)id_vat_group, unit_cost);
			RaisePropertyChanged("UnitCost_Vat");
		}

		/// <summary>
		///
		/// </summary>
		private void update_UnitCostSubTotal()
		{
			SubTotalUnitCost = _unit_cost * _quantity;
			RaisePropertyChanged("SubTotalUnitCost");
		}

		/// <summary>
		///
		/// </summary>
		private void update_UnitCostSubTotalVAT()
		{
			SubTotalUnitCost_Vat = _UnitCost_Vat * _quantity;
			RaisePropertyChanged("SubTotalUnitCost_Vat");
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		private decimal get_SalesPrice()
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
                if (PriceList_ID == 0)
                {
                    PriceList_ID = CurrentSession.PriceLists.Where(x => x.is_active == true && x.is_default == true && x.id_company == CurrentSession.Id_Company).Select(x => x.id_price_list).FirstOrDefault();
                }

                //Step 2. Get Price in Currency.
                if (CurrencyFX_ID > 0)
                {
                    using (db db = new db())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.Configuration.LazyLoadingEnabled = false;

                        app_currencyfx app_currencyfx = db.app_currencyfx.Find(CurrencyFX_ID);

                        //Check if we have available Price for this Product, Currency, and List.
                        item_price item_price = db.item_price.Where(x => x.id_item == id_item
                                                                 && x.id_currency == app_currencyfx.id_currency
                                                                 && x.id_price_list == PriceList_ID).FirstOrDefault();

                        if (item_price != null)
                        {   //Return Perfect Value
                            return item_price.value;
                        }
                        else
                        {
                            //If Perfect Value not found, get one pased on Product and List. (Ignore Currency and Convert Later basd on Current Rate.)
                            List<item_price> ItemPrices = db.item_price.Where(x => x.id_item == id_item).ToList();

                            if (ItemPrices.Where(x => x.id_price_list == PriceList_ID).Any())
                            {
                                item_price = ItemPrices.Where(x => x.id_price_list == PriceList_ID).FirstOrDefault();
                                int FxRate = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == item_price.id_currency).Select(x => x.id_currencyfx).FirstOrDefault();
                                return Currency.convert_Values(item_price.value, FxRate, CurrencyFX_ID, App.Modules.Sales);
                            }
                            else if (ItemPrices.Where(x => x.id_currency == app_currencyfx.id_currency).Any())
                            {
                                item_price = ItemPrices.Where(x => x.id_currency == app_currencyfx.id_currency).FirstOrDefault();
                                int FxRate = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == item_price.id_currency).Select(x => x.id_currencyfx).FirstOrDefault();
                                return Currency.convert_Values(item_price.value, FxRate, CurrencyFX_ID, App.Modules.Sales);
                            }
                            else if (ItemPrices.FirstOrDefault() != null)
                            {
                                item_price = ItemPrices.FirstOrDefault();
                                int FxRate = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == item_price.id_currency).Select(x => x.id_currencyfx).FirstOrDefault();
                                return Currency.convert_Values(item_price.value, FxRate, CurrencyFX_ID, App.Modules.Sales);
                            }
                        }
                    }
                }
            }

            return 0;
        }

        #region Discount Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="oldDiscount"></param>
        /// <param name="value"></param>
        /// <param name="unit_cost"></param>
        private void ApplyDiscount_UnitPrice(decimal oldDiscount, decimal value, decimal unit_cost)
        {
            if (this.unit_price > 0)
            {
                this.unit_price = Discount.Calculate_Discount(oldDiscount, value, unit_cost);
                RaisePropertyChanged("unit_price");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="discountvat"></param>
        private void Calculate_UnitDiscount(decimal discountvat)
        {
            decimal calc_discount = Vat.return_ValueWithoutVAT((int)id_vat_group, discountvat);

            ApplyDiscount_UnitPrice(_discount, calc_discount, unit_price);
            _discount = calc_discount;
            Calculate_SubTotalDiscount(_discount);
            RaisePropertyChanged("discount");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="discount"></param>
        private void Calculate_UnitVatDiscount(decimal discount)
        {
            _DiscountVat = Vat.return_ValueWithVAT((int)id_vat_group, discount);
            Calculate_SubTotalVatDiscount(_DiscountVat);
            RaisePropertyChanged("DiscountVat");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="discount"></param>
        private void Calculate_SubTotalDiscount(decimal discount)
        {
            Discount_SubTotal = discount * _quantity;
            RaisePropertyChanged("Discount_SubTotal");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="DiscountVat"></param>
        private void Calculate_SubTotalVatDiscount(decimal DiscountVat)
        {
            _Discount_SubTotal_Vat = DiscountVat * _quantity;
            RaisePropertyChanged("Discount_SubTotal_Vat");
        }

        private void Calculate_PercentageDiscount(decimal Percentage)
        {
            discount = unit_price * Percentage;
            RaisePropertyChanged("discount");
        }

        #endregion Discount Methods

        #endregion Methods

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
                    if (State == System.Data.Entity.EntityState.Added || State == System.Data.Entity.EntityState.Modified)
                    {
                        ApplyDiscount_UnitPrice(_discount, value, unit_price);
                    }

                    Calculate_UnitVatDiscount(value);
                    Calculate_SubTotalDiscount(value);

                    _discount = value;
                    RaisePropertyChanged("discount");
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
                    if (State > 0)
                    {
                        Calculate_UnitDiscount(value);
                        Calculate_SubTotalVatDiscount(value);
                    }
                    _DiscountVat = value;
                    RaisePropertyChanged("DiscountVat");
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
        [NotMapped]
        public decimal DiscountPercentage_SubTotal_Vat
        {
            get { return _DiscountPercentage_SubTotal_Vat; }
            set
            {
                if (_DiscountPercentage_SubTotal_Vat != value)
                {
                    Calculate_PercentageDiscount(value);

                    _DiscountPercentage_SubTotal_Vat = value;
                    RaisePropertyChanged("DiscountPercentage_SubTotal_Vat");
                }
            }
        }

        private decimal _DiscountPercentage_SubTotal_Vat;

        #endregion Discount Calculations
    }
}