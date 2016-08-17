using MySql.Data.MySqlClient;
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
    public partial class ItemPriceList : Page
    {
        public ItemPriceList()
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

            Data.ProductDSTableAdapters.PriceListTableAdapter PriceListTableAdapter = new Data.ProductDSTableAdapters.PriceListTableAdapter();

            DataTable dt = new DataTable();
            dt = PriceListTableAdapter.GetDataBy();

            reportDataSource1.Name = "ItemPriceList"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.ItemPriceList.rdlc";

            ProductDS.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
    }
}
