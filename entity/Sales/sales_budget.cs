namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class sales_budget : CommercialHead, IDataErrorInfo
    {
        public sales_budget()
        {
            is_head = true;
            trans_date = DateTime.Now;
            delivery_date = DateTime.Now;
           
            Properties.Settings _settings = new Properties.Settings();
            id_company = _settings.company_ID;
            id_user = _settings.user_ID;
            if (_settings.branch_ID > 0) { id_branch = _settings.branch_ID; }
            if (_settings.terminal_ID > 0) { id_terminal = _settings.terminal_ID; }

            sales_budget_detail = new List<sales_budget_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_budget { get; set; }
        public int id_opportunity { get; set; }

        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
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
                    foreach (sales_budget_detail _sales_budget_detail in sales_budget_detail)
                    {
                        _sales_budget_detail.State = System.Data.Entity.EntityState.Modified;
                        _sales_budget_detail.CurrencyFX_ID = _id_currencyfx;
                    }
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
                foreach (sales_budget_detail _sales_budget_detail in sales_budget_detail)
                {
                    _GrandTotal += _sales_budget_detail.SubTotal_Vat;
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



                decimal OriginalValue = GrandTotal * DiscountPercentage;
                if (OriginalValue != 0)
                {
                    decimal DifferenceValue = OriginalValue / sales_budget_detail.Count;
                    foreach (var item in sales_budget_detail)
                    {
                        DifferenceValue = DifferenceValue / item.quantity;
                        item.discount = DifferenceValue;
                        item.RaisePropertyChanged("discount");
                    }


                }
            }
        }
        private decimal _DiscountPercentage;

        //TimeCapsule
        public ICollection<sales_budget> older { get; set; }
        public sales_budget newer { get; set; }

        public virtual crm_opportunity crm_opportunity { get; set; }
        public virtual ICollection<sales_budget_detail> sales_budget_detail { get; set; }
        public virtual ICollection<sales_order> sales_order { get; set; }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();
                
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
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
                    if (id_contact == 0)
                        return "Contact needs to be selected";
                    //if (contact == null)
                    //    return "Contact needs to be selected";
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
