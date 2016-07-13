namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class hr_contract : Audit
    {
        public hr_contract()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            is_active = true;

            start_date = DateTime.Now;
            end_date = DateTime.Now.AddYears(1);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_hr_contract { get; set; }
        public int id_contact { get; set; }
        public int? id_branch { get; set; }
        public int? id_department { get; set; }
        public int? id_currency { get; set; }
        public decimal base_salary { get; set; }

        public string codigo { get { return _codigo; } set { _codigo = value; RaisePropertyChanged("codigo"); } }
       public  string _codigo;
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public DateTime end_trial_period { get; set; }
        public string comment { get; set; }

        [Required]
        public bool is_active { get; set; }

        public virtual contact contact { get; set; }
        public virtual app_branch app_branch { get; set; }
        public virtual app_department app_department { get; set; }
        public virtual app_currency app_currency { get; set; }
    }
}
