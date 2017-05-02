
using entity;
using Syncfusion.Windows.Reports.Designer;
using Syncfusion.Windows.Shared;
using System;
using System.IO;
using System.Windows;

namespace cntrl.Reports
{
    public partial class ReportEditor
    {
        public string Path { get; set; }
        public App.Names Application { get; set; }
        public ReportEditor()
        {
            InitializeComponent();

          //  SkinStorage.SetVisualStyle(this, "Office2013");
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {


            string ReportName = Path.Replace("cntrl.Reports.", "");
            ReportName = ReportName.Remove(0, ReportName.IndexOf(".") + 1);
            string path = entity.Brillo.IO.CreateIfNotExists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\" + Application + "\\" + ReportName);
            if (entity.Brillo.IO.FileExists(path))
            {
                // ReportDesigner.ReportModule = Report.Application.ToString();
                designer.ReportPath = @path;
                designer.open();
            }
            else
            {
                CopyResource(Path, path);
            }

        }
        private void CopyResource(string resourceName, string file)
        {
            using (Stream resource = GetType().Assembly
                                              .GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                {
                    throw new ArgumentException("No such resource", "resourceName");
                }
                using (Stream output = File.OpenWrite(file))
                {
                    resource.CopyTo(output);
                }
            }
        }
    }
}