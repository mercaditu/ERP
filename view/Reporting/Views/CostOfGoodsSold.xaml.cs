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

namespace Cognitivo.Reporting.Views
{
    public partial class CostOfGoodsSold : Page
    {
        public DateTime StartDate 
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        private DateTime _StartDate = DateTime.Now.AddMonths(-1);
        public DateTime EndDate 
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
        private DateTime _EndDate = DateTime.Now.AddDays(+1);

        public CostOfGoodsSold()
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

            reportDataSource1.Name = "CostOfGoodsSold"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = SalesDB.CostOfGoodsSold;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.CostOfGoodsSold.rdlc";

            SalesDB.EndInit();

            //fill data
            Data.SalesDSTableAdapters.CostOfGoodsSoldTableAdapter CostOfGoodsSoldTableAdapter = new Data.SalesDSTableAdapters.CostOfGoodsSoldTableAdapter();
            CostOfGoodsSoldTableAdapter.ClearBeforeFill = true;
            CostOfGoodsSoldTableAdapter.Fill(SalesDB.CostOfGoodsSold, StartDate, EndDate);

            this.reportViewer.RefreshReport();
        }
    }
}
