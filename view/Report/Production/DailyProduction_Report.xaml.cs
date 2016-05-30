using cntrl.Controls;
using entity;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Cognitivo.Report
{
    public partial class DailyProduction_Report : Page
    {
        ReportPage ReportPage = null; // Application.Current.Windows.OfType<ReportPage>() as ReportPage;

        db db = new db();

        public DailyProduction_Report()
        {
            InitializeComponent();
        }

        private void rpt_Loaded(object sender, RoutedEventArgs e)
        {
            ReportPage = Application.Current.Windows.OfType<ReportPage>().FirstOrDefault() as ReportPage;

            QueryBuilder();
        }

        private void QueryBuilder()
        {
            var predicate = PredicateBuilder.True<entity.production_execution_detail>();

          
            if (ReportPage.Contact != null)
            {
                predicate = predicate.And(x => x.contact == ReportPage.Contact);
            }

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<production_execution_detail> production_execution_detailList = db.production_execution_detail.Where(predicate).ToList();
            reportDataSource.Value = production_execution_detailList
               .Select(g => new
            {
               id_project=g.project_task!=null?g.project_task.project.id_project:0,
               project_name = g.project_task!=null?g.project_task.project.name:"",
               id_contact =g.id_contact!=null?g.id_contact:0,
               contact_name = g.contact!=null?g.contact.name:"",
               id_item = g.id_item,
               item_name = g.item.name,
               quantity = g.quantity,
               id_task = g.project_task!=null?g.project_task.id_project_task:0,
               task_name = g.project_task!=null?g.project_task.name:""
            }).ToList();

            reportViewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\DailyProduction.rdlc"; // Path of the rdlc file
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();
        }   
    }
}
