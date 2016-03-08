namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;

    public partial class project_task : Audit
    {
        Project.clsproject objclsproject = new Project.clsproject();
        db db = new db();
        public project_task()
        {
            project_task_dimension = new List<project_task_dimension>();
           
            trans_date = DateTime.Now;
            child= new List<project_task>();
            is_active = true;
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_project_task { get; set; }
        public int id_project { get; set; }

     
        public Status.Project? status { get; set; }
        public Status.ProjectStatus? ProjectStatus { get; set; }
        public int? id_item
        {
            get { return _id_item; }
            set
            {
                if (_id_item != value)
                {
                    _id_item = value;
                 

                    
                }
            }
        }
        private int? _id_item;

        public string item_description
        {
            get { return _item_description; }
            set { _item_description = value; RaisePropertyChanged("item_description"); }
        }
        private string _item_description;

        public string code { get { return _code; } set { _code = value; RaisePropertyChanged("code"); } }
        private string _code;

        public decimal? quantity_est
        {
            get { return _quantity_est; }
            set
            {
                _quantity_est = value;
                RaisePropertyChanged("quantity_est");

                if (parent != null && parent.items!=null)
                {
                    if (!parent.items.is_autorecepie )
                    {
                        parent.quantity_est = objclsproject.getsumquantity(parent.id_project_task, parent.child);
                        parent.RaisePropertyChanged("quantity_est");
                    }
                  
                }

            }
        }
        private decimal? _quantity_est;
        public decimal? unit_cost_est
        {
            get { return _unit_cost_est; }
            set
            {
                _unit_cost_est = value;
                RaisePropertyChanged("unit_cost_est");

                if (parent != null)
                {
                    parent.unit_cost_est = objclsproject.getsumunitcost(parent.id_project_task, parent.child);
                    parent.RaisePropertyChanged("unit_cost_est");
                }

            }
        }
        private decimal? _unit_cost_est;
        public DateTime? start_date_est { get; set; }
        public DateTime? end_date_est { get; set; }
        public DateTime? trans_date { get; set; }
        public bool is_active { get; set; }

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

                    foreach (var task in child)
                    {
                        if (task.status != Status.Project.Rejected )
                            task.IsSelected = value;
                        
                    }
                }
            }
        }
        private bool _is_selected;
        //[NotMapped]
        //public bool IsChecked;
        public virtual project project { get; set; }
        public virtual item items
        {
            get { return _items; }
            set
            {
                if (value!=null)
                {
                    _items = value;
                    RaisePropertyChanged("items");
                    _item_description = items.name;
                    RaisePropertyChanged("item_description");
                }
              
            }
        }
        item _items;
        public virtual ICollection<project_task_dimension> project_task_dimension { get; set; }
        public virtual IEnumerable<item_request_detail> item_request_detail { get; set; }
        public virtual ICollection<sales_budget_detail> sales_budget_detail { get; set; }
        public virtual IEnumerable<purchase_order_detail> purchase_order_detail { get; set; }
        public virtual ICollection<purchase_invoice_detail> purchase_invoice_detail { get; set; }
        public virtual ICollection<sales_order_detail> sales_order_detail { get; set; }
        public virtual sales_order_detail sales_detail { get; set; }
        public virtual ICollection<sales_invoice_detail> sales_invoice_detail { get; set; }

        public virtual ICollection<project_task> child
        {
            get { return _child; }
            set
            {
                _child = value; 
            }
        }
        private ICollection<project_task> _child;
        //TreeView Heirarchy Fields
        public virtual project_task parent
        {
            get { return _parent; }
            set
            {
                if (value!=null)
                {
                    _parent = value;
                    RaisePropertyChanged("parent");
                    if (parent != null && parent.items!=null)
                    {
                        if (!parent.items.is_autorecepie)
                        {


                            parent.quantity_est = objclsproject.getsumquantity(parent.id_project_task, parent.child);
                            parent.quantity_est += quantity_est;
                            parent.RaisePropertyChanged("quantity_est");
                        }
                    }
                }
            
            }
        }
        private project_task _parent;
    }
}
