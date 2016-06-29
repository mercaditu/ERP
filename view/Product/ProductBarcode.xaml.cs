using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Data.Entity;
using System.Data;
using entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Threading.Tasks;
using cntrl.Controls;
using Microsoft.Reporting.WinForms;


namespace Cognitivo.Product
{
    /// <summary>
    /// Interaction logic for ProductBarcode.xaml
    /// </summary>
    public partial class ProductBarcode : Page
    {
        ItemDB ItemDB = null;
        CollectionViewSource itemViewSource;
        public ProductBarcode()
        {
            InitializeComponent();
            itemViewSource = FindResource("itemViewSource") as CollectionViewSource;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ItemDB = new ItemDB();
            ItemDB.items.Load();
            itemViewSource.Source = ItemDB.items.Local.OrderBy(x => x.name);
            string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\item";
            string[] files = System.IO.Directory.GetFiles(@PathFull);
            foreach (string file in files)
            {
                cmbDocument.Items.Add(file);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<item> itemList = itemViewSource.View.OfType<item>().ToList().Where(x => x.IsSelected).ToList();
            foreach (item item in itemList)
            {
                PrintReport(item);
            }
        }
        void PrintReport(item item)
        {

        }

        void DisplayReport(item item, string Filename)
        {
            string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\item";
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = ItemDB.items.Where(x => x == item)
                          .Select(g => new
                          {
                              item_name = g.name,
                              item_code = g.code,
                              item_brand = g.item_brand != null ? g.item_brand.name : ""
                          }).ToList();

            reportViewer.LocalReport.ReportPath = PathFull + "\\" + Filename; // Path of the rdlc file
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();
        }

        private void cmbDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            item item=itemViewSource.View.CurrentItem as item;
            DisplayReport(item, cmbDocument.Text+".rdlc");
        }
    }
}
