namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class payment_promissory_note : Audit, IDataErrorInfo
    {
        public payment_promissory_note()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            trans_date = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_note { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_contact { get; set; }

        public string note_number { get; set; }
        public Status.Documents status { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_currencyfx { get; set; }
        [Required]
        public decimal value { get; set; }
        public decimal interest { get; set; }
        [Required]
        public DateTime trans_date { get; set; }
        public DateTime expiry_date { get; set; }

        [NotMapped]
        public string NumberWatermark { get; set; }

        public virtual contact contact { get; set; }
        public virtual IEnumerable<payment_schedual> payment_schedual { get; set; }

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
                if (columnName == "id_contact")
                {
                    if (id_contact == 0)
                        return "Contact needs to be selected";
                }
                return "";
            }
        }
    }
}
