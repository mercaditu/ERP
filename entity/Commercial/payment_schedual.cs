namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class payment_schedual : Audit
    {
        public payment_schedual()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            can_calculate = true;
            child = new List<payment_schedual>();

            expire_date = DateTime.Now;
            timestamp = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_payment_schedual { get; set; }
        public int? id_purchase_invoice { get; set; }
        public int? id_purchase_return { get; set; }
        public int? id_sales_invoice { get; set; }
        public int? id_sales_return { get; set; }
        public int? id_sales_order { get; set; }
        public int? id_purchase_order { get; set; }
        public int? id_note { get; set; }
        public int? id_payment_detail { get; set; }
        public Status.Documents_General status { get; set; }
        public int id_contact { get; set; }
        public int id_currencyfx { get; set; }

        public string number { get; set; }

        public decimal debit { 
            get 
            { 
                return _debit; 
            } 
            set 
            {
                if (_debit != value)
                {
                    _debit = value;
                    RaisePropertyChanged("debit");
                    RaisePropertyChanged("AccountReceivableBalance"); 
                }
            } 
        }
        Decimal _debit;

        public decimal credit { get; set; }

        public bool can_calculate { get; set; }

        //   Not Mapped Properties
        #region NotMapped

        [NotMapped]
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

                    if (number != null || number != "")
                    {
                        using (db db = new db())
                        {
                            app_document_range _app_range = db.app_document_range.Find(_id_range);

                            if (_app_range != null)
                            {
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

        [NotMapped]
        public string NumberWatermark { get; set; }

        [NotMapped]
        public decimal AccountPayableBalance
        {
            get
            {
                return credit - (child.Count() > 0 ? child.Sum(y => y.debit) : 0);
            }
            set
            {
                _AccountPayableBalance = value;
                RaisePropertyChanged("AccountPayableBalance");
            }
        }
        decimal _AccountPayableBalance;

        [NotMapped]
        public decimal AccountReceivableBalance
        {
            get
            {
                return debit - (child.Count() > 0 ? child.Sum(y => y.credit) : 0);
            }
            set
            {
                _AccountReceivableBalance = value;
                RaisePropertyChanged("AccountReceivableBalance");
            }
        }
        decimal _AccountReceivableBalance;

        #endregion

        public DateTime trans_date { get; set; }
        public DateTime expire_date { get; set; }

        //Hierarchy
        public virtual ICollection<payment_schedual> child { get; set; }
        public virtual payment_schedual parent { get; set; }

        public virtual payment_detail payment_detail { get; set; }
        public virtual app_currencyfx app_currencyfx { get; set; }
        public virtual sales_invoice sales_invoice { get; set; }
        public virtual purchase_invoice purchase_invoice { get; set; }
        public virtual sales_order sales_order { get; set; }
        public virtual purchase_order purchase_order { get; set; }
        public virtual purchase_return purchase_return { get; set; }
        public virtual sales_return sales_return { get; set; }
        public virtual payment_promissory_note payment_promissory_note { get; set; }
        public virtual contact contact { get; set; }
    }
}
