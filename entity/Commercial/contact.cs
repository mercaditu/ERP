namespace entity
{
    using entity.Class;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;

    public partial class contact : AuditGeneric, IDataErrorInfo, INotifyPropertyChanged
    {
        [TypeConverter(typeof(EnumDescriptionTypeConverter))]
        public enum CivilStatus
        {
            [LocalizedDescription("Soltero/a")]
            Single,

            [LocalizedDescription("Casado/a")]
            Married,

            [LocalizedDescription("Separado/a")]
            Seperated,

            [LocalizedDescription("Divorciado/a")]
            Divorced,

            [LocalizedDescription("Viudo/a")]
            Widowed
        }

        [TypeConverter(typeof(EnumDescriptionTypeConverter))]
        public enum Genders
        {
            [LocalizedDescription("Male")]
            Male,

            [LocalizedDescription("Female")]
            Female
        }

        public enum BloodTypes
        {
            [LocalizedDescription("O+")]
            Op,

            [LocalizedDescription("O-")]
            On,

            [LocalizedDescription("A+")]
            Ap,

            [LocalizedDescription("A-")]
            An,

            [LocalizedDescription("B+")]
            Bp,

            [LocalizedDescription("B-")]
            Bn,

            [LocalizedDescription("AB+")]
            ABp,

            [LocalizedDescription("AB-")]
            ABn,
        }

        public enum Methods
        {

        }

        public contact()
        {
            id_user = CurrentSession.Id_User;
            id_company = CurrentSession.Id_Company;
            is_active = true;
            timestamp = DateTime.Now;
            trans_code_exp = DateTime.Now;

            contact_tag_detail = new List<contact_tag_detail>();
            contact_subscription = new List<contact_subscription>();
            item_asset_maintainance_detail = new List<item_asset_maintainance_detail>();
            hr_contract = new List<hr_contract>();
            hr_education = new List<hr_education>();
            hr_timesheet = new List<hr_timesheet>();
            hr_family = new List<hr_family>();
            hr_talent_detail = new List<hr_talent_detail>();
            contact_field_value = new List<contact_field_value>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_contact { get; set; }

        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_contact_role { get; set; }

        public int? id_contract { get; set; }
        public int? id_currency { get; set; }
        public int? id_cost_center { get; set; }
        public int? id_price_list { get; set; }
        public int? id_sales_rep { get; set; }
        public int? id_geography { get; set; }
        public int? id_bank { get; set; }

        public Methods? method { get; set; }

        [Required]
        public string name { get; set; }

        [NotMapped]
        public string FirstName
        {
            get
            {
                if (name != null)
                {
                    if (name.Contains(","))
                    {
                        _FirstName = name.Substring(name.IndexOf(",") + 1);
                    }
                }

                return _FirstName;
            }
            set
            {
                _FirstName = value;
                _FirstName = _FirstName.Replace(",", "");
                name = _LastName + ", " + _FirstName;
            }
        }

        private string _FirstName;

        [NotMapped]
        public string LastName
        {
            get
            {
                if (name != null)
                {
                    if (name.Contains(","))
                    {
                        _LastName = name.Substring(0, name.IndexOf(",") + 1);
                    }
                }
                return _LastName;
            }
            set
            {
                _LastName = value;
                _LastName = _LastName.Replace(",", "");
                name = _LastName + ", " + _FirstName;
            }
        }

        private string _LastName;

        public string alias { get; set; }
        public string code { get; set; }
        public string gov_code { get; set; }
        public string trans_code { get; set; }
        public DateTime? trans_code_exp { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public decimal? credit_limit { get; set; }
        public int? lead_time { get; set; }
        public decimal? geo_lat { get; set; }
        public decimal? geo_long { get; set; }

       public string verification_code { get; set; }
        public string LongLat
        {
            get
            {
                if (string.IsNullOrEmpty(_LongLat))
                {
                    _LongLat = geo_long.ToString().Replace(",", ".") + "," + geo_lat.ToString().Replace(",", ".");
                }
                return _LongLat;
            }
            set
            {
                if (_LongLat != value)
                {
                    _LongLat = value;
                    RaisePropertyChanged("LongLat");

                    var items = _LongLat.Split(',');
                    geo_long = decimal.Parse(items[0].Trim(), CultureInfo.InvariantCulture);
                    geo_lat = decimal.Parse(items[1].Trim(), CultureInfo.InvariantCulture);
                }
            }
        }
        private string _LongLat;

        public bool is_customer
        {
            get { return _is_customer; }
            set
            {
                _is_customer = _is_customer != value ? value : _is_customer;
                RaisePropertyChanged("is_customer");
            }
        }

        private bool _is_customer;

        public bool is_supplier
        {
            get { return _is_supplier; }
            set
            {
                _is_supplier = _is_supplier != value ? value : _is_supplier;
                RaisePropertyChanged("is_supplier");
            }
        }

        private bool _is_supplier;

        public bool is_employee { get; set; }

        public bool is_sales_rep { get; set; }
        public bool is_active { get; set; }
        public string comment { get; set; }
        public bool is_person { get; set; }
        public DateTime? date_birth { get; set; }
        public Genders? gender { get; set; }
        public string social_code { get; set; }
        public BloodTypes? blood_type { get; set; }
        public CivilStatus? marital_status { get; set; }

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

        //Calculated Fields
        [NotMapped]
        public decimal? credit_availability { get; set; }

        [NotMapped]
        public decimal GrandTotal
        {
            get
            {
                _GrandTotal = 0;
                if (contact_subscription.Count() > 0)
                {
                    foreach (contact_subscription _contact_subscription in contact_subscription)
                    {
                        _GrandTotal += _contact_subscription.SubTotal_Vat;
                    }

                    if (child != null)
                    {
                        foreach (contact contact in child)
                        {
                            if (contact != null)
                            {
                                foreach (contact_subscription _contact_subscription in contact.contact_subscription)
                                {
                                    _GrandTotal += _contact_subscription.SubTotal_Vat;
                                }
                            }
                        }
                    }
                }

                return Math.Round(_GrandTotal, 2);
            }
            set
            {
                if (_GrandTotal != value)
                {
                    _GrandTotal = value;
                    RaisePropertyChanged("GrandTotal");
                }
            }
        }

        private decimal _GrandTotal;

        [NotMapped]
        public int? Age { get; set; }

        public virtual app_cost_center app_cost_center { get; set; }
        public virtual app_contract app_contract { get; set; }
        public virtual contact_role contact_role { get; set; }
        public virtual item_price_list item_price_list { get; set; }
        public virtual sales_rep sales_rep { get; set; }
        public virtual app_currency app_currency { get; set; }
        public virtual app_geography app_geography { get; set; }
        public virtual app_bank app_bank { get; set; }

        //Heirarchy Nav Properties
        public virtual ICollection<contact> child { get; set; }

        public virtual contact parent { get; set; }

        //Navigation Properties
        public virtual ICollection<contact_field_value> contact_field_value { get; set; }

        public virtual ICollection<contact_subscription> contact_subscription { get; set; }

        public virtual IEnumerable<item_brand> item_brand { get; set; }
        public virtual IEnumerable<item_transfer> item_transfer { get; set; }
        public virtual IEnumerable<purchase_tender_contact> purchase_tender_contact { get; set; }
        public virtual IEnumerable<purchase_order> purchase_order { get; set; }
        public virtual IEnumerable<purchase_invoice> purchase_invoice { get; set; }
        public virtual IEnumerable<purchase_return> purchase_return { get; set; }
        public virtual IEnumerable<purchase_packing> purchase_packing { get; set; }

        public virtual IEnumerable<app_location> app_location { get; set; }

        public virtual IEnumerable<sales_budget> sales_budget { get; set; }
        public virtual IEnumerable<sales_order> sales_order { get; set; }
        public virtual IEnumerable<sales_invoice> sales_invoice { get; set; }
        public virtual IEnumerable<sales_packing> sales_packing { get; set; }
        public virtual IEnumerable<sales_return> sales_return { get; set; }

        public virtual IEnumerable<payment_schedual> payment_schedual { get; set; }
        public virtual IEnumerable<payment_withholding_tax> payment_withholding_tax { get; set; }

        public virtual IEnumerable<project_event> project_costing { get; set; }
        public virtual IEnumerable<project> project { get; set; }
        public virtual ICollection<production_execution_detail> production_execution_detail { get; set; }
        public virtual IEnumerable<production_service_account> production_service_account { get; set; }
        public virtual IEnumerable<production_account> production_account { get; set; }

        public virtual ICollection<contact_tag_detail> contact_tag_detail { get; set; }
        public virtual ICollection<hr_contract> hr_contract { get; set; }
        public virtual ICollection<hr_education> hr_education { get; set; }
        public virtual ICollection<hr_family> hr_family { get; set; }
        public virtual ICollection<hr_talent_detail> hr_talent_detail { get; set; }
        public virtual IEnumerable<hr_timesheet> hr_timesheet { get; set; }
        public virtual ICollection<item_asset_maintainance_detail> item_asset_maintainance_detail { get; set; }
        public virtual ICollection<item_asset> item_asset { get; set; }
        public virtual ICollection<hr_position> hr_position { get; set; }

        #region Validation

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
                //if (columnName == "gov_code")
                //{
                //    if (String.IsNullOrEmpty(gov_code))
                //        return "Gov Code needs to be filled";
                //}
                if (columnName == "id_contact_role")
                {
                    if (id_contact_role == 0)
                        return "Contact role needs to be selected";
                }

                if (columnName == "code")
                {
                    if (string.IsNullOrEmpty(code) == false)
                    {
                        if (State == System.Data.Entity.EntityState.Added || State == System.Data.Entity.EntityState.Modified)
                        {
                            using (db db = new db())
                            {
                                if (db.contacts.Any(x => x.code == code && x.id_contact != id_contact))
                                {
                                    return "Duplicate Contact Code";
                                }
                            }
                        }
                        
                    }
                }
                return "";
            }
        }

        #endregion Validation

        public void Check_CreditAvailability()
        {
            if (credit_limit != null)
            {
                if (credit_limit > 0 && id_contact != 0)
                {
                    using (db db = new db())
                    {
                        decimal pending = db.payment_schedual
                                            .Where(x => x.id_contact == id_contact)
                                            .Sum(x => (decimal?)(x.debit - x.credit)) ?? 0;
                        credit_availability = credit_limit - pending;
                        RaisePropertyChanged("credit_availability");
                    }
                }
            }
        }
    }
}