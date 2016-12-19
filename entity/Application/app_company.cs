namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_company : Email, IDataErrorInfo
    {
        public app_company()
        {
            is_active = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_company { get; set; }
        public int? id_geography { get; set; }
        [Required]
        public string name { get; set; }
        public string alias { get; set; }
        [Required]
        public string gov_code { get; set; }
        [Required]
        public string address { get; set; }
        public string domain { get; set; }
        public string hash_debehaber { get; set; }
        public string representative_name { get; set; }
        public string representative_gov_code { get; set; }
        public string accountant_name { get; set; }
        public string accountant_gov_code { get; set; }
        public bool is_active
        {
            get { return _is_active; }
            set
            {
                if (_is_active != value)
                {
                    _is_active = value;
                    RaisePropertyChanged("is_active");
                }
            }
        }
        private bool _is_active;
        public string version { get; set; }
        public string seats { get; set; }

        public virtual app_company_interest app_company_interest { get; set; }
        public virtual ICollection<app_branch> app_branch { get; set; }

        public virtual IEnumerable<item_inventory_dimension> item_inventory_dimension { get; set; }

        public virtual IEnumerable<app_account> app_account { get; set; }
        public virtual IEnumerable<app_account_detail> app_account_detail { get; set; }
        public virtual IEnumerable<app_condition> app_condition { get; set; }
        public virtual IEnumerable<app_contract> app_contract { get; set; }
        public virtual IEnumerable<app_cost_center> app_cost_center { get; set; }
        public virtual IEnumerable<app_currency> app_currency { get; set; }

        public virtual IEnumerable<app_geography> app_geography { get; set; }

        public virtual IEnumerable<app_dimension> app_dimension { get; set; }
        public virtual IEnumerable<app_document> app_document { get; set; }
        public virtual IEnumerable<app_document_range> app_document_range { get; set; }
        public virtual IEnumerable<app_measurement> app_measurement { get; set; }
        public virtual IEnumerable<contact> contact { get; set; }
        public virtual IEnumerable<contact_role> contact_role { get; set; }
        public virtual IEnumerable<impex> impex { get; set; }
        public virtual IEnumerable<payment> payment { get; set; }
        public virtual IEnumerable<payment_detail> payment_detail { get; set; }
        public virtual IEnumerable<payment_type> payment_type { get; set; }
        public virtual IEnumerable<payment_schedual> payment_schedual { get; set; }
        public virtual IEnumerable<payment_withholding_tax> payment_withholding_tax { get; set; }


        public virtual IEnumerable<sales_invoice> sales_invoice { get; set; }
        public virtual IEnumerable<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual IEnumerable<sales_budget> sales_budget { get; set; }
        public virtual IEnumerable<sales_budget_detail> sales_budget_detail { get; set; }
        public virtual IEnumerable<sales_order> sales_order { get; set; }
        public virtual IEnumerable<sales_order_detail> sales_order_detail { get; set; }
        public virtual IEnumerable<purchase_packing> sales_packinglist { get; set; }
        public virtual IEnumerable<sales_rep> sales_rep { get; set; }
        public virtual IEnumerable<sales_return> sales_return { get; set; }
        public virtual IEnumerable<sales_return_detail> sales_return_detail { get; set; }

        public virtual IEnumerable<security_role> security_role { get; set; }
        public virtual IEnumerable<security_user> security_user { get; set; }

        public virtual IEnumerable<purchase_invoice> purchase_invoice { get; set; }
        public virtual IEnumerable<purchase_invoice_detail> purchase_invoice_detail { get; set; }
        public virtual IEnumerable<purchase_order> purchase_order { get; set; }
        public virtual IEnumerable<purchase_order_detail> purchase_order_detail { get; set; }
        public virtual IEnumerable<purchase_return> purchase_return { get; set; }
        public virtual IEnumerable<purchase_return_detail> purchase_return_detail { get; set; }

        public virtual IEnumerable<purchase_tender> purchase_tender { get; set; }
        public virtual IEnumerable<purchase_tender_item> purchase_tender_item { get; set; }
        public virtual IEnumerable<purchase_tender_contact> purchase_tender_contact { get; set; }

        public virtual IEnumerable<project> project { get; set; }
        public virtual IEnumerable<project_task> project_task { get; set; }

        public virtual IEnumerable<project_template> project_template { get; set; }
        public virtual IEnumerable<project_template_detail> project_template_detail { get; set; }

        public virtual IEnumerable<item_price_list> item_price_list { get; set; }
        public virtual IEnumerable<item_product> item_product { get; set; }
        public virtual IEnumerable<item_movement> item_movement { get; set; }
        public virtual IEnumerable<item_transfer> item_transfer { get; set; }
        public virtual IEnumerable<item_inventory> item_inventory { get; set; }

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
                if (columnName == "gov_code")
                {
                    if (string.IsNullOrEmpty(gov_code))
                        return "Gov code needs to be filled";
                }
                if (columnName == "address")
                {
                    if (string.IsNullOrEmpty(address))
                        return "Addrss needs to be filled";
                }
                return "";
            }
        }
    }
}
