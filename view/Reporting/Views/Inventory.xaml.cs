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
    /// <summary>
    /// Interaction logic for SalesByTag.xaml
    /// </summary>
    public partial class Inventory : Page
    {
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
        private DateTime _EndDate = DateTime.Now.AddDays(+1);

        public Inventory()
        {
            InitializeComponent();
            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            this.reportViewer.Reset();
            //
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.ProductDS ProductDS = new Data.ProductDS();

            ProductDS.BeginInit();

            reportDataSource1.Name = "Inventory"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ProductDS.INVENTORY;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.Inventory.rdlc";

            ProductDS.EndInit();

            //fill data
            Data.ProductDSTableAdapters.INVENTORYTableAdapter INVENTORYTableAdapter = new Data.ProductDSTableAdapters.INVENTORYTableAdapter();
            INVENTORYTableAdapter.ClearBeforeFill = true;
            INVENTORYTableAdapter.Fill(ProductDS.INVENTORY, EndDate);

            this.reportViewer.RefreshReport();
        }
    }
}
