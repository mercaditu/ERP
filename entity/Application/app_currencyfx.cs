namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    
    public partial class app_currencyfx : Audit, IDataErrorInfo
    {
        public enum CurrencyFXTypes
        {
            Transaction = 0,
            Accounting = 1,
            Impex = 2
        }

        public app_currencyfx()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            is_active = true;
            timestamp = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_currencyfx { get; set; }
        [Required]
        public int id_currency { get; set; }
        [Required]
        public decimal buy_value { get; set; }
        [Required]
        public decimal sell_value { get; set; }
        [Required]
        public bool is_active { get; set; }
        public bool is_reverse { get; set; }

        public CurrencyFXTypes type { get; set; }

        public virtual app_currency app_currency { get; set; }
        public virtual IEnumerable<app_account_detail> app_account_detail { get; set; }
        public virtual IEnumerable<item_movement_value> item_movement_detail { get; set; }
        public virtual IEnumerable<sales_invoice> sales_invoice { get; set; }
        public virtual IEnumerable<sales_return> sales_return { get; set; }
        public virtual IEnumerable<sales_order> sales_order { get; set; }
        public virtual IEnumerable<sales_budget> sales_budget { get; set; }
        public virtual IEnumerable<purchase_invoice> purchase_invoice { get; set; }
        public virtual IEnumerable<purchase_return> purchase_return { get; set; }
        public virtual IEnumerable<purchase_order> purchase_order { get; set; }
        public virtual IEnumerable<purchase_tender_contact> purchase_tender_contact_detail { get; set; }
        public virtual IEnumerable<payment_detail> payment_detail { get; set; }
        public virtual IEnumerable<payment_schedual> payment_schedual { get; set; }
        public virtual IEnumerable<payment_withholding_tax> payment_withholding_tax { get; set; }
        public virtual IEnumerable<accounting_journal_detail> accounting_journal_detail { get; set; }
        public virtual IEnumerable<project_event> project_event { get; set; }
        public virtual IEnumerable<payment_promissory_note> payment_promissory_note { get; set; }

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
                if (columnName == "buy_value")
                {
                    if (buy_value == 0)
                        return "Buy value needs to be filled";
                }
                if (columnName == "sell_value")
                {
                    if (sell_value == 0)
                        return "Sell value needs to bo filled";
                }
                return "";
            }
        }
    }
}
