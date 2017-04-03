using Syncfusion.Windows.Shared;
using System.Windows;

namespace entity.Reports
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
            this.ReportDesignerControl.OpenReport(path);
        }
    }
}