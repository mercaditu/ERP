namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class item_inventory : Audit
    {
        public item_inventory()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            trans_date = DateTime.Now;
            status = Status.Documents.Pending;
            item_inventory_detail = new List<item_inventory_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_inventory { get; set; }

        public int id_branch { get; set; }
        public int code { get; set; }
        public string comment { get; set; }
        public DateTime trans_date { get; set; }

        public Status.Documents status
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

        private Status.Documents _status;

        public bool is_archived { get { return _is_archived; } set { _is_archived = value; RaisePropertyChanged("is_archived"); } }
        private bool _is_archived;

        [NotMapped]
        public string GroupBatchCode
        {
            get { return _GroupBatchCode; }
            set
            {
                if (_GroupBatchCode != value)
                {
                    _GroupBatchCode = value;

                    foreach (item_inventory_detail detail in item_inventory_detail.Where(x => x.IsSelected))
                    {
                        detail.batch_code = _GroupBatchCode;
                        detail.RaisePropertyChanged("batch_code");
                    }
                }
            }
        }
        private string _GroupBatchCode;

        [NotMapped]
        public DateTime GroupExpiryDate
        {
            get { return _GroupExpiryDate; }
            set
            {
                if (_GroupExpiryDate != value)
                {
                    _GroupExpiryDate = value;

                    foreach (item_inventory_detail detail in item_inventory_detail.Where(x => x.IsSelected))
                    {
                        detail.expire_date = _GroupExpiryDate;
                        detail.RaisePropertyChanged("expire_date");
                    }
                }
            }
        }
        private DateTime _GroupExpiryDate;

        public virtual app_branch app_branch { get; set; }
        public virtual ICollection<item_inventory_detail> item_inventory_detail { get; set; }
    }
}