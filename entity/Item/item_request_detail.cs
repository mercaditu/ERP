namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public partial class item_request_detail : Audit, IDataErrorInfo
    {
        public enum Urgencies
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

        public item_request_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            date_needed_by = DateTime.Now;
            item_request_dimension = new List<item_request_dimension>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_item_request_detail { get; set; }

        public int id_item_request { get; set; }
        public int? id_project_task { get; set; }
        public int? id_sales_order_detail { get; set; }
        public int? id_order_detail { get; set; }
        public int? id_maintainance_detail { get; set; }

        public int id_item { get; set; }
        public decimal max_value { get; set; }
        public decimal quantity { get; set; }

        [NotMapped]
        public decimal Balance
        {
            get { return quantity - item_request_decision.Sum(x => x.quantity); }
            set { _Balance = value; }
        }

        private decimal _Balance;

        public DateTime date_needed_by { get; set; }
        public string comment { get; set; }

        public Urgencies urgency { get; set; }

        [NotMapped]
        public string DimensionString
        {
            get
            {
                string s = string.Empty;

                foreach (item_request_dimension dimensionList in item_request_dimension)
                {
                    if (dimensionList.app_dimension != null && dimensionList.app_measurement != null)
                    {
                        s = s + dimensionList.app_dimension.name + ": " + dimensionList.value + " x " + dimensionList.app_measurement.name;
                    }
                }

                return s;
            }
        }

        public virtual ICollection<item_request_decision> item_request_decision { get; set; }
        public virtual ICollection<item_request_dimension> item_request_dimension { get; set; }
        public virtual item_request item_request { get; set; }
        public virtual item item { get; set; }
        public virtual sales_order_detail sales_order_detail { get; set; }
        public virtual project_task project_task { get; set; }
        public virtual production_order_detail production_order_detail { get; set; }
        public virtual item_asset_maintainance_detail item_asset_maintainance_detail { get; set; }

        public int GetTotalDecision()
        {
            int i = 0;

            foreach (item_request_decision decision in item_request_decision)
            {
                i += 1;
            }

            return i;
        }

        #region Error

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
                //if (columnName == "qty")
                //{
                //    if (qty == 0)
                //        return "qty needs to be selected";
                //}

                //if (columnName == "value")
                //{
                //    if (value == 0)
                //        return "Value needs to be filled";
                //}
                return "";
            }
        }

        #endregion Error
    }
}