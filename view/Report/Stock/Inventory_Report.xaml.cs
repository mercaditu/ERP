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
    public partial class Inventory_Report : Page
    {
        ReportPage ReportPage = null; // Application.Current.Windows.OfType<ReportPage>() as ReportPage;

        db db = new db();

        public Inventory_Report()
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
            var predicate = PredicateBuilder.True<entity.item_inventory>();

           

            //if (ReportPage.start_Range != Convert.ToDateTime("1/1/0001"))
            //{
            //    predicate = predicate.And(x => x.trans_date >= ReportPage.start_Range);

            //}
            //if (ReportPage.end_Range != Convert.ToDateTime("1/1/0001"))
            //{
            //    predicate = predicate.And(x => x.trans_date <= ReportPage.end_Range);

            //}
         

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<item_inventory> item_inventoryList = db.item_inventory.Where(predicate).ToList();
            var inventoryList = item_inventoryList.Where(x => x.trans_date.Date >= ReportPage.start_Range && x.trans_date <= ReportPage.end_Range)
                  .Join(db.item_inventory_detail, u => u.id_inventory, sid => sid.id_inventory, (Inventory, Iid) => new { Inventory, Iid }).Select(g => new
              {
                  id_branch = g.Inventory != null ? g.Inventory.id_branch : 0,
                  branch_name = g.Inventory != null ? g.Inventory.app_branch.name : "",
                  trans_date = g.Inventory != null ? g.Inventory.trans_date.ToString() : "",
                  item_name=g.Iid.item_product!=null?g.Iid.item_product.item!=null?g.Iid.item_product.item.name:"":"",
                  id_item = g.Iid.item_product != null ? g.Iid.item_product.item != null ? g.Iid.item_product.id_item : 0 : 0,
                  value=g.Iid.value_counted
              }).ToList();

         

            reportDataSource.Value = inventoryList;
            reportViewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\MatrixInventory.rdlc"; // Path of the rdlc file
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();
        }
   
    }
}
