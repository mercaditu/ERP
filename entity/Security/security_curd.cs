
namespace entity
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class security_curd : INotifyPropertyChanged
    {
        public security_curd()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_curd { get; set; }
        public int id_role { get; set; }
        public App.Names id_application { get; set; }
        public bool can_create { get; set; }
        public bool can_read { get; set; }
        public bool can_update { get; set; }
        public bool can_delete { get; set; }

        public bool can_approve { get; set; }
        public bool can_annul { get; set; }

        [NotMapped]
        public bool has_full_control
        {
            get
            {
                return _has_full_control;
            }
            set
            {
                if (value != _has_full_control)
                {
                    _has_full_control = value;
                    can_create = _has_full_control;
                    RaisePropertyChanged("can_create");
                    can_read = _has_full_control;
                    RaisePropertyChanged("can_read");
                    can_update = _has_full_control;
                    RaisePropertyChanged("can_update");
                    can_delete = _has_full_control;
                    RaisePropertyChanged("can_delete");

                    can_approve = _has_full_control;
                    RaisePropertyChanged("can_approve");
                    can_annul = _has_full_control;
                    RaisePropertyChanged("can_annul");
                }
            }
        }
        bool _has_full_control = false;

        public virtual security_role security_role { get; set; }
    }
}
