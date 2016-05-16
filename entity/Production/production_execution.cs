namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;


    public partial class production_execution : Audit
    {
        public production_execution()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            trans_date = DateTime.Now;
            production_execution_detail = new List<production_execution_detail>();
            status = Status.Documents_General.Pending;
            
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_production_execution { get; set; }
        [Required]
        public int id_production_line { get; set; }
        public int id_production_order { get; set; }
        public int? id_weather { get; set; }
        public DateTime trans_date { get; set; }
        public Status.Documents_General status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("status"); }
        }
        Status.Documents_General _status;

        public virtual production_line production_line { get; set; }
        public virtual production_order production_order { get; set; }
        public virtual ICollection<production_execution_detail> production_execution_detail { get; set; }
        

        #region Validation
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

                if (columnName == "id_production_line")
                {
                    if (id_production_line == 0)
                        return "production line needs to be selected";
                }
                return "";
            }
        }
        #endregion
    }
}
