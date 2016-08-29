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
    public partial class CurrentAccount_Customer : Page
    {
       

        public CurrentAccount_Customer()
        {
            InitializeComponent();
            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            this.reportViewer.Reset();

            //
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.FinanceDS FinanceDS = new Data.FinanceDS();

            FinanceDS.BeginInit();

            reportDataSource1.Name = "CurrentAccount_Customer"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = FinanceDS.CurrentAccount_Customer;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.CurrentAccount_Customer.rdlc";

            FinanceDS.EndInit();

            //fill data
            Data.FinanceDSTableAdapters.CurrentAccount_CustomerTableAdapter CurrentAccount_CustomerTableAdapter = new Data.FinanceDSTableAdapters.CurrentAccount_CustomerTableAdapter();
            CurrentAccount_CustomerTableAdapter.ClearBeforeFill = true;
            CurrentAccount_CustomerTableAdapter.Fill(FinanceDS.CurrentAccount_Customer);

            this.reportViewer.RefreshReport();
        }
    }
}
