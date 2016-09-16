
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class item_request : Audit
    {
      
        public item_request()
        {
            item_request_detail = new List<item_request_detail>();
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
                        is_head = true;
            status = Status.Documents_General.Pending;
            request_date = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_request { get; set; }
        public int? id_project { get; set; }
        public int? id_sales_order { get; set; }
        public int? id_production_order { get; set; }
        public int? id_currency { get; set; }
        public int? id_branch { get; set; }
        public int? id_department { get; set; }
           
        public DateTime request_date { get; set; }
        public string name { get; set; }
        public string comment { get; set; }
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
                            app_document_range _app_range = db.app_document_range.Where(x => x.id_range == _id_range).FirstOrDefault();
                            if (_app_range != null)
                            {
                                app_branch app_branch = db.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault();    
                                if (app_branch != null)
                                {
                                    Brillo.Logic.Range.branch_Code = app_branch.code;
                                }

                                security_user security_user = db.security_user.Where(x => x.id_user == id_user).FirstOrDefault();
                                if (security_user != null)
                                {
                                    Brillo.Logic.Range.user_Code = security_user.code;
                                }

                                project projects = db.projects.Where(x => x.id_project == id_project).FirstOrDefault();
                                if (projects != null)
                                {
                                    Brillo.Logic.Range.project_Code = projects.code;
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

        
        /// <summary>
        /// 
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string NumberWatermark { get; set; }

        [NotMapped]
        public int TotalSelected { get; set; }

        public virtual sales_order sales_order { get; set; }
        public virtual project project { get; set; }
        public virtual production_order production_order { get; set; }
        public virtual ICollection<item_request_detail> item_request_detail { get; set; }
        public virtual ICollection<item_transfer> item_transfer { get; set; }
        public virtual security_user request_user { get; set; }
        public virtual app_currency app_currency { get; set; }
        public virtual app_department app_department { get; set; }
        public virtual app_branch app_branch { get; set; }

        public void GetTotalDecision()
        {
            int i = 0;
            
            foreach (item_request_detail detail in item_request_detail)
            {
                i += detail.GetTotalDecision();
            }

            TotalSelected = i;
            RaisePropertyChanged("TotalSelected");
        }

        //public string Error
        //{
        //    get
        //    {
        //        StringBuilder error = new StringBuilder();

        //        // iterate over all of the properties
        //        // of this object - aggregating any validation errors
        //        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
        //        foreach (PropertyDescriptor prop in props)
        //        {
        //            String propertyError = this[prop.Name];
        //            if (propertyError != string.Empty)
        //            {
        //                error.Append((error.Length != 0 ? ", " : "") + propertyError);
        //            }
        //        }

        //        return error.Length == 0 ? null : error.ToString();
        //    }
        //}
        //public string this[string columnName]
        //{
        //    get
        //    {
        //        return "";
        //    }
        //}
    }
}
