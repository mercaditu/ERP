namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class purchase_tender : Audit
    {
        public purchase_tender()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            if (CurrentSession.Id_Terminal > 0) { id_terminal = CurrentSession.Id_Terminal; }
            if (CurrentSession.Id_Branch > 0) { id_branch = CurrentSession.Id_Branch; }

            trans_date = DateTime.Now;
            status = Status.Documents_General.Pending;

            purchase_order = new List<purchase_order>();
            purchase_tender_contact_detail = new List<purchase_tender_contact>();
            purchase_tender_item_detail = new List<purchase_tender_item>();
            item_request_decision = new List<item_request_decision>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_tender { get; set; }

        public int id_branch { get; set; }

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

        public int? id_department { get; set; }
        public int? id_project { get; set; }
        public int? id_weather { get; set; }
        public string name { get; set; }
        public short code { get; set; }
        public string comment { get; set; }
        public DateTime trans_date { get; set; }

        public bool is_archived { get { return _is_archived; } set { _is_archived = value; RaisePropertyChanged("is_archived"); } }
        private bool _is_archived;

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
                                if (_app_range.range_template.Contains("#Branch"))
                                {
                                    Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == id_branch).Select(y => y.code).FirstOrDefault();
                                }

                                if (_app_range.range_template.Contains("#Terminal"))
                                {
                                    Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == id_terminal).Select(y => y.code).FirstOrDefault(); ;
                                }

                                if (_app_range.range_template.Contains("#User"))
                                {
                                    Brillo.Logic.Range.user_Code = db.security_user.Where(x => x.id_user == id_user).Select(y => y.code).FirstOrDefault();
                                }

                                if (_app_range.range_template.Contains("#Project"))
                                {
                                    Brillo.Logic.Range.project_Code = db.projects.Where(x => x.id_project == id_project).Select(y => y.code).FirstOrDefault();
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
        public int? id_terminal { get; set; }
        public string number { get; set; }

        /// <summary>
        ///
        /// </summary>
        [NotMapped]
        public string NumberWatermark { get; set; }

        public virtual IEnumerable<app_weather> app_weather { get; set; }
        public virtual ICollection<purchase_order> purchase_order { get; set; }
        public virtual ICollection<purchase_tender_contact> purchase_tender_contact_detail { get; set; }
        public virtual ICollection<purchase_tender_item> purchase_tender_item_detail { get; set; }
        public virtual app_branch app_branch { get; set; }
        public virtual app_terminal app_terminal { get; set; }
        public virtual app_document_range app_document_range { get; set; }
        public virtual project project { get; set; }
        public virtual ICollection<item_request_decision> item_request_decision { get; set; }
    }
}