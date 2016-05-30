namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;
  

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

        /// <summary>
        /// 
        /// </summary>
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

                    if (State == System.Data.Entity.EntityState.Added || State == System.Data.Entity.EntityState.Modified || State == 0)
                    {
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
                                if (db.security_user.Where(x => x.id_user == id_user).FirstOrDefault() != null)
                                {
                                    Brillo.Logic.Range.user_Code = db.security_user.Where(x => x.id_user == id_user).FirstOrDefault().code;
                                }
                             

                                NumberWatermark = Brillo.Logic.Range.calc_Range(_app_range, false);
                                RaisePropertyChanged("NumberWatermark");
                            }
                        }
                    }
                }

            }
        }
        private int? _id_range;

        #region Document Range => Navigation
        public virtual app_document_range app_document_range { get; set; }
        #endregion

        [NotMapped]
        public string NumberWatermark { get; set; }
        public string note_number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_branch { get; set; }
        #region Branch => Navigation
        public virtual app_branch app_branch { get; set; }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public int? id_terminal { get; set; }
        #region Terminal => Navigation
        public virtual app_terminal app_terminal { get; set; }
        #endregion
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
