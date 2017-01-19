
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class impex : Audit, IDataErrorInfo
    {
        public enum _impex_type
        {
            Import = 1,
            Export = 2
        }

        public impex()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            is_active = true;
            etd = DateTime.Now;
            eta = DateTime.Now.AddDays(30);

            impex_expense = new List<impex_expense>();
            impex_import = new List<impex_import>();
            impex_export = new List<impex_export>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_impex { get; set; }

        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_incoterm { get; set; }

        [Required]
        //[CustomValidation(typeof(entity.Class.EntityValidation), "CheckId")]
        public int id_contact { get; set; }
        public Status.Documents_General status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                RaisePropertyChanged("status");
            }
        }
        private Status.Documents_General _status;
        public string number { get; set; }

        [Required]
        public _impex_type impex_type { get; set; }
        public DateTime etd { get; set; }
        public DateTime eta { get; set; }
        public bool is_active { get; set; }
        public int id_currency { get; set; }
        public decimal fx_rate { get; set; }

        public DateTime? est_shipping_date { get; set; }
        public DateTime? real_shipping_date { get; set; }
        public DateTime? est_landed_date { get; set; }
        public DateTime? real_landed_date { get; set; }
        public DateTime? real_arrival_date { get; set; }
        public virtual impex_incoterm impex_incoterm { get; set; }
        public virtual contact contact { get; set; }
        public virtual ICollection<impex_expense> impex_expense { get; set; }
        public virtual ICollection<impex_import> impex_import { get; set; }
        public virtual ICollection<impex_export> impex_export { get; set; }

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
                if (columnName == "id_incoterm")
                {
                    if (id_incoterm == 0)
                        return "Incoterm needs to be selected";
                }
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
