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
    public partial class TechnicalReport : Page
    {
        public TechnicalReport()
        {
            InitializeComponent();



            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {



            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();


            //fill data
            Class.project Project = new Class.project();
            DataTable dt = Project.TechnicalReport(CurrentSession.Id_Company);

            reportDataSource1.Name = "DataSet1";
            reportDataSource1.Value = dt;
            ReportPanel.ReportDt = dt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.TechnicalReport.rdlc";


            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();


        }
        public void Filter(object sender, RoutedEventArgs e)
        {

            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();

            reportDataSource1.Name = "DataSet1"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt; //SalesDB.SalesByDate;
            reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.TechnicalReport.rdlc";


            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }
    }
}



