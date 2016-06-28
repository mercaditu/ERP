
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
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

        /// <summary>
        /// 
        /// </summary>
        public int? id_contact { get { return _id_contact; } set { _id_contact = value; RaisePropertyChanged("id_contact"); } }
        private int? _id_contact;

        public virtual contact contact { get { return _contact; } set { _contact = value; RaisePropertyChanged("contact"); } }
        private contact _contact;

        /// <summary>
        /// 
        /// </summary>
        public int? id_journal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual accounting_journal accounting_journal { get; set; }

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

                    using (db db = new db())
                    {
                        if (db.app_document_range.Where(x => x.id_range == _id_range).FirstOrDefault() != null)
                        {
                            app_document_range _app_range = db.app_document_range.Where(x => x.id_range == _id_range).FirstOrDefault();
                            Brillo.Logic.Range.branch_Code = db.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = db.app_terminal.Where(x => x.id_terminal == id_terminal).FirstOrDefault().code;
                            NumberWatermark = Brillo.Logic.Range.calc_Range(_app_range, true);
                            number = NumberWatermark;
                            RaisePropertyChanged("NumberWatermark");
                            RaisePropertyChanged("number");
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
                foreach (payment_detail _payment_detail in payment_detail)
                {
                    _GrandTotal += _payment_detail.value;
                }
                return Math.Round(_GrandTotal, 2);
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
        public int id_currencyfx
        {
            get
            {
                return _id_currencyfx;
            }
            set
            {
                _id_currencyfx = value;
                RaisePropertyChanged("GrandTotal");
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
    }
}
