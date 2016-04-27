namespace entity
{
    using entity.Brillo;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class project_task : Audit
    {
        Project.clsproject objclsproject = new Project.clsproject();

        public project_task()
        {
            project_task_dimension = new List<project_task_dimension>();
            sales_order_detail = new List<sales_order_detail>();
            production_execution_detail = new List<production_execution_detail>();
            production_order_detail = new List<production_order_detail>();
            sales_order_detail = new List<sales_order_detail>();
            trans_date = DateTime.Now;
            child = new List<project_task>();
            is_active = true;
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_project_task { get; set; }
        public int id_project { get; set; }

        public Status.Project? status { get; set; }

        public int? id_item
        {
            get { return _id_item; }
            set
            {
                if (_id_item != value)
                {
                    _id_item = value;

                    if (project != null)
                    {

                        if (project.CurrecyFx_ID != null)
                        {
                            _unit_price_vat = get_SalesPrice((int)id_item, project.contact, (int)project.CurrecyFx_ID);
                            RaisePropertyChanged("unit_price_vat");
                        }

                    }

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

               

                if (parent != null && parent.items != null)
                {
                    if (!parent.items.is_autorecepie)
                    {
                        parent.quantity_est = objclsproject.getsumquantity(parent.id_project_task, parent.child);
                        parent.RaisePropertyChanged("quantity_est");
                    }
                }

                if (this.items != null)
                {


                    if (this.items.is_autorecepie)
                    {

                        if (child.Count > 0)
                        {
                            foreach (project_task project_task in child)
                            {
                                project_task.quantity_est = project_task.items.item_recepie_detail.FirstOrDefault().quantity * this.quantity_est;
                                project_task.RaisePropertyChanged("quantity_est");



                            }

                        }


                    }
                }

            }
        }
        private decimal? _quantity_est;

        [NotMapped]
        public decimal? quantity_exec{get;set;}
      
        public decimal? unit_cost_est
        {
            get { return _unit_price_vat; }
            set
            {
                _unit_price_vat = value;
                RaisePropertyChanged("unit_cost_est");

                //if (parent != null)
                //{
                //    parent.unit_cost_est = objclsproject.getsumunitcost(parent.id_project_task, parent.child);
                //    parent.RaisePropertyChanged("unit_cost_est");
                //}

            }
        }
        private decimal? _unit_cost_est;

        [NotMapped]
        public decimal? unit_price_vat
        {
            get { return _unit_price_vat; }
            set
            {

                _unit_price_vat = value;
                RaisePropertyChanged("unit_price_vat");



            }
        }
        private decimal? _unit_price_vat;
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
                        if (task.status != Status.Project.Rejected)
                            task.IsSelected = value;
                    }
                    if (project != null)
                    {
                        project.Update_SelectedCount();
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
                if (value != null)
                {
                    _items = value;
                    RaisePropertyChanged("items");
                    _item_description = items.name;
                    RaisePropertyChanged("item_description");
                }

            }
        }

        item _items;


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

                    if (State == System.Data.Entity.EntityState.Added || State == System.Data.Entity.EntityState.Modified || State == 0)
                    {
                        using (db db = new db())
                        {
                            if (db.app_document_range.Where(x => x.id_range == _id_range).FirstOrDefault() != null)
                            {
                                app_document_range _app_range = db.app_document_range.Where(x => x.id_range == _id_range).FirstOrDefault();
                                if (project != null)
                                {


                                    if (db.app_branch.Where(x => x.id_branch == project.id_branch).FirstOrDefault() != null)
                                    {
                                        Brillo.Logic.Range.branch_Code = db.app_branch.Where(x => x.id_branch == project.id_branch).FirstOrDefault().code;
                                    }

                                    if (db.security_user.Where(x => x.id_user == id_user).FirstOrDefault() != null)
                                    {
                                        Brillo.Logic.Range.user_Code = db.security_user.Where(x => x.id_user == id_user).FirstOrDefault().code;
                                    }
                                    if (db.projects.Where(x => x.id_project == id_project).FirstOrDefault() != null)
                                    {
                                        Brillo.Logic.Range.project_Code = db.projects.Where(x => x.id_project == id_project).FirstOrDefault().code;
                                    }
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


        /// <summary>
        /// 
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string NumberWatermark { get; set; }

        #region Document Range => Navigation
        public virtual app_document_range app_document_range { get; set; }
        #endregion

        public virtual ICollection<project_task_dimension> project_task_dimension { get; set; }
        public virtual ICollection<production_order_detail> production_order_detail { get; set; }
        public virtual ICollection<production_execution_detail> production_execution_detail
        {
            get
            {
               
               
                return _production_execustion_detail; 
            }
            set
            {
                _production_execustion_detail = value;
                
              
            }
        }
        ICollection<production_execution_detail> _production_execustion_detail;
        public virtual IEnumerable<item_request_detail> item_request_detail { get; set; }
        public virtual ICollection<sales_budget_detail> sales_budget_detail { get; set; }
        public virtual IEnumerable<purchase_order_detail> purchase_order_detail { get; set; }
        public virtual ICollection<purchase_invoice_detail> purchase_invoice_detail { get; set; }
        public virtual ICollection<sales_order_detail> sales_order_detail { get; set; }
        public virtual ICollection<purchase_tender_item> purchase_tender_item { get; set; }
        public virtual sales_order_detail sales_detail { get; set; }
        public virtual ICollection<sales_invoice_detail> sales_invoice_detail { get; set; }
        public decimal get_SalesPrice(int id_item, contact Contact, int CurrencyFX_ID)
        {
            int PriceList_ID = 0;
            if (id_item > 0)
            {
                if (Contact != null)
                {
                    if (Contact.id_price_list != null)
                    {
                        PriceList_ID = (int)Contact.id_price_list;
                    }
                    else
                    {
                        PriceList_ID = 0;
                    }
                }

                //Step 1. If 'PriceList_ID' is 0, Get Default PriceList.
                if (PriceList_ID == 0 && PriceList_ID != null)
                {
                    using (db db = new db())
                    {
                        if (db.item_price_list.Where(x => x.is_active == true && x.id_company == Properties.Settings.Default.company_ID) != null)
                        {
                            PriceList_ID = db.item_price_list.Where(x => x.is_active == true && x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_price_list;
                        }
                    }
                }

                //Step 1 1/2. Check if Quantity gets us a better Price List.


                //Step 2. Get Price in Currency.
                using (db db = new db())
                {
                    app_currencyfx app_currencyfx = null;
                    if (db.app_currencyfx.Where(x => x.id_currencyfx == CurrencyFX_ID).FirstOrDefault() != null)
                    {
                        app_currencyfx = db.app_currencyfx.Where(x => x.id_currencyfx == CurrencyFX_ID).FirstOrDefault();

                        //Check if we have available Price for this Product, Currency, and List.
                        item_price item_price = db.item_price.Where(x => x.id_item == id_item
                                                                 && x.id_currency == app_currencyfx.id_currency
                                                                 && x.id_price_list == PriceList_ID)
                                                                 .FirstOrDefault();

                        if (item_price != null)
                        {   //Return Perfect Value
                            return item_price.value;
                        }
                        else
                        {
                            //If Perfect Value not found, get one pased on Product and List. (Ignore Currency and Convert Later basd on Current Rate.)
                            if (db.item_price.Where(x => x.id_item == id_item && x.id_price_list == PriceList_ID).FirstOrDefault() != null)
                            {
                                item_price = db.item_price.Where(x => x.id_item == id_item && x.id_price_list == PriceList_ID).FirstOrDefault();
                                app_currencyfx = db.app_currencyfx.Where(x => x.id_currency == item_price.id_currency && x.is_active == true).FirstOrDefault();
                                return Currency.convert_BackValue(item_price.value, app_currencyfx.id_currencyfx, App.Modules.Sales);
                            }


                        }
                    }

                }
            }

            return 0;
        }
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
                if (value != null)
                {
                    _parent = value;
                    RaisePropertyChanged("parent");
                    if (parent != null && parent.items != null)
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
