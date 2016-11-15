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
    public partial class Project : Page
    {
        public Project()
        {
            InitializeComponent();

            using (db db = new db())
            {
                db.projects.Where(x => x.id_company == CurrentSession.Id_Company).OrderBy(y => y.name).ToList();
                cbxProject.ItemsSource = db.projects.Local;

            }

            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            project projects = cbxProject.SelectedItem as project;

            if (projects != null)
            {
                this.reportViewer.Reset();

                Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                //  Data.ProjectDS ProjectDS = new Data.ProjectDS();

                //ProjectDS.BeginInit();

                //  Data.ProjectDSTableAdapters.ProjectTableAdapter ProjectTableAdapter = new Data.ProjectDSTableAdapters.ProjectTableAdapter();

                //fill data
                //  ProjectTableAdapter.ClearBeforeFill = true;
                Class.project Project = new Class.project();
                DataTable dt = Project.GetProject(projects.id_project);

                reportDataSource1.Name = "Project";
                reportDataSource1.Value = dt;
                this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.Project.rdlc";

              //  ProjectDS.EndInit();

                this.reportViewer.Refresh();
                this.reportViewer.RefreshReport();

            }
        }
    }
}




//using entity;
//using Microsoft.Reporting.WinForms;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

//namespace Cognitivo.Reporting.Views
//{
//    /// <summary>
//    /// Interaction logic for SalesByItem.xaml
//    /// </summary>
//    public partial class Project : Page
//    {
//        public Project()
//        {
//            InitializeComponent();

//            using (db db = new db())
//            {
//                db.projects.Where(x => x.id_company == CurrentSession.Id_Company).OrderBy(y => y.name).ToList();
//                cbxProject.ItemsSource = db.projects.Local;

//            }

//            Fill(null, null);
//        }

//        public void Fill(object sender, EventArgs e)
//        {
//            project projects = cbxProject.SelectedItem as project;

//            if (projects != null)
//            {


//                this.reportViewer.Reset();

//                Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
//                Class.project Project = new Class.project();
//                DataTable dt = new DataTable();

//                dt = Project.Return_ParentTask(Convert.ToInt16(cbxProject.SelectedValue));





//                reportDataSource1.Name = "DataSet1"; //Name of the report dataset in our .RDLC file
//                reportDataSource1.Value = dt; //SalesDB.SalesByDate;
//                this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
//                this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.TaskViewParent.rdlc";
//                reportViewer.LocalReport.SubreportProcessing += new

//                           SubreportProcessingEventHandler(SetSubDataSource);



//                this.reportViewer.Refresh();
//                this.reportViewer.RefreshReport();

//            }

//        }
//        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)

//        {
//            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
//            Class.project Project = new Class.project();
//            DataTable dt = new DataTable();

//            dt = Project.Return_ParentTask(1);





//            reportDataSource1.Name = "DataSet1"; //Name of the report dataset in our .RDLC file
//            reportDataSource1.Value = dt; //SalesDB.SalesByDate;

//            e.DataSources.Add(reportDataSource1);

//        }

//    }

//}
