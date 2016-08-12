
namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using entity.Brillo;

    public partial class contact_subscription : Audit
    {
       public enum Billng_Cycles
        {
            Daily,
            Weekly,
            Monthly
        }

        public contact_subscription()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            is_active = true;
            start_date = DateTime.Now;
            end_date = DateTime.Now.AddDays(365);
            unit_price = 0;
            quantity = 1;
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_subscription { get; set; }
        public int id_contact { get; set; }
        public int id_item { get; set; }
        public int id_contract { get; set; }

        [NotMapped]
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

        [NotMapped]
        public decimal UnitPrice_Vat
        {
            get { return _UnitPrice_Vat; }
            set
            {
                if (_UnitPrice_Vat != value)
                {
                    if (_UnitPrice_Vat == 0)
                    {
                        _UnitPrice_Vat = value;
                        RaisePropertyChanged("UnitPrice_Vat");
                    }
                    else
                    {
                        _UnitPrice_Vat = value;
                        RaisePropertyChanged("UnitPrice_Vat");
                        update_UnitPrice_WithoutVAT();
                    }
                }
                update_SubTotalVAT();
            }
        }
        private decimal _UnitPrice_Vat;
        
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

                if (contact!=null)
                {
                    contact.RaisePropertyChanged("GrandTotal");
                }
              
            }
        }
        private decimal _SubTotal_Vat;

        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public Billng_Cycles bill_cycle { get; set; }
        public short bill_on { get; set; }
        public bool is_active { get; set; }

        public virtual contact contact { get; set; }
        public virtual item item { get; set; }

        [NotMapped]
        public virtual app_currency app_currency { get; set; }


        #region Methods



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
            UnitPrice_Vat = Vat.return_ValueWithVAT((int)id_vat_group, _unit_price);
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

        #endregion
    }
}
