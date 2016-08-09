namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    public partial class item_transfer : Audit, IDataErrorInfo
    {
        public enum Transfer_type
        {
            movemnent = 0,
            transfer = 1
        }
        public item_transfer()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            if (CurrentSession.Id_Terminal > 0) { id_terminal = CurrentSession.Id_Terminal; }
            if (CurrentSession.Id_Branch > 0) { id_branch = CurrentSession.Id_Branch; }

            item_transfer_detail = new List<item_transfer_detail>();
            trans_date = DateTime.Now;
            timestamp = DateTime.Now;
            status = Status.Transfer.Pending;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_transfer { get; set; }
        public int? id_weather { get; set; }

        public Status.Transfer status 
        { 
            get
            {
                return _status;
            } 
            set
            {
                if (_status != value)
	            {
                    _status = value;
                    RaisePropertyChanged("status");
	            }
            }
        }
        private Status.Transfer _status;

        public int? id_item_request { get; set; }
        public int? id_project { get; set; }
        public int? id_department { get; set; }
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

                    if (State == System.Data.Entity.EntityState.Added || State == System.Data.Entity.EntityState.Modified)
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
                                if (db.projects.Where(x => x.id_project == id_project).FirstOrDefault() != null)
                                {
                                    Brillo.Logic.Range.project_Code = db.projects.Where(x => x.id_project == id_project).FirstOrDefault().code;
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

        public string number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string NumberWatermark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? id_terminal { get; set; }
        #region Terminal => Navigation
        public virtual app_terminal app_terminal { get; set; }
        #endregion
        public string comment { get; set; }
        public DateTime trans_date { get; set; }
        public Transfer_type transfer_type { get; set; }

        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_branch { get; set; }
        #region Branch => Navigation
        public virtual app_branch app_branch { get; set; }
        #endregion

    
        

        public virtual ICollection<item_transfer_detail> item_transfer_detail { get; set; }

        public virtual app_document_range app_document_range { get; set; }
        public virtual app_weather app_weather { get; set; }
        public virtual app_department app_department { get; set; }
        public virtual project project { get; set; }
        public virtual item_request item_request { get; set; }
        public virtual app_location app_location_origin { get; set; }
        public virtual app_location app_location_destination { get; set; }

       // [CustomValidation(typeof(Class.EntityValidation), "Checkbranch")]
        public virtual app_branch app_branch_origin { get; set; }
        
       // [CustomValidation(typeof(Class.EntityValidation), "Checkbranch")]
        public virtual app_branch app_branch_destination { get; set; }
        
        public virtual contact employee { get; set; }
        public virtual security_user user_requested { get; set; }
        public virtual security_user user_given { get; set; }

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
                if (columnName == "app_branch_origin" || columnName == "app_branch_destination")
                {
                    if (app_branch_origin == app_branch_destination)
                        return "please select diffrent origin and destination";
                }
                return "";
            }
        }

      
          
        
    }
}
