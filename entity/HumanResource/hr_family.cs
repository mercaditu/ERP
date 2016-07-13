namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class hr_family : Audit
    {
       
        public enum Relationship
        {
            Child,
            Parent,
            Sibiling,
            GrandParent,
            Partner
        }

        public hr_family()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            date_birth = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_family { get; set; }
        public int id_contact { get; set; }

        [Required]
        public string name { get; set; }
        public Relationship relationship { get; set; }
        public DateTime date_birth
        {
            get { return _date_birth; }
            set
            {
                _date_birth = value;
                RaisePropertyChanged("date_birth");

                Age = DateTime.Today.Year - date_birth.Year;
                if (date_birth > DateTime.Today.AddYears(-Age)) Age--;
                RaisePropertyChanged("Age");
            }
        }
        private DateTime _date_birth;

        public string telephone_emergency { get; set; }

        [NotMapped]
        public int Age { get; set; }

        public virtual contact contact { get; set; }
    }
}
