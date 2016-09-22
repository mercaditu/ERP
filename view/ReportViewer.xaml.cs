using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace View
{
    /// <summary>
    /// Interaction logic for ReportViewer.xaml
    /// </summary>
    public partial class ReportViewer : Window
    {
        List<Cognitivo.Project.PrintingPress.calc_Cost> final_cost = new List<Cognitivo.Project.PrintingPress.calc_Cost>();

        public ReportViewer()
        {
            InitializeComponent();
        }


        public void loadReport(ref TabControl CostTab)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";

            final_cost.Clear();
            foreach (TabItem item in CostTab.Items)
            {
                if (item.Name != "ProjectTab")
                {
                    Frame costframe = (Frame)item.Content;
                    Cognitivo.Project.PrintingPressCalculationPage PritingPressPage = (Cognitivo.Project.PrintingPressCalculationPage)costframe.Content;
                    if (PritingPressPage._calc_Cost_BestPrinter != null)
                    {
                        final_cost.Add(PritingPressPage._calc_Cost_BestPrinter);
                    }
                }
            }


            reportDataSource.Value = final_cost;

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = path + "\\CogntivoERP";
            string SubFolder = "";
            SubFolder = "\\TemplateFiles";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Directory.CreateDirectory(path + SubFolder);
                File.Copy("..\\..\\Project\\PrintingPressReport\\PrintingPress.rdlc", path + SubFolder + "\\PrintingPress.rdlc");
            }
            else if (!Directory.Exists(path + SubFolder))
            {
                Directory.CreateDirectory(path + SubFolder);
                File.Copy("..\\..\\Project\\PrintingPressReport\\PrintingPress.rdlc", path + SubFolder + "\\PrintingPress.rdlc");

            }
            else if (!File.Exists(path + SubFolder + "\\Sales_Invoice.rdlc"))
            {
                File.Copy("..\\..\\Project\\PrintingPressReport\\PrintingPress.rdlc", path + SubFolder + "\\PrintingPress.rdlc");
            }
            reportViewer.LocalReport.ReportPath = path + SubFolder + "\\PrintingPress.rdlc"; // Path of the rdlc file

            reportViewer.LocalReport.ReportPath = "..\\..\\Project\\PrintingPressReport\\PrintingPress.rdlc"; // Path of the rdlc file

            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();
        }
    }
}
