using entity;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Product
{
    /// <summary>
    /// Interaction logic for ProductBarcode.xaml
    /// </summary>
    public partial class ProductBarcode : Page
    {
        private ItemDB ItemDB = null;
        private CollectionViewSource itemViewSource;

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
            CreateFile();
            string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\item";
            string[] files = System.IO.Directory.GetFiles(@PathFull);
            foreach (string file in files)
            {
                cmbDocument.Items.Add(file);
            }
        }

        private void CreateFile()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP";

            //If path (CognitivoERP) does not exist, create path.
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            string SubFolder = "\\TemplateFiles";

            //If path (TemplateFiles) does not exist, create path
            if (!Directory.Exists(path + SubFolder))
            {
                Directory.CreateDirectory(path + SubFolder);
            }
            if (!Directory.Exists(path + SubFolder + "\\item"))
            {
                Directory.CreateDirectory(path + SubFolder + "\\item");
            }

            string[] files = System.IO.Directory.GetFiles(@AppDomain.CurrentDomain.BaseDirectory + "\\item");
            foreach (string file in files)
            {
                if (!File.Exists(path + SubFolder + "\\item" + "\\" + file + ".rdlc"))
                {
                    //Add Logic
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\item" + "\\" + file + ".rdlc"))
                    {
                        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\item" + "\\" + file + ".rdlc",
                               path + SubFolder + "\\" + file + ".rdlc");
                    }
                }
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

        private void PrintReport(item item)
        {
        }

        private void DisplayReport(item item, string Filename)
        {
            string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\item";
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = ItemDB.items.Where(x => x == item)
                          .Select(g => new
                          {
                              Product_name = g.name,
                              Product_code = g.code,
                              item_brand = g.item_brand != null ? g.item_brand.name : ""
                          }).ToList();

            reportViewer.LocalReport.ReportPath = PathFull + "\\" + Filename; // Path of the rdlc file
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();
        }

        private void cmbDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            item item = itemViewSource.View.CurrentItem as item;
            DisplayReport(item, cmbDocument.Text + ".rdlc");
        }
    }
}