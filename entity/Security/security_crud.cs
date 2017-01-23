
namespace entity
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class security_crud : INotifyPropertyChanged
    {
        public security_crud()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_crud { get; set; }
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

        [NotMapped]
        public bool all
        {
            get { return _all; }
            set
            {
                if (value != _all)
                {
                    _all = value;

                    if (_all == false)
                    {
                        can_read = false;
                        RaisePropertyChanged("can_read");

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
                    else
                    {
                        can_read = true;
                        RaisePropertyChanged("can_read");

                        can_create = true;
                        RaisePropertyChanged("can_create");

                        can_update = true;
                        RaisePropertyChanged("can_update");

                        can_delete = true;
                        RaisePropertyChanged("can_delete");

                        can_approve = true;
                        RaisePropertyChanged("can_approve");

                        can_annul = true;
                        RaisePropertyChanged("can_annul");
                    }
                }
            }
        }
        private bool _all;

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
                }
            }
        }
        bool _can_read = false;

        public bool can_update { get; set; }
        public bool can_delete { get; set; }

        public bool can_approve { get; set; }
        public bool can_annul
        {
            get
            {
                return _can_annul;
            }
            set
            {
                if (value != _can_annul)
                {
                    _can_annul = value;
                    RaisePropertyChanged("can_annul");
                }
            }
        }
        bool _can_annul = false;

        public virtual security_role security_role { get; set; }
    }
}
