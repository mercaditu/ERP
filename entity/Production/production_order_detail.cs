namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Threading;

    public partial class production_order_detail : Audit
    {
        Project.clsproject objclsproject = new Project.clsproject();
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


            //System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        CalcExecutedQty_TimerTaks();
            //        Thread.Sleep(5000);
            //    }
            //});
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_order_detail { get; set; }
        public int id_production_order { get; set; }
        public int? id_project_task { get; set; }
        public int? id_item { get; set; }
        public int? movement_id { get; set; }
        public string name { get; set; }
        public decimal quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    RaisePropertyChanged("quantity");


                    if (production_order!=null)
                    {
                        if (parent != null && parent.item != null && production_order.types != entity.production_order.ProductionOrderTypes.Fraction)
                        {
                            if (!parent.item.is_autorecepie)
                            {
                                parent.quantity = objclsproject.getsumquantityProduction(parent.id_order_detail, parent.child);
                                parent.RaisePropertyChanged("quantity");
                            }


                        }

                    }
                  
                    if (this.item != null)
                    {


                        if (this.item.is_autorecepie)
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

   

        public Status.Production? status { get; set; }
        public string code
        {
            get { return _code; }
            set { _code = value; RaisePropertyChanged("code"); }
        }
        private string _code;

        //[NotMapped]
        //public bool TrickleDown_IsSelected 
        //{
        //    get { return _TrickleDown_IsSelected; }
        //    set { _TrickleDown_IsSelected = value; }
        //}
        //private bool _TrickleDown_IsSelected = true;

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

                    //if (parent != null)
                    //{
                    //    parent.IsSelected = value;
                    //    parent.RaisePropertyChanged("IsSelected");
                    //}

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
                if (item_request_detail!=null)
                {
                    if (item_request_detail.Count()>0)
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
            set { _is_request=value; }
        }
        bool _is_request;

        [Required]
        public DateTime trans_date { get; set; }

        public DateTime? start_date_est { get; set; }
        public DateTime? end_date_est { get; set; }

        //Self Referencing
        public virtual production_order_detail parent { get; set; }
        public virtual ICollection<production_order_detail> child { get; set; }
        public virtual ICollection<production_order_dimension> production_order_dimension { get; set; }
        public virtual ICollection<production_execution_detail> production_execution_detail { get; set; }
        public virtual ICollection<item_request_detail> item_request_detail { get; set; }
        public virtual production_order production_order { get; set; }
        public virtual project_task project_task { get; set; }
        public virtual item item { get; set; }

        #region Methods

        public void ApproveOnlyParent_NotChild()
        {

        }

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

        #endregion
    }
}
