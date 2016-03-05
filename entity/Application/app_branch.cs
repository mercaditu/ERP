namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    

    public partial class app_branch : Audit, IDataErrorInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_branch { get; set; }
        public int? id_geography { get; set; }
        public int? id_vat { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string code { get; set; }

        public decimal? geo_lat { get; set; }
        public decimal? geo_long { get; set; }

        public bool can_stock { get; set; }
        public bool can_invoice { get; set; }
        public bool is_active { get; set; }

        public app_branch()
        {
            id_company = CurrentSession.Id_Branch;
            id_user = CurrentSession.Id_User;
            is_head = true;
            is_active = true;
            app_location = new List<app_location>();
            app_terminal = new List<app_terminal>();
        }

        public virtual app_vat app_vat { get; set; }
        public virtual app_geography app_geography { get; set; }

        public virtual ICollection<item_request> item_request { get; set; }
        public virtual ICollection<app_location> app_location { get; set; }
        public virtual ICollection<app_terminal> app_terminal { get; set; }

        //Stock
        public virtual IEnumerable<item_transfer> item_transfer { get; set; }

        //Purchase
        public virtual IEnumerable<purchase_invoice> purchase_invoice { get; set; }
        public virtual IEnumerable<purchase_tender> purchase_tender { get; set; }
        public virtual IEnumerable<purchase_order> purchase_order { get; set; }
        public virtual IEnumerable<purchase_return> purchase_return { get; set; }

        //Sales
        public virtual ICollection<sales_budget> sales_budget { get; set; }
        public virtual IEnumerable<sales_invoice> sales_invoice { get; set; }
        public virtual IEnumerable<sales_order> sales_order { get; set; }
        public virtual IEnumerable<sales_return> sales_return { get; set; }

        //Projects
        public virtual IEnumerable<project> project { get; set; }

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
                if (columnName == "code")
                {
                    if (String.IsNullOrEmpty(code))
                        return "Code needs to be filled";
                }
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
