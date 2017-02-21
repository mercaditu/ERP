namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public partial class project : Audit, IDataErrorInfo
    {
        public project()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            is_active = true;
            project_task = new List<project_task>();
            project_tag_detail = new List<project_tag_detail>();
            est_start_date = DateTime.Now.Date;
            est_end_date = DateTime.Now.Date;
            sales_invoice = new List<sales_invoice>();
            sales_order = new List<sales_order>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_project { get; set; }

        public int? id_project_template { get; set; }
        public int? id_branch { get; set; }
        public int? id_contact { get; set; }

        public int? id_currency
        {
            get { return _id_currency; }
            set
            {
                if (_id_currency != value)
                {
                    _id_currency = value;
                }
            }
        }

        private int? _id_currency;

        [NotMapped]
        public int? CurrecyFx_ID
        {
            get
            {
                if (id_currency != null)
                {
                    using (db db = new db())
                    {
                        app_currencyfx app_currencyfx = db.app_currencyfx.Where(x => x.id_currency == id_currency && x.is_active).FirstOrDefault();
                        if (app_currencyfx != null)
                        {
                            return app_currencyfx.id_currencyfx;
                        }
                    }
                }
                return null;
            }
            set
            {
                if (_CurrecyFx_ID != value)
                {
                    _CurrecyFx_ID = value;
                }
            }
        }

        private int? _CurrecyFx_ID;

        [Required]
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("name");
                }
            }
        }

        private string _name;

        public string code { get; set; }
        public string comment { get; set; }
        public DateTime? est_start_date { get; set; }
        public DateTime? est_end_date { get; set; }

        public int priority { get; set; }
        public bool is_active { get; set; }

        public bool is_archived { get { return _is_archived; } set { _is_archived = value; RaisePropertyChanged("is_archived"); } }
        private bool _is_archived;

        [NotMapped]
        public int SelectedCount { get; set; }

        [NotMapped]
        public bool is_Executable { get; set; }
        
        public virtual app_branch app_branch { get; set; }
        public virtual contact contact { get; set; }
        public virtual app_currency app_currency { get; set; }
        public virtual project_template project_template { get; set; }

        public virtual ICollection<sales_invoice> sales_invoice { get; set; }
        public virtual ICollection<sales_order> sales_order { get; set; }
        public virtual IEnumerator<purchase_order> purchase_order { get; set; }
        public virtual IEnumerator<purchase_tender> purchase_tender { get; set; }
        public virtual IEnumerable<item_request> item_request { get; set; }
        public virtual ICollection<project_task> project_task { get; set; }
        public virtual ICollection<project_tag_detail> project_tag_detail { get; set; }
        public virtual ICollection<production_order> production_order { get; set; }

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
                //apply property level validation rules
                if (columnName == "name")
                {
                    if (string.IsNullOrEmpty(name))
                        return "Name needs to be selected";
                }
                return "";
            }
        }

        public void Update_SelectedCount()
        {
            int i = 0;

            foreach (project_task detail in project_task.Where(x => x.IsSelected))
            {
                i += 1;
            }

            SelectedCount = i;
            RaisePropertyChanged("SelectedCount");
        }
    }
}