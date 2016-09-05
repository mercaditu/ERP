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
    public partial class Contact : Page
    {
        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }

        public Contact()
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

            reportDataSource1.Name = "CustomerList"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = SalesDB.CustomerList;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.CustomerList.rdlc";

            SalesDB.EndInit();

            //fill data
            Data.SalesDSTableAdapters.CustomerListTableAdapter CustomerListTableAdapter = new Data.SalesDSTableAdapters.CustomerListTableAdapter();
            CustomerListTableAdapter.ClearBeforeFill = true;
            CustomerListTableAdapter.Fill(SalesDB.CustomerList, IsCustomer, IsSupplier);

            this.reportViewer.RefreshReport();
        }
    }
}
