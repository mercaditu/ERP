namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Audit : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public int id_company { get; set; }
        public int id_user { get; set; }
        public bool is_head { get; set; }

        public DateTime timestamp
        {
            get
            {
                return _timestamp;
            }
            set
            {
                if (_timestamp != value)
                {
                    _timestamp = value;
                }
            }
        }

        public DateTime _timestamp;

        public bool is_read { get; set; }

        //Data
        [NotMapped]
        public bool HasErrors { get; set; }

        [NotMapped]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (value != _IsSelected)
                {
                    _IsSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        private bool _IsSelected;

        [NotMapped]
        public System.Data.Entity.EntityState State
        {
            get
            {
                return _State;
            }
            set
            {
                if (value != _State)
                {
                    _State = value;
                    RaisePropertyChanged("State");
                }
            }
        }

        private System.Data.Entity.EntityState _State;

        public virtual app_company app_company { get; set; }
        public virtual security_user security_user { get; set; }
    }
}