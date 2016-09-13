
namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel;
    using System.Text;
    using System.Linq;

    public partial class sales_order_detail : CommercialSalesDetail, IDataErrorInfo
    {
        public sales_order_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            quantity = 1;

             sales_invoice_detail = new List<sales_invoice_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_order_detail { get; set; }
        public int id_sales_order { get; set; }
        public int? movement_id { get; set; }
        public int? id_sales_budget_detail { get; set; }
        [NotMapped]
        public decimal balance
        {
            get
            {
                _balance = quantity - sales_invoice_detail.Sum(x => x.quantity);
                return _balance;
            }
            set
            {
                _balance = value;
            }
        }
        decimal _balance;


        #region "Nav Properties"

        public virtual sales_order sales_order
        {
            get
            {
                return _sales_order;
            }
            set
            {
                if (value != null)
                {
                    if (_sales_order != value)
                    {

                        _sales_order = value;
                        CurrencyFX_ID = value.id_currencyfx;


                    }
                }
                else
                {
                    _sales_order = null;
                    RaisePropertyChanged("sales_order");

                }
            }
        }
        private sales_order _sales_order;



        public virtual sales_budget_detail sales_budget_detail { get; set; }
        public virtual IEnumerable<sales_packing_detail> sales_packing_detail { get; set; }
        public virtual ICollection<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual IEnumerable<item_request_detail> item_request_detail { get; set; }
        #endregion

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
                // apply property level validation rules
                if (columnName == "id_item")
                {
                    if (id_item == 0)
                        return "Item needs to be selected";
                }
                if (columnName == "quantity")
                {
                    if (quantity == 0)
                        return "Quantity can not be zero";
                }
                if (columnName == "unit_price")
                {
                    if (unit_price < 0)
                        return "Cannot be less than Zero";
                }
                return "";
            }
        }
        #endregion
    }
}
