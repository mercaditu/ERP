using Syncfusion.Windows.Reports.Designer;
using Syncfusion.Windows.Shared;
using System;
using System.Windows;

namespace cntrl.Reports
{
    public partial class ReportDesigner
    {
        public string ReportModule { get; set; }
        public string ReportPath { get; set; }

        public ReportDesigner()
        {
            InitializeComponent();

            SkinStorage.SetVisualStyle(this, "Office2013");
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ReportDesignerControl.DesignMode = DesignMode.RDLC;

            //if (ReportModule!="" && ReportPath!="")
            //{
            //    string path = entity.Brillo.IO.CreateIfNotExists(Environment.SpecialFolder.MyDocuments + "\\CogntivoERP\\" + ReportModule + "\\" + ReportPath);
            //    if (entity.Brillo.IO.FileExists(path))
            //    {
            //        this.ReportDesignerControl.OpenReport(path);
            //    }
            //}
           
           
           
        }
    }
}