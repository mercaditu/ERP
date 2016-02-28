namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_geography : Audit, IDataErrorInfo
    {
        
        public app_geography()
        {
            is_active = true;
            child = new List<app_geography>();
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_geography { get; set; }
       
        [Required]
        public string name { get; set; }
        public string code { get; set; }
        public Status.geo_types type { get; set; }
        public decimal? geo_long { get; set; }
        public decimal? geo_lat { get; set; }
        public bool is_active { get; set; }

        //Heirarchy Nav Properties
        public app_geography parent { get; set; }
        public ICollection<app_geography> child { get; set; }

        //Nav Properites
        public ICollection<app_bank> app_bank { get; set; }
        public ICollection<app_branch> app_branch { get; set; }
        public ICollection<contact> contact { get; set; }

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
                if (columnName == "name")
                {
                    if (String.IsNullOrEmpty(name))
                        return "Name needs to be filled";
                }
                return "";
            }
        }
    }
}
