namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;

    public partial class item_price : Audit, IDataErrorInfo
    {
        //

        public item_price()
        {
            Brillo.General general = new Brillo.General();

            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            id_currency = general.Get_Currency(Properties.Settings.Default.company_ID);
            id_price_list = general.get_price_list(Properties.Settings.Default.company_ID);
            min_quantity = 1;
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
        decimal _value;

        [NotMapped]
        public decimal valuewithVAT
        {
            get { return _valuewithVAT; }
            set
            {
                if (_valuewithVAT != value)
                {
                    //_valuewithVAT = value;

                    if (_valuewithVAT == 0)
                    {
                        _valuewithVAT = value;
                        RaisePropertyChanged("valuewithVAT");
                    }
                    else
                    {
                        _valuewithVAT = value;
                        RaisePropertyChanged("valuewithVAT");
                        _value = Brillo.Vat.return_ValueWithoutVAT(item.id_vat_group, _valuewithVAT);
                        RaisePropertyChanged("value");
                    }
                }
            }
        }
        decimal _valuewithVAT;

        public void return_ValueWithVAT()
        {
              int id_vat_group;
              if (id_item>0)
              {
                  using (db db = new db())
                  {
                      id_vat_group = db.items.Where(x => x.id_item == id_item).FirstOrDefault().id_vat_group;
                  }
                  _valuewithVAT = Brillo.Vat.return_ValueWithVAT(id_vat_group, _value);
                  
              }
              else
              {
                 // id_vat_group = db.items.Where(x => x.id_item == id_item).FirstOrDefault().id_vat_group;
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
                //if (columnName == "value")
                //{
                //    if (value == 0)
                //        return "Value needs to be filled";
                //}
                return "";
            }
        }
    }
}
