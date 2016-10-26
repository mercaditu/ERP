namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;

    public partial class sales_order : CommercialHead, IDataErrorInfo
    {

        public sales_order()
         {
            is_head = true;
            trans_date = DateTime.Now;
            delivery_date = DateTime.Now;
            status = Status.Documents_General.Pending;

            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            if (CurrentSession.Id_Branch > 0) { id_branch = CurrentSession.Id_Branch; }
            if (CurrentSession.Id_Terminal > 0) { id_terminal = CurrentSession.Id_Terminal; }

            sales_order_detail = new List<sales_order_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_order { get; set; }
        
        public int? id_sales_budget { get; set; }
        public int id_opportunity { get; set; }
        
        public int id_currencyfx
        {
            get
            {
                return _id_currencyfx;
            }
            set
            {
                _id_currencyfx = value;
                RaisePropertyChanged("id_currencyfx");

                if (State != System.Data.Entity.EntityState.Unchanged && State > 0)
                {
                    foreach (sales_order_detail _sales_order_detail in sales_order_detail)
                    {
                        _sales_order_detail.State = System.Data.Entity.EntityState.Modified;
                        _sales_order_detail.CurrencyFX_ID = _id_currencyfx;
                    }
                    RaisePropertyChanged("GrandTotal");
                }
            }
        }
        private int _id_currencyfx;
        
        public DateTime? delivery_date { get; set; }
        public DateTime? valid_date { get; set; }

        [NotMapped]
        public new decimal GrandTotal
        {
            get
            {
                _GrandTotal = 0;
                foreach (sales_order_detail _purchase_invoice_detail in sales_order_detail)
                {
                    _GrandTotal += _purchase_invoice_detail.SubTotal_Vat;
                }
                return Math.Round(_GrandTotal, 2);
            }
            set
            {
                _GrandTotal = value;
                RaisePropertyChanged("GrandTotal");
            }
        }
        private decimal _GrandTotal;

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

                decimal Discounted_GrandTotalValue = GrandTotal * DiscountPercentage;

                if (Discounted_GrandTotalValue != 0 && GrandTotal > 0)
                {
                    foreach (sales_order_detail detail in this.sales_order_detail.Where(x => x.quantity > 0))
                    {
                        decimal WeightedAvg = detail.SubTotal_Vat / GrandTotal;
                        detail.DiscountVat = (WeightedAvg * Discounted_GrandTotalValue) / detail.quantity;
                        detail.RaisePropertyChanged("DiscountVat");
                        RaisePropertyChanged("GrandTotal");
                    }
                }
            }
        }
        private decimal _DiscountPercentage;
        [NotMapped]
        public decimal DiscountWithoutPercentage
        {
            get { return _DiscountWithoutPercentage; }
            set
            {

                _DiscountWithoutPercentage = value;
                RaisePropertyChanged("DiscountWithoutPercentage");

                decimal DiscountValue = value;
                if (DiscountValue != 0)
                {
                    decimal PerRawDiscount = DiscountValue / sales_order_detail.Where(x => x.quantity > 0).Count();
                    foreach (var item in sales_order_detail.Where(x => x.quantity > 0))
                    {

                        item.DiscountVat = PerRawDiscount / item.quantity;
                        item.RaisePropertyChanged("DiscountVat");
                        RaisePropertyChanged("GrandTotal");
                    }
                }
                else
                {
                    foreach (var item in sales_order_detail.Where(x => x.quantity > 0))
                    {

                        item.DiscountVat = 0;
                        item.RaisePropertyChanged("DiscountVat");
                        RaisePropertyChanged("GrandTotal");
                    }
                }

            }
        }
        private decimal _DiscountWithoutPercentage;

        //TimeCapsule
        public ICollection<sales_order> older { get; set; }
        public sales_order newer { get; set; }
        
        public virtual sales_budget sales_budget { get; set; }
        public virtual crm_opportunity crm_opportunity { get; set; }

        public virtual IEnumerable<payment_schedual> payment_schedual { get; set; }
        public virtual ICollection<sales_invoice> sales_invoice { get; set; }
        public virtual ICollection<sales_order_detail> sales_order_detail { get; set; }
        public virtual IEnumerable<item_request> item_request { get; set; }
       

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();
                
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
                // apply property level validation rules
                if (columnName == "id_contact")
                {
                    //if (id_contact == 0)
                    //    return "Contact needs to be selected";
                    if (contact == null)
                        return "Contact needs to be selected";
                }
                if (columnName == "id_branch")
                {
                    if (id_branch == 0)
                        return "Branch needs to be selected";
                }
                if (columnName == "id_condition")
                {
                    if (id_condition == 0)
                        return "Condition needs to be selected";
                }
                if (columnName == "id_contract")
                {
                    if (id_contract == 0)
                        return "Contract needs to be selected";
                }
                if (columnName == "id_currencyfx")
                {
                    if (id_currencyfx == 0)
                        return "Currency needs to be selected";
                }
                return "";
            }
        }
    }
}
