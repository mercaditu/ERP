namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_document_range : Audit, IDataErrorInfo
    {
        public app_document_range()
        {
            is_active = true;
            can_print = true;
            use_default_printer = false;
            expire_date = DateTime.Now.AddYears(1);
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_range { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_document { get; set; }

        public int? id_branch { get; set; }
        public int? id_terminal { get; set; }

        [Required]
        public short range_start { get; set; }
        [Required]
        public short range_current { get; set; }
        [Required]
        public short range_end { get; set; }
        public string range_padding { get; set; }
        [Required]
        public string range_template { get; set; }

        public string code { get; set; }
        public DateTime? expire_date { get; set; }

        public bool use_default_printer { get; set; }
        public string printer_name { get; set; }
        public bool can_print { get; set; }
        public bool is_active { get; set; }
        
        public virtual app_document app_document { get; set; }
        public virtual app_branch app_branch { get; set; }
        public virtual app_terminal app_terminal { get; set; }
        public virtual IEnumerable<sales_budget> sales_budget { get; set; }
        public virtual IEnumerable<sales_order> sales_order { get; set; }
        public virtual IEnumerable<sales_invoice> sales_invoice { get; set; }
        public virtual IEnumerable<sales_return> sales_return { get; set; }
        public virtual IEnumerable<purchase_order> purchase_order { get; set; }
        public virtual IEnumerable<payment_withholding_tax> payment_withholding_tax { get; set; }
        public virtual IEnumerable<payment> payment { get; set; }
        public virtual IEnumerable<payment_detail> payment_detail { get; set; }
        public virtual IEnumerable<project_task> project_task { get; set; }
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
                if (columnName == "id_document")
                {
                    if (id_document == 0)
                        return "Document needs to be Selected";
                }
                if (columnName == "range_start")
                {
                    if (range_start < 0)
                        return "Start range needs to be filled";
                }
                if (columnName == "range_current")
                {
                    if (range_current < 0)
                        return "Current range needs to be filled";
                }
                if (columnName == "range_end")
                {
                    if (range_end <= 0)
                        return "End range needs to be filled";
                }
                if (columnName == "range_template")
                {
                    if (string.IsNullOrEmpty(range_template))
                        return "Range template needs to be filled";
                }
                return "";
            }
        }
    }
}
