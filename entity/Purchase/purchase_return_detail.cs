
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class purchase_return_detail : CommercialPurchaseDetail, INotifyPropertyChanged, IDataErrorInfo
    {
        public purchase_return_detail()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
            quantity = 1;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_return_detail { get; set; }
        public int id_purchase_return { get; set; }
        public int? id_purchase_invoice_detail { get; set; }

        public bool has_return { get; set; }

        public virtual purchase_return purchase_return { get; set; }
        public virtual purchase_invoice_detail purchase_invoice_detail { get; set; }
        public virtual ICollection<purchase_return_dimension> purchase_return_dimension { get; set; }

        #region "Validation"
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
                if (columnName == "quantity")
                {
                    if (quantity == 0)
                        return "Quantity can not be zero";
                }
                return "";
            }
        }
        #endregion
    }
}
