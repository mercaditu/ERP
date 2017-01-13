namespace entity
{
    using entity.Class;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public partial class schedual_detail : AuditGeneric, IDataErrorInfo, INotifyPropertyChanged
    {




        public schedual_detail()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_schedual_detail { get; set; }
        public DateTime trans_date { get; set; }
        public int? id_payment_deatil { get; set; }
        public int? id_sales_deatil { get; set; }
        public int? id_sales_packing_deatil { get; set; }
        public int? id_geography { get; set; }
       



        #region Validation

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
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
             
                return "";
            }
        }

        #endregion

    }
}
