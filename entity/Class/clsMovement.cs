using System.ComponentModel;

namespace entity.Class
{
    public class clsMovement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

     
        public string item_code { get; set; }
        public string item_name { get; set; }
        public string lot_number { get; set; }
        public string exp_date { get; set; }

    }

   
}