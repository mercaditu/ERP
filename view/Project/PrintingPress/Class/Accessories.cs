using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Cognitivo.Project.PrintingPress
{
    public class Accessory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Consumption { get; set; }
        public decimal Cost { get; set; }
        public decimal Calc_Cost { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }

    public class Ink : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Consumption { get; set; }
        public decimal Cost { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
     public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }

   public class Toner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
    }
}
