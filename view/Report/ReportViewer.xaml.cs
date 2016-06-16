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
using System.IO;
using cntrl.Controls;


namespace Cognitivo.Report
{
    public partial class ReportViewer : MetroWindow
    {
        public entity.App.Names Reports { get; set; }
        db db = new db();

        public DateTime start_Range
        {
            get { return _start_Range; }
            set
            {
                if (_start_Range != value)
                {
                    _start_Range = value;
                }
            }
        }
        private DateTime _start_Range = DateTime.Now.AddMonths(-1);


        public DateTime end_Range
        {
            get { return _end_Range; }
            set
            {
                if (_end_Range != value)
                {
                    _end_Range = value;
                }
            }
        }
        private DateTime _end_Range = DateTime.Now;

        public ReportViewer()
        {
            InitializeComponent();
        }

        private void btnGridSearch(object sender, RoutedEventArgs e)
        {
            ListBoxItem ListBoxItem = NavList.SelectedItem as ListBoxItem;
            
        }

    }
}
