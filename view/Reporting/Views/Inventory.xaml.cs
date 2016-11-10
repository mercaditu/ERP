﻿using System.Data;
using System.Windows;
using System.Windows.Controls;
using entity;

namespace Cognitivo.Reporting.Views
{
    public partial class Inventory : Page
    {
        public Inventory()
        {
            InitializeComponent();            
            Fill(null, null);
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.ProductDS ProductDS = new Data.ProductDS();

            ProductDS.BeginInit();

            Data.ProductDSTableAdapters.InventorySummaryTableAdapter InventorySummaryTableAdapter = new Data.ProductDSTableAdapters.InventorySummaryTableAdapter();
                
            DataTable dt = new DataTable();

            //if (ReportPanel.Branch != null)
            //{
            //    dt = InventorySummaryTableAdapter.GetDataByBranch(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company, ReportPanel.Branch.id_branch);
            //}
            //else
            //{
                dt = InventorySummaryTableAdapter.GetDataByDate(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);
//            }

          
                ReportPanel.ReportDt = dt;
          
            reportDataSource1.Name = "InventorySummary"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.InventorySummary.rdlc";

            ProductDS.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }

        public void Filter(object sender, RoutedEventArgs e)
        {
         
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
         
            reportDataSource1.Name = "InventorySummary"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.InventorySummary.rdlc";

         

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
     
    }
}
