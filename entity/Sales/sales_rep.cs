namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class sales_rep : Audit, IDataErrorInfo
    {
        public enum SalesRepType
        {
            Salesman = 1,
            Collector = 2,
            PurchaseAgent = 3
        }

        public sales_rep()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            sales_order = new List<sales_order>();
            sales_invoice = new List<sales_invoice>();
            contacts = new List<contact>();
            is_active = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_rep { get; set; }

        public int id_contact { get; set; }

        [Required]
        public SalesRepType enum_type { get; set; }

        [Required]
        public string name { get; set; }

        public string code { get; set; }
        public decimal? commision_base { get; set; }
        public string commision_calculation { get; set; }
        public bool is_active { get; set; }
        public bool is_collection_agent { get; set; }

        public decimal monthly_goal { get; set; }

        [NotMapped]
        public decimal daily_goal
        {
            get { _daily_goal = monthly_goal / 30; return _daily_goal; }
            set { _daily_goal = value; monthly_goal = value * 30; RaisePropertyChanged("monthly_goal"); }
        }

        private decimal _daily_goal;

        public virtual IEnumerable<sales_budget> sales_budget { get; set; }
        public virtual IEnumerable<sales_order> sales_order { get; set; }
        public virtual IEnumerable<sales_invoice> sales_invoice { get; set; }
        public virtual IEnumerable<contact> contacts { get; set; }
        public virtual IEnumerable<payment> payment { get; set; }

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
                if (columnName == "enum_type")
                {
                    if (enum_type == 0)
                        return "Type needs to be selected";
                }
                return "";
            }
        }
    }
}