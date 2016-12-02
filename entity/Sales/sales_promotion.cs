namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;

    public partial class sales_promotion : Audit, IDataErrorInfo
    {
        public enum Types
        {
            //Discount_onGrandTotal = 1,
            //Discount_onQuantityTotal = 2,
            //Discount_onQuantityRow = 3,
            Discount_onTag = 4,
            //Discount_onBrand = 5,
            Discount_onItem = 6,
            BuyThis_GetThat = 7,
            BuyTag_GetThat = 8
            //BuyThis_Discount_OnSecond = 9
        }

        public sales_promotion()
        {
            is_head = true;
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            date_start = DateTime.Now;
            date_end = DateTime.Now.AddDays(10);
            quantity_step = 1;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_promotion { get; set; }
        public Types type { get; set; }
        public string name { get; set; }
        public int reference
        {
            get { return _reference; }
            set
            {

                if (_reference!=value)
                {
                    using (db db = new db())
                    {
                        item _item = db.items.Where(x => x.id_item == value).FirstOrDefault();
                        if (_item != null)
                        {
                            itemRef = _item.name;
                            RaisePropertyChanged("itemref");
                        }
                    }
                }
                _reference = value;


            }
        }
        int _reference;
        [NotMapped]
        public string itemRef { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_end { get; set; }
        public decimal quantity_min { get; set; }
        public decimal quantity_max { get; set; }
        public decimal quantity_step { get; set; }
        public bool is_percentage { get; set; }
        public decimal result_value { get; set; }
        public decimal result_step { get; set; }
        public int reference_bonus {
            get { return _reference_bonus; }
            set
            {

                if (_reference_bonus != value)
                {
                    using (db db = new db())
                    {
                        item _item = db.items.Where(x => x.id_item == value).FirstOrDefault();
                        if (_item != null)
                        {
                            itemRefbonus = _item.name;
                            RaisePropertyChanged("itemRefbonus");
                        }
                    }
                }
                _reference_bonus = value;


            }
        }
        int _reference_bonus;
        [NotMapped]
        public string itemRefbonus { get; set; }
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

                return "";
            }
        }
    }
}
