
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;

    public partial class sales_packing : Audit, IDataErrorInfo
    {
        public sales_packing()
        {
            is_head = true;
            
            sales_packing_detail = new List<sales_packing_detail>();
            
            trans_date = DateTime.Now;

            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            if (CurrentSession.Id_Branch > 0) { id_branch = CurrentSession.Id_Branch; }
            if (CurrentSession.Id_Terminal > 0) { id_terminal = CurrentSession.Id_Terminal; }

            status = Status.Documents_General.Pending;
          
            
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_packing { get; set; }
        public int id_opportunity { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_contact { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_branch { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_terminal { get; set; }
        public int? id_range
        {
            get
            {
                return _id_range;
            }
            set
            {
                if (_id_range != value)
                {
                    _id_range = value;

                    using (db db = new db())
                    {
                        if (db.app_document_range.Where(x => x.id_range == _id_range).FirstOrDefault() != null)
                        {
                            app_document_range _app_range = db.app_document_range.Where(x => x.id_range == _id_range).FirstOrDefault();
                            if (db.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault() != null)
                            {
                                Brillo.Logic.Range.branch_Code = db.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault().code;
                            }
                            if (db.app_terminal.Where(x => x.id_terminal == id_terminal).FirstOrDefault() != null)
                            {
                                Brillo.Logic.Range.terminal_Code = db.app_terminal.Where(x => x.id_terminal == id_terminal).FirstOrDefault().code;
                            }
                            NumberWatermark = Brillo.Logic.Range.calc_Range(_app_range, false);
                            RaisePropertyChanged("NumberWatermark");
                        }
                    }
                }
            }
        }
        private int? _id_range; 
        public string number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string NumberWatermark { get; set; }

        public string comment { get; set; }
        public DateTime trans_date { get; set; }
        public Status.PackingTypes packing_type { get; set; }
        public Status.Documents_General status
        {
            get { return _status; }
            set { 
                _status = value; 
                RaisePropertyChanged("status"); }
        }
        Status.Documents_General _status;

        public bool is_issued { get; set; }

        //TimeCapsule
        public ICollection<sales_packing> older { get; set; }
        public sales_packing newer { get; set; }

        public virtual ICollection<sales_packing_detail> sales_packing_detail { get; set; }
        public virtual ICollection<item_movement> item_movement { get; set; }
      
        public virtual contact contact
        {
            get { return _contact; }
            set { _contact = value; RaisePropertyChanged("Contact"); }
        }
        contact _contact;

        public virtual crm_opportunity crm_opportunity { get; set; }
        public virtual app_document_range app_document_range { get; set; }
        public virtual app_branch app_branch { get; set; }
        public virtual app_terminal app_terminal { get; set; }

        [NotMapped]
        public bool selected { get; set; }

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
                // apply property level validation rules
                if (columnName == "id_contact")
                {
                    if (id_contact == 0)
                        return "Contact needs to be selected";
                }
                if (columnName == "id_branch")
                {
                    if (id_branch == 0)
                        return "Branch needs to be selected";
                }
                return "";
            }
        }
    }
}
