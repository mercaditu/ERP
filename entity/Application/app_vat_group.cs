namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_vat_group : Audit, IDataErrorInfo
    {
        public app_vat_group()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            is_active = true;
            app_vat_group_details = new List<app_vat_group_details>();
            sales_invoice_detail = new List<sales_invoice_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_vat_group { get; set; }

        [Required]
        public string name { get; set; }

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
        public bool is_default { get; set; }
        public int? cloud_id { get; set; }

        public virtual ICollection<app_vat_group_details> app_vat_group_details { get; set; }
        public virtual ICollection<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual ICollection<sales_budget_detail> sales_budget_detail { get; set; }
        public virtual ICollection<sales_order_detail> sales_order_detail { get; set; }

        internal static object Where(Func<object, object> p)
        {
            throw new NotImplementedException();
        }

        public virtual ICollection<sales_return_detail> sales_return_detail { get; set; }
        public virtual ICollection<purchase_invoice_detail> purchase_invoice_detail { get; set; }
        public virtual ICollection<purchase_order_detail> purchase_order_detail { get; set; }
        public virtual ICollection<purchase_return_detail> purchase_return_detail { get; set; }
        public virtual ICollection<item> item { get; set; }

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
                return "";
            }
        }
    }
}