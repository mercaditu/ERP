namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public partial class production_order : Audit
    {
        public enum ProductionOrderTypes
        {
            Production,
            Fraction,
            Internal
        }

        public production_order()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            trans_date = DateTime.Now;
            production_order_detail = new List<production_order_detail>();
            item_request = new List<item_request>();
            item_request_decision = new List<item_request_decision>();

            id_branch = CurrentSession.Id_Branch;
            id_terminal = CurrentSession.Id_Terminal;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_production_order { get; set; }

        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_production_line { get; set; }

        public int? id_weather { get; set; }

        public int? id_project { get; set; }

        public int id_branch { get; set; }

        public int id_terminal { get; set; }

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

                    if (State == System.Data.Entity.EntityState.Added || State == System.Data.Entity.EntityState.Modified) // || State == 0
                    {
                        using (db db = new db())
                        {
                            if (db.app_document_range.Where(x => x.id_range == _id_range).Any())
                            {
                                app_document_range _app_range = db.app_document_range.Where(x => x.id_range == _id_range).FirstOrDefault();

                                if (db.app_branch.Where(x => x.id_branch == id_branch).Any())
                                {
                                    Brillo.Logic.Range.branch_Code = db.app_branch.Where(x => x.id_branch == id_branch).Select(x => x.code).FirstOrDefault();
                                }
                                if (db.app_terminal.Where(x => x.id_terminal == id_terminal).Any())
                                {
                                    Brillo.Logic.Range.terminal_Code = db.app_terminal.Where(x => x.id_terminal == id_terminal).Select(x => x.code).FirstOrDefault();
                                }
                                if (db.security_user.Where(x => x.id_user == id_user).Any())
                                {
                                    Brillo.Logic.Range.user_Code = db.security_user.Where(x => x.id_user == id_user).Select(x => x.code).FirstOrDefault();
                                }
                                if (db.projects.Where(x => x.id_project == id_project).Any())
                                {
                                    Brillo.Logic.Range.project_Code = db.projects.Where(x => x.id_project == id_project).Select(x => x.code).FirstOrDefault();
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

        #endregion Document Range => Navigation

        /// <summary>
        ///
        /// </summary>
        [NotMapped]
        public int SelectedCount { get; set; }

        /// <summary>
        ///
        /// </summary>
        [NotMapped]
        public string NumberWatermark { get; set; }

        public string work_number { get; set; }

        public string project_cost_center { get; set; }

        public Status.Production? status { get; set; }

        public string name { get; set; }

        public string barcode { get; set; }

        [Required]
        public DateTime trans_date { get; set; }

        public DateTime? start_date_est { get; set; }

        public DateTime? end_date_est { get; set; }

        public bool is_archived { get { return _is_archived; } set { _is_archived = value; RaisePropertyChanged("is_archived"); } }
        private bool _is_archived;

        public ProductionOrderTypes type { get; set; }

        public virtual production_line production_line { get; set; }

        public virtual project project { get; set; }

        //public virtual app_branch app_branch { get; set; }

        public virtual ICollection<production_order_detail> production_order_detail { get; set; }
        public virtual ICollection<item_request> item_request { get; set; }
        public virtual ICollection<item_request_decision> item_request_decision { get; set; }

        #region Error

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

                if (columnName == "id_production_line")
                {
                    if (id_production_line == 0)
                        return "Please select a Production Line";
                }
                return "";
            }
        }

        #endregion Error

        public void Update_SelectedCount()
        {
            int i = 0;
            foreach (production_order_detail detail in production_order_detail.Where(x => x.IsSelected))
            {
                i += 1;
            }

            SelectedCount = i;
            RaisePropertyChanged("SelectedCount");
        }
    }
}