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
    public partial class ProjectExecution : Page
    {
        public ProjectExecution()
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
                    Data.ProjectDS ProjectDS = new Data.ProjectDS();

                    ProjectDS.BeginInit();

                    Data.ProjectDSTableAdapters.Project_VS_ProductionTableAdapter Project_VS_ProductionTableAdapter = new Data.ProjectDSTableAdapters.Project_VS_ProductionTableAdapter();

                    //fill data
                    Project_VS_ProductionTableAdapter.ClearBeforeFill = true;
                    DataTable dt = Project_VS_ProductionTableAdapter.GetData(projects.id_project);

                    reportDataSource1.Name = "Project_VS_Production";
                    reportDataSource1.Value = dt;
                    this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                    this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.Project_VS_Production.rdlc";

                    ProjectDS.EndInit();

                    this.reportViewer.Refresh();
                    this.reportViewer.RefreshReport();
                
            }
        }
    }
}
