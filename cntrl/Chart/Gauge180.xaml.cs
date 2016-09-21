using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace cntrl.Chart
{
    /// <summary>
    /// Interaction logic for Gauge180.xaml
    /// </summary>
    public partial class Gauge180 : UserControl, INotifyPropertyChanged
    {
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }
        private double _value;

        public Func<double, string> Formatter { get; set; }

        public Gauge180()
        {
            InitializeComponent();

            Value = 65;
            Formatter = x => x + " Km/Hr";

            DataContext = this;
        }


    }
}
