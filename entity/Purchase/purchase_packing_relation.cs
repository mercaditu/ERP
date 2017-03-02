namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class purchase_packing_relation : IDataErrorInfo
    {
        public purchase_packing_relation()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_packing_relation { get; set; }

        [Required]
        public int id_purchase_invoice { get; set; }

        [Required]
        public int id_purchase_packing { get; set; }

        public virtual purchase_invoice purchase_invoice { get; set; }
        public virtual purchase_packing purchase_packing { get; set; }

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
                    if (id_purchase_packing == 0)
                        return "Packing list needs to be selected";
                }
                return "";
            }
        }
    }
}