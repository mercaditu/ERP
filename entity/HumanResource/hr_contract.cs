namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class hr_contract : Audit
    {
        public hr_contract()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            is_active = true;

            start_date = DateTime.Now;
            end_date = DateTime.Now.AddYears(1);
        }

        public enum WorkTypes
        {
            Monthly,
            Daily
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_hr_contract { get; set; }

        public int id_contact { get; set; }
        public int? id_branch { get; set; }
        public int? id_department { get; set; }
        public int? id_currency { get; set; }

        public decimal base_salary
        {
            get { return _base_salary; }
            set
            {
                if (_base_salary != value)
                {
                    _base_salary = value;
                    RaisePropertyChanged("base_salary");
                    RaisePropertyChanged("Hourly");
                    RaisePropertyChanged("Daily");
                }
            }
        }

        private decimal _base_salary;

        [NotMapped]
        public decimal Hourly
        {
            get
            {
                if (base_salary > 0)
                {
                    return base_salary / 184;
                }

                return 0;
            }
        }

        [NotMapped]
        public decimal Daily
        {
            get
            {
                if (base_salary > 0)
                {
                    return base_salary / 30;
                }

                return 0;
            }
        }

        public string codigo { get { return _codigo; } set { _codigo = value; RaisePropertyChanged("codigo"); } }
        private string _codigo;

        public int? id_position { get; set; }

        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public DateTime end_trial_period { get; set; }
        public string comment { get; set; }
        public WorkTypes work_type { get; set; }

        [Required]
        public bool is_active { get; set; }

        public virtual hr_position hr_position { get; set; }
        public virtual contact contact { get; set; }
        public virtual app_branch app_branch { get; set; }
        public virtual app_department app_department { get; set; }
        public virtual app_currency app_currency { get; set; }
    }
}