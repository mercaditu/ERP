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
    public partial class Project : Page
    {
        public Project()
        {
            InitializeComponent();


            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {


            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            //  Data.ProjectDS ProjectDS = new Data.ProjectDS();

            //ProjectDS.BeginInit();

            //  Data.ProjectDSTableAdapters.ProjectTableAdapter ProjectTableAdapter = new Data.ProjectDSTableAdapters.ProjectTableAdapter();

            //fill data
            //  ProjectTableAdapter.ClearBeforeFill = true;
            Class.project Project = new Class.project();
            DataTable dt = Project.GetProject(CurrentSession.Id_Company);
            ReportPanel.ReportDt = dt;
            reportDataSource1.Name = "Project";
            reportDataSource1.Value = dt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.Project.rdlc";

            //  ProjectDS.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();


        }
        public void Filter(object sender, RoutedEventArgs e)
        {

            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();

            reportDataSource1.Name = "Project"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt; //SalesDB.SalesByDate;
            reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.Project.rdlc";


            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }
    }
}



