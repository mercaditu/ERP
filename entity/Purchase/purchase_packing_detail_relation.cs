
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace entity
{
    public class purchase_packing_detail_relation
    {
        public purchase_packing_detail_relation()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_packing_detail_relation { get; set; }

        [Required]
        public int id_purchase_invoice_detail { get; set; }

        [Required]
        public int id_purchase_packing_detail { get; set; }

        public virtual purchase_invoice_detail purchase_invoice_detail { get; set; }
        public virtual purchase_packing_detail purchase_packing_detail { get; set; }

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
                    if (id_purchase_packing_detail == 0)
                        return Brillo.Localize.PleaseSelect;
                }
                return "";
            }
        }
    }
}
