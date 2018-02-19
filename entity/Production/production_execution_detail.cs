namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class production_execution_detail : Audit
    {
        public production_execution_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            trans_date = DateTime.Now;
            timestamp = DateTime.Now;
            is_input = false;
            child = new List<production_execution_detail>();
            production_execution_dimension = new List<production_execution_dimension>();
            start_date = DateTime.Now;
            end_date = DateTime.Now;
            item_movement = new List<item_movement>();
            item_movement_archive = new List<item_movement_archive>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_execution_detail { get; set; }

        public int? id_order_detail { get; set; }
        public int? id_project_task { get; set; }
        public int? id_service_account { get; set; }

        public int? id_time_coefficient { get; set; }
        public int? id_contact { get; set; }
        public int? id_item { get; set; }
        public int? movement_id { get; set; }
        public string name { get; set; }
        public Status.Production? status { get; set; }

        [Required]
        public decimal quantity { get; set; }

        public decimal? geo_lat { get; set; }
        public decimal? geo_long { get; set; }

        public DateTime start_date
        {
            get { return _start_date; }
            set
            {
                if (value != _start_date)
                {
                    _start_date = value;

                    if (_start_date > _end_date)
                    {
                        _end_date = _start_date;
                    }
                }
            }
        }

        private DateTime _start_date = DateTime.Now;

        public DateTime end_date
        {
            get
            {
                return _end_date;
            }
            set
            {
                if (value != _end_date)
                {
                    _end_date = value;
                    TimeSpan time = end_date.Subtract(start_date);

                    _hour = time.Hours + (Convert.ToDecimal(time.Minutes) / 60);
                    RaisePropertyChanged("hours");

                    if (id_time_coefficient > 0 && quantity == 0)
                    {
                        quantity = Convert.ToDecimal(time.Hours + (Convert.ToDecimal(time.Minutes) / 60));
                        RaisePropertyChanged("quantity");
                    }

                    if (_start_date > _end_date)
                    {
                        _end_date = _start_date;
                    }
                }
            }
        }

        private DateTime _end_date = DateTime.Now;

        public decimal unit_cost { get; set; }
        public bool is_accounted { get; set; }

        [NotMapped]
        public decimal hours
        {
            get
            {
                return _hour;
            }
            set
            {
                _hour = value;
            }
        }

        private decimal _hour;

        [Required]
        public bool is_input { get; set; }

        [Required]
        public DateTime trans_date { get; set; }

        public string batch { get; set; }
        public DateTime? expiry_date { get; set; }

     
        [NotMapped]
        public string DimensionString
        {
            get
            {
                string s = string.Empty;

                foreach (production_execution_dimension dimensionList in production_execution_dimension)
                {
                    if (dimensionList.app_dimension != null && dimensionList.app_measurement != null)
                    {
                        s = s + dimensionList.app_dimension.name + ": " + dimensionList.value + " x " + dimensionList.app_measurement.name;
                    }
                }

                return s;
            }
        }

        //Heirarchy
        public virtual production_execution_detail parent { get; set; }

        public virtual ICollection<production_execution_detail> child { get; set; }
        public virtual ICollection<production_execution_dimension> production_execution_dimension { get; set; }
        public virtual ICollection<production_account> production_account { get; set; }
        public virtual production_service_account production_service_account { get; set; }
        public virtual ICollection<item_movement> item_movement { get; set; }
        public virtual ICollection<item_movement_archive> item_movement_archive { get; set; }
        public virtual hr_time_coefficient hr_time_coefficient { get; set; }
        public virtual production_order_detail production_order_detail { get; set; }
        public virtual project_task project_task { get; set; }
        public virtual item item { get; set; }
        public virtual contact contact { get; set; }
    }
}