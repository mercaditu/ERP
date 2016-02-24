
namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class sales_packing_relation : IDataErrorInfo
    {
        public sales_packing_relation() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_packing_relation { get; set; }
        [Required]

        public long id_sales_invoice_detail { get; set; }
        [Required]
        public int id_sales_packing_detail { get; set; }

        public virtual sales_invoice_detail sales_invoice_detail { get; set; }
        public virtual sales_packing_detail sales_packing_detail { get; set; }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

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
              
                if (columnName == "id_sales_packinglist")
                {
                    if (id_sales_packing_detail == 0)
                        return "Packing list needs to be selected";
                }
                return "";
            }
        }
    }
}