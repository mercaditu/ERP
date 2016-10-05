namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;
    using Class;

    public partial class item : AuditGeneric, IDataErrorInfo
    {
        [TypeConverter(typeof(EnumDescriptionTypeConverter))]
        public enum item_type
        {
            [LocalizedDescription("Product")]
            Product = 1,
            [LocalizedDescription("RawMaterial")]
            RawMaterial = 2,
            [LocalizedDescription("Service")]
            Service = 3,
            [LocalizedDescription("FixedAssets")]
            FixedAssets = 4,
            [LocalizedDescription("Task")]
            Task = 5,
            [LocalizedDescription("Supplies")]
            Supplies = 6,
            [LocalizedDescription("ServiceContract")]
            ServiceContract = 7
        }

        public item()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;

            is_head = true;
            is_active = true;
            is_autorecepie = false;

            unit_cost = 0;

            item_price = new List<item_price>();
            item_attachment = new List<item_attachment>();
            item_tag_detail = new List<item_tag_detail>();
            item_product = new List<item_product>();
            item_service = new List<item_service>();
            item_asset = new List<item_asset>();
            item_property = new List<item_property>();
            item_dimension = new List<item_dimension>();
            project_task = new List<project_task>();
            item_recepie = new List<item_recepie>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item { get; set; }
     
        [Required]
        [CustomValidation(typeof(EntityValidation), "CheckId")]
        public item_type id_item_type { get; set; }

        [Required]
        [CustomValidation(typeof(EntityValidation), "CheckId")]
        public int id_vat_group
        {
            get { return _id_vat_group; }
            set
            {
                if (_id_vat_group != value)
                {
                    _id_vat_group = value;
                    if (item_price != null)
                    {
                        foreach (item_price _item_price in item_price)
                        {
                            _item_price.return_ValueWithVAT(_id_vat_group);
                        }
                    }
                }
            }
        }
        private int _id_vat_group;

        public int? id_brand { get; set; }
        public int? id_measurement { get; set; }
        public short? id_name_template { get; set; }

        [Required]
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("name");
                if (code=="" || code==null)
                {
                    _code = value;
                    RaisePropertyChanged("code");
                }
            }
        }
        string _name;
        [Required]
        public string code { get { return _code; } set { _code = value; RaisePropertyChanged("code"); } }
        string _code;
        public string variation { get; set; }
        public string description { get; set; }
        public decimal? unit_cost { get; set; }
        public bool is_autorecepie { get; set; }
        public bool is_active { get; set; }

        [NotMapped]
        public bool is_shared
        {
            get
            {
                if (id_company == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value == true)
                {
                    id_company = null;
                    this.app_company = null;
                }
                else
                {
                    id_company = CurrentSession.Id_Company;
                }
            }
        }

        public virtual ICollection<item_price> item_price { get; set; }
        public virtual ICollection<contact_subscription> contact_subscription { get; set; }
        public virtual ICollection<item_attachment> item_attachment { get; set; }
        public virtual ICollection<item_tag_detail> item_tag_detail { get; set; }
        public virtual IEnumerable<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual IEnumerable<sales_order_detail> sales_order_detail { get; set; }
        public virtual IEnumerable<purchase_invoice_detail> purchase_invoice_detail { get; set; }
        public virtual IEnumerable<purchase_order_detail> purchase_order_detail { get; set; }
        public virtual IEnumerable<purchase_return_detail> purchase_return_detail { get; set; }
        public virtual IEnumerable<purchase_tender_item> purchase_tender_item_detail { get; set; }
        public virtual IEnumerable<purchase_packing_detail> purchase_packing_detail { get; set; }
        public virtual IEnumerable<item_asset_maintainance_detail> item_asset_maintainance_detail { get; set; }
        public virtual IEnumerable<sales_return_detail> sales_return_detail { get; set; }
        public virtual IEnumerable<sales_packing_detail> sales_packing_detail { get; set; }
        public virtual IEnumerable<sales_budget_detail> sales_budget_detail { get; set; }

        public virtual ICollection<project_task> project_task { get; set; }
        public virtual IEnumerable<project_template_detail> project_template_detail { get; set; }
        public virtual IEnumerable<production_order_detail> production_order_detail { get; set; }
        public virtual IEnumerable<production_execution_detail> production_execution_detail { get; set; }
        public virtual IEnumerable<production_account> production_account { get; set; }

        public virtual IEnumerable<item_request> item_request { get; set; }

        public virtual ICollection<project_event> project_event { get; set; }
        public virtual ICollection<project_event_fixed> project_event_fixed { get; set; }
        public virtual ICollection<project_event_variable> project_event_variable { get; set; }

        public virtual ICollection<item_product> item_product { get; set; }
        public virtual ICollection<item_service> item_service { get; set; }
        public virtual ICollection<item_asset> item_asset { get; set; }
        public virtual ICollection<item_property> item_property { get; set; }
        public virtual ICollection<item_dimension> item_dimension { get; set; }
        public virtual ICollection<item_recepie_detail> item_recepie_detail { get; set; }
        public virtual ICollection<item_recepie> item_recepie { get; set; }
        public virtual item_brand item_brand { get; set; }
        public virtual app_vat_group app_vat_group { get; set; }
        public virtual app_measurement app_measurement { get; set; }

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
                    string propertyError = this[prop.Name];
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
                    if (string.IsNullOrEmpty(name))
                        return "Name needs to be filled";
                }
                if (columnName == "code")
                {
                    if (string.IsNullOrEmpty(code))
                    {
                        return "Code needs to be filled";
                    }
                    else if (State == System.Data.Entity.EntityState.Added || State == System.Data.Entity.EntityState.Modified)
                    {
                        using (db db = new db())
                        {
                            if (db.items.Any(x => x.code == code && x.id_item != id_item))
                            {
                               return "Duplicate Code Not Allowed"; 
                            }
                        }
                    }
                }
                if (columnName == "id_item_type")
                {
                    if (id_item_type == 0)
                        return "Item type needs to be selected";
                }
                if (columnName == "id_vat_group")
                {
                    if (id_vat_group == 0)
                        return "Default Vat  needs to be selected";
                }
                return "";
            }
        }
    }
}
