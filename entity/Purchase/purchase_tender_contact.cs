
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class purchase_tender_contact : Audit
    {
        public purchase_tender_contact()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            if (contact!=null)
            {
                recieve_date_est = DateTime.Now.AddDays((double)contact.lead_time);
            }
            purchase_tender_detail = new List<purchase_tender_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_tender_contact { get; set; }
        public int id_purchase_tender { get; set; }
        public int id_contact { get; set; }
        public int id_contract { get; set; }
        public int id_condition { get; set; }
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

               
                    foreach (purchase_tender_detail _purchase_tender_detail in purchase_tender_detail)
                    {
                        _purchase_tender_detail.State = System.Data.Entity.EntityState.Modified;
                        _purchase_tender_detail.CurrencyFX_ID = _id_currencyfx;
                    }
                    RaisePropertyChanged("GrandTotal");
               
                
            } 
        }
        int _id_currencyfx;
        public string comment { get; set; }
        public DateTime? recieve_date_est { get; set; }

        [NotMapped]
        public decimal GrandTotal
        {
            get
            {
                _GrandTotal = 0;
                foreach (purchase_tender_detail _purchase_tender_detail in purchase_tender_detail)
                {
                    _GrandTotal += _purchase_tender_detail.SubTotal_Vat;
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
        public virtual purchase_tender purchase_tender { get; set; }
        public virtual app_currencyfx app_currencyfx { get; set; }
        public virtual contact contact { get; set; }
        public virtual app_condition app_condition { get; set; }
        public virtual app_contract app_contract { get; set; }
      
        public virtual ICollection<purchase_tender_detail> purchase_tender_detail { get; set; }


    }
}
