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
        public int typeref { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_end { get; set; }
        public bool result_type { get; set; }
        public decimal result_per { get; set; }
        public decimal result_value { get; set; }

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
