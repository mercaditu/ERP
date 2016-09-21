using LiveCharts;
using System;
using System.Windows.Controls;

namespace cntrl.Chart
{
    public partial class StackedArea : UserControl
    {
        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public StackedArea()
        {
            InitializeComponent();
        }
    }
}
