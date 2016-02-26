namespace entity
{
    using System;
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
                    calc_credit(_GrandTotal);
                }
            }
        }  
        #region Contact => Variables & Navigation
        private int _id_contact;
        public virtual contact contact { get { return _contact; } set { _contact = value; RaisePropertyChanged("contact"); } }
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
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
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

        #region Document Range => Navigation
        public virtual app_document_range app_document_range { get; set; }
        #endregion

        /// <summary>
        /// NotMapped. Sets the Credit Limit.
        /// </summary>
        [NotMapped]
        public decimal CreditLimit { get; set; }

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
                _status = value;
                RaisePropertyChanged("status");
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
        public string NumberWatermark { get; set; }

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

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool is_accounted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public  decimal GrandTotal
        {
            get { return Math.Round(_GrandTotal, 2); }
            set
            {
                if (_GrandTotal != value)
                {
                    _GrandTotal = value;
                 
                    RaisePropertyChanged("GrandTotal");
                    calc_credit(_GrandTotal);
                }
            }
        }
        #region GrandTotal => Variable
        private decimal _GrandTotal;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal Rate_Current { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string barcode { get; set; }

        #region Navigation
        public virtual app_currencyfx app_currencyfx { get; set; }
        #endregion

        #region Methods

        public void generate_barcode()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected void calc_credit(decimal GrandTotal)
        {
            if (app_currencyfx != null && contact != null)
            {
                decimal rate = app_currencyfx.buy_value;

                if (contact.credit_availability != null)
                {
                    if (contact.credit_availability > 0)
                    {
                        CreditLimit = (decimal)contact.credit_availability * rate;
                        CreditLimit = CreditLimit - GrandTotal;
                        RaisePropertyChanged("CreditLimit");
                    }
                }
            }
        }

        /// <summary>
        /// Returns the Item Price marked as Default by the Company
        /// </summary>
        /// <returns>Item Price</returns>
        public item_price_list get_Default()
        {
            using (db db = new db())
            {
                if (db.item_price_list.Where(x => x.is_active == true && x.id_company == Properties.Settings.Default.company_ID) != null)
                {
                    return db.item_price_list.Where(x => x.is_active == true && x.id_company == Properties.Settings.Default.company_ID).FirstOrDefault();
                }
            }
            return null;
        }

        public decimal get_PurchasePrice(item_product item_product, int id_currencyfx, App.Names App_Name)
        {
            using (db db = new db())
            {
                if (db.item_movement.Where(x => x.item_product == item_product).LastOrDefault() != null)
                {
                    return db.item_movement.Where(x => x.item_product == item_product).LastOrDefault().item_movement_value.Sum(x => x.unit_value);
                }
            }
            return 0;
        }

        #endregion
    }
}
