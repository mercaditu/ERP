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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public int id_company { get; set; }
        public int id_user { get; set; }
        public bool is_head { get; set; }
        public DateTime timestamp { get; set; }

        //Data
        [NotMapped]
        public bool HasErrors { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
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
        System.Data.Entity.EntityState _State;
        
        public virtual app_company app_company { get; set; }
        public virtual security_user security_user { get; set; }
    }
}
