using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace entity.Class
{
    public class Impex_CostDetail : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string number { get; set; }
        public int? id_item { get; set; }
        public int id_invoice { get; set; }
        public int id_invoice_detail { get; set; }
        public string item_code { get; set; }
        public string item { get; set; }
        public decimal quantity { get; set; }
        public decimal unit_cost { get; set; }
        public decimal unit_Importcost { get; set; }
        public decimal cost { get; set; }
        public decimal sub_total { get { return _sub_total; } set { _sub_total = value; RaisePropertyChanged("sub_total"); } }
        private decimal _sub_total = 0;
        public decimal prorated_cost { get { return _prorated_cost; } set { _prorated_cost = value; RaisePropertyChanged("prorated_cost"); } }
        private decimal _prorated_cost;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
    public class Impex_Products : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int? id_item { get; set; }
        public string item { get; set; }
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
