namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class security_user : Email, IDataErrorInfo
    {
        public security_user()
        {
            id_company = CurrentSession.Id_Company;
            is_active = true;
            trans_date = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_user { get; set; }
        public int? id_company { get; set; }

        public int? id_contact { get; set; }
        public int? id_question { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_role { get; set; }
        [Required]
        public string name 
        {
            get
            {
                return _name;
            }
            set 
            {
               _name = value;

                string domain = string.Empty;
                if (app_company != null)
                {
                    domain = app_company.domain;
                    if (domain != null)
                    {
                        email = _name + "@" + domain.Replace("www.", "").Replace("http://", "");
                        RaisePropertyChanged("email");
                    }
                    else
                    {
                        email = _name + "@";
                        RaisePropertyChanged("email");
                    }
                }
            }
        }
        private string _name = string.Empty;

        [Required]
        public string password { get; set; }

        public string name_full { get; set; }
        
        public string email { get; set; }
        public string email_username { get; set; }
        public string email_password { get; set; }

        public string code { get; set; }
        public string security_answer { get; set; }
        public bool is_active { get; set; }
        public DateTime? trans_date { get; set; }
        public int? id_created_user { get; set; }
        [NotMapped]
        public System.Data.Entity.EntityState State
        {
            get
            {
                return _State;
            }
            set
            {
                if (value != _State)
                {
                    _State = value;
                    RaisePropertyChanged("State");
                }
            }
        }
        System.Data.Entity.EntityState _State;

        public virtual security_role security_role { get; set; }
        //public virtual contact contact { get; set; }
        public virtual app_company app_company { get; set; }
        public virtual security_question security_question { get; set; }
        public virtual IEnumerable<project_task> project_task { get; set; }
        public virtual IEnumerable<project_task_dimension> project_task_dimension { get; set; }

        public virtual IEnumerable<security_user> child { get; set; }
        public virtual security_user parent { get; set; }
       // public virtual security_user Createdby { get; set; }
        public virtual IEnumerable<item_inventory> item_inventory { get; set; }

        public virtual IEnumerable<accounting_chart> accounting_chart { get; set; }
        public virtual IEnumerable<accounting_budget> accounting_budget { get; set; }
        public virtual IEnumerable<accounting_cycle> accounting_cycle { get; set; }
        public virtual IEnumerable<accounting_journal_detail> accounting_journal_detail { get; set; }

        public virtual IEnumerable<sales_budget> sales_budget { get; set; }
        public virtual IEnumerable<sales_budget_detail> sales_budget_detail { get; set; }
        public virtual IEnumerable<sales_invoice> sales_invoice { get; set; }

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
                    if (string.IsNullOrEmpty(name))
                        return "Name needs to be filled";
                }
                if (columnName == "password")
                {
                    if (string.IsNullOrEmpty(password))
                        return "Password needs to be filled";
                }
                if (columnName == "id_role")
                {
                    if (id_role == 0)
                        return "Role needs to be selected";
                }
                return "";
            }
        }
    }
}
