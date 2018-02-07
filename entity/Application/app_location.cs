namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_location : Audit, IDataErrorInfo
    {
        public app_location()
        {
            is_active = true;
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            is_read = false;
            timestamp = DateTime.Now;
            item_movement = new List<item_movement>();
            //item_mov_archive = new List<item_mov_archive>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_location { get; set; }

        [Required]
        public int id_branch { get; set; }

        public int? id_contact { get; set; }

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

        private bool _is_active; public bool is_default { get; set; }

        public virtual app_branch app_branch { get; set; }
        public virtual contact contact { get; set; }

        public virtual ICollection<item_movement> item_movement { get; set; }
        //public virtual ICollection<item_mov_archive>  item_mov_archive { get; set; }
        public virtual IEnumerable<item_transfer> item_transfer_origin { get; set; }
        public virtual IEnumerable<item_transfer> item_transfer_destination { get; set; }
        public virtual IEnumerable<item_inventory> item_inventory { get; set; }
        public virtual IEnumerable<item_request_decision> item_request_decision { get; set; }

        public virtual IEnumerable<purchase_order_detail> purchase_order_detail { get; set; }
        public virtual IEnumerable<purchase_invoice_detail> purchase_invoice_detail { get; set; }
        public virtual IEnumerable<purchase_return_detail> purchase_return_detail { get; set; }

        public virtual IEnumerable<sales_budget_detail> sales_budget_detail { get; set; }
        public virtual IEnumerable<sales_order_detail> sales_order_detail { get; set; }
        public virtual IEnumerable<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual IEnumerable<sales_return_detail> sales_return_detail { get; set; }
        public virtual IEnumerable<sales_packing_detail> sales_packing_detail { get; set; }
        public virtual IEnumerable<purchase_packing_detail> purchase_packing_detail { get; set; }

        public virtual IEnumerable<production_line> production_line { get; set; }

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
                //if (columnName == "id_branch")
                //{
                //    if (id_branch == 0)
                //        return "Branch needs to be selected";
                //}
                return "";
            }
        }
    }
}