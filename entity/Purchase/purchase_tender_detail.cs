
namespace entity
{
    using entity.Brillo;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class purchase_tender_detail : Audit
    {
        public purchase_tender_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            status = Status.Documents_General.Pending;
            purchase_tender_detail_dimension = new List<purchase_tender_detail_dimension>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_tender_detail { get; set; }
        public int id_purchase_tender_contact { get; set; }
        public int id_purchase_tender_item { get; set; }

        public Status.Documents_General status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("status"); }
        }
        Status.Documents_General _status;

        public string item_description { get; set; }

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
                    if (purchase_tender_item != null)
                    {
                        _Quantity_Factored = Brillo.ConversionFactor.Factor_Quantity(purchase_tender_item.item, quantity);
                        RaisePropertyChanged("Quantity_Factored");
                    }

                }

            }
        }
        private decimal _quantity;

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
                    if (purchase_tender_item != null)
                    {
                        quantity = Brillo.ConversionFactor.Factor_Quantity_Back(purchase_tender_item.item, Quantity_Factored);
                        RaisePropertyChanged("quantity");
                    }
                }
            }
        }
        private decimal _Quantity_Factored;

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
        [NotMapped]
        public decimal UnitCost_Vat
        {
            get { return _UnitCost_Vat; }
            set
            {
                if (_UnitCost_Vat != value)
                {
                    if (_UnitCost_Vat == 0)
                    {
                        _UnitCost_Vat = value;
                        RaisePropertyChanged("UnitCost_Vat");
                    }
                    else
                    {
                        _UnitCost_Vat = value;
                        RaisePropertyChanged("UnitCost_Vat");
                        update_UnitPrice_WithoutVAT();
                    }
                }
                update_SubTotalVAT();
            }
        }
        private decimal _UnitCost_Vat;
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

        [NotMapped]
        public string DimensionString
        {
            get
            {
                string s = string.Empty;

                foreach (purchase_tender_detail_dimension dimensionList in purchase_tender_detail_dimension)
                {
                    if (dimensionList.app_dimension != null && dimensionList.app_measurement != null)
                    {
                        s = s + dimensionList.app_dimension.name + ": " + dimensionList.value + " x " + dimensionList.app_measurement.name;
                    }
                }

                return s;
            }
        }
        private string _DimensionString;

        public virtual purchase_tender_contact purchase_tender_contact { get; set; }
        public virtual purchase_tender_item purchase_tender_item
        {
            get
            {
                return _purchase_tender_item;
            }
            set
            {
                _purchase_tender_item = value;
                if (quantity > 0)
                {
                    _Quantity_Factored = Brillo.ConversionFactor.Factor_Quantity(purchase_tender_item.item, quantity);
                    RaisePropertyChanged("Quantity_Factored");
                }

            }
        }
        purchase_tender_item _purchase_tender_item;

        public virtual IEnumerable<purchase_order_detail> purchase_order_detail { get; set; }
        public virtual ICollection<purchase_tender_detail_dimension> purchase_tender_detail_dimension { get; set; }



        #region Methods



        /// <summary>
        /// 
        /// </summary>
        private void update_UnitPrice_WithoutVAT()
        {
            unit_cost = Vat.return_ValueWithoutVAT((int)id_vat_group, UnitCost_Vat);
            RaisePropertyChanged("unit_cost");
        }

        /// <summary>
        /// 
        /// </summary>
        private void update_UnitPriceVAT()
        {
            UnitCost_Vat = Vat.return_ValueWithVAT((int)id_vat_group, _unit_cost);
            RaisePropertyChanged("UnitCost_Vat");
        }

        /// <summary>
        /// 
        /// </summary>
        private void update_SubTotal()
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
