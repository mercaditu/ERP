namespace entity
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_contract_detail : Audit, IDataErrorInfo
    {
        public app_contract_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            is_order = false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_contract_detail { get; set; }

        [Required]
        public int id_contract { get; set; }

        /// <summary>
        /// Coefficient > decimal representation of 100% (ex=> 0.3 = 30%) of the total amount to be paid.
        /// </summary>
        [Required]
        public decimal coefficient { get; set; }

        /// <summary>
        /// Interval > Interval of Days between one payment and the next. (ex=> 30 = payment in 30 days from date of transaction)
        /// </summary>
        [Required]
        public short interval { get; set; }

        public bool is_order { get; set; }

        public virtual app_contract app_contract { get; set; }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
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
                if (columnName == "coefficient")
                {
                    if (coefficient == 0)
                        return "Coefficient needs to be filled";
                }
                if (columnName == "interval")
                {
                    if (interval < 0)
                        return "interval needs to be filled";
                }
                return "";
            }
        }
    }
}