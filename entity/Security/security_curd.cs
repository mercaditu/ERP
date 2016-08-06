
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
        public bool can_create
        {
            get
            {
                return _can_create;
            }
            set
            {
                if (value != _can_create)
                {
                    _can_create = value;
                    RaisePropertyChanged("can_create");
                }
            }
        }
        bool _can_create = false;

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

                    if (_can_read == false)
                    {
                        can_create = false;
                        RaisePropertyChanged("can_create");

                        can_update = false;
                        RaisePropertyChanged("can_update");

                        can_delete = false;
                        RaisePropertyChanged("can_delete");

                        can_approve = false;
                        RaisePropertyChanged("can_approve");

                        can_annul = false;
                        RaisePropertyChanged("can_annul");
                    }
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
