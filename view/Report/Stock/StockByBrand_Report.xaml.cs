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
    public partial class StockByBrand_Report : Page
    {
        ReportPage ReportPage = null; // Application.Current.Windows.OfType<ReportPage>() as ReportPage;

        db db = new db();

        public StockByBrand_Report()
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

            //if (ReportPage.start_Range != Convert.ToDateTime("1/1/0001"))
            //{
            //    predicate = predicate.And(x => x.trans_date >= ReportPage.start_Range);
            //}
            //if (ReportPage.end_Range != Convert.ToDateTime("1/1/0001"))
            //{
            //    predicate = predicate.And(x => x.trans_date <= ReportPage.end_Range);
            //}

            if (ReportPage.BrandArray != null)
            {
                predicate = predicate.And(x => ReportPage.BrandArray.Contains(x.item_product.item != null ? x.item_product.item.item_brand != null ? x.item_product.item.item_brand.name : "" : ""));
            }
            if (ReportPage.Item != null)
            {
                predicate = predicate.And(x => (x.item_product != null ? x.item_product.id_item : 0) == ReportPage.Item.id_item);
            }

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<item_movement> item_movementList = db.item_movement.Where(predicate).ToList();
            var movementlist = item_movementList.Where(x => x.trans_date.Date >= ReportPage.start_Range && x.trans_date <= ReportPage.end_Range)
                 .Select(g => new
             {
                 id_item_product = g.id_item_product,
                 item_code = g.item_product.item != null ? g.item_product.item.code : "",
                 item_name = g.item_product.item != null ? g.item_product.item.name : "",
                 brand_name = g.item_product.item != null ? g.item_product.item.item_brand != null ? g.item_product.item.item_brand.name : "" : "",
                 id_brand = g.item_product.item != null ? g.item_product.item.id_brand : 0,
                 id_branch = g.app_location != null ? g.app_location.id_branch : 0,
                 branch_name = g.app_location != null ? g.app_location.app_branch.name : "",
                 Tag = g.item_product != null ? g.item_product.item != null ? ReportPage.GetTag(g.item_product.item.item_tag_detail.ToList()) : "" : "",
                 Stock = (g.credit - g.debit),
                 value = (g.item_movement_value.Sum(x => x.unit_value)) * (g.item_product.item != null ? g.item_product.item.unit_cost : 0),
             }).ToList();

           if (ReportPage.TagArray!=null)
           {
               movementlist = movementlist.Where(x => x.Tag.ToLower().Contains(ReportPage.TagArray.ToLower().ToString())).ToList();
           }

           reportDataSource.Value = movementlist;
            reportViewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\StockByBrand.rdlc"; // Path of the rdlc file
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();

        }
       
    }
}
