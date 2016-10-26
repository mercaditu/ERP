
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;


    public partial class sales_return : CommercialHead, IDataErrorInfo
    {
               
        public sales_return()
        {
            is_head = true;
            trans_date = DateTime.Now;

            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            if (CurrentSession.Id_Branch > 0) { id_branch = CurrentSession.Id_Branch; }
            if (CurrentSession.Id_Terminal > 0) { id_terminal = CurrentSession.Id_Terminal; }

            sales_return_detail = new List<sales_return_detail>();
            payment_schedual = new List<payment_schedual>();
           
            status = Status.Documents_General.Pending;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_return { get; set; }

        public int id_opportunity { get; set; }
        public int? id_sales_invoice { get; set; }

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
                if(_id_currencyfx != value)
                {
                    _id_currencyfx = value;
                    RaisePropertyChanged("id_currencyfx");

                    if (State != System.Data.Entity.EntityState.Unchanged && State > 0)
                    {
                        foreach (sales_return_detail _sales_return_detail in sales_return_detail)
                        {
                            _sales_return_detail.State = System.Data.Entity.EntityState.Modified;
                            _sales_return_detail.CurrencyFX_ID = _id_currencyfx;
                        }
                        RaisePropertyChanged("GrandTotal");
                    }
                }       
            }
        }
        private int _id_currencyfx;

        public bool is_accounted { get; set; }

        [NotMapped]
        public new decimal GrandTotal
        {
            get
            {
                _GrandTotal = 0;
                foreach (sales_return_detail _sales_return_detail in sales_return_detail)
                {
                    _GrandTotal += _sales_return_detail.SubTotal_Vat;
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
                if (value <= 1)
                {
                    _DiscountPercentage = value;
                    RaisePropertyChanged("DiscountPercentage");

                    decimal DiscountValue = GrandTotal * DiscountPercentage;
                    if (DiscountValue != 0)
                    {
                        decimal PerRawDiscount = DiscountValue / sales_return_detail.Where(x => x.quantity > 0).Count();
                        foreach (var item in sales_return_detail.Where(x => x.quantity > 0))
                        {

                            item.DiscountVat = PerRawDiscount / item.quantity;
                            item.RaisePropertyChanged("DiscountVat");
                            RaisePropertyChanged("GrandTotal");
                        }
                    }
                    else
                    {
                        foreach (var item in sales_return_detail.Where(x => x.quantity > 0))
                        {

                            item.DiscountVat = 0;
                            item.RaisePropertyChanged("DiscountVat");
                            RaisePropertyChanged("GrandTotal");
                        }
                    }
                }
                else
                {
                    _DiscountPercentage = value;
                    RaisePropertyChanged("DiscountPercentage");
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
                    decimal PerRawDiscount = DiscountValue / sales_return_detail.Where(x => x.quantity > 0).Count();
                    foreach (var item in sales_return_detail.Where(x => x.quantity > 0))
                    {

                        item.DiscountVat = PerRawDiscount / item.quantity;
                        item.RaisePropertyChanged("DiscountVat");
                        RaisePropertyChanged("GrandTotal");
                    }
                }
                else
                {
                    foreach (var item in sales_return_detail.Where(x => x.quantity > 0))
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
        public ICollection<sales_return> older { get; set; }
        public sales_return newer { get; set; }

        public virtual ICollection<payment_schedual> payment_schedual { get; set; }
        public virtual ICollection<sales_return_detail> sales_return_detail { get; set; }
        public virtual sales_invoice sales_invoice { get; set; }
        public virtual crm_opportunity crm_opportunity { get; set; }
        
        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
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
                    if (id_contact == 0)
                        return "Contact needs to be selected";
                }
                if (columnName == "id_currencyfx")
                {
                    if (id_currencyfx == 0)
                        return "Currency needs to be selected";
                }
                if (columnName == "id_sales_invoice")
                {
                    if (id_sales_invoice == 0)
                        return "Sales Invoice needs to be selected";
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
