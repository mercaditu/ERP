namespace entity
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CommercialVAT : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public app_vat app_vat { get; set; }
        public decimal value { get; set; }
    }
}
