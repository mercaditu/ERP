
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class payment : Audit
    {

        public payment()
        {
            is_head = true;
            trans_date = DateTime.Now;
            status = Status.Documents_General.Pending;
            payment_detail = new List<payment_detail>();

            //Session Variables
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            if (CurrentSession.Id_Branch > 0) { id_branch = CurrentSession.Id_Branch; }
            if (CurrentSession.Id_Terminal > 0) { id_terminal = CurrentSession.Id_Terminal; }
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_payment { get; set; }
        public int? id_weather { get; set; }
        public int? id_sales_rep { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? id_contact { get { return _id_contact; } set { _id_contact = value; RaisePropertyChanged("id_contact"); } }
        private int? _id_contact;

        public virtual contact contact { get { return _contact; } set { _contact = value; RaisePropertyChanged("contact"); } }
        private contact _contact;

        public bool is_accounted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Status.Documents_General status { get; set; }

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
                            app_document_range _app_range = db.app_document_range.Where(x => x.id_range == _id_range).FirstOrDefault();

                            if (_app_range != null)
                            {
                                Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == id_branch).FirstOrDefault().code;
                                Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == id_terminal).FirstOrDefault().code;
                                NumberWatermark = Brillo.Logic.Range.calc_Range(_app_range, false);
                                number = NumberWatermark;
                                RaisePropertyChanged("NumberWatermark");
                                RaisePropertyChanged("number");
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
        public decimal GrandTotal
        {
            get
            {
                return _GrandTotal;
            }
            set
            {
                _GrandTotal = value;
                RaisePropertyChanged("GrandTotal");
            }
        }
        private decimal _GrandTotal;

        [NotMapped]
        public decimal GrandTotalDetail
        {
            get
            {
                _GrandTotalDetail = 0;
                foreach (payment_detail _payment_detail in payment_detail)
                {
                    _GrandTotalDetail += _payment_detail.ValueInDefaultCurrency;
                }
                return Math.Round(_GrandTotalDetail, 2);
            }
            set
            {
                _GrandTotalDetail = value;
                RaisePropertyChanged("GrandTotalDetail");
            }
        }
        private decimal _GrandTotalDetail;

        [NotMapped]
        public decimal GrandTotalDetailValue
        {
            get
            {
                _GrandTotalDetailValue = 0;
                foreach (payment_detail _payment_detail in payment_detail)
                {
                    _GrandTotalDetailValue += _payment_detail.value;
                }
                return Math.Round(_GrandTotalDetailValue, 2);
            }
            set
            {
                _GrandTotalDetailValue = value;
                RaisePropertyChanged("GrandTotalDetailValue");
            }
        }
        private decimal _GrandTotalDetailValue;

        [NotMapped]
        public decimal Balance { get; set; }

        [NotMapped]
        public int id_currencyfx
        {
            get
            {
                return _id_currencyfx;
            }
            set
            {
                _id_currencyfx = value;
                RaisePropertyChanged("id_currencyfx");
            }
        }
        private int _id_currencyfx;

      

        /// <summary>
        /// 
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// 
        /// </summary>     
        public int? id_branch { get; set; }
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
        [NotMapped]
        public string NumberWatermark { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime trans_date { get; set; }

        public virtual ICollection<payment_detail> payment_detail { get; set; }

        public virtual sales_rep sales_rep { get; set; }
    }
}
