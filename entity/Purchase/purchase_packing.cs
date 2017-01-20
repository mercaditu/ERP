
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;

    public partial class purchase_packing : Audit, IDataErrorInfo
    {
        public purchase_packing()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            //if (Properties.Settings.Default.terminal_ID > 0) { app_terminal.id_terminal = Properties.Settings.Default.terminal_ID; }
            is_head = true;

            purchase_packing_detail = new List<purchase_packing_detail>();
            purchase_packing_relation = new List<purchase_packing_relation>();
            trans_date = DateTime.Now;
           // Properties.Settings _settings = new Properties.Settings();
            //if (_settings.branch_ID > 0) { id_branch = _settings.branch_ID; }
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_packing { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_contact { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_branch { get; set; }
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
                            Brillo.Logic.Range.branch_Code = db.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault().code;
                           // Brillo.Logic.Range.terminal_Code = db.app_terminal.Where(x => x.id_terminal == app_terminal.id_terminal).FirstOrDefault().code;
                            NumberWatermark = Brillo.Logic.Range.calc_Range(_app_range, true);
                            RaisePropertyChanged("NumberWatermark");
                        }
                    }
                }
            }
        }
        private int? _id_range;

        public int? id_item_asset { get; set; }
        public DateTime? eta { get; set; }
        public DateTime? etd { get; set; }
        public string driver { get; set; }
        public string licence_no { get; set; }
        public string avg_distance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string NumberWatermark { get; set; }
        public string number { get; set; }
        public string comment { get; set; }
        public DateTime trans_date { get; set; }
        /// <summary>
        /// 
        /// </summary>
      
        #region Terminal => Navigation
        public virtual app_terminal app_terminal { get; set; }
        #endregion
        public Status.Documents_General status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("status"); }
        }
        Status.Documents_General _status;

        //TimeCapsule
        public ICollection<purchase_packing> older { get; set; }
        public purchase_packing newer { get; set; }
   
        public virtual ICollection<purchase_packing_detail> purchase_packing_detail { get; set; }
        public virtual ICollection<purchase_packing_relation> purchase_packing_relation { get; set; }

        public virtual contact contact
        {
            get { return _contact; }
            set { _contact = value; RaisePropertyChanged("Contact"); }
        }
        contact _contact;

        public virtual app_branch app_branch { get; set; }
        public virtual item_asset item_asset { get; set; }


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
