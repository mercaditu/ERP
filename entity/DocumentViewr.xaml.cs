using System.Windows.Controls;

namespace entity
{
    public partial class DocumentViewer : UserControl
    {
        public DocumentViewer()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Reports.ReportDesigner window = new Reports.ReportDesigner
            {
                Title = "Designer",
                path = reportViewer.ReportPath
            };

            window.ShowDialog();
        }
    }
}