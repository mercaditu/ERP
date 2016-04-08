namespace entity
{
    using Brillo;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public partial class purchase_return : CommercialHead, IDataErrorInfo
    {
        public purchase_return()
        {
            is_head = true;
            purchase_return_detail = new List<purchase_return_detail>();
            payment_schedual = new List<payment_schedual>();

            trans_date = DateTime.Now;

            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            if (CurrentSession.Id_Branch > 0) { id_branch = CurrentSession.Id_Branch; }
            if (CurrentSession.Id_Terminal > 0) { id_terminal = CurrentSession.Id_Terminal; }
            item_movement = new List<item_movement>();
            //Get Status.
            status = Status.Documents_General.Pending;
         
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_return { get; set; }

        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_purchase_invoice { get; set; }

        public Status.ReturnTypes return_type { get; set; }

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
                    foreach (purchase_return_detail _purchase_return_detail in purchase_return_detail)
                    {
                        _purchase_return_detail.State = System.Data.Entity.EntityState.Modified;
                        _purchase_return_detail.CurrencyFX_ID = _id_currencyfx;
                    }
                }
            }
        }
        private int _id_currencyfx;

        public int? id_journal { get; set; }

        [NotMapped]
        public new decimal GrandTotal
        {
            get
            {
                _GrandTotal = 0;
                foreach (purchase_return_detail _purchase_invoice_detail in purchase_return_detail)
                {
                    _GrandTotal += _purchase_invoice_detail.SubTotal_Vat;
                }
                return Math.Round(_GrandTotal, 2);
            }
            set
            {
                decimal OriginalValue = value - _GrandTotal;
                if (OriginalValue != 0)
                {
                    decimal DifferenceValue = OriginalValue / purchase_return_detail.Sum(x => x.quantity);
                    foreach (var item in purchase_return_detail)
                    {
                        item.UnitCost_Vat = item.UnitCost_Vat + DifferenceValue;
                        item.RaisePropertyChanged("UnitCost_Vat");
                    }

                    _GrandTotal = value;
                    RaisePropertyChanged("GrandTotal");
                }
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
                    decimal DifferenceValue = OriginalValue / purchase_return_detail.Count;
                    foreach (var item in purchase_return_detail)
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
        public ICollection<purchase_return> older { get; set; }
        public purchase_return newer { get; set; }

        public virtual ICollection<purchase_return_detail> purchase_return_detail { get; set; }
        public virtual IEnumerable<payment_schedual> payment_schedual { get; set; }
        public virtual purchase_invoice purchase_invoice { get; set; }
        public virtual ICollection<item_movement> item_movement { get; set; }
        
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
                }
                if (columnName == "id_purchase_invoice")
                {
                    if (id_contact == 0)
                        return "Invoice needs to be selected";
                }
                if (columnName == "id_currencyfx")
                {
                    if (id_currencyfx == 0)
                        return "Currency needs to be selected";
                }
                if (columnName == "return_type")
                {
                    if (return_type == 0)
                        return "Sales Return needs to be selected";
                }
                return "";
            }
        }
    }
}
