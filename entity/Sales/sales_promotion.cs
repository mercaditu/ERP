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
      public  enum Type
        {
            //Discount_onGrandTotal = 1,
            //Discount_onQuantityTotal = 2,
            //Discount_onQuantityRow = 3,
            //Discount_onTag = 4,
            //Discount_onBrand = 5,
            //Discount_onItem = 6,
            BuyThis_GetThat = 7,
            //BuyThis_Discount_OnSecond = 8
        }
   
        public sales_promotion()
        {
            is_head = true;
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;   
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_promotion { get; set; }
        public Type types { get; set; }
        public string name { get; set; }
        public int reference { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_end { get; set; }
        public decimal quantity_min { get; set; }
        public decimal quantity_max { get; set; }
        public decimal quantity_step { get; set; }
        public bool is_percentage { get; set; }
        public decimal result_value { get; set; }
        public decimal result_step { get; set; }
        public int reference_bonus { get; set; }

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
