using System.Windows.Controls;

namespace entity
{
    public partial class DocumentViewr : UserControl
    {
        public DocumentViewr()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Reports.ReportDesigner window = new Reports.ReportDesigner
            {
                Title = "Report",
                path = reportViewer.ReportPath
            };

            window.ShowDialog();
        }
    }
}