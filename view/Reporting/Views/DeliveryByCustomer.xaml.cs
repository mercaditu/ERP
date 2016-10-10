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
    public partial class DeliveryByCustomer : Page
    {


        public DeliveryByCustomer()
        {
            InitializeComponent();

            //using(db db = new db())
            //{
            //    db.app_branch.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).OrderBy(y => y.name).ToList();
            //    cbxBranch.ItemsSource = db.app_branch.Local;
            //}

            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            //app_branch app_branch = cbxBranch.SelectedItem as app_branch;

            //if (app_branch != null)
            //{
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.SalesDS SalesDB = new Data.SalesDS();

            SalesDB.BeginInit();

            Data.SalesDSTableAdapters.DeliveryByCustomerTableAdapter DeliveryByCustomerTableAdapter = new Data.SalesDSTableAdapters.DeliveryByCustomerTableAdapter();
            DataTable dt = DeliveryByCustomerTableAdapter.GetData(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);
            if (ReportPanel.ReportDt == null)
            {
                ReportPanel.ReportDt = dt;
            }
            reportDataSource1.Name = " DeliveryByCustomer"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports. DeliveryByCustomer.rdlc";

            SalesDB.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
            //}
        }
    }
}
