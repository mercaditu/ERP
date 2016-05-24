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
    public partial class HistoricStockLevels_Report : Page
    {
        ReportPage ReportPage = null; // Application.Current.Windows.OfType<ReportPage>() as ReportPage;

        db db = new db();

        public HistoricStockLevels_Report()
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
            var predicate = PredicateBuilder.True<entity.item_movement>();

          

            if (ReportPage.start_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date >= ReportPage.start_Range);

            }
            if (ReportPage.end_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date <= ReportPage.end_Range);

            }
           

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<item_movement> item_movementList = db.item_movement.Where(predicate).ToList();
            reportDataSource.Value = item_movementList
                .Select(g => new
            {
                id_item_product = g.id_item_product,
                item_name=g.item_product.item!=null?g.item_product.item.name:"",
                Stock=(g.credit-g.debit),
            }).ToList();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = path + "\\CogntivoERP";
            string SubFolder = "";
            SubFolder = "\\Reports";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Directory.CreateDirectory(path + SubFolder);
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\HistoricStockLevels.rdlc", path + SubFolder + "\\HistoricStockLevels.rdlc");
            }
            else if (!Directory.Exists(path + SubFolder))
            {
                Directory.CreateDirectory(path + SubFolder);
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\HistoricStockLevels.rdlc", path + SubFolder + "\\HistoricStockLevels.rdlc");

            }
            else if (!File.Exists(path + SubFolder + "\\HistoricStockLevels.rdlc"))
            {
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\HistoricStockLevels.rdlc", path + SubFolder + "\\HistoricStockLevels.rdlc");
            }


            reportViewer.LocalReport.ReportPath = path + SubFolder + "\\HistoricStockLevels.rdlc"; // Path of the rdlc file
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();



        }   
    }
}
