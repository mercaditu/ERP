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
using Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution;

namespace Cognitivo.Reporting.Views
{
    public partial class StockFlowDimension : Page
    {
        public StockFlowDimension()
        {
            InitializeComponent();
            Fill(null, null);
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.ProductDS ProductDB = new Data.ProductDS();

            ProductDB.BeginInit();

            Data.ProductDSTableAdapters.StockFlowDimensionTableAdapter StockFlowDimensionTableAdapter = new Data.ProductDSTableAdapters.StockFlowDimensionTableAdapter();

            DataTable dt = new DataTable();

            //if (ReportPanel.Branch != null)
            //{
            //    dt = StockFlowDimensionTableAdapter.GetDataByBranch(CurrentSession.Id_Company, ReportPanel.Branch.id_branch, ReportPanel.StartDate, ReportPanel.EndDate);
            //}
            //else
            //{
                dt = StockFlowDimensionTableAdapter.GetDataBy(CurrentSession.Id_Company, ReportPanel.StartDate, ReportPanel.EndDate);
            //}
       
                ReportPanel.ReportDt = dt;
            
            //ReportParameter[] parameters = new ReportParameter[x+1];

            reportDataSource1.Name = "StockFlowDimension"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.StockFlowDimension.rdlc";
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //this.reportViewer.LocalReport.SetParameters("EndDate", dtEndDate, false);

            ProductDB.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }

        public void Filter(object sender, RoutedEventArgs e)
        {
         
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
           
            //ReportParameter[] parameters = new ReportParameter[x+1];

            reportDataSource1.Name = "StockFlowDimension"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.StockFlowDimension.rdlc";
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //this.reportViewer.LocalReport.SetParameters("EndDate", dtEndDate, false);

           

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
    }
}
