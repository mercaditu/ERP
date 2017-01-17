namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel;
    using System.Text;
    using System.Linq;
    using System;

    public partial class sales_budget_detail : CommercialSalesDetail, IDataErrorInfo
    {
        public sales_budget_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            quantity = 1;
            sales_order_detail = new List<sales_order_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_budget_detail { get; set; }
        public int id_sales_budget { get; set; }
        public int? movement_id { get; set; }
        [NotMapped]
        public decimal balance
        {
            get
            {

                _balance = quantity - sales_order_detail.Sum(x => x.quantity);
                return _balance;
            }
            set
            {
                _balance = value;
            }
        }
        decimal _balance;
        public DateTime? expire_date { get; set; }
        public string batch_code { get; set; }


        #region "Foreign Key"
        public virtual sales_budget sales_budget
        {
            get { return _sales_budget; }
            set
            {
                if (value!=null)
                {
                    if (_sales_budget != value)
                    {
                        _sales_budget = value;
                        CurrencyFX_ID = value.id_currencyfx;
                    }
                }
                else
                {
                    _sales_budget = null;
                    RaisePropertyChanged("sales_budget");
                    
                }
               
            }
        }
        private sales_budget _sales_budget;
        public virtual ICollection<sales_order_detail> sales_order_detail { get; set; }
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
