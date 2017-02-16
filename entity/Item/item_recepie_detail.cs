namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class item_recepie_detail : Audit, IDataErrorInfo
    {
        public item_recepie_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            is_active = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_recepie_detail { get; set; }

        [Required]
        public decimal id_recepie { get; set; }

        [Required]
        public decimal id_item { get; set; }

        [Required]
        public decimal quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    RaisePropertyChanged("quantity");

                    if (item != null)
                    {
                        _Quantity_Factored = Brillo.ConversionFactor.Factor_Quantity(item, Convert.ToDecimal(quantity), 1);
                        RaisePropertyChanged("_Quantity_Factored");
                    }
                }
            }
        }

        private decimal _quantity;

        [NotMapped]
        public decimal Quantity_Factored
        {
            get { return _Quantity_Factored; }
            set
            {
                if (_Quantity_Factored != value)
                {
                    _Quantity_Factored = value;
                    RaisePropertyChanged("Quantity_Factored");

                    _quantity = Brillo.ConversionFactor.Factor_Quantity_Back(item, Quantity_Factored, 1);
                    RaisePropertyChanged("value_counted");
                }
            }
        }

        private decimal _Quantity_Factored;

        [Required]
        public bool is_active { get; set; }

        [NotMapped]
        public decimal est_cost { get { return _est_cost; } set { _est_cost = value; RaisePropertyChanged("est_cost"); } }

        private decimal _est_cost;

        public virtual item_recepie item_recepie { get; set; }
        public virtual item item { get; set; }

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
                if (columnName == "value")
                {
                    if (quantity == 0)
                        return "Value needs to be filled";
                }

                return "";
            }
        }
    }
}