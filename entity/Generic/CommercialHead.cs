namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CommercialHead : Audit
    {

        /// <summary>
        /// Contact ID
        /// </summary>
        [Required]
        public int id_contact
        {
            get { return _id_contact; }
            set
            {
                if (value != _id_contact)
                {
                    _id_contact = value;
                    RaisePropertyChanged("id_contact");
                   //calc_credit(_GrandTotal);
                }
            }
        }
        #region Contact => Variables & Navigation
        private int _id_contact;
        public virtual contact contact
        {
            get { return _contact; }
            set
            {
                _contact = value;
                RaisePropertyChanged("contact");
            }
        }
        private contact _contact;
        #endregion

        #region Contact Ref => Navigation
        public virtual contact contact_ref { get { return _contact_ref; } set { _contact_ref = value; RaisePropertyChanged("contact_ref"); } }
        contact _contact_ref;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public int? id_sales_rep { get; set; }
        #region Sales Rep => Navigation
        public virtual sales_rep sales_rep { get; set; }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public int? id_weather { get; set; }
        #region Weather => Navigation
        public virtual app_weather app_weather { get; set; }
        #endregion

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

        /// <summary>
        /// 
        /// </summary>
        [Required]

        public int id_contract
        {
            get { return _id_contract; }
            set
            {
                if (value != _id_contract)
                {
                    _id_contract = value;
                    RaisePropertyChanged("id_contract");
                }
            }
        }
        #region Contract => Navigation
        private int _id_contract;
        public virtual app_contract app_contract { get { return _app_contract; } set { _app_contract = value; RaisePropertyChanged("app_contract"); } }
        private app_contract _app_contract;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_condition
        {
            get { return _id_condition; }
            set
            {
                if (value != _id_condition)
                {
                    _id_condition = value;
                    RaisePropertyChanged("id_condition");
                }
            }
        }
        #region Condition => Navigation
        private int _id_condition;
        public virtual app_condition app_condition { get { return _app_condition; } set { _app_condition = value; RaisePropertyChanged("app_condition"); } }
        private app_condition _app_condition;
        #endregion

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
                    RaisePropertyChanged("NumberWatermark");
                    //if (State == System.Data.Entity.EntityState.Added || State == System.Data.Entity.EntityState.Modified)
                    //{
                    //    using (db db = new db())
                    //    {
                    //        app_document_range app_document_range = db.app_document_range.Where(x => x.id_range == value).FirstOrDefault();
                    //        if (app_document_range != null)
                    //        {
                    //            code = app_document_range.code;
                    //        }
                    //    }
                    //}
                }
            }
        }
        private int? _id_range;

        #region Document Range => Navigation
        public virtual app_document_range app_document_range { get; set; }
        #endregion

        /// <summary>
        /// NotMapped. Sets the Credit Limit.
        /// </summary>
        //[NotMapped]
        //public decimal CreditLimit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? id_project { get; set; }
        #region Project => Navigation
        public virtual project project { get; set; }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public Status.Documents_General status
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
        private Status.Documents_General _status;

        /// <summary>
        /// 
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string NumberWatermark
        {
            get
            {
                if ((_NumberWatermark == null || _NumberWatermark == string.Empty) && (number == null || number == string.Empty) && id_range > 0)
                {
                    if (State == System.Data.Entity.EntityState.Added || State == System.Data.Entity.EntityState.Modified)
                    {
                        using (db db = new db())
                        {
                            app_document_range _app_range = db.app_document_range.Find(_id_range);

                            if (_app_range != null)
                            {
                                if (id_branch > 0)
                                {
                                    Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == id_branch).FirstOrDefault().code;
                                }
                                if (id_terminal > 0)
                                {
                                    Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == id_terminal).FirstOrDefault().code;
                                }
                                if (db.security_user.Find(id_user) != null)
                                {
                                    Brillo.Logic.Range.user_Code = db.security_user.Find(id_user).code;
                                }
                                if (db.projects.Find(id_project) != null)
                                {
                                    Brillo.Logic.Range.project_Code = db.projects.Find(id_project).code;
                                }

                                return Brillo.Logic.Range.calc_Range(_app_range, false);
                            }
                        }
                    }
                }
                return _NumberWatermark;
            }
            set
            {
                if (_NumberWatermark != value)
                {
                    _NumberWatermark = value;
                }
            }
        }
        private string _NumberWatermark;

        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime trans_date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool is_impex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool is_issued { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal GrandTotal
        {
            get { return Math.Round(_GrandTotal, 2); }
            set
            {
                if (_GrandTotal != value)
                {
                    _GrandTotal = value;

                    RaisePropertyChanged("GrandTotal");
                    //calc_credit(_GrandTotal);
                }
            }
        }
        private decimal _GrandTotal;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal Rate_Current { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string barcode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Status.TransactionTypes trans_type { get; set; }



        [NotMapped]
        public ICollection<CommercialVAT> CommercialVAT { get; set; }


        #region Navigation

        public virtual app_currencyfx app_currencyfx
        {
            get { return _app_currencyfx; }
            set { _app_currencyfx = value; RaisePropertyChanged("app_currencyfx"); }
        }
        private app_currencyfx _app_currencyfx;

        #endregion

        #region Methods

        #endregion
    }
}
