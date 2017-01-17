
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;

    public partial class purchase_order_detail : CommercialPurchaseDetail, IDataErrorInfo
    {
        public purchase_order_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            purchase_order_dimension = new List<purchase_order_dimension>();
            purchase_invoice_detail = new List<purchase_invoice_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_order_detail { get; set; }
        public int id_purchase_order { get; set; }
        public int? id_purchase_tender_detail { get; set; }

        public DateTime expire_date { get; set; }
        public string batch_code { get; set; }

        #region "Navigation Properties"

        public virtual purchase_order purchase_order
        {
            get { return _purchase_order; }
            set
            {
                if (value != null)
                {
                    if (_purchase_order != value)
                    {
                        _purchase_order = value;
                        CurrencyFX_ID = value.id_currencyfx;
                    }
                }
                else
                {
                    _purchase_order = null;
                    RaisePropertyChanged("sales_invoice");
                }
            }
        }
        purchase_order _purchase_order;
        public virtual purchase_tender_detail purchase_tender_detail { get; set; }

        public virtual IEnumerable<purchase_packing_detail> purchase_packing_detail { get; set; }
        public virtual ICollection<purchase_invoice_detail> purchase_invoice_detail { get; set; }
        public virtual ICollection<purchase_order_dimension> purchase_order_dimension { get; set; }
        public virtual ICollection<production_service_account> production_service_account { get; set; }
        public virtual ICollection<production_account> production_account { get; set; }

        #endregion

        public decimal GetDimensionValue()
        {
            decimal Dimension = 1M;
            if (purchase_order_dimension != null)
            {
                foreach (purchase_order_dimension _purchase_order_dimension in purchase_order_dimension)
                {
                    Dimension = Dimension * _purchase_order_dimension.value;
                }
            }

            return Dimension;
        }
        [NotMapped]
        public decimal balance
        {
            get
            {
                if (purchase_invoice_detail!=null)
                {
                    _balance = quantity - purchase_invoice_detail.Sum(x => x.quantity);
                    return _balance;
                }
                return 0;
            }
            set
            {
                _balance = value;
            }
        }
        decimal _balance;
        #region "Validation"
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
                if (columnName == "quantity")
                {
                    if (quantity == 0)
                        return "Quantity can not be zero";
                }
                if (columnName == "id_cost_center")
                {
                    if (id_cost_center == 0)
                        return "Cost Center can not be zero";
                }
                if (columnName == "unit_cost")
                {
                    if (unit_cost < 0)
                        return "Cannot be less than Zero";
                }
                return "";
            }
        }
        #endregion
    }
}
