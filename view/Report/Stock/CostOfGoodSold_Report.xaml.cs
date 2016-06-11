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
    public partial class CostOfGoodSold_Report : Page
    {
        ReportPage ReportPage = null; // Application.Current.Windows.OfType<ReportPage>() as ReportPage;

        db db = new db();

        public CostOfGoodSold_Report()
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
           

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1"; 
         

            var movement =
              (from items in db.items
               join item_product in db.item_product on items.id_item equals item_product.id_item
                   into its
               from p in its
               join item_movement in db.item_movement on p.id_item_product equals item_movement.id_item_product
               into IMS
               from a in IMS
               join AM in db.app_branch on a.app_location.id_branch equals AM.id_branch
               where a.status == Status.Stock.InStock
               && a.trans_date <= ReportPage.end_Range
            
               group a by new { a.item_product, a.app_location ,a.sales_invoice_detail}
                   into last
                   select new
                   {
                       trans_date = last.Max(x => x.trans_date).ToString(),
                       branch_code=last.Key.app_location.app_branch.code,
                       branch_name = last.Key.app_location.app_branch.name,
                       code = last.Key.item_product.item.code,
                       name = last.Key.item_product.item.name,
                       location = last.Key.app_location.name,
                       itemid = last.Key.item_product.item.id_item,
                       quantity = last.Sum(x => x.credit) - last.Sum(x => x.debit),
                       unit_cost=last.Key.sales_invoice_detail.unit_cost,
                       sub_total_cost = (last.Sum(x => x.credit) - last.Sum(x => x.debit)) * (last.Key.sales_invoice_detail.unit_cost),
                       id_item_product = last.Key.item_product.id_item_product,
                       measurement = last.Key.item_product.item.app_measurement.code_iso,
                       id_location = last.Key.app_location.id_location
                   }).ToList().OrderBy(y => y.name);




            reportDataSource.Value = movement;
            reportViewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\CostOfGoodSold.rdlc"; // Path of the rdlc file
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();
        }
   
    }
}
