namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;

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
            id_user =  CurrentSession.Id_User;
            is_head = true;
            Properties.Settings _settings = new Properties.Settings();
            trans_date = DateTime.Now;
            production_order_detail = new List<production_order_detail>();
            item_request = new List<item_request>();           
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_production_order { get; set; }

        [Required]
        public int id_production_line { get; set; }
        public int? id_weather { get; set; }

        public int? id_project { get; set; }

        public int id_branch
        {
            get
            {
                if (_Id_Branch == 0)
                {
                    _Id_Branch = Properties.Settings.Default.branch_ID;
                };
                return _Id_Branch;
            }
            set { _Id_Branch = value; }
        }
        int _Id_Branch;

        public int id_terminal
        {
            get
            {
                if (_Id_terminal == 0)
                {
                    _Id_terminal = Properties.Settings.Default.terminal_ID;
                };
                return _Id_terminal;
            }
            set { _Id_terminal = value; }
        }
        int _Id_terminal;

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
        #region Document Range => Navigation
        public virtual app_document_range app_document_range { get; set; }
        #endregion

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

        public ProductionOrderTypes types { get; set; }

        public virtual production_line production_line { get; set; }
        
        public virtual project project { get; set; }

        public virtual ICollection<production_order_detail> production_order_detail { get; set; }
        public virtual ICollection<item_request> item_request { get; set; }

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
                        return "production line needs to be selected";
                }
                return "";
            }
        }
        #endregion

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
