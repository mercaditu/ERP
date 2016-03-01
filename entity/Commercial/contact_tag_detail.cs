namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class contact_tag_detail : Audit, IDataErrorInfo
    {
        public contact_tag_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_contact_tag_detail { get; set; }
        public int id_contact { get; set; }
        [Required]
        public int id_tag { get; set; }

        public virtual contact contact { get; set; }
        public virtual contact_tag contact_tag { get; set; }

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
                if (columnName == "id_tag")
                {
                    if (id_tag == 0)
                        return "Contact Tag needs to be selected";
                }
                return "";
            }
        }
    }
}

