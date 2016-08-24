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
    public partial class SalesByItem : Page
    {
        public SalesByItem()
        {
            InitializeComponent();

            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.SalesDS SalesDB = new Data.SalesDS();

            SalesDB.BeginInit();

            Data.SalesDSTableAdapters.SalesByItemTableAdapter SalesByItemTableAdapter = new Data.SalesDSTableAdapters.SalesByItemTableAdapter();

            //fill data
            SalesByItemTableAdapter.ClearBeforeFill = true;
            DataTable dt = new DataTable();

            if (ReportPanel.Branch != null)
            {
                dt = SalesByItemTableAdapter.GetDataByBranch(ReportPanel.StartDate, ReportPanel.EndDate, ReportPanel.Branch.id_branch, CurrentSession.Id_Company);
            }
            else
            {
                dt = SalesByItemTableAdapter.GetDataByGeneral(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);
            }

            reportDataSource1.Name = "SalesByItem"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesByItem.rdlc";

            SalesDB.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
    }
}
