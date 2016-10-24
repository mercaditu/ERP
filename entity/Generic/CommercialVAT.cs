namespace entity
{
    using System.ComponentModel;

    public partial class CommercialVAT : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public app_vat app_vat { get; set; }
        public decimal value { get; set; }
    }
}
