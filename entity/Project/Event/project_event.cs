using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace entity
{
    public partial class project_event : Audit, IDataErrorInfo
    {
        public project_event()
        {
            id_user = Properties.Settings.Default.user_ID;
            is_active = true;
            trans_date = DateTime.Now;
            id_company = Properties.Settings.Default.company_ID;
            project_event_fixed = new List<project_event_fixed>();
            project_event_variable = new List<project_event_variable>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_project_event { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_project_event_template { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_item { get; set; }
        public int? id_contact { get; set; }
        [Required]
        public string name { get { return _name; } set { _name = value; RaisePropertyChanged("name"); } }
        string _name;
        public int quantity_adult { get; set; }
        public int quantity_child { get; set; }
        public DateTime trans_date { get; set; }
        public bool is_active { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int id_currencyfx { get; set; }

        public virtual item item { get; set; }
        public virtual project_event_template project_event_template { get; set; }
        public virtual contact contact { get; set; }
       
        public virtual ICollection<project_event_fixed> project_event_fixed { get; set; }
        public virtual ICollection<project_event_variable> project_event_variable { get; set; }

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
                if (columnName == "name")
                {
                    if (string.IsNullOrEmpty(name))
                        return "Name needs to be filled";
                }

                if (columnName == "id_item")
                {
                    if (id_item == 0)
                        return "Item needs to be selected";
                }

                if (columnName == "id_project_template")
                {
                    if (id_project_event_template == 0)
                        return "Template needs to be selected";
                }

                return "";
            }
        }
    }
}