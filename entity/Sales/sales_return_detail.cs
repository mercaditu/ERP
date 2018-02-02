namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class sales_return_detail : CommercialSalesDetail, IDataErrorInfo
    {
        public sales_return_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            quantity = 1;
            item_movement = new List<item_movement>();
            item_mov_archive = new List<item_mov_archive>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_return_detail { get; set; }

        public int id_sales_return { get; set; }
        public bool has_return { get; set; }

        public DateTime? expire_date { get; set; }
        public string batch_code { get; set; }

        [NotMapped]
        public decimal Balance
        {
            get
            {
                if (sales_invoice_detail != null)
                {
                    return sales_invoice_detail.Balance;
                }

                return _Balance;
            }
            set { _Balance = value; }
        }

        private decimal _Balance;

        #region "Foreign Key"

        public virtual sales_return sales_return
        {
            get { return _sales_return; }
            set
            {
                if (value != null)
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
        public virtual ICollection<item_movement> item_movement { get; set; }
        public virtual ICollection<item_mov_archive> item_mov_archive { get; set; }

        #endregion "Foreign Key"

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

        public int? movement_id { get; set; }

        public string this[string columnName]
        {
            get
            {
                // apply property level validation rules
                if (columnName == "id_item")
                {
                    if (id_item == 0)
                        return Brillo.Localize.PleaseSelect;
                }
                if (columnName == "quantity")
                {
					if (quantity == 0)
					{
						return "Quantity cannot be zero";
					}
					else if (Quantity_InStockLot != null)
					{
						if (Quantity_InStockLot < quantity)
						{
							return "Stock Exceeded";
						}
					}

					if (sales_invoice_detail != null)
                    {
                        if (Balance < quantity)
                        {
                            return "Sales Balance = " + Balance + ". You cannot Return a greater amount";
                        }
                    }
                }
                if (columnName == "unit_price")
                {
                    if (unit_price == 0)
                        return "Unit Price needs to be filled";
                }
                return "";
            }
        }

        #endregion "Validation"
    }
}