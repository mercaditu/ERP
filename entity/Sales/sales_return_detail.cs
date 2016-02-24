
namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel;
    using System.Text;

    public partial class sales_return_detail : CommercialSalesDetail, IDataErrorInfo
    {
        public sales_return_detail()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
            quantity = 1;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_return_detail { get; set; }
        public int id_sales_return { get; set; }


        public long? id_sales_invoice_detail { get; set; }

        public bool has_return { get; set; }

        #region "Foreign Key"
        public virtual sales_return sales_return
        {
            get { return _sales_return; }
            set
            {
                if (value!=null)
                {
                    if (_sales_return != value)
                    {
                        _sales_return = value;
                        CurrencyFX_ID = value.id_currencyfx;
                    }
                }
                else
                {
                    _sales_return = null;
                    RaisePropertyChanged("sales_return ");
                }
               

            }
        }   
        private sales_return _sales_return;

        public virtual sales_invoice_detail sales_invoice_detail { get; set; }
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
                if (columnName == "id_item")
                {
                    if (id_item == 0)
                        return "Item needs to be selected";
                }
                if (columnName == "quantity")
                {
                    if (quantity == 0)
                        return "Quantity cannot be zero";
                }
                if (columnName == "unit_price")
                {
                    if (unit_price == 0)
                        return "Unit Price needs to be filled";
                }
                return "";
            }
        }
        #endregion
    }
}
