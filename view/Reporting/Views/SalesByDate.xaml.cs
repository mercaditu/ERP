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

namespace Cognitivo.Reporting.Views
{
    public partial class SalesByDate : Page
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

        public SalesByDate()
        {
            InitializeComponent();
            
            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.SalesDB SalesDB = new Data.SalesDB();

            SalesDB.BeginInit();

            Data.SalesDBTableAdapters.SalesByDateTableAdapter SalesByDateTableAdapter = new Data.SalesDBTableAdapters.SalesByDateTableAdapter();
            DataTable dt = SalesByDateTableAdapter.GetData(StartDate, EndDate);

            reportDataSource1.Name = "SalesByDate"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesByDate.rdlc";

            SalesDB.EndInit();

            //fill data
            //SalesByDateTableAdapter.ClearBeforeFill = true;
            //SalesByDateTableAdapter.Fill(SalesDB.SalesByDate, StartDate, EndDate);

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
    }
}
