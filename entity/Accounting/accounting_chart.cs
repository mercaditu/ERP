namespace entity
{
    using entity.Class;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class accounting_chart : Audit, IDataErrorInfo
    {
        Project.clsproject objclsproject = new Project.clsproject();

        [TypeConverter(typeof(EnumDescriptionTypeConverter))]
        public enum ChartType
        {
            [LocalizedDescription("Assets")]
            Assets,
            [LocalizedDescription("Liabilities")]
            Liability,
            [LocalizedDescription("Equity")]
            Equity,
            [LocalizedDescription("Revenue")]
            Revenue,
            [LocalizedDescription("Expenses")]
            Expenses
        }

        //[TypeConverter(typeof(EnumDescriptionTypeConverter))]
        public enum ChartSubType
        {
            FixedAsset = 1,
            Cash = 2,
            Inventory = 3,
            AccountsReceivable = 4,
            //Liability
            AccountsPayable = 5,
            VAT = 6,
            BankLoan = 7,
            BankOverDraft = 8,
            Debentures = 9,
            //Equity
            Shares = 10,
            RetainedEarnings = 11,
            RevaluationSurplus = 12,
            //Income
            Revenue = 13,
            Gains = 14,
            //Expenses
            HumanResourceWage = 15,
            AdministrationExpense = 16,
            CostOfGoodsSold = 17,
            Depreciation = 18,
            ImpairmentLoss = 19
        }

        public accounting_chart()
        {

            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
            is_active = true;

            child = new List<accounting_chart>();
            accounting_journal_detail = new List<accounting_journal_detail>();

            if (parent != null)
            {
                chart_type = parent.chart_type;
                chartsub_type = parent.chartsub_type;
                code = parent.code + ".";
            }
            else
            {
                can_transact = false;
            }
            child_total = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_chart { get; set; }
        [Required]
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("name");
            }
        }
        private string _name;
        [Required]
        public string code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged("code");
            }
        }
        private string _code;

        //Clasification
        public ChartType chart_type
        {
            get { return _chart_type; }
            set
            {
                if (value != _chart_type)
                {
                    _chart_type = value;
                    change2_Null();
                    if (child != null)
                    {
                        foreach (accounting_chart chart in child)
                        {
                            chart.chart_type = value;
                        }
                    }
                   
                }
            }
        }
        public ChartType _chart_type;

        public ChartSubType chartsub_type
        {
            get { return _chartsub_type; }
            set
            {
                if (value != _chartsub_type)
                {
                    _chartsub_type = value;
                    change2_Null();
                    if (child != null)
                    {
                        foreach (accounting_chart chart in child)
                        {
                            chart.chartsub_type = value;
                        }
                    }
                  
                }
            }
        }
        public ChartSubType _chartsub_type;
  
        //Relation FK
        int? _id_account;
        public int? id_account 
        { 
            get 
            {
                return _id_account ;
            } 
            set 
            {
                _id_account=value;
            } 
        }
        int? _id_contact;
        public int? id_contact
        {
            get
            {
                return _id_contact;
            }
            set
            {
                _id_contact = value;
             
            }
        }
        int? _id_tag;
        public int? id_tag
        {
            get
            {
                return _id_tag;
            }
            set
            {
                    _id_tag = value;
               
            }
        }
        int? _id_item_asset_group;
        public int? id_item_asset_group
        {
            get
            {
                return _id_item_asset_group;
            }
            set
            {
             
                _id_item_asset_group =value;
            
            }
        }
        int? _id_vat;
        public int? id_vat
        {
            get
            {
                return _id_vat;
            }
            set
            {
              
                _id_vat =value;
                
            }
        }
        int? _id_department;
        public int? id_department
        {
            get
            {
                return _id_department;
            }
            set
            {
               
                _id_department = value;
               
            }
        }

        int? _id_cost_center;
        public int? id_cost_center
        {
            get
            {
                return _id_cost_center;
            }
            set
            {
                _id_cost_center = value;
            }
        }

        public bool is_generic
        {
            get
            {
                return _is_generic;
            }
            set
            {
                _is_generic = value;
                if (_is_generic==true)
                {
                    change2_Null();
                }
               // 
            }
        }
        private bool _is_generic; 

        //Properties
        public bool is_active { get; set; }
        public bool can_transact { get; set; }

        [NotMapped]
        public decimal? child_total
        {
            get
            {
                if (child.Count == 0)
                {
                    _child_total = objclsproject.getsumAccounting_chart(this);
                }
                RaisePropertyChanged("child_total");

                return _child_total;
            }
            set
            {
                if (parent != null)
                {
                    parent.child_total += value;
                }
                _child_total = value;
            }
        }
        private decimal? _child_total;

        //Heirarchy Navigation Properties
        public virtual accounting_chart parent { get; set; }
        public virtual ICollection<accounting_chart> child { get; set; }

        //Navigation Properites
        public virtual ICollection<accounting_journal_detail> accounting_journal_detail { get; set; }
        public virtual IEnumerable<accounting_budget> accounting_budget { get; set; }

        public virtual app_account app_account { get; set; }
        public virtual contact contact { get; set; }
        public virtual item_tag item_tag { get; set; }
        public virtual item_asset_group item_asset_group { get; set; }
        public virtual app_vat app_vat { get; set; }
        public virtual app_department app_department { get; set; }
        public virtual app_cost_center app_cost_center { get; set; }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    String propertyError = this[prop.Name];
                    if (propertyError != string.Empty)
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                return error.Length == 0 ? null : error.ToString();
            }
        }
        public string this[string columnName]
        {
            get
            {
                // apply property level validation rules
                if (columnName == "name")
                {
                    if (String.IsNullOrEmpty(name))
                        return "Name needs to be filled";
                }
                if (columnName == "code")
                {
                    if (String.IsNullOrEmpty(code))
                        return "Code needs to be filled";
                }
                return "";
            }
        }


        private void change2_Null()
        {
            _id_contact = null;
            _id_tag = null;
            _id_item_asset_group = null;
            _id_vat = null;
            _id_department = null;
            _id_cost_center = null;
            _id_account = null;
            RaisePropertyChanged("id_contact");
            RaisePropertyChanged("id_tag");
            RaisePropertyChanged("id_item_asset_group");
            RaisePropertyChanged("id_department");
            RaisePropertyChanged("id_vat");
            RaisePropertyChanged("id_account");
            RaisePropertyChanged("id_cost_center ");
            RaisePropertyChanged("chart_type");
        }
    }
}
