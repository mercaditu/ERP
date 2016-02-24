using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Cognitivo.Class
{
    class clsaccesories : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string name;

        public string nameProperty
        {
            get { return name; }
            set { name = value; }
        }
        private decimal? cost;

        public decimal? costProperty
        {
            get { return cost; }
            set { cost = value; }
        }

        private decimal? Totalcost;

        public decimal? TotalcostProperty
        {
            get { return Totalcost; }
            set { Totalcost = value; RaisePropertyChanged("TotalcostProperty"); }
        }

        private int qty;

        public int qtyProperty
        {
            get { return qty; }
            set { qty = value; }
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
