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
            entity.Reports.ReportDesigner window = new entity.Reports.ReportDesigner
            {
                Title = "Report",
                path = reportViewer.ReportPath
            };

            window.ShowDialog();
        }

      
    }
}