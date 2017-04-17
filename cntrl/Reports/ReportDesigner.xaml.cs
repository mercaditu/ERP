using Syncfusion.Windows.Reports.Designer;
using Syncfusion.Windows.Shared;
using System;
using System.IO;
using System.Windows;

namespace cntrl.Reports
{
    /// <summary>
    /// Interaction logic for ReportDesigner.xaml
    /// </summary>
    public partial class ReportDesigner
    {
        public string path { get; set; }

        public ReportDesigner()
        {
            InitializeComponent();

            SkinStorage.SetVisualStyle(this, "Office2013");
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ReportDesignerControl.DesignMode = DesignMode.RDLC;
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
            if (File.Exists(path + SubFolder + "\\SalesAnalysis.rdlc"))
            {
                try
                {
                    this.ReportDesignerControl.OpenReport(@path + SubFolder + "\\SalesAnalysis.rdlc");
                }
                catch (Exception ex)
                {

                   
                }
               
            }
           
        }
    }
}