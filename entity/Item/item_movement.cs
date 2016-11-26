namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class item_movement : Audit
    {
        public item_movement()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            item_movement_value = new List<item_movement_value>();
            item_movement_dimension = new List<item_movement_dimension>();
            timestamp = DateTime.Now;
            trans_date = DateTime.Now;
            debit = 0;
            credit = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_movement { get; set; }
        public int id_item_product { get; set; }
        public int? id_transfer_detail { get; set; }
        public int? id_execution_detail { get; set; }
        public int? id_purchase_invoice_detail { get; set; }
        public int? id_purchase_return_detail { get; set; }
        public int? id_sales_invoice_detail { get; set; }
        public int? id_sales_return_detail { get; set; }
        public int? id_inventory_detail { get; set; }
        public int? id_sales_packing_detail { get; set; }
        public int id_location { get; set; }
        public Status.Stock status { get; set; }
        [Required]
     
        public decimal debit
        {
            get
            {
                return _debit;
            }
            set
            {
                _debit = value;
                RaisePropertyChanged("debit");
            }
        }
        decimal _debit = 0;
        [Required]
        public decimal credit
        {
            get
            {
                return _credit;
            }
            set
            {
                _credit = value;
                RaisePropertyChanged("credit");
            }
        }
        decimal _credit = 0;

        public string comment { get; set; }
        public string code { get; set; }
        public DateTime? expire_date { get; set; }
        public DateTime trans_date { get; set; }

        [NotMapped]
        public decimal avlquantity
        {
            get
            {
                if (child!=null)
                {
                    _avlquantity = credit - (child.Count() > 0 ? child.Sum(y => y.debit) : 0);
                }
                else
                {
                    _avlquantity = credit;
                }

               
                return _avlquantity;
            }
            set
            {
                _avlquantity = value;
            }
        }
        private decimal _avlquantity;

        //Heirarchy For Movement
        public virtual ICollection<item_movement> child { get; set; }
        public virtual item_movement parent { get; set; }

        public virtual app_location app_location { get; set; }

        public virtual sales_packing_detail sales_packing_detail { get; set; }
        public virtual item_transfer_detail item_transfer_detail { get; set; }
        public virtual production_execution_detail production_execution_detail { get; set; }
        public virtual purchase_invoice_detail purchase_invoice_detail { get; set; }
        public virtual purchase_return_detail purchase_return_detail { get; set; }
        public virtual sales_invoice_detail sales_invoice_detail { get; set; }
        public virtual sales_return_detail sales_return_detail { get; set; }
        public virtual ICollection<item_movement_value> item_movement_value { get; set; }
        public virtual ICollection<item_movement_dimension> item_movement_dimension { get; set; }
        public virtual item_product item_product
        {
            get { return _item_product; }
            set
            {
                _item_product = value;
            }
        }
        
        item_product _item_product;

        #region Methods

        public bool Find_NewParent()
        {
            try
            {
                // fifo query
                // check availability
                // assign to new parent (break up if necesary)

                return true;
            }
            catch
            {
                return false;   
            }
        } 

        #endregion
    }
}
