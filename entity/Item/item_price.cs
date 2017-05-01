namespace entity
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public partial class item_price : Audit, IDataErrorInfo
    {
        public item_price()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;

            min_quantity = 1;

            if (item != null && item.State > 0)
            {
                id_currency = CurrentSession.Currency_Default.id_currency;
                id_price_list = CurrentSession.PriceLists.Where(x => x.is_default).FirstOrDefault().id_price_list;
            }
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_price { get; set; }

        public virtual item item { get; set; }

        [Required]
        public int id_item { get; set; }

        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_price_list { get; set; }

        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_currency { get; set; }

        public decimal value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    return_ValueWithVAT();
                }
            }
        }

        private decimal _value;

        [NotMapped]
        public decimal valuewithVAT
        {
            get { return _valuewithVAT; }
            set
            {
                if (_valuewithVAT != value)
                {
                    _valuewithVAT = value;
                    RaisePropertyChanged("valuewithVAT");
                    _value = Brillo.Vat.return_ValueWithoutVAT(item.id_vat_group, _valuewithVAT);
                    RaisePropertyChanged("value");
                }
            }
        }

        private decimal _valuewithVAT;

        public void return_ValueWithVAT(int id_vat_group)
        {
            if (id_item > 0)
            {
                _valuewithVAT = Brillo.Vat.return_ValueWithVAT(id_vat_group, _value);
            }
            else
            {
                // id_vat_group = db.items.Where(x => x.id_item == id_item).FirstOrDefault().id_vat_group;
                _valuewithVAT = Brillo.Vat.return_ValueWithVAT(item.id_vat_group, _value);
            }

            RaisePropertyChanged("valuewithVAT");
        }

        public void return_ValueWithVAT()
        {
            int id_vat_group;
            if (id_item > 0)
            {
                using (db db = new db())
                {
                    id_vat_group = db.items.Where(x => x.id_item == id_item).FirstOrDefault().id_vat_group;
                }
                _valuewithVAT = Brillo.Vat.return_ValueWithVAT(id_vat_group, _value);
            }
            else
            {
                _valuewithVAT = Brillo.Vat.return_ValueWithVAT(item.id_vat_group, _value);
            }

            RaisePropertyChanged("valuewithVAT");
        }

        public decimal min_quantity { get; set; }

        public virtual item_price_list item_price_list { get; set; }

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
                if (columnName == "id_item")
                {
                    if (item == null)
                        return "Item needs to be selected";
                }
                // apply property level validation rules
                if (columnName == "id_price_list")
                {
                    if (id_price_list == 0)
                        return "Price list needs to be selected";
                }
                if (columnName == "id_currency")
                {
                    if (id_currency == 0)
                        return "Currency needs to be selected";
                }
                return "";
            }
        }
    }
}