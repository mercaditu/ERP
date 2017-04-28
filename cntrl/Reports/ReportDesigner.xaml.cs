using Syncfusion.Windows.Reports.Designer;
using Syncfusion.Windows.Shared;
using System;
using System.IO;
using System.Windows;

namespace cntrl.Reports
{
    public partial class ReportDesigner
    {
        FileInfo info = null;
        public string ReportModule { get; set; }
        public string ReportPath
        {
            get { return _ReportPath; }
            set
            {
                _ReportPath = value;
                this.ReportDesignerControl.DesignMode = DesignMode.RDLC;

                if (ReportModule != "" && value != "")
                {
                    string path = @Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/CogntivoERP/" + ReportModule + @"/SalesAnalysis.rdlc";
                    info = new FileInfo(path);
                    string absolutepath = info.Directory + @"\SalesAnalysis.rdlc";

                    if (info.Exists)
                    {
                        this.ReportDesignerControl.OpenReport(absolutepath);

                    }

                    else
                    {
                        MessageBox.Show("Following Report path was invalid : " + absolutepath + " Please provide propert path ", "File Location ");
                    }

                  
                }
            }
        }
        string _ReportPath;

        public ReportDesigner()
        {
            InitializeComponent();

            SkinStorage.SetVisualStyle(this, "Office2013");
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {



        }
    }
}