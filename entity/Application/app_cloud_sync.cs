namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Net;
    using System.Text;

    public partial class app_cloud_sync: Audit, IDataErrorInfo
    {
        public app_cloud_sync()
        {
           
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
        }
       public enum SyncTypes
        {
            Complete=1,
            Transaction=2,
            Base=3,
            Vat=4,
            Contract=5,
            Item=6,
            Contact=7,
            Promostion=8



        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_cloud_sync { get; set; }

      
        [Required]
        public SyncTypes type { get; set; }

        [Required]
        public string comment { get; set; }

        [Required]
        public HttpStatusCode status { get; set; }






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
    }
}