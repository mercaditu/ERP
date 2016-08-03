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
            
            using(db db = new db())
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

                Data.SalesDSTableAdapters.SalesByDateTableAdapter SalesByDateTableAdapter = new Data.SalesDSTableAdapters.SalesByDateTableAdapter();
                
                if (true)
                {
                    DataTable dt = SalesByDateTableAdapter.GetDataByBranch(StartDate, EndDate, app_branch.id_branch);
                }
                else
                {
                    DataTable dt = SalesByDateTableAdapter.GetDataByGeneral(StartDate, EndDate);
                }

                reportDataSource1.Name = "SalesByDate"; //Name of the report dataset in our .RDLC file
                reportDataSource1.Value = dt; //SalesDB.SalesByDate;
                this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesByDate.rdlc";

                SalesDB.EndInit();

                this.reportViewer.Refresh();
                this.reportViewer.RefreshReport();   
            }
        }
    }
}
