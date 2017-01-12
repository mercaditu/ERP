namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class loyalty_member : Audit, IDataErrorInfo
    {
        public loyalty_member()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            loyalty_member_detail = new List<loyalty_member_detail>();
            is_active = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_member { get; set; }

        public int? id_contact { get; set; }

        [Required]
        public string number { get; set; }

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
        private bool _is_active;

        public virtual contact contact { get; set; }
        public virtual ICollection<loyalty_member_detail> loyalty_member_detail { get; set; }
      
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
                if (columnName == "number")
                {
                    if (string.IsNullOrEmpty(number))
                        return "Membership requires a Number.";
                }
                return "";
            }
        }
    }
}
