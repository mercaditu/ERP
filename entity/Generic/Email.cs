namespace entity
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Email : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        [NotMapped]
        public bool IsSelected { get; set; }

        public string email_imap { get; set; }
        public string email_smtp { get; set; }
        public short email_port_out { get; set; }
        public short email_port_in { get; set; }
    }
}
