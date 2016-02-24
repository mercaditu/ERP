
namespace entity
{
    using entity.Brillo;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;

    public partial class payment_detail : Audit, IDataErrorInfo
    {
        public payment_detail()
        {
           
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
            trans_date = DateTime.Now;
            payment_schedual = new List<payment_schedual>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_payment_detail { get; set; }
        public int id_payment { get; set; }
        public int? id_sales_return { get; set; }
        public int? id_purchase_return { get; set; }
         
        public int? id_account { get; set; }
        [Required]
      
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
            }
        }
        int _id_currencyfx;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public App.Names App_Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_payment_type { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public short? payment_type_ref { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public decimal value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime trans_date { get; set; }

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

                    using (db db = new db())
                    {
                        if (db.app_document_range.Where(x => x.id_range == _id_range).FirstOrDefault() != null)
                        {
                            app_document_range _app_range = db.app_document_range.Where(x => x.id_range == _id_range).FirstOrDefault();
                            Brillo.Logic.Range.branch_Code = db.app_branch.Where(x => x.id_branch == payment.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = db.app_terminal.Where(x => x.id_terminal == payment.id_terminal).FirstOrDefault().code;
                            NumberWatermark = Brillo.Logic.Range.calc_Range(_app_range, true);
                            RaisePropertyChanged("NumberWatermark");
                        }
                    }
                }
            }
        }
        #region Document Range => Navigation & Variables
        private int? _id_range;
        public virtual app_document_range app_document_range { get; set; }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string payment_type_number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string NumberWatermark { get; set; }

        public virtual payment payment { get; set; }
        public virtual payment_type payment_type { get; set; }

        public virtual ICollection<payment_schedual> payment_schedual { get; set; }
        public virtual app_account app_account { get; set; }
        public virtual app_bank app_bank { get; set; }
        public virtual app_currencyfx app_currencyfx { get; set; }

        public virtual ICollection<payment_type_detail> payment_type_detail { get; set; }
        public virtual IEnumerable<app_account_detail> app_account_detail { get; set; }

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
                if (columnName == "value")
                {
                    if (value == 0)
                        return "Amount needs to be filled";
                }
          
               
                if (columnName == "id_payment_type")
                {
                    if (id_payment_type == 0)
                        return "Payment type needs to be selected";
                }
                return "";
            }
        }
    }
}
