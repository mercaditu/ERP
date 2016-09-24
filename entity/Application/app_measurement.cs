namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_measurement : AuditGeneric, IDataErrorInfo
    {
        public app_measurement()
        {
            id_company = entity.Properties.Settings.Default.company_ID;
            is_active = true;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            item_conversion_factor = new List<item_conversion_factor>();
            item_dimension = new List<item_dimension>();
            item_movement_dimension = new List<item_movement_dimension>();
            item_inventory_dimension = new List<item_inventory_dimension>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_measurement { get; set; }
        [CustomValidation(typeof(entity.Class.EntityValidation), "CheckId")]
        public short id_measurement_type { get; set; }
        [Required]
        public string name { get; set; }
        public string code_iso { get; set; }
        public bool is_active
        {
            get { return _is_active; }
            set
            {
                if (_is_active != value)
                {
                    _is_active = value;
                    RaisePropertyChanged("is_active");
                }
            }
        }
        private bool _is_active;
        public virtual app_measurement_type app_measurement_type { get; set; }
        public virtual IEnumerable<project_task_dimension> project_task_dimension { get; set; }
        public virtual ICollection<item_dimension> item_dimension { get; set; }
        public virtual ICollection<item_conversion_factor> item_conversion_factor { get; set; }
        public virtual ICollection<item_request_dimension> item_request_dimension { get; set; }
        public virtual ICollection<purchase_tender_dimension> purchase_tender_dimension { get; set; }
        public virtual ICollection<item_movement_dimension> item_movement_dimension { get; set; }
        public virtual ICollection<item_inventory_dimension> item_inventory_dimension { get; set; }

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
                    if (string.IsNullOrEmpty(name))
                        return "Name needs to be filled";
                }
                if (columnName == "id_measurement_type")
                {
                    if (id_measurement_type == 0)
                        return "Measurement type needs to be selected";
                }
                return "";
            }
        }
    }
}
