namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class item_movement : Audit
    {
        public enum Actions
        {
            ReApprove,
            Delete,
            NotProcess
        }

        public enum ActionsStatus
        {
            Green,
            Red
        }

        public item_movement()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            item_movement_value = new List<item_movement_value>();
            item_movement_dimension = new List<item_movement_dimension>();
            item_request_decision = new List<item_request_decision>();
            timestamp = DateTime.Now;
            trans_date = DateTime.Now;
            debit = 0;
            credit = 0;
            child = new List<item_movement>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_movement
        {
            get
            {
                return _id_movement;
            }
            set
            {
                _id_movement = value;
                if (parent == null)
                {
                    code = _id_movement.ToString();
                }
                else
                {
                    code = parent.code;
                }
            }
        }

        private long _id_movement;
        public int id_item_product { get; set; }
        public int? id_transfer_detail { get; set; }
        public int? id_execution_detail { get; set; }
        public int? id_purchase_invoice_detail { get; set; }
        public int? id_purchase_return_detail { get; set; }
        public int? id_sales_invoice_detail { get; set; }
        public int? id_sales_return_detail { get; set; }
        public int? id_inventory_detail { get; set; }
        public int? id_sales_packing_detail { get; set; }
        public int? id_purchase_packing_detail { get; set; }
        public int id_location { get; set; }
        public long? id_movement_value_rel { get; set; }
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

        private decimal _debit = 0;

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

        private decimal _credit = 0;

        public string comment { get; set; }
        public string code { get; set; }

        public DateTime? expire_date { get; set; }
        public DateTime trans_date { get; set; }


        [NotMapped]
        public string DimensionComment
        {
            get
            {
                if (this.item_movement_dimension.Count() > 0)
                {
                    string i = " ";
                    foreach (item_movement_dimension dimension in item_movement_dimension)
                    {
                        i += dimension.value.ToString("N2") + " x ";
                    }

                    return comment + i;
                }
                return comment;
            }
        }

        [NotMapped]
        public Actions Action { get; set; }

        [NotMapped]
        public ActionsStatus ActionStatus { get; set; }

        [NotMapped]
        public decimal avlquantity
        {
            get
            {
                if (child != null)
                {
                    return credit - (child.Count() > 0 ? child.Sum(y => y.debit) : 0);
                }
                else
                {
                    return credit;
                }
            }
            set
            {
                _avlquantity = value;
            }
        }

        private decimal _avlquantity;

        public string barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                BarCode_Number = value;
            }
        }
        private string _barcode = "";

        [NotMapped]
        public string BarCode_Number { get; set; }


        //Heirarchy For Movement
        public virtual ICollection<item_movement> child { get; set; }

        public virtual item_movement parent { get; set; }

        public virtual app_location app_location { get; set; }

        public virtual purchase_packing_detail purchase_packing_detail { get; set; }
        public virtual sales_packing_detail sales_packing_detail { get; set; }
        public virtual item_transfer_detail item_transfer_detail { get; set; }
        public virtual production_execution_detail production_execution_detail { get; set; }
        public virtual purchase_invoice_detail purchase_invoice_detail { get; set; }
        public virtual purchase_return_detail purchase_return_detail { get; set; }
        public virtual sales_invoice_detail sales_invoice_detail { get; set; }
        public virtual sales_return_detail sales_return_detail { get; set; }
        public virtual ICollection<item_movement_value> item_movement_value { get; set; }
        public virtual ICollection<item_movement_dimension> item_movement_dimension { get; set; }
        public virtual item_movement_value_rel item_movement_value_rel { get; set; }
        public virtual ICollection<item_request_decision> item_request_decision { get; set; }

        public virtual item_product item_product
        {
            get { return _item_product; }
            set
            {
                _item_product = value;
            }
        }

        private item_product _item_product;

        #region Methods

        public void Update_ChildVales(decimal BaseValue, bool SkipCurrentItemUpdate,DateTime EntryDate)
        {
            if (SkipCurrentItemUpdate == false)
            {

                //if (this.item_movement_value.Count() > 0)
                //{
                //    this.item_movement_value.FirstOrDefault().unit_value = BaseValue;
                //}


                item_movement_value_detail item_movement_value_detail = new item_movement_value_detail();
                item_movement_value_detail.unit_value = BaseValue;
                item_movement_value_detail.comment ="Update Cost";
                this.item_movement_value_rel.item_movement_value_detail.Add(item_movement_value_detail);
             


              

            }

            if (this.sales_invoice_detail != null)
            {
                sales_invoice_detail.unit_cost = Brillo.Currency.convert_Values
                    (
                    BaseValue,
                    CurrentSession.Get_Currency_Default_Rate().id_currencyfx,
                    sales_invoice_detail.sales_invoice.id_currencyfx,
                    App.Modules.Sales
                    );
            }

            foreach (item_movement this_child in child)
            {
                this_child.Update_ChildVales(item_movement_value_rel.total_value, true,DateTime.Now);
            }
        }
        public void Update_ChildBatch(string BatchCode,DateTime? ExpireDate)
        {
            foreach (item_movement item_movement in child)
            {
                item_movement.code = BatchCode;
                item_movement.expire_date = expire_date??null;
                item_movement.Update_ChildBatch(BatchCode, ExpireDate);
            }
        }

        #endregion Methods
    }
}