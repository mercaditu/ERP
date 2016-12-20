using System;
using System.Windows.Controls;

namespace cntrl.Chart
{
    public partial class DataGridControl : UserControl //, INotifyPropertyChanged
    {
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                //OnPropertyChanged("Value");
            }
        }
        private double _value;


        public Func<double, string> Formatter { get; set; }

        public DataGridControl()
        {
            InitializeComponent();
        }
    }
}
