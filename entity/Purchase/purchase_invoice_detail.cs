namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;
    using System;

    public partial class purchase_invoice_detail : CommercialPurchaseDetail, IDataErrorInfo
    {
        public purchase_invoice_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            quantity = 1;
            purchase_invoice_dimension = new List<purchase_invoice_dimension>();
            item_movement = new List<item_movement>();
            timestamp = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_invoice_detail { get; set; }

        public int id_purchase_invoice { get; set; }
        public int? id_purchase_order_detail { get; set; }

        [NotMapped]
        public decimal avlquantity
        {
            get
            {
                if (item_movement.Count>0)
                {
                    return item_movement.Sum(x=>x.avlquantity);
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                _avlquantity = value;
            }
        }

        private decimal _avlquantity;

        #region "Navigation Properties"

        public virtual purchase_order_detail purchase_order_detail { get; set; }

        public virtual purchase_packing_detail purchase_packing_detail { get; set; }

        public virtual purchase_invoice purchase_invoice
        {
            get { return _purchase_invoice; }
            set
            {
                if (value != null)
                {
                    if (_purchase_invoice != value)
                    {
                        _purchase_invoice = value;
                        bool a = _purchase_invoice.displayexpire;
                        CurrencyFX_ID = value.id_currencyfx;
                    }
                }
                else
                {
                    _purchase_invoice = null;
                    RaisePropertyChanged("purchase_invoice");
                }
            }
        }

        private purchase_invoice _purchase_invoice;

        public virtual IEnumerable<purchase_return_detail> purchase_return_detail { get; set; }

        public virtual ICollection<purchase_invoice_dimension> purchase_invoice_dimension
        {
            get
            {
                return _purchase_invoice_dimension;
            }
            set
            {
                _purchase_invoice_dimension = value;
            }
        }

        private ICollection<purchase_invoice_dimension> _purchase_invoice_dimension;
        public virtual ICollection<item_movement> item_movement { get; set; }
        public virtual ICollection<production_service_account> production_service_account { get; set; }
        public virtual ICollection<production_account> production_account { get; set; }
        public virtual project_task project_task { get; set; }

        #endregion "Navigation Properties"

        #region "Validation"

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
                if (columnName == "quantity")
                {
                    if (quantity == 0)
                        return "Quantity can not be zero";
                }
                if (columnName == "id_vat_group")
                {
                    if (id_vat_group == 0)
                        return "Vat Group can not be zero";
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

        #endregion "Validation"

        public decimal GetDimensionValue()
        {
            decimal Dimension = 1M;
            if (purchase_invoice_dimension != null)
            {
                foreach (purchase_invoice_dimension _purchase_invoice_dimension in purchase_invoice_dimension)
                {
                    Dimension = Dimension * _purchase_invoice_dimension.value;
                }
            }

            return Dimension;
        }
    }
}