
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
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_tender_detail { get; set; }
        public int id_purchase_tender_contact { get; set; }
        public int id_purchase_tender_item { get; set; }
      
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
        public virtual purchase_tender_contact purchase_tender_contact { get; set; }
        public virtual purchase_tender_item purchase_tender_item { get; set; }
    
        public virtual IEnumerable<purchase_order_detail> purchase_order_detail { get; set; }


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
