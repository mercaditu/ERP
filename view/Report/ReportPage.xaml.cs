using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using entity;

namespace Cognitivo.Report
{
    public partial class ReportPage : MetroWindow
    {
        public entity.App.Names Reports { get; set; }

        public ReportPage()
        {
            InitializeComponent();
        }

        private void btnGridSearch(object sender, RoutedEventArgs e)
        {

        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            flyFilter.IsOpen = true;
        }
    }
}
