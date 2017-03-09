using Syncfusion.Windows.Reports.Designer;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace entity.Reports
{
    /// <summary>
    /// Interaction logic for ReportDesigner.xaml
    /// </summary>
    public partial class ReportDesigner 
    {
        public string path { get; set; }
        public ReportDesigner()
        {
            InitializeComponent();

            SkinStorage.SetVisualStyle(this, "Office2013");

        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
         
            this.ReportDesignerControl.OpenReport(path);
            
        }
    }
}
