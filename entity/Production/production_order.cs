namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class production_order : Audit
    {
        public production_order()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            Properties.Settings _settings = new Properties.Settings();
            trans_date = DateTime.Now;
            production_order_detail = new List<production_order_detail>();
            item_request = new List<item_request>();

           
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_production_order { get; set; }

        [Required]
        public int id_production_line { get; set; }
        public int? id_weather { get; set; }

        public int? id_project { get; set; }
        public string work_number { get; set; }
        public string project_cost_center { get; set; }

        public Status.Production? status { get; set; }
        public string name { get; set; }
        public string barcode { get; set; }

        [Required]
        public DateTime trans_date { get; set; }

        public DateTime? start_date_est { get; set; }
        public DateTime? end_date_est { get; set; }
        
        public virtual production_line production_line { get; set; }
        public virtual project project { get; set; }

        public virtual ICollection<production_order_detail> production_order_detail { get; set; }
        public virtual ICollection<item_request> item_request { get; set; }
        public virtual IEnumerable<production_execution> production_execution { get; set; }

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
               
                if (columnName == "id_production_line")
                {
                    if (id_production_line == 0)
                        return "production line needs to be selected";
                }
                return "";
            }
        }

    }
}
