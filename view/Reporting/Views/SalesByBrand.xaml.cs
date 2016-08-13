using entity;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace Cognitivo.Reporting.Views
{
    /// <summary>
    /// Interaction logic for SalesByItem.xaml
    /// </summary>
    public partial class SalesByBrand : Page
    {
        public SalesByBrand()
        {
            InitializeComponent();
            Fill(null, null);
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            if (ReportPanel.Branch != null)
            {
                this.reportViewer.Reset();

                Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                Data.SalesDS SalesDB = new Data.SalesDS();

                SalesDB.BeginInit();

                Data.SalesDSTableAdapters.SalesByBrandTableAdapter SalesByBrandTableAdapter = new Data.SalesDSTableAdapters.SalesByBrandTableAdapter();

                //fill data
                SalesByBrandTableAdapter.ClearBeforeFill = true;
                DataTable dt = SalesByBrandTableAdapter.GetData(ReportPanel.StartDate, ReportPanel.EndDate, ReportPanel.Branch.id_branch);

                reportDataSource1.Name = "SalesByBrand"; //Name of the report dataset in our .RDLC file
                reportDataSource1.Value = dt;
                this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesByBrand.rdlc";

                SalesDB.EndInit();

                this.reportViewer.Refresh();
                this.reportViewer.RefreshReport();   
            }
        }

        private void reportViewer_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {

        }
    }
}
