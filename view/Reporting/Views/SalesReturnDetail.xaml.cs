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
using System.Windows.Forms;
using entity;

namespace Cognitivo.Reporting.Views
{
    public partial class SalesReturnDetail : Page
    {
      

        public SalesReturnDetail()
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

                Data.SalesDSTableAdapters.SalesReturnDetailTableAdapter SalesReturnDetailTableAdapter = new Data.SalesDSTableAdapters.SalesReturnDetailTableAdapter();
                DataTable dt = SalesReturnDetailTableAdapter.GetData(ReportPanel.StartDate, ReportPanel.EndDate);

                reportDataSource1.Name = "SalesReturnDetail"; //Name of the report dataset in our .RDLC file
                reportDataSource1.Value = dt; //SalesDB.SalesByDate;
                this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesReturnDetail.rdlc";

                SalesDB.EndInit();

                this.reportViewer.Refresh();
                this.reportViewer.RefreshReport();
        }
    }
}
