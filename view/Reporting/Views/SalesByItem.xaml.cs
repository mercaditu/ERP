using entity;
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
    /// <summary>
    /// Interaction logic for SalesByItem.xaml
    /// </summary>
    public partial class SalesByItem : Page
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

        public SalesByItem()
        {
            InitializeComponent();

            using (db db = new db())
            {
                db.app_branch.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).OrderBy(y => y.name).ToList();
                cbxBranch.ItemsSource = db.app_branch.Local;
            }

            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            app_branch app_branch = cbxBranch.SelectedItem as app_branch;

            if (app_branch != null)
            {
                this.reportViewer.Reset();

                Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                Data.SalesDS SalesDB = new Data.SalesDS();

                SalesDB.BeginInit();

                Data.SalesDSTableAdapters.SalesByItemTableAdapter SalesByItemTableAdapter = new Data.SalesDSTableAdapters.SalesByItemTableAdapter();

                //fill data
                SalesByItemTableAdapter.ClearBeforeFill = true;
                DataTable dt = SalesByItemTableAdapter.GetData(StartDate, EndDate, app_branch.id_branch);

                reportDataSource1.Name = "SalesByItem"; //Name of the report dataset in our .RDLC file
                reportDataSource1.Value = dt;
                this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesByItem.rdlc";

                SalesDB.EndInit();

                this.reportViewer.Refresh();
                this.reportViewer.RefreshReport();   
            }
        }
    }
}
