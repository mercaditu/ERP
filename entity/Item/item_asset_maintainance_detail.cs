namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class item_asset_maintainance_detail : Audit
    {
        public item_asset_maintainance_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            item_request_detail = new List<item_request_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_maintainance_detail { get; set; }

        public int id_maintainance { get; set; }

        public int? id_item { get; set; }
        public string item_description { get; set; }
        public decimal quantity { get; set; }
        public decimal unit_cost { get; set; }
        public int? id_currencyfx { get; set; }
        public int? id_time_coefficient { get; set; }
        public int? id_contact { get; set; }

        public DateTime start_date
        {
            get { return _start_date; }
            set
            {
                if (value != _start_date)
                {
                    _start_date = value;
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

                    _hour = (decimal)time.TotalMinutes / 60;
                    RaisePropertyChanged("hours");
                }
            }
        }

        private DateTime _end_date = DateTime.Now;

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

                    if (item_asset_maintainance != null)
                    {
                        item_asset_maintainance.Update_SelectedCount();
                    }
                }
            }
        }

        private bool _is_selected;

        //Nav Properties
        public virtual item_asset_maintainance item_asset_maintainance { get; set; }

        public virtual item item { get; set; }
        public virtual app_currencyfx app_currencyfx { get; set; }
        public virtual hr_time_coefficient hr_time_coefficient { get; set; }
        public virtual contact contact { get; set; }
        public virtual ICollection<item_request_detail> item_request_detail { get; set; }
    }
}