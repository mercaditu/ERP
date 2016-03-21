
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

        public bool can_read
        {
            get
            {
                return _can_read;
            }
            set
            {
                if (value != _can_read)
                {
                    _can_read = value;
                    RaisePropertyChanged("can_read");

                    can_create = _can_read;
                    RaisePropertyChanged("can_create");

                    can_update = _can_read;
                    RaisePropertyChanged("can_update");

                    can_delete = _can_read;
                    RaisePropertyChanged("can_delete");

                    can_approve = _can_read;
                    RaisePropertyChanged("can_approve");

                    can_annul = _can_read;
                    RaisePropertyChanged("can_annul");
                }
            }
        }
        bool _can_read = false;

        public bool can_update { get; set; }
        public bool can_delete { get; set; }

        public bool can_approve { get; set; }
        public bool can_annul { get; set; }

        public virtual security_role security_role { get; set; }
    }
}
