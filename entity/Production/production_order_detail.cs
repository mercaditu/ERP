namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class production_order_detail : Audit
    {
        private Project.clsproject objclsproject = new Project.clsproject();

        public production_order_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            child = new List<production_order_detail>();
            production_order_dimension = new List<production_order_dimension>();
            production_execution_detail = new List<production_execution_detail>();
            item_request_detail = new List<item_request_detail>();
            trans_date = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_order_detail { get; set; }

        public int id_production_order { get; set; }
        public int? id_project_task { get; set; }
        public int? id_item { get; set; }
        public int? movement_id { get; set; }
        public string name
        {
            get
            {
                if (string.IsNullOrEmpty(_name) && id_item > 0)
                {
                    using (db db = new db())
                    {
                        _name = db.items.Find(id_item).name;
                    }
                }

                return _name;
            }
            set
            {
                _name = value;
            }
        }
        private string _name;

        public bool is_archived { get { return _is_archived; } set { _is_archived = value; RaisePropertyChanged("is_archived"); } }
        private bool _is_archived;

        public decimal quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    RaisePropertyChanged("quantity");

                    //CALCULATES NON RECEPIE ITEMS. NORMAL PRODUCTS OR RAW MATERIALS.
                    if (production_order != null)
                    {
                        if (parent != null && parent.item != null && production_order.type != production_order.ProductionOrderTypes.Fraction)
                        {
                            //PROTECTS CHILD FROM UPDATING PARENTS.
                            if (parent.item.item_recepie.Count == 0)
                            {
                                parent.quantity = objclsproject.getsumquantityProduction(parent.id_order_detail, parent.child);
                                parent.RaisePropertyChanged("quantity");
                            }
                        }
                    }

                    //CALCULATES FOR CHILD RECEPIES
                    if (item != null)
                    {
                        if (item.item_recepie.Count > 0)
                        {
                            if (child.Count > 0)
                            {
                                foreach (production_order_detail production_order_detail in child)
                                {
                                    production_order_detail.quantity = production_order_detail.item.item_recepie_detail.FirstOrDefault().quantity * this.quantity;
                                    production_order_detail.RaisePropertyChanged("quantity");
                                }
                            }
                        }
                    }
                }
            }
        }

        private decimal _quantity;

        [NotMapped]
        public decimal? quantity_exe
        { get; set; }

        [NotMapped]
        public decimal? Unit_Cost_exe
        { get; set; }

        public Status.Production? status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    if (value != null)
                    {
                        _status = (Status.Production)value;
                        RaisePropertyChanged("status");
                    }

                }
            }
        }

        private Status.Production _status = Status.Production.Pending;

        public string code
        {
            get { return _code; }
            set { _code = value; RaisePropertyChanged("code"); }
        }

        private string _code;

        [NotMapped]
        public new bool IsSelected
        {
            get { return _is_selected; }
            set
            {
                if (value != _is_selected)
                {
                    _is_selected = value;
                    RaisePropertyChanged("IsSelected");

                    if (child != null)
                    {
                        if (child.Count() > 0)
                        {
                            foreach (production_order_detail _child in child)
                            {
                                _child.IsSelected = value;
                                _child.RaisePropertyChanged("IsSelected");
                            }
                        }
                    }

                    if (production_order != null)
                    {
                        production_order.Update_SelectedCount();
                    }
                }
            }
        }

        private bool _is_selected;

        public bool is_input { get; set; }

        [NotMapped]
        public bool is_request
        {
            get
            {
                if (item_request_detail != null)
                {
                    if (item_request_detail.Count() > 0)
                    {
                        _is_request = true;
                    }
                    else
                    {
                        _is_request = false;
                    }
                }
                return _is_request;
            }
            set { _is_request = value; }
        }

        private bool _is_request;

        [Required]
        public DateTime trans_date { get; set; }

        public DateTime? start_date_est { get; set; }
        public DateTime? end_date_est { get; set; }

        /// <summary>
        /// Percentage of Order Completion.
        /// </summary>
        public decimal completed
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;
                RaisePropertyChanged("completed");

                percent = (_completed * 100).ToString() + " %";
                RaisePropertyChanged("percent");
            }
        }

        private decimal _completed;

        public decimal importance { get; set; }

        [NotMapped]
        public string percent { get; set; }
        [NotMapped]
        public bool OutOfStock { get; set; }

        //Self Referencing
        public virtual production_order_detail parent { get; set; }

        public virtual ICollection<production_order_detail> child { get; set; }
        public virtual ICollection<production_service_account> production_service_account { get; set; }
        public virtual ICollection<production_account> production_account { get; set; }
        public virtual ICollection<production_order_dimension> production_order_dimension { get; set; }
        public virtual ICollection<production_execution_detail> production_execution_detail { get; set; }
        public virtual ICollection<item_request_detail> item_request_detail { get; set; }
        public virtual production_order production_order { get; set; }
        public virtual project_task project_task { get; set; }
        public virtual item item { get; set; }

        #region Methods

        /// <summary>
        /// Gets the Total Quantity based on Executed Values from Production Execution.
        /// </summary>
        public void CalcExecutedQty_TimerTaks()
        {
            try
            {
                if (production_execution_detail != null && production_execution_detail.Count > 0)
                {
                    using (db db = new db())
                    {
                        quantity_exe = db.production_execution_detail.Where(y => y.id_order_detail == id_order_detail).Sum(x => x.quantity);
                        RaisePropertyChanged("quantity_exe");
                    }
                }
            }
            catch { }
        }

        public void CalcExecutedCost_TimerTaks()
        {
            try
            {
                if (production_execution_detail != null && production_execution_detail.Count > 0)
                {
                    using (db db = new db())
                    {
                        Unit_Cost_exe = db.production_execution_detail.Where(y => y.id_order_detail == id_order_detail).Sum(x => x.unit_cost);
                        RaisePropertyChanged("Unit_Cost_exe");
                    }
                }
            }
            catch { }
        }

        #endregion Methods
    }
}